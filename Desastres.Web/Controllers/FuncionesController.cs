using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Desastres.Web.Data;
using Desastres.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Desastres.Web.Helpers;
using Desastres.Web.Models;

namespace Desastres.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FuncionesController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;

        public FuncionesController(DataContext context,
            ICombosHelper combosHelper
            )
        {
            _dataContext = context;
            _combosHelper = combosHelper;
        }

        // GET: Funciones
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Entidades
                .Include(f => f.Funciones).
                OrderBy(e => e.NombreEntidad)
                .ToListAsync());
            
        }
        
        // GET: Funciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {

                ViewBag.Mensaje = TempData["Mensaje"].ToString();

            }
            catch
            {

            }
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _dataContext.Entidades
                .Include(f => f.Funciones)                 
                .ThenInclude(e=> e.TipoDesastres)                
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funcion == null)
            {
                return NotFound();
            }

            return View(funcion);
        }

        // GET: Funciones/Create
        public IActionResult AddDesastre(int? id)
        {
            
            var model = new FuncionViewModel
            {
                EntidadesId=id.Value,
                Desastres = _combosHelper.GetComboDesastresTypes()                
            };
            return View(model);
        }
        
        // POST: Funciones/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDesastre(FuncionViewModel view)
        {
            if (ModelState.IsValid)
            {
                var funcionentidad = new Funcion
                {
                    TipoDesastresId= view.DesastresId,
                    EntidadesId= view.EntidadesId

                };
                _dataContext.Add(funcionentidad);
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        TempData["Mensaje"] = "Ese desastres ya se encuentra registrado.";
                        ViewBag.Mensaje = TempData["Mensaje"];
                    }
                    else
                    {
                        TempData["Mensaje"] = "Ese desastres ya se encuentra registrado.";
                        ViewBag.Mensaje = TempData["Mensaje"];
                    }
                }
                return Redirect("~/Funciones/Details/" + funcionentidad.EntidadesId);
            }
            return RedirectToAction(nameof(Index));
            
        }
        // GET: Funciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _dataContext.Funciones.RemoveRange(_dataContext.Funciones.Where(c => c.EntidadesId == id));

            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> DeleteDesastre(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var desastre = await _dataContext.Funciones           
            .FirstOrDefaultAsync(m => m.TipoDesastresId ==id);
            if (desastre == null)
            {
                return NotFound();
            }

            _dataContext.Funciones.Remove(desastre);
            await _dataContext.SaveChangesAsync();
            return Redirect("~/Funciones/Details/" + desastre.EntidadesId);

        }
    }
}

