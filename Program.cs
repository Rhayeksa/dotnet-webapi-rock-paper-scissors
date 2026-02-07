using dotnet_webapi_rock_paper_scissors.Src.Api.Routes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(); // swagger
// builder.Logging.SetMinimumLevel(LogLevel.Debug);
// builder.Logging.SetMinimumLevel(LogLevel.Information);

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

