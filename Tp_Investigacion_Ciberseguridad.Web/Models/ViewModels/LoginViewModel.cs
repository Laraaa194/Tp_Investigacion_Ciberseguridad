using System.ComponentModel.DataAnnotations;

namespace Tp_Investigacion_Ciberseguridad.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        //Falso por defecto para que no se recuerde la sesión
        public bool RememberMe { get; set; }
    }
}
