using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1.first_learn.models;
using Microsoft.EntityFrameworkCore;

namespace _1.first_learn.database
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {
            
        }

        public DbSet<Stock> Stocks {get; set;}
        public DbSet<Comment> Comments {get; set;}
    }
}