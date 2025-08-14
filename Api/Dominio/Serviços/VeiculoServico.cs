using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task IncluirAsync(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        await _contexto.SaveChangesAsync();
    }

    public async Task ApagarAsync(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Veiculo?> BuscarPorIdAsync(int id)
    {
        return await _contexto.Veiculos.FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Veiculo>> TodosAsync(int? pagina = 1, string? nome = null)
    {
        var query = _contexto.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => v.Nome.ToLower().Contains(nome.ToLower()));
        }
            
        int itensPorPagina = 10;
        if (pagina != null && pagina > 0)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
            
        return await query.ToListAsync();
    }
}