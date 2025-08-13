using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// --- Configuração da Chave JWT (Corrigida) ---
var keyString = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(keyString))
{
    // Usar uma chave padrão apenas para desenvolvimento se não houver nenhuma no appsettings
    keyString = "uma-chave-super-secreta-e-longa-para-testes-locais-123456";
}
var key = Encoding.UTF8.GetBytes(keyString);

// --- Configuração de Autenticação e Autorização ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
builder.Services.AddAuthorization();

// --- Injeção de Dependência ---
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

// --- Configuração do Swagger para aceitar o Token ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // Corrigido
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- Configuração do DbContext (Modo correto) ---
builder.Services.AddDbContext<DbContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("mysql");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();
#endregion

#region Middlewares
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Endpoints

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administradores
string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(keyString)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(key);
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim> {
        new(ClaimTypes.Email, administrador.Email), // Usando tipos padrão de Claim
        new(ClaimTypes.Role, administrador.Perfil),
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(8), // Duração mais razoável
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Endpoint de Login (agora async)
app.MapPost("/administradores/login", async ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = await administradorServico.LoginAsync(loginDTO);
    if (adm != null)
    {
        var token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado { Email = adm.Email, Perfil = adm.Perfil, Token = token });
    }
    return Results.Unauthorized();

}).WithTags("Administradores").AllowAnonymous();


// Endpoint para criar Administrador (agora async)
app.MapPost("/administradores", async ([FromBody] AdministradorDTO dto, IAdministradorServico servico) =>
{
    var administrador = new Administrador { Email = dto.Email, Senha = dto.Senha, Perfil = dto.Perfil?.ToString() ?? "Editor" };
    await servico.IncluirAsync(administrador);
    return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView { Id = administrador.Id, Email = administrador.Email, Perfil = administrador.Perfil });

}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");
#endregion

#region Veiculos
// Função de validação movida para um local mais apropriado ou substituída por FluentValidation no futuro
ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao { Mensagens = new List<string>() };
    if (string.IsNullOrEmpty(veiculoDTO.Nome)) validacao.Mensagens.Add("O nome é obrigatório.");
    if (string.IsNullOrEmpty(veiculoDTO.Marca)) validacao.Mensagens.Add("A marca é obrigatória.");
    if (veiculoDTO.Ano < 1950) validacao.Mensagens.Add("Veículo muito antigo, apenas aceito após 1950.");
    return validacao;
}

// Endpoint para criar Veículo (agora async)
app.MapPost("/veiculos", async ([FromBody] VeiculoDTO dto, IVeiculoServico servico) =>
{
    var validacao = ValidaDTO(dto);
    if (validacao.Mensagens.Any()) return Results.BadRequest(validacao);

    var veiculo = new Veiculo { Nome = dto.Nome, Marca = dto.Marca, Ano = dto.Ano };
    await servico.IncluirAsync(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);

}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");


// Endpoint para buscar todos os Veículos (agora async)
app.MapGet("/veiculos", async ([FromQuery] int? pagina, IVeiculoServico servico) =>
{
    var veiculos = await servico.TodosAsync(pagina);
    return Results.Ok(veiculos);

}).AllowAnonymous().WithTags("Veiculos");


// Endpoint para buscar Veículo por ID (agora async)
app.MapGet("/veiculos/{id}", async ([FromRoute] int id, IVeiculoServico servico) =>
{
    var veiculo = await servico.BuscarPorIdAsync(id);
    return veiculo == null ? Results.NotFound() : Results.Ok(veiculo);

}).AllowAnonymous().WithTags("Veiculos");


// Endpoint para atualizar Veículo (agora async)
app.MapPut("/veiculos/{id}", async ([FromRoute] int id, [FromBody] VeiculoDTO dto, IVeiculoServico servico) =>
{
    var validacao = ValidaDTO(dto);
    if (validacao.Mensagens.Any()) return Results.BadRequest(validacao);

    var veiculo = await servico.BuscarPorIdAsync(id);
    if (veiculo == null) return Results.NotFound();

    veiculo.Nome = dto.Nome;
    veiculo.Marca = dto.Marca;
    veiculo.Ano = dto.Ano;

    await servico.AtualizarAsync(veiculo);
    return Results.Ok(veiculo);

}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Veiculos");


// Endpoint para apagar Veículo (agora async)
app.MapDelete("/veiculos/{id}", async ([FromRoute] int id, IVeiculoServico servico) =>
{
    var veiculo = await servico.BuscarPorIdAsync(id);
    if (veiculo == null) return Results.NotFound();

    await servico.ApagarAsync(veiculo);
    return Results.NoContent();

}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veiculos");

#endregion

#endregion

app.Run();