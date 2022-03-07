using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DevExtremeSuperMarket.Models;
using Microsoft.IdentityModel.Protocols;

namespace DevExtremeSuperMarket.Models
{
    public partial class SuperMarketDB : DbContext
    {
        public SuperMarketDB()
        {
        }
        public SuperMarketDB(DbContextOptions<SuperMarketDB> options)
        
            : base(options)
        { 
        }
        
        public virtual DbSet<SampleOrder> ordres { get; set; }
        
    }
}
