using Microsoft.Extensions.Configuration;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class DataBaseFixture: IDisposable
    {
        public MyShopContext Context { get;private set; }
        public DataBaseFixture()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            var options = new DbContextOptionsBuilder<MyShopContext>()
                .UseSqlServer("Server=srv2\\pupils;Database=Test214859456;Trusted_Connection=True;TrustServerCertificate=True")
                .Options;
            Context = new MyShopContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
