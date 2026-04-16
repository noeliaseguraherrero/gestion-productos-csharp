using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Practica1.Datos;
using Practica1.Modelos;

namespace Practica1.Pages.Usuarios
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Usuario> Usuarios { get; set; }

        // Propiedades para crear/editar usuario
        [BindProperty]
        public Usuario NuevoUsuario { get; set; }

        public async Task OnGetAsync()
        {
            Usuarios = await _context.Usuarios.ToListAsync();
        }

        // 1. CREAR USUARIO
        public async Task<IActionResult> OnPostCrearAsync()
        {
            if (string.IsNullOrEmpty(NuevoUsuario.Nombre) || string.IsNullOrEmpty(NuevoUsuario.Contraseña))
            {
                TempData["Mensaje"] = "Nombre y contraseña son obligatorios";
                TempData["Tipo"] = "error";
                return RedirectToPage();
            }

            NuevoUsuario.Activo = true;
            NuevoUsuario.Rol = NuevoUsuario.Rol ?? "Usuario";

            _context.Usuarios.Add(NuevoUsuario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Usuario creado con éxito";
            TempData["Tipo"] = "success";
            return RedirectToPage();
        }

        // 2. ELIMINAR USUARIO
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Ojo: Esto borrará también sus productos si no tienes configurado el borrado en cascada
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Usuario eliminado permanentemente";
                TempData["Tipo"] = "success";
            }
            return RedirectToPage();
        }

        // 3. CAMBIAR ESTADO (Activo/Inactivo)
        public async Task<IActionResult> OnPostCambiarEstadoAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = !usuario.Activo;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        // 4. CAMBIAR CONTRASEÑA / ROL (Editar)
        public async Task<IActionResult> OnPostEditarAsync(int id, string nuevaPass, string nuevoRol)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                if (!string.IsNullOrEmpty(nuevaPass)) usuario.Contraseña = nuevaPass;
                usuario.Rol = nuevoRol;
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Usuario actualizado";
                TempData["Tipo"] = "success";
            }
            return RedirectToPage();
        }
    }
}