using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class PrestamosController : Controller
{
    private readonly IPrestamoRepositorio _prestamoRepositorio;

    public PrestamosController(IPrestamoRepositorio prestamoRepositorio)
    {
        _prestamoRepositorio = prestamoRepositorio;
    }

    public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
    {
        ViewData["Title"] = "Reporte de prestamos";
        var fechaFin = hasta?.Date ?? DateTime.Today;
        var fechaInicio = desde?.Date ?? fechaFin.AddMonths(-1);

        if (fechaInicio > fechaFin)
        {
            ModelState.AddModelError(string.Empty, "La fecha desde no puede ser mayor que la fecha hasta.");
            fechaInicio = fechaFin;
        }

        ViewData["Desde"] = fechaInicio.ToString("yyyy-MM-dd");
        ViewData["Hasta"] = fechaFin.ToString("yyyy-MM-dd");

        var reporte = ModelState.IsValid
            ? await _prestamoRepositorio.ReporteAsync(fechaInicio, fechaFin)
            : Enumerable.Empty<PrestamoReporte>();

        return View(reporte);
    }
}
