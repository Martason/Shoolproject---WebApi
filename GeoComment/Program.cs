using System.Text;
using GeoComment.Models;
using GeoComment.Services;
using GeoComment.Services.Data;
using GeoComment.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

#region Konfigurering

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<DatabaseHandler>();
builder.Services.AddScoped<GeoCommentManager>();
builder.Services.AddScoped<GeoUserService>();
builder.Services.AddScoped<JwtManager>();
builder.Services.AddDbContext<GeoCommentDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("default")));

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<GeoCommentDbContext>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(0, 1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
});

builder.Services.AddSwaggerGen(options =>
{
    //Lite oklart här
    options.AddSecurityDefinition("BearerToken", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
    });

    //Slipper lägga in versioner själv
    options.OperationFilter<AddApiVersionExampleValueOperationFilter>();

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

        //options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // this will validate the 3rd part of the jwt token using the secret that we added in the appsettings and verify we have generated the jwt token
            IssuerSigningKey = new SymmetricSecurityKey(key), // Add the secret key to our Jwt encryption
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true,
        };
    });

#endregion

#region Middleware Pipelining

var app = builder.Build();


// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<DatabaseHandler>();

    if (app.Environment.IsDevelopment())
    {
        //await database.Recreate();
        await database.CreateIfNotExist();
        await database.SeedTestData();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

#region Server start
app.Run();
#endregion