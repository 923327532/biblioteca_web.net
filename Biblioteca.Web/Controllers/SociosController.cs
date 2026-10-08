using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class SociosController : Controller
{
    private readonly ISocioRepositorio _socioRepositorio;

    public SociosController(ISocioRepositorio socioRepositorio)
    {
        _socioRepositorio = socioRepositorio;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Socios";
        var socios = await _socioRepositorio.ListarAsync();
        return View(socios);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nuevo socio";
        return View(new Socio());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Socio socio)
    {
        ViewData["Title"] = "Nuevo socio";
        if (ModelState.IsValid && await _socioRepositorio.ExisteDniAsync(socio.DNI))
        {
            ModelState.AddModelError(nameof(Socio.DNI), "Ya existe un socio registrado con este DNI.");
        }

        if (!ModelState.IsValid)
        {
            return View(socio);
        }

        await _socioRepositorio.CrearAsync(socio);
        TempData["Mensaje"] = "Socio registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
