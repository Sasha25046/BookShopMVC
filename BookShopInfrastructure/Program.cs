using BookShopInfrastructure;
using BookShopInfrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ������� �������� ������ � ��������� �����������.
builder.Services.AddControllersWithViews();

// ������� �������� ��.
builder.Services.AddDbContext<BookShopContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

// ������� ProductDataPortServiceFactory � DI ���������
builder.Services.AddScoped<ProductDataPortServiceFactory>();

var app = builder.Build();

// ������������ HTTP-���������.
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
