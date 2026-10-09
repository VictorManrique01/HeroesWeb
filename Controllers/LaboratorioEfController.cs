using HeroesWeb.Data;
using HeroesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroesWeb.Controllers;

public class LaboratorioEfController : Controller
{
    private readonly HeroesContext _context;

    public LaboratorioEfController(HeroesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var heroe = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == id);

        if (heroe is null)
        {
            return NotFound();
        }

        return View(heroe);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        string? nombre,
        bool guardar = false)
    {
        var heroe = await _context.Heroes
            .FirstOrDefaultAsync(h => h.Id == id);

        if (heroe is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim().Length > 100)
        {
            ModelState.AddModelError(
                nameof(Heroes.Nombre),
                "Ingrese un nombre de 1 a 100 caracteres.");
            return View(heroe);
        }

        var nombreOriginal = heroe.Nombre;
        heroe.Nombre = nombre.Trim();

        // Se ejecuta explícitamente para observar el estado durante el laboratorio.
        _context.ChangeTracker.DetectChanges();
        var estadoAntes = _context.Entry(heroe).State;

        if (guardar)
        {
            await _context.SaveChangesAsync();

            TempData["Resultado"] =
                $"Antes: {estadoAntes}. Después: {_context.Entry(heroe).State}. " +
                "Se ejecutó SaveChangesAsync; compruebe el dato en SQL Server.";

            return RedirectToAction(nameof(Editar), new { id });
        }

        ViewData["Resultado"] =
            $"Original: {nombreOriginal}. En memoria: {heroe.Nombre}. " +
            $"Estado: {estadoAntes}. No se llamó a SaveChangesAsync.";

        return View(heroe);
    }
}
