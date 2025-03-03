using SimpliHR.Services.Configurations;
using SimpliHR.Services.DBContext;
using Microsoft.EntityFrameworkCore;
using SimpliHR.Core.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddDbContext<SimpliDbContext>(ServiceLifetime.Transient);
//builder.Services.AddDbContext<SimpliDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ConString"));
//});
builder.Services.AddScoped<SimpliDbContext>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
