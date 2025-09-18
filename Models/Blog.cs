using System;

namespace webcrafters.be_ASP.NET_Core_project.Models
{
    public class Blog
    {
        public int BlogId { get; set; }

        public string BlogTitel { get; set; }          // NOT NULL
        public string? BlogCategorie { get; set; }     // NULL mogelijk
        public string? BlogCategorie2 { get; set; }
        public string? BlogCategorie3 { get; set; }

        public string BlogTag { get; set; }            // NOT NULL
        public string? BlogTag2 { get; set; }
        public string? BlogTag3 { get; set; }

        public string BlogOndertitel { get; set; }     // NOT NULL
        public string BlogMainImage { get; set; }      // NOT NULL
        public string Afbeeldingsbron { get; set; }    // NOT NULL

        public string? BlogTekst1 { get; set; }
        public string? BlogTussentitel1 { get; set; }
        public string? BlogTekst2 { get; set; }
        public string? BlogTussentitel2 { get; set; }
        public string? BlogTekst3 { get; set; }
        public string? BlogTussentitel3 { get; set; }

        public string BlogAutheur { get; set; }        // NOT NULL
        public DateTime BlogToegevoegd { get; set; }   // NOT NULL

        public string Comments { get; set; }           // NOT NULL (maar leeg toegestaan)
    }

}