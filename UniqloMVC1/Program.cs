using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniqloMVC1.DataAccess;
using UniqloMVC1.FileExtensions;
using UniqloMVC1.Helpers;
using UniqloMVC1.Models;
using UniqloMVC1.Services.Abstractions;
using WebUniqlo.Services.Interfaces;

namespace UniqloMVC1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<UniqloDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSql"));
              
            });


            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijk1mnopqrstuvwxyz0123456789._";
                opt.Password.RequiredLength =3;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Lockout.MaxFailedAccessAttempts = 10;
                opt.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(15);

            }).AddDefaultTokenProviders().AddEntityFrameworkStores<UniqloDbContext>();
            builder.Services.ConfigureApplicationCookie(x =>
            {
                x.AccessDeniedPath = "/Home/AccessDenied";
                
            });

            ///////////////////////////////////////////////////////////////////

            builder.Services.AddScoped<IEmailService, EmailService>();
            SmtpOptions opt = new();
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

            /////////////////////////////////////////////////////////////////

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseUserSeed();



            app.MapControllerRoute(
                name: "register",
                pattern: "register",
                defaults: new {controller="Account", action="Register"}
                );

            app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
              );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
