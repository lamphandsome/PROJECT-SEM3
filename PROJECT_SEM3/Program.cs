using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.ChatHub;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddSignalR();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddIdentity<Users, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        }
        )
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.AddRazorPages()
.AddRazorRuntimeCompilation();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();


        app.MapHub<ChatHub>("/chatHub");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}