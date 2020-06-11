using Microsoft.AspNetCore.Identity;
using Desastres.Web.Data.Entities;
using System.Threading.Tasks;
using Desastres.Web.Models;

namespace Desastres.Web.Helpers
{
    public interface IUserHelper
    {
        Task<Usuario> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(Usuario usuario, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(Usuario usuario, string roleName);

        Task<bool> IsUserInRoleAsync(Usuario usuario, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<bool> DeleteUserAsync(string email);

        Task<IdentityResult> UpdateUserAsync(Usuario usuario);

        Task<IdentityResult> ChangePasswordAsync(Usuario user, string oldPassword, string newPassword);

        Task<string> GenerateEmailConfirmationTokenAsync(Usuario user);

        Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token);

        Task<Usuario> GetUserByIdAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(Usuario user);

        Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string password);


    }
}

