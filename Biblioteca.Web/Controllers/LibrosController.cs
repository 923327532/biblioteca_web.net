using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Web.Controllers;

public class LibrosController : Controller
{
    private readonly ILibroRepositorio _libroRepositorio;

    public LibrosController(ILibroRepositorio libroRepositorio)
    {
        _libroRepositorio = libroRepositorio;
    }

    public async Task<IActionResult> Index(string? titulo)
    {
        ViewData["Title"] = "Libros";
        ViewData["TituloBusqueda"] = titulo;
        var libros = await _libroRepositorio.ListarAsync(titulo);
        return View(libros);
    }

    public async Task<IActionResult> Details(int id)
    {
        ViewData["Title"] = "Detalle de libro";
        var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
        return libro is null ? NotFound() : View(libro);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Nuevo libro";
        await CargarAutoresAsync();
        return View(new Libro());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Libro libro)
    {
        ViewData["Title"] = "Nuevo libro";
        if (!ModelState.IsValid)
        {
            await CargarAutoresAsync(libro.AutorId);
            return View(libro);
        }

        await _libroRepositorio.CrearAsync(libro);
        TempData["Mensaje"] = "Libro registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Editar libro";
        var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
        if (libro is null)
        {
            return NotFound();
        }

        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Libro libro)
    {
        ViewData["Title"] = "Editar libro";
        if (id != libro.LibroId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await CargarAutoresAsync(libro.AutorId);
            return View(libro);
        }

        await _libroRepositorio.ActualizarAsync(libro);
        TempData["Mensaje"] = "Libro actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        ViewData["Title"] = "Eliminar libro";
        var libro = await _libroRepositorio.ObtenerPorIdAsync(id);
        return libro is null ? NotFound() : View(libro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _libroRepositorio.EliminarAsync(id);
        TempData["Mensaje"] = "Libro eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarAutoresAsync(int? autorId = null)
    {
        var autores = await _libroRepositorio.ListarAutoresAsync();
        ViewBag.Autores = new SelectList(autores, "AutorId", "Nombre", autorId);
    }
}
