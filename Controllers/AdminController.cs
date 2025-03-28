using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VentaPOS.Data;
using VentaPOS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RolesModel = VentaPOS.Models.Roles;

namespace VentaPOS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Empresa,Administrador")]
    public class AdminController : Controller
    {
        private readonly VentaPosContext _context;

        public AdminController(VentaPosContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            // Obtener estadísticas básicas para el dashboard
            ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
            ViewBag.TotalSuscripciones = await _context.Suscripciones.CountAsync();
            ViewBag.SuscripcionesActivas = await _context.Suscripciones.CountAsync(s => s.Activa == true);
            ViewBag.PlanesTotales = await _context.Planes.CountAsync();

            return View();
        }

        // GET: Admin/Suscripciones
        public async Task<IActionResult> Suscripciones()
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            // Obtener todas las suscripciones con los datos relacionados
            var suscripciones = await _context.Suscripciones
                .Include(s => s.Plan)
                .Include(s => s.Empresa)
                .OrderByDescending(s => s.Activa)
                .ThenByDescending(s => s.FechaInicio)
                .ToListAsync();

            return View(suscripciones);
        }

        // GET: Admin/EditarSuscripcion/5
        public async Task<IActionResult> EditarSuscripcion(int? id)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            if (id == null)
            {
                return NotFound();
            }

            // Obtener la suscripción con los datos relacionados
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                .Include(s => s.Empresa)
                .Include(s => s.Usuarios)
                .FirstOrDefaultAsync(s => s.SuscripcionID == id);

            if (suscripcion == null)
            {
                return NotFound();
            }

            // Preparar listas desplegables
            ViewBag.Planes = new SelectList(await _context.Planes.ToListAsync(), "PlanID", "Nombre", suscripcion.PlanID);
            
            return View(suscripcion);
        }

        // POST: Admin/EditarSuscripcion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarSuscripcion(int id, [Bind("SuscripcionID,PlanID,FechaInicio,FechaFin,FormaPago,Pagado,Activa,MaxUsuarios")] Suscripciones suscripcion)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            if (id != suscripcion.SuscripcionID)
            {
                return NotFound();
            }

            // Obtener la suscripción original para mantener campos que no se modifican
            var suscripcionOriginal = await _context.Suscripciones
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SuscripcionID == id);

            if (suscripcionOriginal == null)
            {
                return NotFound();
            }

            // Mantener el UsuarioAdminId y EmpresaRut originales
            suscripcion.UsuarioAdminId = suscripcionOriginal.UsuarioAdminId;
            suscripcion.EmpresaRut = suscripcionOriginal.EmpresaRut;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suscripcion);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Suscripción actualizada correctamente";
                    return RedirectToAction(nameof(Suscripciones));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuscripcionExists(suscripcion.SuscripcionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si hay errores, volver a preparar las listas desplegables
            ViewBag.Planes = new SelectList(await _context.Planes.ToListAsync(), "PlanID", "Nombre", suscripcion.PlanID);
            
            return View(suscripcion);
        }

        // GET: Admin/CambiarPlanUsuario
        public async Task<IActionResult> CambiarPlanUsuario()
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            // Preparar el modelo para la búsqueda de usuarios
            var viewModel = new BuscarUsuarioViewModel();
            
            return View(viewModel);
        }

        // POST: Admin/BuscarUsuarioPlan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarUsuarioPlan(BuscarUsuarioViewModel model)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            if (string.IsNullOrWhiteSpace(model.TerminoBusqueda))
            {
                ModelState.AddModelError("TerminoBusqueda", "Ingrese un término de búsqueda");
                return View("CambiarPlanUsuario", model);
            }

            // Buscar usuarios que coincidan con el término de búsqueda
            var usuariosEncontrados = await _context.Usuarios
                .Where(u => u.Correo.Contains(model.TerminoBusqueda) || 
                       u.Nombre.Contains(model.TerminoBusqueda))
                .ToListAsync();

            model.UsuariosEncontrados = usuariosEncontrados;
            
            return View("CambiarPlanUsuario", model);
        }

        // GET: Admin/GestionarPlanesUsuario/5
        public async Task<IActionResult> GestionarPlanesUsuario(int? id)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            if (id == null)
            {
                return NotFound();
            }

            // Obtener el usuario con su empresa
            var usuario = await _context.Usuarios
                .Include(u => u.Empresa)
                .Include(u => u.Suscripciones)
                .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null || usuario.Empresa == null)
            {
                return NotFound();
            }

            // Obtener todas las suscripciones de la empresa
            var suscripcionesAdmin = await _context.Suscripciones
                .Include(s => s.Plan)
                .Include(s => s.Empresa)
                .Where(s => s.EmpresaRut == usuario.Empresa.Rut)
                .OrderByDescending(s => s.Activa)
                .ThenByDescending(s => s.FechaInicio)
                .ToListAsync();

            ViewBag.SuscripcionesAdmin = suscripcionesAdmin;
            
            // Obtener todas las suscripciones donde el usuario es miembro (pero no de su empresa)
            var suscripcionesMiembro = await _context.Suscripciones
                .Include(s => s.Plan)
                .Include(s => s.Empresa)
                .Where(s => s.Usuarios.Any(u => u.UsuarioId == usuario.UsuarioId) && 
                           s.EmpresaRut != usuario.Empresa.Rut)
                .ToListAsync();

            ViewBag.SuscripcionesMiembro = suscripcionesMiembro;
            
            // Preparar lista de planes disponibles
            ViewBag.Planes = await _context.Planes.ToListAsync();

            return View(usuario);
        }

        // POST: Admin/CambiarPlanAdministrado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPlanAdministrado(int usuarioId, int suscripcionId, int nuevoPlanID)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            // Obtener la suscripción con la empresa
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                .Include(s => s.Empresa)
                .FirstOrDefaultAsync(s => s.SuscripcionID == suscripcionId);

            if (suscripcion == null)
            {
                TempData["Error"] = "La suscripción no existe";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }

            // Obtener el usuario y verificar que pertenezca a la empresa
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId && u.EmpresaRut == suscripcion.EmpresaRut);

            if (usuario == null)
            {
                TempData["Error"] = "El usuario no pertenece a la empresa de esta suscripción";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }

            // Obtener el nuevo plan
            var nuevoPlan = await _context.Planes.FindAsync(nuevoPlanID);
            if (nuevoPlan == null)
            {
                TempData["Error"] = "El plan seleccionado no existe";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }

            try
            {
                // Actualizar el plan de la suscripción
                suscripcion.PlanID = nuevoPlanID;
                
                // Actualizar el máximo de usuarios si el nuevo plan lo define
                if (nuevoPlan.MaxUsuarios.HasValue)
                {
                    suscripcion.MaxUsuarios = nuevoPlan.MaxUsuarios.Value;
                }
                
                _context.Update(suscripcion);
                await _context.SaveChangesAsync();
                
                TempData["Mensaje"] = $"Plan cambiado correctamente a {nuevoPlan.Nombre} para la empresa {suscripcion.Empresa.NombreEmpresa}";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar el plan: {ex.Message}";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }
        }

        // POST: Admin/AsignarPlanUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarPlanUsuario(int usuarioId, int planID)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            // Obtener el usuario y su empresa
            var usuario = await _context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);

            if (usuario == null || usuario.Empresa == null)
            {
                TempData["Error"] = "El usuario no existe o no está asociado a una empresa";
                return RedirectToAction("CambiarPlanUsuario");
            }

            // Obtener el plan
            var plan = await _context.Planes.FindAsync(planID);
            if (plan == null)
            {
                TempData["Error"] = "El plan seleccionado no existe";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }

            try
            {
                // Desactivar suscripciones anteriores de la empresa
                var suscripcionesAnteriores = await _context.Suscripciones
                    .Where(s => s.EmpresaRut == usuario.Empresa.Rut && s.Activa == true)
                    .ToListAsync();

                foreach (var suscripcionAnterior in suscripcionesAnteriores)
                {
                    suscripcionAnterior.Activa = false;
                }

                // Crear una nueva suscripción
                var suscripcion = new Suscripciones
                {
                    EmpresaRut = usuario.Empresa.Rut,
                    PlanID = planID,
                    FechaInicio = DateTime.Now,
                    FechaFin = DateTime.Now.AddMonths(1),
                    Activa = true,
                    MaxUsuarios = plan.MaxUsuarios ?? 1
                };

                _context.Suscripciones.Add(suscripcion);
                await _context.SaveChangesAsync();

                // Asociar usuarios existentes de la empresa a la nueva suscripción
                var usuariosEmpresa = await _context.Usuarios
                    .Where(u => u.EmpresaRut == usuario.Empresa.Rut)
                    .Take(suscripcion.MaxUsuarios)
                    .ToListAsync();

                foreach (var usuarioEmpresa in usuariosEmpresa)
                {
                    usuarioEmpresa.Suscripciones.Add(suscripcion);
                }

                await _context.SaveChangesAsync();
                
                TempData["Mensaje"] = $"Se ha asignado el plan {plan.Nombre} a la empresa {usuario.Empresa.NombreEmpresa}";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al asignar el plan: {ex.Message}";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }
        }

        // POST: Admin/DesactivarSuscripcion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarSuscripcion(int usuarioId, int suscripcionId)
        {
            // Verificar que el usuario sea administrador
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }

            // Obtener la suscripción
            var suscripcion = await _context.Suscripciones.FindAsync(suscripcionId);
            if (suscripcion == null)
            {
                TempData["Error"] = "La suscripción no existe";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }

            try
            {
                // Desactivar la suscripción
                suscripcion.Activa = false;
                _context.Update(suscripcion);
                await _context.SaveChangesAsync();
                
                TempData["Mensaje"] = "Suscripción desactivada correctamente";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al desactivar la suscripción: {ex.Message}";
                return RedirectToAction("GestionarPlanesUsuario", new { id = usuarioId });
            }
        }

        // GET: Admin/AccesoDenegado
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // Método auxiliar para verificar si el usuario actual es administrador
        private bool EsAdministrador()
        {
            // Cambiar a esta implementación
            return User.Identity.IsAuthenticated && 
                  (User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Empresa") || 
                   User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Administrador"));
        }

        private bool SuscripcionExists(int id)
        {
            return _context.Suscripciones.Any(e => e.SuscripcionID == id);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Empresa,Administrador")]
        public async Task<IActionResult> GestionUsuarios()
        {
            if (!EsAdministrador())
            {
                return RedirectToAction("AccesoDenegado");
            }
            // Resto del código...
            
            // Añadir un valor de retorno por defecto
            return View();
        }
    }
} 