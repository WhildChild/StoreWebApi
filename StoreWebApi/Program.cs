using Microsoft.EntityFrameworkCore;
using StoreWebApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
RegisterServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

IServiceCollection RegisterServices(IServiceCollection services) 
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddDbContext<ApplicationContext>();
    

    services.AddTransient<ProductService>();
    services.AddTransient<CustomerService>();
    services.AddTransient<OrderService>();
    services.AddMemoryCache();

    return services;
}