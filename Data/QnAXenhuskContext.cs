using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QnAXenhusk.Models;

namespace QnAXenhusk.Data
{
    public class QnAXenhuskContext : DbContext
    {
        public QnAXenhuskContext (DbContextOptions<QnAXenhuskContext> options)
            : base(options)
        {
        }

        public DbSet<QnAXenhusk.Models.QnAs> QnAs { get; set; } = default!;
    }
}
