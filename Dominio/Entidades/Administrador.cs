using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Administrador
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;
    [Required(ErrorMessage = "O campo e-mail é obrigatório")]
    [StringLength(255)]
    public string Email { get; set; } = default!;
    [Required(ErrorMessage = "O campo senha é obrigatório")]
    [StringLength(50)]
    public string Senha { get; set; } = default!;
    [StringLength(10)]
    public string Perfil { get; set; } = default!;
}