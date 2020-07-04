using Desastres.Web.Data;
using Desastres.Web.Data.Entities;
using Desastres.Web.Helpers;
using Desastres.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Soccer.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Desastres.Web.Controllers
{
    public class EmergenciasController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConfiguration _configuration;

        public EmergenciasController(DataContext context,
            ICombosHelper combosHelper,
            IMailHelper mailHelper,
            IImageHelper imageHelper,
            IConfiguration configuration
           )
        {
            _dataContext = context;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
        }
        [Authorize(Roles = "Encargado")]
        // GET: Emergencias
        public IActionResult Index()
        {
            var usuario = HttpContext.User.Identity.Name;

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

                                   where f.EntidadesId == entidadid

                                   select new DataViewModel
                                   {
                                       NombreApellido = e.NombreApellido,
                                       direccion = e.direccion,
                                       FechaLocal = e.FechaLocal,
                                       DesastreId = e.Desastre.Id,
                                       NombreDesastres = d.NombreDesastre,
                                       EmergenciaId = e.Id

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
                FechaIncidente = DateTime.UtcNow,
                fecha = DateTime.Now.ToString("d"),
                hora = DateTime.Now.ToString("hh:mm tt")
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
                    EnviarEmail(view, path);
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

            view.Desastres = _combosHelper.GetComboDesastresTypes();
            return View(view);
        }
        public void EnviarEmail(EmergenciaViewModel view, string? path)
        {
            var listEnvioMensajes = (from d in _dataContext.TipoDesastres
                                     join f in _dataContext.Funciones
                                     on d.Id equals f.TipoDesastresId
                                     join en in _dataContext.Entidades
                                     on f.EntidadesId equals en.Id
                                     join e in _dataContext.Encargados
                                     on en.Id equals e.Usuarios.Entidades.Id

                                     where d.Id == view.DesastresId

                                     select new
                                     {
                                         e.Usuarios.Email,
                                         e.Usuarios.PhoneNumber,
                                         en.email,
                                         en.telefono,
                                         d.NombreDesastre


                                     }).ToList();

            foreach (var item in listEnvioMensajes)
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri("http://www.altiria.net"),

                    Timeout = TimeSpan.FromSeconds(60)
                };


                var postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("cmd", _configuration["Sms:cmd"]),
                    new KeyValuePair<string, string>("domainId", _configuration["Sms:domainId"]),
                    new KeyValuePair<string, string>("login", _configuration["Sms:login"]),
                    new KeyValuePair<string, string>("passwd", _configuration["Sms:passwd"]),
                    new KeyValuePair<string, string>("dest", "57" + item.telefono),
                    new KeyValuePair<string, string>("dest", "57" + item.PhoneNumber),
                    new KeyValuePair<string, string>("msg", "Se reporto " + item.NombreDesastre +
                    " En la dirección: " + view.direccion + " Reportado por : " + view.NombreApellido +
                    " Teléfono:" + view.telefono +
                    " Fecha y Hora:" + view.FechaLocal)
                };

                HttpContent content = new FormUrlEncodedContent(postData);
                string err = "";
                string resp = "";
                try
                {

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/api/http")
                    {
                        Content = content
                    };
                    content.Headers.ContentType.CharSet = "UTF-8";
                    request.Content.Headers.ContentType =
                      new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    var responseString = response.Content.ReadAsStringAsync();
                    resp = responseString.Result;
                }
                catch (Exception e)
                {
                    err = e.Message;
                }
                finally
                {
                    if (err != "")
                    {
                        Console.WriteLine(err);
                    }
                    else
                    {
                        Console.WriteLine(resp);
                    }
                }
                if (path != null)
                {
                    _mailHelper.SendMail(item.Email, "Desastre Reportado", "Se reporto " + item.NombreDesastre +
                    "<br> En la dirección: " + view.direccion + "<br>Reportado: " + view.NombreApellido +
                    "<br> Teléfono:" + view.telefono +
                    "<br> Fecha y Hora:" + view.FechaLocal +
                    "<br>  <img src='https://atenciondesastres.azurewebsites.net" + path.Substring(1) + "' alt='Image' style='width: 200px; height: 200px; max - width: 100 %; height: auto; ' />");
                    _mailHelper.SendMail(item.email, "Desastre Reportado", "Se reporto " + item.NombreDesastre +
                    "<br> En la dirección: " + view.direccion + "<br>Reportado: " + view.NombreApellido +
                    "<br> Teléfono:" + view.telefono +
                    "<br> Fecha y Hora:" + view.FechaLocal +
                     "<br>  <img src='https://atenciondesastres.azurewebsites.net" + path.Substring(1) + "' alt='Image' style='width: 200px; height: 200px; max - width: 100 %; height: auto; ' />");

                }
                else
                {
                    _mailHelper.SendMail(item.Email, "Desastre Reportado", "Se reporto " + item.NombreDesastre +
                     "<br> En la dirección: " + view.direccion + "<br>Reportado: " + view.NombreApellido +
                      "<br> Teléfono:" + view.telefono +
                     "<br> Fecha y Hora:" + view.FechaLocal +
                     "<br> Mo inserto imagen del incidente");

                    _mailHelper.SendMail(item.email, "Desastre Reportado", "Se reporto " + item.NombreDesastre +
                        "<br> En la dirección: " + view.direccion + "<br>Reportado: " + view.NombreApellido +
                         "<br> Teléfono:" + view.telefono +
                        "<br> Fecha y Hora:" + view.FechaLocal +
                        "<br> Mo inserto imagen del incidente");
                }
            }

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
