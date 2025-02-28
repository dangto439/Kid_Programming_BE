using KidPrograming.Contract.Repositories.Interfaces;
using KidPrograming.Contract.Services.Interfaces;
using KidPrograming.Repositories.Repositories;
using KidPrograming.Services.Infrastructure;
using KidPrograming.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KidPrograming.Services
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices(configuration);
            services.AddRepository();
            services.AddAutoMapper();
        }
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<Authentication>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IChapterService, ChapterService>();  
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ILessonService, LessonService>();
        }
        private static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
