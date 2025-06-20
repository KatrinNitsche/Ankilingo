using AnkiLingo.Data;
using AnkiLingo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AnkiLingoUploader
{
    public class DataService
    {
        public DataService()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // or specify the path to the API project
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            var services = new ServiceCollection();
            services.AddDbContext<DataContext>(opt =>
                opt.UseSqlServer(connectionString));

            services.AddScoped<ICourseService, CourseService>();

            var serviceProvider = services.BuildServiceProvider();
            var courseService = serviceProvider.GetRequiredService<ICourseService>();
        }
    }
}
