using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.ExcelCreate.Hubs;
using RabbitMQ.ExcelCreate.Models;
using RabbitMQ.ExcelCreate.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });

builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlSever"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{

    opt.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<AppDbContext>();



builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{

    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    await appDbContext.Database.MigrateAsync();


    if (!appDbContext.Users.Any())
    {
        userManager.CreateAsync(new IdentityUser() { UserName = "deneme", Email = "deneme@outlook.com" }, "Password12*").Wait();


        userManager.CreateAsync(new IdentityUser() { UserName = "deneme2", Email = "deneme2@outlook.com" }, "Password12*").Wait();
    }


}




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

app.UseEndpoints(endpoints =>
            {

                endpoints.MapHub<MyHub>("/MyHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

app.Run();
