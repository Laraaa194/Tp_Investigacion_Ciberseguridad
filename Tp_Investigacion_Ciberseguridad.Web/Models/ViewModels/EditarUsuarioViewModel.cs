using System.ComponentModel.DataAnnotations;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels
{
    public class EditarUsuarioViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 50 caracteres")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; }
    }
}
