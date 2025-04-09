using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VentaPOS.Data;
using VentaPOS.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Filters;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using RolesModel = VentaPOS.Models.Roles;

namespace VentaPOS.Controllers
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var empresaRut = context.HttpContext.Session.GetString("EmpresaRut");
            var userRole = context.HttpContext.Session.GetString("RolUsuario");
            var userId = context.HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(empresaRut) || string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId))
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    var returnUrl = context.HttpContext.Request.Path.ToString();
                    context.Result = new RedirectToActionResult("Login", "Usuarios", new { returnUrl });
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }

    public class UsuariosController : Controller
    {
        private readonly VentaPosContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(VentaPosContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Usuarios/Registro
        public async Task<IActionResult> Registro(string? plan = null)
        {
            // Cargar la lista de empresas para el dropdown
            ViewData["Empresas"] = new SelectList(await _context.Empresas.ToListAsync(), "Rut", "NombreEmpresa");
            
            var modelo = new RegistroUsuarioViewModel();
            if (!string.IsNullOrEmpty(plan))
            {
                modelo.PlanSeleccionado = plan;
                ViewData["PlanSeleccionado"] = plan;
            }
            
            return View(modelo);
        }

        // POST: Usuarios/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroUsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validar RUT único
                if (await _context.Empresas.AnyAsync(e => e.Rut == model.Rut))
                {
                    ModelState.AddModelError("Rut", "El RUT ya está registrado");
                    ViewData["Empresas"] = new SelectList(await _context.Empresas.ToListAsync(), "Rut", "NombreEmpresa");
                    return View(model);
                }

                // Verificar si el correo ya está registrado
                var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == model.Correo);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado");
                    ViewData["Empresas"] = new SelectList(await _context.Empresas.ToListAsync(), "Rut", "NombreEmpresa");
                    return View(model);
                }

                try
                {
                    // Crear la empresa
                    var empresa = new Empresa
                    {
                        Rut = model.Rut,
                        NombreEmpresa = model.NombreEmpresa,
                        Telefono = model.Telefono,
                        Direccion = model.Direccion,
                        FechaRegistro = DateTime.Now,
                        Activo = true,
                        Contraseña = Encoding.UTF8.GetBytes(model.Password) // Almacenar contraseña sin hash para empresa
                    };

                    _context.Empresas.Add(empresa);
                    await _context.SaveChangesAsync();

                    // Crear un usuario administrador para la empresa
                    var usuarioAdmin = new Usuario
                    {
                        Nombre = model.NombreAdmin,
                        Correo = model.Correo,
                        Telefono = model.Telefono,
                        EmpresaRut = empresa.Rut,
                        FechaRegistro = DateTime.Now,
                        Activo = true,
                        Contraseña = Encoding.UTF8.GetBytes(HashPassword(model.Password)) // Almacenar hash BCrypt
                    };

                    _context.Usuarios.Add(usuarioAdmin);
                    await _context.SaveChangesAsync();

                    // Asignar el rol de Empresa al usuario
                    var rolEmpresa = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Empresa");
                    if (rolEmpresa == null)
                    {
                        // Crear el rol si no existe
                        rolEmpresa = new Roles
                        {
                            Nombre = "Empresa",
                            Descripcion = "Rol para administradores de empresa",
                            EmpresaRut = empresa.Rut
                        };
                        _context.Roles.Add(rolEmpresa);
                        await _context.SaveChangesAsync();
                    }

                    usuarioAdmin.Rols.Add(rolEmpresa);
                    await _context.SaveChangesAsync();

                    // Crear roles predeterminados para la empresa
                    var rolesPrederminados = new[] { "Administrador", "Cajero", "Atendedor" };
                    foreach (var rolNombre in rolesPrederminados)
                    {
                        // Buscar primero si existe el rol para cualquier empresa
                        var rolExistente = await _context.Roles
                            .FirstOrDefaultAsync(r => r.Nombre == rolNombre);
                            
                        if (rolExistente != null)
                        {
                            // Si el rol existe, crear una copia para esta empresa
                            var nuevoRol = new Roles
                            {
                                Nombre = rolNombre,
                                Descripcion = rolExistente.Descripcion ?? $"Rol de {rolNombre}",
                                EmpresaRut = empresa.Rut
                            };
                            _context.Roles.Add(nuevoRol);
                        }
                        else
                        {
                            // Si no existe, crear uno nuevo
                            var nuevoRol = new Roles
                            {
                                Nombre = rolNombre,
                                Descripcion = $"Rol de {rolNombre}",
                                EmpresaRut = empresa.Rut
                            };
                            _context.Roles.Add(nuevoRol);
                        }
                    }
                    await _context.SaveChangesAsync();

                    // Asignar el rol de Administrador al usuario admin
                    var rolAdmin = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Administrador" && r.EmpresaRut == empresa.Rut);
                    if (rolAdmin != null)
                    {
                        usuarioAdmin.Rols.Add(rolAdmin);
                        await _context.SaveChangesAsync();
                    }

                    // Crear una suscripción demo automáticamente
                    var planDemo = await _context.Planes.FirstOrDefaultAsync(p => p.Nombre.ToLower().Contains("demo"));
                    if (planDemo == null)
                    {
                        // Si no existe un plan demo, crear uno
                        planDemo = new Planes
                        {
                            Nombre = "Demo",
                            MaxUsuarios = 2,
                            Precio = 0
                        };
                        _context.Planes.Add(planDemo);
                        await _context.SaveChangesAsync();
                    }

                    // Crear la suscripción demo
                    var suscripcionDemo = new Suscripciones
                    {
                        EmpresaRut = empresa.Rut,
                        PlanID = planDemo.PlanID,
                        FechaInicio = DateTime.Now,
                        FechaFin = DateTime.Now.AddDays(7),
                        FormaPago = "Demo",
                        Pagado = true,
                        Activa = true,
                        MaxUsuarios = planDemo.MaxUsuarios ?? 2
                    };

                    _context.Suscripciones.Add(suscripcionDemo);
                    await _context.SaveChangesAsync();

                    // Asociar el usuario admin a la suscripción demo
                    usuarioAdmin.Suscripciones.Add(suscripcionDemo);
                    await _context.SaveChangesAsync();

                    // Redirigir al login
                    TempData["MensajeExito"] = "¡Registro exitoso! Tu empresa ha sido registrada correctamente. Por favor, inicia sesión.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al registrar: {ex.Message}");
                    ViewData["Empresas"] = new SelectList(await _context.Empresas.ToListAsync(), "Rut", "NombreEmpresa");
                    return View(model);
                }
            }

            ViewData["Empresas"] = new SelectList(await _context.Empresas.ToListAsync(), "Rut", "NombreEmpresa");
            return View(model);
        }

        // GET: Usuarios/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Primero intentar encontrar un usuario
                    var usuario = await _context.Usuarios
                        .Include(u => u.Empresa)
                        .Include(u => u.Rols)
                        .FirstOrDefaultAsync(u => u.Correo == model.Correo);

                    if (usuario != null)
                    {
                        string storedHash = Encoding.UTF8.GetString(usuario.Contraseña);
                        bool isValidPassword = false;

                        try
                        {
                            isValidPassword = BCrypt.Net.BCrypt.Verify(model.Password, storedHash);
                        }
                        catch (BCrypt.Net.SaltParseException)
                        {
                            string storedPassword = Encoding.UTF8.GetString(usuario.Contraseña);
                            if (model.Password == storedPassword)
                            {
                                usuario.Contraseña = Encoding.UTF8.GetBytes(HashPassword(model.Password));
                                await _context.SaveChangesAsync();
                                isValidPassword = true;
                            }
                        }

                        if (isValidPassword)
                        {
                            // Verificar si la empresa tiene una suscripción activa
                            var suscripcionActiva = await _context.Suscripciones
                                .AnyAsync(s => s.EmpresaRut == usuario.EmpresaRut && 
                                             s.Activa == true && 
                                             s.FechaFin > DateTime.Now);

                            if (!suscripcionActiva)
                            {
                                ModelState.AddModelError("", "La empresa no tiene una suscripción activa. Por favor, contacte al administrador.");
                                return View(model);
                            }

                            await SetupUserSession(usuario, model.RememberMe);
                            
                            // Verificar el rol del usuario para la redirección
                            var rolUsuario = usuario.Rols.FirstOrDefault()?.Nombre ?? "Usuario";
                            if (rolUsuario == "Empresa" || rolUsuario == "Administrador")
                            {
                                return RedirectToAction("Bienvenida");
                                }
                                else
                                {
                                // Si es un usuario (cajero, atendedor, etc.), redirigir a la lista de ventas
                                return RedirectToAction("ListaVentas", "Ventas");
                            }
                        }
                    }
                    else
                    {
                        // Si no se encuentra como usuario, buscar como empresa
                        var empresa = await _context.Empresas
                            .FirstOrDefaultAsync(e => e.Rut == model.Correo);

                        if (empresa != null)
                        {
                            string storedPassword = Encoding.UTF8.GetString(empresa.Contraseña);
                            if (model.Password == storedPassword)
                            {
                                // Verificar si la empresa tiene una suscripción activa
                                var suscripcionActiva = await _context.Suscripciones
                                    .AnyAsync(s => s.EmpresaRut == empresa.Rut && 
                                                 s.Activa == true && 
                                                 s.FechaFin > DateTime.Now);

                                await SetupEmpresaSession(empresa, model.RememberMe);

                                if (!suscripcionActiva)
                                {
                                    // Si no tiene suscripción activa, establecer un mensaje pero seguir a bienvenida
                                    TempData["Mensaje"] = "Su empresa no tiene una suscripción activa. Por favor, seleccione un plan para acceder a todas las funcionalidades.";
                                    return RedirectToAction("SolicitarPlan");
                                }

                                return RedirectToAction("Bienvenida");
                            }
                        }
                    }

                    ModelState.AddModelError("", "Correo electrónico o contraseña incorrectos");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el inicio de sesión");
                    ModelState.AddModelError("", "Error al procesar el inicio de sesión");
                }
            }
            
            return View(model);
        }

        private async Task SetupUserSession(Usuario usuario, bool rememberMe)
        {
            HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
            HttpContext.Session.SetString("EmpresaRut", usuario.EmpresaRut);
            HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
            HttpContext.Session.SetString("RolUsuario", usuario.Rols.FirstOrDefault()?.Nombre ?? "Usuario");
            HttpContext.Session.SetString("UserName", usuario.Nombre);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim("EmpresaRut", usuario.EmpresaRut),
                new Claim("NombreUsuario", usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rols.FirstOrDefault()?.Nombre ?? "Usuario")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        private async Task SetupEmpresaSession(Empresa empresa, bool rememberMe)
        {
            HttpContext.Session.SetString("EmpresaRut", empresa.Rut);
            HttpContext.Session.SetString("NombreUsuario", empresa.NombreEmpresa);
            HttpContext.Session.SetString("RolUsuario", "Empresa");
            HttpContext.Session.SetString("UserName", empresa.NombreEmpresa);
            HttpContext.Session.SetString("UserId", "0");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, empresa.Rut),
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim("EmpresaRut", empresa.Rut),
                new Claim("NombreUsuario", empresa.NombreEmpresa),
                new Claim(ClaimTypes.Role, "Empresa")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                AllowRefresh = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Empresa,Administrador")]
        public async Task<IActionResult> GestionUsuarios(string rut = null)
        {
            try
            {
                // Verificar la sesión primero
                string empresaRut = rut ?? HttpContext.Session.GetString("EmpresaRut");
                string rolUsuario = HttpContext.Session.GetString("RolUsuario");

                // Si no hay sesión, intentar obtener de los claims
                if (string.IsNullOrEmpty(empresaRut))
                {
                    empresaRut = User.Claims.FirstOrDefault(c => c.Type == "EmpresaRut")?.Value;
                    rolUsuario = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                }

                // Verificar si tenemos la información necesaria
                if (string.IsNullOrEmpty(empresaRut) || string.IsNullOrEmpty(rolUsuario))
                {
                    TempData["Error"] = "Sesión expirada. Por favor, inicie sesión nuevamente.";
                    return RedirectToAction("Login", new { returnUrl = "/Usuarios/GestionUsuarios" });
                }

                // Verificar si el rol es válido
                if (rolUsuario != "Empresa" && rolUsuario != "Administrador")
                {
                    TempData["Error"] = "No tiene permisos para acceder a esta sección.";
                    return RedirectToAction("Login");
                }

                // Cargar usuarios con sus roles
            var usuarios = await _context.Usuarios
                .Include(u => u.Rols)
                    .Where(u => u.EmpresaRut == empresaRut)
                    .OrderBy(u => u.Nombre)
                .ToListAsync();

                // Cargar la suscripción activa
                var suscripcionActiva = await _context.Suscripciones
                    .Include(s => s.Plan)
                    .Where(s => s.EmpresaRut == empresaRut && s.Activa)
                    .Select(s => new
                    {
                        s.MaxUsuarios,
                        Plan = s.Plan,
                        UsuariosCount = _context.Usuarios.Count(u => u.EmpresaRut == empresaRut && u.Activo)
                    })
                    .FirstOrDefaultAsync();

                if (suscripcionActiva != null)
                {
                    ViewBag.PuedeCrearUsuarios = suscripcionActiva.UsuariosCount < suscripcionActiva.MaxUsuarios;
            ViewBag.MaxUsuarios = suscripcionActiva.MaxUsuarios;
                    ViewBag.UsuariosActuales = suscripcionActiva.UsuariosCount;
            ViewBag.Plan = suscripcionActiva.Plan.Nombre;
                }
                else
                {
                    ViewBag.PuedeCrearUsuarios = false;
                    ViewBag.MaxUsuarios = 0;
                    ViewBag.UsuariosActuales = usuarios.Count;
                    ViewBag.Plan = "Sin Plan Activo";
                }

                ViewBag.EmpresaRut = empresaRut;
            return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GestionUsuarios");
                TempData["Error"] = "Ocurrió un error al procesar la solicitud.";
                return RedirectToAction("Login", new { returnUrl = "/Usuarios/GestionUsuarios" });
            }
        }

        public async Task<IActionResult> Bienvenida()
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Cargar la suscripción activa con sus relaciones de manera explícita
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                    .Where(s => s.EmpresaRut == empresaRut && s.Activa)
                    .Select(s => new
                    {
                        s.SuscripcionID,
                        s.FechaInicio,
                        s.FechaFin,
                        s.Activa,
                        s.MaxUsuarios,
                        Plan = s.Plan,
                        UsuariosCount = s.Usuarios.Count
                    })
                    .FirstOrDefaultAsync();

                if (suscripcion != null)
                {
                    ViewBag.Suscripcion = new
                    {
                        suscripcion.SuscripcionID,
                        suscripcion.FechaInicio,
                        suscripcion.FechaFin,
                        suscripcion.Activa,
                        suscripcion.MaxUsuarios,
                        suscripcion.Plan,
                        suscripcion.UsuariosCount
                    };
                    ViewBag.UsuariosActuales = suscripcion.UsuariosCount;

                    if (suscripcion.Plan.Nombre.ToLower().Contains("demo"))
                    {
                        var diasRestantes = (suscripcion.FechaFin - DateTime.Now).Days;
                        ViewBag.DemoActiva = true;
                        ViewBag.DiasRestantes = diasRestantes > 0 ? diasRestantes : 0;
                    }
                }

                // Cargar el historial de pagos de manera explícita
                var pagos = await _context.Suscripciones
                    .Include(s => s.Plan)
                    .Where(s => s.EmpresaRut == empresaRut)
                    .OrderByDescending(s => s.FechaInicio)
                    .Select(s => new
                    {
                        FechaPago = s.FechaInicio,
                        Plan = s.Plan.Nombre,
                        s.Pagado,
                        Monto = s.Plan.Precio ?? 0
                    })
                .ToListAsync();

                ViewBag.Pagos = pagos;

                // Cargar usuarios
                var usuarios = await _context.Usuarios
                    .Where(u => u.EmpresaRut == empresaRut)
                    .OrderBy(u => u.Nombre)
                    .ToListAsync();

                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página de bienvenida");
                TempData["Error"] = "Ocurrió un error al cargar la información.";
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }

        public async Task<IActionResult> SolicitarPlan()
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login");
            }

            try
            {
                // Obtener los planes usando la consulta específica
                var planes = await _context.Planes
                    .Select(p => new
                    {
                        p.PlanID,
                        p.Nombre,
                        p.MaxUsuarios,
                        p.Precio
                    })
                    .Where(p => !p.Nombre.ToLower().Contains("demo"))
                    .OrderBy(p => p.PlanID)
                    .ToListAsync();

                // Convertir a lista de Planes
                var planesLista = planes.Select(p => new Planes
                {
                    PlanID = p.PlanID,
                    Nombre = p.Nombre,
                    MaxUsuarios = p.MaxUsuarios,
                    Precio = p.Precio
                }).ToList();

                // Log para debugging
                foreach (var plan in planesLista)
                {
                    _logger.LogInformation($"Plan cargado - ID: {plan.PlanID}, Nombre: {plan.Nombre}, Usuarios: {plan.MaxUsuarios}, Precio: {plan.Precio}");
                }

                ViewBag.Planes = planesLista;
                return View(planesLista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los planes");
                TempData["Error"] = "Ocurrió un error al cargar los planes disponibles.";
                return RedirectToAction("Bienvenida");
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // GET: Usuarios/ObtenerDetallesUsuario
        [HttpGet]
        public async Task<IActionResult> ObtenerDetallesUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rols)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener todos los roles disponibles para la empresa
            var rolesDisponibles = await _context.Roles
                .Where(r => r.EmpresaRut == usuario.EmpresaRut)
                .ToListAsync();

            ViewBag.RolesDisponibles = new SelectList(rolesDisponibles, "RolId", "Nombre");
            return PartialView("_DetallesUsuarioPartial", usuario);
        }

        // GET: Usuarios/ObtenerFormularioCreacion
        [HttpGet]
        public async Task<IActionResult> ObtenerFormularioCreacion(string empresaRut)
        {
            var rolesDisponibles = await _context.Roles
                .Where(r => r.EmpresaRut == empresaRut)
                .ToListAsync();

            ViewBag.RolesDisponibles = new SelectList(rolesDisponibles, "RolId", "Nombre");
            ViewBag.EmpresaRut = empresaRut;

            return PartialView("_CrearUsuarioPartial", new Usuario());
        }

        // POST: Usuarios/CrearUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario([FromBody] Usuario usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Nombre))
                    return Json(new { success = false, message = "El nombre es requerido" });

                if (string.IsNullOrEmpty(usuario.Correo))
                    return Json(new { success = false, message = "El correo es requerido" });

                // Verificar si el correo ya está registrado y activo
            var usuarioExistente = await _context.Usuarios
                    .Include(u => u.Empresa)
                    .FirstOrDefaultAsync(u => u.Correo == usuario.Correo && u.Activo);

            if (usuarioExistente != null)
            {
                    return Json(new { success = false, message = $"El correo electrónico ya está registrado y activo en la empresa {usuarioExistente.Empresa.NombreEmpresa}" });
                }

                // Si existe el correo pero está inactivo, verificar que no sea en la misma empresa
                var usuarioInactivo = await _context.Usuarios
                    .Include(u => u.Empresa)
                    .FirstOrDefaultAsync(u => u.Correo == usuario.Correo && !u.Activo && u.EmpresaRut == usuario.EmpresaRut);
                
                if (usuarioInactivo != null)
                {
                    return Json(new { success = false, message = $"El correo electrónico ya existe como usuario inactivo en tu empresa. Por favor, reactiva el usuario existente en lugar de crear uno nuevo." });
                }

                // Verificar si la empresa existe
                var empresa = await _context.Empresas.FindAsync(usuario.EmpresaRut);
                if (empresa == null)
                    return Json(new { success = false, message = "La empresa no existe" });

                // Verificar límite de usuarios según el plan
                var suscripcionActiva = await _context.Suscripciones
                    .Where(s => s.EmpresaRut == usuario.EmpresaRut && s.Activa)
                    .FirstOrDefaultAsync();

                if (suscripcionActiva == null)
                    return Json(new { success = false, message = "No hay una suscripción activa" });

                var usuariosActivos = await _context.Usuarios
                    .CountAsync(u => u.EmpresaRut == usuario.EmpresaRut && u.Activo);

                if (usuariosActivos >= suscripcionActiva.MaxUsuarios)
                    return Json(new { success = false, message = "Ha alcanzado el límite de usuarios para su plan actual" });

                // Crear el nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Telefono = usuario.Telefono,
                    EmpresaRut = usuario.EmpresaRut,
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    Contraseña = Encoding.UTF8.GetBytes(HashPassword("123456"))
                };

                // Procesar roles
                if (usuario.Rols != null && usuario.Rols.Any())
                {
                    var roleIds = usuario.Rols.Select(r => r.RolId).ToList();
                    var roles = await _context.Roles
                        .Where(r => roleIds.Contains(r.RolId) && r.EmpresaRut == usuario.EmpresaRut)
                        .ToListAsync();

                    nuevoUsuario.Rols = new List<Roles>();
                    foreach (var rol in roles)
                    {
                        nuevoUsuario.Rols.Add(rol);
                    }
                }

                // Agregar el usuario a la base de datos
                _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

                // Asociar el usuario con la suscripción activa
                nuevoUsuario.Suscripciones = new List<Suscripciones> { suscripcionActiva };
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Usuario creado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return Json(new { success = false, message = "Error al crear el usuario: " + ex.Message });
            }
        }

        // POST: Usuarios/ActualizarUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.Nombre))
                    return Json(new { success = false, message = "El nombre es requerido" });

                if (string.IsNullOrEmpty(usuario.Correo))
                    return Json(new { success = false, message = "El correo es requerido" });

                var usuarioExistente = await _context.Usuarios
                    .Include(u => u.Rols)
                    .FirstOrDefaultAsync(u => u.UsuarioId == usuario.UsuarioId);

                if (usuarioExistente == null)
                    return Json(new { success = false, message = "Usuario no encontrado" });

                // Verificar si el correo ya está en uso por otro usuario
                var existeCorreo = await _context.Usuarios
                    .AnyAsync(u => u.Correo == usuario.Correo && u.UsuarioId != usuario.UsuarioId);
                if (existeCorreo)
                    return Json(new { success = false, message = "El correo electrónico ya está en uso por otro usuario" });

                // Actualizar propiedades básicas
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.Telefono = usuario.Telefono;
                usuarioExistente.Activo = usuario.Activo;

                // Actualizar roles si se proporcionaron
                if (usuario.Rols != null && usuario.Rols.Any())
                {
                    // Limpiar roles existentes
                    usuarioExistente.Rols.Clear();

                    // Obtener los roles de la base de datos
                    var roleIds = usuario.Rols.Select(r => r.RolId).ToList();
                    var roles = await _context.Roles
                        .Where(r => roleIds.Contains(r.RolId))
                .ToListAsync();

                    // Asignar nuevos roles
                    foreach (var rol in roles)
                    {
                        usuarioExistente.Rols.Add(rol);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Usuario actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return Json(new { success = false, message = "Error al actualizar el usuario: " + ex.Message });
            }
        }

        // POST: Usuarios/EliminarUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            try
            {
                // Realizar eliminación lógica
                usuario.Activo = false;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Usuario desactivado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desactivar usuario");
                return Json(new { success = false, message = "Error al desactivar el usuario: " + ex.Message });
            }
        }

        // POST: Usuarios/CambiarContraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContraseña(int id, string nuevaContraseña)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            try
            {
                usuario.Contraseña = Encoding.UTF8.GetBytes(HashPassword(nuevaContraseña));
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Contraseña actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar la contraseña: " + ex.Message });
            }
        }

        // POST: Usuarios/SuscribirPlan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuscribirPlan(int planId)
        {
            try
            {
                _logger.LogInformation($"Iniciando suscripción para planId: {planId}");
                
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                    _logger.LogWarning("Sesión expirada al intentar suscribir plan");
                    return Json(new { success = false, message = "Sesión expirada. Por favor, inicie sesión nuevamente." });
                }

                // Obtener el plan seleccionado
                var plan = await _context.Planes.FindAsync(planId);
                if (plan == null)
                {
                    _logger.LogWarning($"Plan no encontrado: {planId}");
                    return Json(new { success = false, message = "Plan no encontrado." });
                }

                // Desactivar suscripciones anteriores
                    var suscripcionesAnteriores = await _context.Suscripciones
                    .Where(s => s.EmpresaRut == empresaRut && s.Activa)
                        .ToListAsync();

                    foreach (var suscripcion in suscripcionesAnteriores)
                    {
                        suscripcion.Activa = false;
                    suscripcion.FechaFin = DateTime.Now;
                    _context.Update(suscripcion);
                }

                // Crear nueva suscripción
                    var nuevaSuscripcion = new Suscripciones
                    {
                        EmpresaRut = empresaRut,
                    PlanID = planId,
                        FechaInicio = DateTime.Now,
                        FechaFin = DateTime.Now.AddMonths(1),
                        FormaPago = "Pendiente",
                        Pagado = false,
                    Activa = false, // Se activará después del pago
                        MaxUsuarios = plan.MaxUsuarios ?? 1
                    };

                    _context.Suscripciones.Add(nuevaSuscripcion);
                    await _context.SaveChangesAsync();

                _logger.LogInformation($"Nueva suscripción creada: {nuevaSuscripcion.SuscripcionID}");

                // Generar URL de pago
                var urlPago = Url.Action("ProcesarPago", "Usuarios", new { suscripcionId = nuevaSuscripcion.SuscripcionID });

                return Json(new { 
                    success = true, 
                    message = "Plan seleccionado correctamente.", 
                    urlPago = urlPago,
                    suscripcionId = nuevaSuscripcion.SuscripcionID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al suscribir plan");
                return Json(new { success = false, message = "Error al procesar la suscripción. Por favor, intente nuevamente." });
            }
        }

        // GET: Usuarios/ProcesarPago
        public async Task<IActionResult> ProcesarPago(int suscripcionId)
        {
            try
            {
                var suscripcion = await _context.Suscripciones
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.SuscripcionID == suscripcionId);

                if (suscripcion == null)
                {
                    TempData["Error"] = "Suscripción no encontrada.";
                    return RedirectToAction("SolicitarPlan");
                }

                ViewBag.DatosBancarios = new
                {
                    Banco = "Banco Estado",
                    TipoCuenta = "Cuenta Vista",
                    NumeroCuenta = "90270832708",
                    Titular = "Soluciones Informáticas Bio Bio",
                    Rut = "77.465.778-9",
                    Email = "contacto@sibb.cl"
                };

                return View(suscripcion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar página de pago");
                TempData["Error"] = "Error al procesar el pago.";
                return RedirectToAction("SolicitarPlan");
            }
        }

        // POST: Usuarios/ConfirmarPago
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPago(int suscripcionId, string numeroComprobante)
        {
            try
            {
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.SuscripcionID == suscripcionId);

                if (suscripcion == null)
                {
                    TempData["Error"] = "Suscripción no encontrada.";
                    return RedirectToAction("SolicitarPlan");
                }

                // Actualizar la suscripción con el comprobante y activarla
                suscripcion.Pagado = true;
                suscripcion.FormaPago = "Transferencia";
                suscripcion.NumeroComprobante = numeroComprobante;
                suscripcion.FechaInicio = DateTime.Now;
                suscripcion.FechaFin = DateTime.Now.AddDays(30);
                suscripcion.Activa = true;

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "¡Pago confirmado! Su suscripción está activa por los próximos 30 días.";
                return RedirectToAction("Bienvenida");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar pago");
                TempData["Error"] = "Error al procesar el pago.";
                return RedirectToAction("SolicitarPlan");
            }
        }

        private string GenerarUrlPago(int suscripcionId, int monto)
        {
            // Redirigir a la página de proceso de pago
            return Url.Action("ProcesarPago", "Usuarios", new { suscripcionId = suscripcionId });
        }
    }
} 
