using Microsoft.EntityFrameworkCore;
using PulsarECommerceData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(config =>
{

    var provider = builder.Configuration.GetValue<string>("DatabaseProvider");
    switch (provider)
    {
        case "PostgreSQL":
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            config.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"),
                p => p.MigrationsAssembly("MigrationPostgreSQL")
                );
            break;
        case "MySql":
            config.UseMySql(
                builder.Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")),
                p => p.MigrationsAssembly("MigrationMySQL")
                );
            break;
        case "SqlServer":
        default:
            config.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
                p => p.MigrationsAssembly("MigrationSqlServer")
                );
            break;
    }
});

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

app.UseCors(config => config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
