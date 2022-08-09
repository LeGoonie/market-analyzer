using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Blockbase.Web.Data.ScaffoldConfig
{
    public class CustomDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            // Services to add to the setup for running at scaffold time

            serviceCollection.AddSingleton<IPluralizer, CustomPluralizer>();
            serviceCollection.AddSingleton<ICSharpEntityTypeGenerator, CustomEntityTypeGenerator>();
            //serviceCollection.AddSingleton<ICSharpDbContextGenerator, CustomDbContextGenerator>();
        }
    }
}