using Microsoft.AspNetCore.Identity;

namespace Tp_Investigacion_Ciberseguridad.Data.Identity
{
    public class IdentityErrorDescriberEsp : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
            => new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"La contraseña debe tener al menos {length} caracteres."
            };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "La contraseña debe tener al menos un carácter especial (no alfanumérico)."
            };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "La contraseña debe tener al menos una letra minúscula (a-z)."
            };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "La contraseña debe tener al menos una letra mayúscula (A-Z)."
            };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "La contraseña debe tener al menos un número (0-9)."
            };

        public override IdentityError DuplicateUserName(string userName)
            => new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"El nombre de usuario '{userName}' ya está en uso."
            };
       
    }
}