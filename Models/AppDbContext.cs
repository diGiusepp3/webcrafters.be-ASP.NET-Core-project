using Microsoft.EntityFrameworkCore;

namespace webcrafters.be_ASP.NET_Core_project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Blog> Blogs { get; set; }  // ✅ Blogs toevoegen

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ContactMessages
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.ToTable("ContactMessages");
                entity.HasKey(e => e.Id);
            });

            // Blogs
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("blog");

                entity.HasKey(e => e.BlogId).HasName("PRIMARY");

                entity.Property(e => e.BlogId).HasColumnName("blog_id");
                entity.Property(e => e.BlogTitel).HasColumnName("blog_titel");
                entity.Property(e => e.BlogCategorie).HasColumnName("blog_categorie");
                entity.Property(e => e.BlogCategorie2).HasColumnName("blog_categorie2");
                entity.Property(e => e.BlogCategorie3).HasColumnName("blog_categorie3");
                entity.Property(e => e.BlogTag).HasColumnName("blog_tag");
                entity.Property(e => e.BlogTag2).HasColumnName("blog_tag2");
                entity.Property(e => e.BlogTag3).HasColumnName("blog_tag3");
                entity.Property(e => e.BlogOndertitel).HasColumnName("blog_ondertitel");
                entity.Property(e => e.BlogMainImage).HasColumnName("blog_main_image");
                entity.Property(e => e.Afbeeldingsbron).HasColumnName("afbeeldingsbron");
                entity.Property(e => e.BlogTekst1).HasColumnName("blog_tekst1");
                entity.Property(e => e.BlogTussentitel1).HasColumnName("blog_tussentitel1");
                entity.Property(e => e.BlogTekst2).HasColumnName("blog_tekst2");
                entity.Property(e => e.BlogTussentitel2).HasColumnName("blog_tussentitel2");
                entity.Property(e => e.BlogTekst3).HasColumnName("blog_tekst3");
                entity.Property(e => e.BlogTussentitel3).HasColumnName("blog_tussentitel3");
                entity.Property(e => e.BlogAutheur).HasColumnName("blog_autheur");
                entity.Property(e => e.BlogToegevoegd).HasColumnName("blog_toegevoegd");
                entity.Property(e => e.Comments).HasColumnName("comments");
            });

        }
    }
}