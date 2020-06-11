using System;
using Desastres.Web.Data.Entities;
using Desastres.Web.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Desastres.Web.Data;

namespace Desastres.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRoles();
            var manager = await CheckUserAsync("Henry", "Guerrero", "haguer78@gmail.com", "3167111278", "Admin");
            var customer = await CheckUserAsync("Henry", "Guerrero", "haguer78@hotmail.com", "3167111278", "Encargado");
            await CheckManagerAsync(manager);
            await CheckOwnerAsync(customer);
            await CheckEntidadesAsync();          


        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Encargado");
        }

        private async Task<Usuario> CheckUserAsync(string Nombres, string Apellidos, string email, string telefono, string rol)
        {
            var usuario = await _userHelper.GetUserByEmailAsync(email);
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Nombres = Nombres,
                    Apellidos = Apellidos,
                    Email = email,
                    UserName = email,
                    PhoneNumber = telefono     
                };

                await _userHelper.AddUserAsync(usuario, "V2lfjgmlhl_");
                await _userHelper.AddUserToRoleAsync(usuario, rol);
            }
            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(usuario);
            await _userHelper.ConfirmEmailAsync(usuario, token);


            return usuario;
        }
        private async Task CheckManagerAsync(Usuario user)
        {
            if (!_dataContext.Administradores.Any())
            {
                _dataContext.Administradores.Add(new Administrador { Usuarios = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckOwnerAsync(Usuario user)
        {
            if (!_dataContext.Encargados.Any())
            {
                _dataContext.Encargados.Add(new Encargado { Usuarios = user });
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckEntidadesAsync()
        {
            if (!_dataContext.Entidades.Any())
            {
                _dataContext.Entidades.Add(new Entidad { NombreEntidad = "Bomberos" });
                _dataContext.Entidades.Add(new Entidad { NombreEntidad = "Cruz Roja" });
                
                await _dataContext.SaveChangesAsync();
            }
        }
      
    }
}
