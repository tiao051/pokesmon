using backend.Infrastructure.Configuration;
using backend.Infrastructure.Persistence;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPersistence()
    .AddApplicationServices()
    .AddJwtAuth(builder.Configuration)
    .AddCorsPolicy(builder.Configuration)
    .AddAuthRateLimiting();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongo = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await IndexInitializer.EnsureIndexesAsync(mongo);
    if (app.Environment.IsDevelopment())
        await DbSeeder.SeedAsync(mongo, builder.Configuration);
}

app.UseExceptionHandler();
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();
