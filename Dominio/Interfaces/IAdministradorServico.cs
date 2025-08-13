// Arquivo: Dominio/Interfaces/IAdministradorServico.cs

using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
    // Métodos agora são assíncronos
    Task<Administrador?> LoginAsync(LoginDTO loginDTO);
    Task<Administrador?> IncluirAsync(Administrador administrador);
    Task<Administrador?> BuscarPorIdAsync(int id);
    Task<List<Administrador>> TodosAsync(int? pagina);
}