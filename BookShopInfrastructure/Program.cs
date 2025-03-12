using BookShopInfrastructure;
using BookShopInfrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Додайте необхідні сервіси в контейнер залежностей.
builder.Services.AddControllersWithViews();

// Додайте контекст БД.
builder.Services.AddDbContext<BookShopContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// Додайте ProductDataPortServiceFactory в DI контейнер
builder.Services.AddScoped<ProductDataPortServiceFactory>();

var app = builder.Build();

// Налаштування HTTP-пайплайна.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
