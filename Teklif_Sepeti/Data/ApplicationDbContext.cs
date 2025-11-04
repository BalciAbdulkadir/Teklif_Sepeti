using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Teklif_Sepeti.Models;


namespace Teklif_Sepeti.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<ProductService> ProductServices { get; set; }

       
    }
}
