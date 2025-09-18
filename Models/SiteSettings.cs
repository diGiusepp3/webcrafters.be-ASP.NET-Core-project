using System.Collections.Generic;
namespace webcrafters.be_ASP.NET_Core_project.Models
{
    public class SiteSettings
    {
        public string SiteName { get; set; } = string.Empty;
        public string LogoPath { get; set; } = string.Empty;
        
        public List<Blog> Blogs { get; set; }
    }
}