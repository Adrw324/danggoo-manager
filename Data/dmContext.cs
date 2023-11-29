using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using danggoo_manager.Models;

namespace danggoo_manager.Data
{
    public class dmContext : DbContext
    {
        public dmContext (DbContextOptions<dmContext> options)
            : base(options)
        {
        }

        public DbSet<danggoo_manager.Models.Record> Record { get; set; } = default!;

        public DbSet<danggoo_manager.Models.Game> Game { get; set; } = default!;
    }
}
