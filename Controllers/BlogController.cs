// FILE: Controllers/BlogController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webcrafters.be_ASP.NET_Core_project.Models;
using System.Linq;
using System.Threading.Tasks;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Haalt de 3 nieuwste blogposts op
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Blogs
                .OrderByDescending(b => b.BlogToegevoegd) // Juiste propertynaam
                .Take(3)
                .ToListAsync();

            return View(posts);
        }

        // ✅ Haalt 1 blogpost op detailpagina
        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.BlogId == id);

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        // ✅ Optioneel: volledige lijst (alle blogs)
        public async Task<IActionResult> All()
        {
            var posts = await _context.Blogs
                .OrderByDescending(b => b.BlogToegevoegd)
                .ToListAsync();

            return View(posts);
        }
    }
}
