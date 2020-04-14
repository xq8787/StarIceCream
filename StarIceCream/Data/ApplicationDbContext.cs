using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarIceCream.Models;

namespace StarIceCream.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CategoryInfo> Categories { get; set; }
        public DbSet<ProductInfo> Products { get; set; }

        public DbSet<EnquiryInfo> Enquiries { get; set; }
    }
}
