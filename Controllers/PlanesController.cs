using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentaPOS.Data;
using VentaPOS.Models;

namespace VentaPOS.Controllers
{
    public class PlanesController : Controller
    {
        private readonly VentaPosContext _context;

        public PlanesController(VentaPosContext context)
        {
            _context = context;
        }

        // GET: Planes
        public async Task<IActionResult> Index()
        {
            try
            {
                // Obtenemos los planes desde la base de datos
                var planes = await _context.Planes.ToListAsync();
                
                // Si no hay planes en la BD, usamos datos predeterminados para la vista
                if (planes == null || !planes.Any())
                {
                    ViewBag.UsandoDatosPredeterminados = true;
                }
                else
                {
                    // Enriquecer los planes con propiedades adicionales para la vista
                    EnriquecerPlanes(planes);
                }
                
                return View(planes);
            }
            catch (Exception ex)
            {
                // Registrar el error y mostrar un mensaje amigable
                Console.WriteLine($"Error al cargar planes: {ex.Message}");
                ViewBag.Error = "Ocurrió un error al cargar los planes. Por favor, intente más tarde.";
                return View(new List<Planes>());
            }
        }
        
        // Método para enriquecer los planes con propiedades adicionales
        private void EnriquecerPlanes(List<Planes> planes)
        {
            // Definir planes predeterminados en código si los necesitamos
            for (int i = 0; i < planes.Count; i++)
            {
                var plan = planes[i];
                
                // Establecer valores para las propiedades adicionales
                var cantidadUsuarios = plan.MaxUsuarios ?? 1;
                var destacado = (i == 1); // Marcar el segundo plan como destacado si hay más de uno
                
                // Establecer precios basados en alguna lógica si no están definidos
                if (!plan.Precio.HasValue)
                {
                    plan.Precio = (i + 1) * 100; // 100, 200, 300...
                }
                
                // Agregar información adicional al ViewData
                ViewData[$"InfoPlan_{plan.PlanID}"] = $"Plan {plan.Nombre} con capacidad para {cantidadUsuarios} usuarios.";
                
                // Agregar características según el nivel del plan
                ViewData[$"Caracteristicas_{plan.PlanID}"] = i switch
                {
                    0 => "Acceso al sistema,Soporte básico,Actualizaciones incluidas",
                    1 => "Acceso al sistema,Soporte avanzado,Actualizaciones incluidas,Funciones premium",
                    _ => "Acceso completo al sistema,Soporte prioritario,Actualizaciones anticipadas,Todas las funciones"
                };
                
                ViewData[$"Destacado_{plan.PlanID}"] = destacado;
            }
        }
    }
} 