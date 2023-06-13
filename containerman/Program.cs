using containerman.Configuration;
using Docker.DotNet;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IDockerClient>(sp =>
{
    var configuration = sp.GetRequiredService<DockerClientConfiguration>();
    return configuration.CreateClient();
});
builder.Services.AddScoped<DockerClientConfiguration>(sp =>
{
    var options = sp.GetRequiredService<IOptions<DockerOptions>>().Value;
    DockerClientConfiguration configuration;
    if (options.Uri is not null)
    {
        configuration = new DockerClientConfiguration(options.Uri);
    }
    else
    {
        configuration = new DockerClientConfiguration();
    }

    return configuration;
});

builder.Services.AddOptions<DockerOptions>()
    .Bind(builder.Configuration.GetSection(DockerOptions.Docker))
    .ValidateDataAnnotations()
    .ValidateOnStart();
    
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
app.UseStatusCodePages();

app.Run();

public partial class Program { }
