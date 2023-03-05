using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movie_Exercise.Models;
using Movie_Exercise.Models.ViewModels;

namespace Movie_Exercise.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Order> Orders { get; set; }   
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderRow> OrderRows { get; set; }
        public DbSet<Movie_Exercise.Models.ViewModels.CartItemVM> CartItemVM { get; set; }
    }
}