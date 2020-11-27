using ImageMasking.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageMasking.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        
        }

        public DbSet<ImageModel> Images{get; set;}
        public DbSet<MaskModel> Masks{get; set;}
        public DbSet<PersonModel> Persons{get; set;}

      
    }
}               