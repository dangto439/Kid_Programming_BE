using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;



namespace KidPrograming.Repositories.Base
{
    public class KidProgramingDbContextFactory : IDesignTimeDbContextFactory<KidProgramingDbContext>
    {
        public KidProgramingDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../KidPrograming")) 
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();


            var connectionString = configuration.GetConnectionString("ConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<KidProgramingDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new KidProgramingDbContext(optionsBuilder.Options);
        }
    }
}
