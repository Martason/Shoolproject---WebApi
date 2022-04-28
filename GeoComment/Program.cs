using GeoComment.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
});
//skapar JSON och tittar på attributen och din kod och sparar dem som en stor sammling 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v0.1", new OpenApiInfo
    {
        Title = "Versioning",
        Version = "v0.1"
    });
    options.SwaggerDoc("v0.2", new OpenApiInfo
    {
        Title = "Versioning",
        Version = "v0.2"
    });
}); 
builder.Services.AddScoped<DatabaseHandler>();
builder.Services.AddDbContext<GeoCommetDBContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("default")));
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(0, 1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});

var app = builder.Build();


// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<DatabaseHandler>();

    if (app.Environment.IsDevelopment())
    {
        await database.Recreate();
    }
    if (app.Environment.IsProduction())
    {
        await database.CreateIfNotExist();
    }

}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // gör att man får tag i de jason obkj man använt för att beskriva sin web api
    //lägger till en visuell representation av din api. 
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/v0.1/swagger.json", "v0.1");
        options.SwaggerEndpoint($"/swagger/v0.2/swagger.json", "v0.2");
    }); 
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
