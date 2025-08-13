// Arquivo: Dominio/Interfaces/IVeiculoServico.cs

using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico
{
    Task IncluirAsync(Veiculo veiculo);
    Task AtualizarAsync(Veiculo veiculo);
    Task ApagarAsync(Veiculo veiculo);
    Task<Veiculo?> BuscarPorIdAsync(int id);
    Task<List<Veiculo>> TodosAsync(int? pagina = 1, string? nome = null);
}