// Students ID: 00013836, 00014725, 00014896
using FastFood.DAL.Interface;
using FastFood.DAL.Models;
using FastFood.DAL.Repositories;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;

string? _connStr = config.GetConnectionString("DBMS_FastFood")
    .Replace("|DataDirectory|", builder.Environment.ContentRootPath); ;


// Repository initialization Task 2 and 3
builder.Services.AddScoped<IRepository<Employee>>(
   p =>
   {
       return new EmployeeDapperRepository(_connStr);
   });

builder.Services.AddScoped<IRepository<Ingredients>>(
   p =>
   {
       return new IngredientsDapperRepository(_connStr);
   });

builder.Services.AddScoped<IRepository<Menu>>(
   p =>
   {
       return new MenuDapperRepository(_connStr);
   });

builder.Services.AddScoped<IRepository<Order>>(
   p =>
   {
       return new OrderDapperRepository(_connStr);
   });
builder.Services.AddScoped<IRepository<Menu_Ingredients>>(
   p =>
   {
       return new Menu_Ingredient_DapperRepository(_connStr);
   });



// Filter injection of interface  task 4
builder.Services.AddScoped<IFilter_Employee>(provider =>
    new EmployeeDapperRepository(_connStr));

// Exporting task 6
builder.Services.AddScoped<I_Export_Employee>(provider =>
    new EmployeeDapperRepository(_connStr));

// Importing task 7
builder.Services.AddScoped<I_ImportExport>(provider =>
    new OrderDapperRepository(_connStr));


// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "ingredient",
    pattern: "{controller=Ingredient}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Menu",
    pattern: "{controller=Menu}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Order",
    pattern: "{controller=Order}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Menu_Ingredient",
    pattern: "{controller=Menu_Ingredient}/{action=Index}/{id?}");

app.Run();
