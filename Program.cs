using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManagerApi.Data;

// Create a WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .Build();

// Configure DbContext with connection string from appsettings.json
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS based on environment
if (builder.Environment.IsDevelopment())
{
    // Configure CORS policy for development environment
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                // During development, allow requests from any origin
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });
}
else
{
    // Configure CORS policy for production environment
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                //To use with Production Website
                builder.WithOrigins("http://")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });
}

// Build the application
var app = builder.Build();

// Configure URLs for the application
app.Urls.Add("http://localhost:5252");
app.Urls.Add("https://localhost:7290");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin"); // Apply CORS policy
app.UseAuthorization();
app.MapControllers();
app.Run();
