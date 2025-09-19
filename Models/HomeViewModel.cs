// FILE: Models/HomeViewModel.cs
namespace webcrafters.be_ASP.NET_Core_project.Models
{
    public class HomeViewModel
    {
        public SiteSettings Settings { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
    }
}