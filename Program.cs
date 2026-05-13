using Microsoft.EntityFrameworkCore;
using kitap.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation.AspNetCore;
using Serilog;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<kitap.Validation.BookCreateValidator>());
builder.Services.AddScoped<kitap.Services.ICategoryService, kitap.Services.CategoryService>();
builder.Services.AddScoped<kitap.Services.IBookService, kitap.Services.BookService>();
builder.Services.AddScoped<kitap.Services.IAuthorService, kitap.Services.AuthorService>();
builder.Services.AddScoped<kitap.Services.IPublisherService, kitap.Services.PublisherService>();
builder.Services.AddScoped<kitap.Services.IBorrowingService, kitap.Services.BorrowingService>();
builder.Services.AddScoped<kitap.Services.IFileService, kitap.Services.FileService>();
builder.Services.AddScoped<kitap.Services.IReviewService, kitap.Services.ReviewService>();
builder.Services.AddScoped<kitap.Core.UnitOfWork.IUnitOfWork, kitap.Core.UnitOfWork.UnitOfWork>();

builder.Services.AddAutoMapper(typeof(kitap.Mapping.MappingProfile));

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddOpenApi();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
      };
});

var app = builder.Build();

app.UseMiddleware<kitap.Middleware.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHangfireDashboard(); // Sadece development modunda dashboard'u açıyoruz
}
app.UseDefaultFiles(); 
app.UseStaticFiles();  
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Arka plan görevini her gün gece yarısı çalışacak şekilde programlıyoruz
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<kitap.Jobs.OverdueBookJob>(
        "check-overdue-books", 
        job => job.CheckOverdueBooksAsync(), 
        Cron.Daily);
}

app.MapControllers();

app.Run();