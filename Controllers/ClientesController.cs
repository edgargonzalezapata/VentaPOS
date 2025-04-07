using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentaPOS.Data;
using VentaPOS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace VentaPOS.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly VentaPosContext _context;

        public ClientesController(VentaPosContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return RedirectToAction("Login", "Usuarios");
                }

                var clientes = await _context.Clientes
                    .Where(c => c.EmpresaRut == empresaRut)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();

                return View(clientes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la lista de clientes: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == id && c.EmpresaRut == empresaRut);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [HttpGet]
        [Route("Clientes/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "ID de cliente inválido" });
                }

                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return Json(new { success = false, message = "Sesión no válida" });
                }

                var cliente = await _context.Clientes
                    .Where(c => c.ClienteId == id && c.EmpresaRut == empresaRut)
                    .FirstOrDefaultAsync();

                if (cliente == null)
                {
                    return Json(new { success = false, message = "Cliente no encontrado" });
                }

                return Json(new { 
                    success = true,
                    clienteId = cliente.ClienteId,
                    rutCliente = cliente.RutCliente,
                    nombre = cliente.Nombre,
                    apellidos = cliente.Apellidos,
                    email = cliente.Email,
                    telefono = cliente.Telefono
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cargar el cliente" });
            }
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [Route("Clientes/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] Cliente cliente)
        {
            try
            {
                if (id != cliente.ClienteId)
                {
                    return Json(new { success = false, message = "ID de cliente no coincide" });
                }

                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return Json(new { success = false, message = "Sesión no válida" });
                }

                var clienteExistente = await _context.Clientes
                    .Where(c => c.ClienteId == id && c.EmpresaRut == empresaRut)
                    .FirstOrDefaultAsync();

                if (clienteExistente == null)
                {
                    return Json(new { success = false, message = "Cliente no encontrado" });
                }

                // Verificar si el RUT ya existe para otro cliente
                var rutExistente = await _context.Clientes
                    .AnyAsync(c => c.RutCliente == cliente.RutCliente 
                               && c.ClienteId != cliente.ClienteId 
                               && c.EmpresaRut == empresaRut);

                if (rutExistente)
                {
                    return Json(new { success = false, message = "El RUT ingresado ya está registrado para otro cliente" });
                }

                clienteExistente.RutCliente = cliente.RutCliente;
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Apellidos = cliente.Apellidos;
                clienteExistente.Email = cliente.Email;
                clienteExistente.Telefono = cliente.Telefono;
                clienteExistente.EmpresaRut = empresaRut; // Asegurarse de mantener el EmpresaRut

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cliente actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar el cliente" });
            }
        }

        // POST: Clientes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == id && c.EmpresaRut == empresaRut);

            if (cliente == null)
            {
                return NotFound();
            }

            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "No se puede eliminar el cliente porque tiene ventas asociadas." });
            }
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.ClienteId == id);
        }

        // POST: Clientes/Create
        [HttpPost]
        [Route("Clientes/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Cliente cliente)
        {
            try
            {
                var empresaRut = HttpContext.Session.GetString("EmpresaRut");
                if (string.IsNullOrEmpty(empresaRut))
                {
                    return Json(new { success = false, message = "Sesión no válida" });
                }

                // Verificar si el RUT ya existe
                var rutExistente = await _context.Clientes
                    .AnyAsync(c => c.RutCliente == cliente.RutCliente && c.EmpresaRut == empresaRut);

                if (rutExistente)
                {
                    return Json(new { success = false, message = "El RUT ingresado ya está registrado" });
                }

                cliente.EmpresaRut = empresaRut;
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cliente creado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear el cliente" });
            }
        }
    }
} 