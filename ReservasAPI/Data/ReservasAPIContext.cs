using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservasAPI.Models;

namespace ReservasAPI.Data
{
    public class ReservasAPIContext : DbContext
    {
        public ReservasAPIContext (DbContextOptions<ReservasAPIContext> options)
            : base(options)
        {
        }

        public DbSet<ReservasAPI.Models.reservations> reservations { get; set; } = default!;
    }
}
