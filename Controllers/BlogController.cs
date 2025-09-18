using Microsoft.AspNetCore.Mvc;
using webcrafters.be_ASP.NET_Core_project.Models;
using System.Linq;

namespace webcrafters.be_ASP.NET_Core_project.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs.ToList();
            return View(blogs);
        }

        public IActionResult Details(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.BlogId == id);

            if (blog == null) return NotFound();

            return View(blog);
        }
    }
}