using Microsoft.EntityFrameworkCore;
using DomainDrivenDesign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
       public  AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Blog> Blogs { get; set; }
    }


}
