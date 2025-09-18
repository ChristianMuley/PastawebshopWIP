using Microsoft.EntityFrameworkCore;
using Pastashop.Data;
using Pastashop.Models;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IOrderNummerGenerator, OrderNummerGenerator>();
builder.Services.AddDbContext<PastashopBestellingenContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PastashopBestellingen"))
);

// Add services to the container.
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseStaticFiles();
app.UseSession();
app.Run();
