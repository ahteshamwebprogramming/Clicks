using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpliHR.Services;
using SimpliHR.Services.Configurations;
using SimpliHR.Services.DBContext;
using System;
using System.Configuration;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

IWebHostEnvironment _env = builder.Environment;
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers().AddControllersAsServices();





builder.Services.AddDbContext<SimpliDbContext>(options => options.UseSqlServer(
  builder.Configuration.GetSection("ConnectionStrings:SimplyConnectionDB").Value,
  sqlServerOptions => sqlServerOptions.CommandTimeout(180))
);

//builder.Services.AddDbContext<SimpliDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:SimplyConnectionDB").Value),
//    option => option.CommandTimeout(60);
//   // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
//});
//string sConStr = builder.Configuration.GetSection("ConnectionStrings:SimplyConnectionDB").Value.ToString();
//builder.Services.AddScoped<IDapperDBSettings,DapperDBSettings()>;
// options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
//builder.Services.AddSingleton<IConfiguration>(Configuration);
builder.Services.AddScoped<DapperDBContext>();
//builder.Services.AddScoped<DapperDBSetting>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());
SqlMapper.AddTypeHandler(new DapperSqlDateOnlyTypeHandler());
//SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
//SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
//For Sessions
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//var supportedCultures = new[] { "en-GB", "de-DE", "fr-FR" };
var supportedCultures = new[] { "en-IN"};
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

builder.Services.AddRequestLocalization(options => {
    options.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
    options.SupportedCultures = localizationOptions.SupportedCultures;
    options.SupportedUICultures = localizationOptions.SupportedUICultures;
});
//For Sessions end
//.AddEntityFrameworkStores<SimpliDbContext>();
//add this: register your db context
//and this: add identity and create the db

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Account/ErrorMessage";
            options.LoginPath = "/Account/Index";
        });



var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//For Sessions
app.UseSession();
//For Sessions end
app.MapRazorPages();

//app.UseExceptionHandler(appBuilder =>
//{
//    app.UseExceptionHandler("/Account/ExceptionMessage");
//});


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "SampleDomainRoute",
       pattern: "{tenant}.simplihrms.com/",
        defaults: new { controller = "Account", action = "Login" });
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");

});

app.Run();
