using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using PulsarECommerceData;
using PulsarECommerceWeb;
using PulsarECommerceWeb.Resources;
using PulsarECommerceWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource));
    });

builder.Services.Configure<RequestLocalizationOptions>(config => {
    var supportedCultures = new[]
    {
        "tr",
        "en",
        "ru"
    };
    config.SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

});

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseLazyLoadingProxies();

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


builder.Services.AddIdentity<AppUser, AppRole>(config =>
{
    config.Password.RequiredLength = builder.Configuration.GetValue<int>("Password:RequiredLength");
    config.Password.RequireLowercase = builder.Configuration.GetValue<bool>("Password:RequireLowercase");
    config.Password.RequireUppercase = builder.Configuration.GetValue<bool>("Password:RequireUppercase");
    config.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Password:RequireNonAlphanumeric");
    config.Password.RequireDigit = builder.Configuration.GetValue<bool>("Password:RequireDigit");
    config.Password.RequiredUniqueChars = builder.Configuration.GetValue<int>("Password:RequiredUniqueChars");

    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    config.Lockout.MaxFailedAccessAttempts = 5;

    config.SignIn.RequireConfirmedEmail = true;

    config.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;

})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddMailKit(optionBuilder =>
{
    optionBuilder.UseMailKit(new MailKitOptions()
    {
        //get options from sercets.json
        Server = builder.Configuration.GetValue<string>("EMail:Server"),
        Port = builder.Configuration.GetValue<int>("EMail:Port"),
        SenderName = builder.Configuration.GetValue<string>("EMail:SenderName"),
        SenderEmail = builder.Configuration.GetValue<string>("EMail:SenderEmail"),

        // can be optional with no authentication 
        Account = builder.Configuration.GetValue<string>("EMail:SenderEmail"),
        Password = builder.Configuration.GetValue<string>("EMail:Password"),
        // enable ssl or tls
        Security = builder.Configuration.GetValue<bool>("EMail:SslEnable")
    });
});

builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UsePulsarECommerceWeb();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
    );


app.MapControllerRoute(
    name: "Brand",
    pattern: "{name}-b-{id}",
    defaults: new { controller = "Home", action = "Brand" }
    );


app.MapControllerRoute(
    name: "Category",
    pattern: "{name}-c-{id}",
    defaults: new { controller = "Home", action = "Category" }
    );

app.MapControllerRoute(
    name: "Product",
    pattern: "{name}-p-{id}",
    defaults: new { controller = "Home", action = "Product" }
    );


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseRequestLocalization();

app.Run();
