
using dotnet_webapi_rock_paper_scissors.Src.Api;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(); // swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // swagger
    app.UseSwaggerUI(); // swagger
}

app.UseHttpsRedirection();
app.MapApiRoutes(); // route mapping
app.Run();

