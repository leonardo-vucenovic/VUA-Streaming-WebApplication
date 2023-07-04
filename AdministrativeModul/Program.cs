using BusinessLayer.DALModels;
using BusinessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<RwaMoviesContext>(options =>
{

    options.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
});
builder.Services.AddAutoMapper(
    typeof(AdministrativeModul.Mapping.AutoMapperProfile),
    typeof(BusinessLayer.Mapping.AutomapperProfile));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUserRepositoryy, UserRepositoryy>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=video}/{action=video}/{id?}");
app.Run();
