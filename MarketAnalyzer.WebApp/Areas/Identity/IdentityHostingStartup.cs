using System;
using MarketAnalyzer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(MarketAnalyzer.WebApp.Areas.Identity.IdentityHostingStartup))]
namespace MarketAnalyzer.WebApp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MarketAnalyzerDB2Context>(options =>
                    options.UseSqlServer("server=tcp:fiou7xui1l.database.windows.net,1433;user=appadmin;password=qwert#4477;database=MarketAnalyzerDB2"));

                services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<MarketAnalyzerDB2Context>();
            });
        }
    }
}