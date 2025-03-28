using Microsoft.AspNetCore.Mvc;
using VentaPOS.Data;

namespace VentaPOS.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult NuevoPedido()
        {
            // Verificar la sesión
            var empresaRut = HttpContext.Session.GetString("EmpresaRut");
            if (string.IsNullOrEmpty(empresaRut))
            {
                return RedirectToAction("Login", "Usuarios");
            }

            // Aquí puedes agregar la lógica para cargar datos necesarios
            // como lista de productos, clientes, etc.
            return View();
        }
    }
} 