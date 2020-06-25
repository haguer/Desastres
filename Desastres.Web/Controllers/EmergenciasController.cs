using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Desastres.Web.Data;
using Desastres.Web.Data.Entities;
using Desastres.Web.Helpers;
using Desastres.Web.Models;
using Soccer.Web.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Desastres.Web.Controllers
{
    public class EmergenciasController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        public EmergenciasController(DataContext context,
            ICombosHelper combosHelper,
            IImageHelper imageHelper)
        {
            _dataContext = context;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
        }
        [Authorize(Roles = "Encargado")]
        // GET: Emergencias
        public IActionResult Index()
        {
           var usuario= HttpContext.User.Identity.Name;

            var Encargado = (from e in _dataContext.Encargados
                             where e.Usuarios.Email == usuario
                             select new
                             {
                                 e.Usuarios.Entidades.Id
                             }

                             ).FirstOrDefault();

            int entidadid = Encargado.Id;
            
            var listEmergencias = (from e in _dataContext.Emergencias
                                   join d in _dataContext.TipoDesastres on e.Desastre.Id equals d.Id
                                   join f in _dataContext.Funciones on d.Id equals f.TipoDesastresId
                                   
                                   where f.EntidadesId==entidadid

                                   select new DataViewModel
                                   {
                                       NombreApellido = e.NombreApellido,
                                       direccion = e.direccion,
                                       FechaLocal = e.FechaLocal,
                                       DesastreId = e.Desastre.Id,
                                       NombreDesastres = d.NombreDesastre,
                                       EmergenciaId= e.Id

                                   }).ToList();
            return View(listEmergencias);


        }
        [Authorize(Roles = "Encargado")]
        // GET: Emergencias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _dataContext.Emergencias
                .Include(o => o.Desastre)               
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }
  
        // GET: Emergencias/Create
        public IActionResult Create()
        {
            var model = new EmergenciaViewModel
            {
                Desastres = _combosHelper.GetComboDesastresTypes(),
                FechaIncidente=DateTime.UtcNow,
                fecha = DateTime.Now.ToString("d"),
                hora =DateTime.Now.ToString("hh:mm tt")
        };
            return View(model);
        }

        // POST: Emergencias/Create        
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmergenciaViewModel view)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (view.FotoDesastre != null)
                {
                    path = await _imageHelper.UploadImageAsync(view.FotoDesastre);
                }

                var emergencia = new Emergencia
                {
                    Nombres = view.Nombres,
                    Apellidos = view.Apellidos,
                    telefono = view.telefono,
                    direccion = view.direccion,
                    FechaIncidente = view.FechaIncidente,
                    FotoRuta = path,
                    Desastre = await _dataContext.TipoDesastres.FindAsync(view.DesastresId)
                };
                _dataContext.Add(emergencia);

                try
                {
                    await _dataContext.SaveChangesAsync();
                    return RedirectToAction("Atencion", "Emergencias");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        ModelState.AddModelError(string.Empty, "Ese Desastre ya se encuentre registrado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    }
                }
            }

            return View(view);
        }

        public IActionResult Atencion()
        {
           
            return View();
        }
        [Authorize(Roles = "Encargado")]
        // GET: Emergencias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencia = await _dataContext.Emergencias
                .Include(d => d.Desastre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (emergencia == null)
            {
                return NotFound();
            }

            var view = new EmergenciaViewModel
            {

                Nombres = emergencia.Nombres,
                Apellidos = emergencia.Apellidos,
                telefono = emergencia.telefono,
                direccion = emergencia.direccion,
                FechaIncidente = emergencia.FechaIncidente,
                FotoRuta = emergencia.FotoRuta,
                Id = emergencia.Id,
                Desastres = _combosHelper.GetComboDesastresTypes(),
                DesastresId = emergencia.Desastre.Id

            };

            return View(view);
        }

        // POST: Emergencias/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmergenciaViewModel view)
        {
            if (ModelState.IsValid)
            {
                string path = view.FotoRuta;

                if (view.FotoDesastre != null)
                {
                    path = await _imageHelper.UploadImageAsync(view.FotoDesastre);
                }

                var emergencia = await _dataContext.Emergencias
                   .Include(o => o.Desastre)
                   .FirstOrDefaultAsync(o => o.Id == view.Id);

                emergencia.Nombres = view.Nombres;
                emergencia.Apellidos = view.Apellidos;
                emergencia.telefono = view.telefono;
                emergencia.direccion = view.direccion;
                emergencia.Desastre = await _dataContext.TipoDesastres.FindAsync(view.DesastresId);
                emergencia.FechaIncidente = view.FechaIncidente;
                emergencia.FotoRuta = path;
                _dataContext.Update(emergencia);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(view);


        }

        [Authorize(Roles = "Encargado")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emergencia = await _dataContext.Emergencias               
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (emergencia == null)
            {
                return NotFound();
            }

            _dataContext.Emergencias.Remove(emergencia);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction($"{nameof(Index)}");
        }

    }
}
