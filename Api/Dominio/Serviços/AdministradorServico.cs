using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto = null!;

    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<Administrador?> LoginAsync(LoginDTO loginDTO)
    {
        // 1. Busca o administrador pelo e-mail
        var adm = await _contexto.Administradores.FirstOrDefaultAsync(a => a.Email == loginDTO.Email);
        if (adm == null)
        {
            return null; // Retorna nulo se o e-mail não existir
        }

        // 2. Verifica se a senha enviada corresponde ao hash salvo no banco
        //    A função BCrypt.Verify faz a comparação segura.
        if (BCrypt.Net.BCrypt.Verify(loginDTO.Senha, adm.Senha))
        {
            return adm; // Senha correta, retorna o administrador
        }

        return null; // Senha incorreta
    }

    public async Task<Administrador?> IncluirAsync(Administrador administrador)
    {
        // Cria o hash da senha usando BCrypt antes de salvar no banco
        administrador.Senha = BCrypt.Net.BCrypt.HashPassword(administrador.Senha);

        _contexto.Administradores.Add(administrador);
        await _contexto.SaveChangesAsync();

        return administrador;
    }

    public async Task<Administrador?> BuscarPorIdAsync(int id)
    {
        return await _contexto.Administradores.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Administrador>> TodosAsync(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();
        int itensPorPagina = 10;

        if (pagina != null && pagina > 0)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
            
        return await query.ToListAsync();
    }
}