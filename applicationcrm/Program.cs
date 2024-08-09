using applicationcrm.Data;
using applicationcrm.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Data Source=LAPTOP-8040K7LK\\SQLEXPRESS;Initial Catalog=mahataCRM1;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));

// Register email sender service
// Ajoutez ceci dans la méthode ConfigureServices
builder.Services.AddScoped< IEmailSender,EmailSender>();

// Configure Quartz.NET
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("ServiceExpiryJob");
    q.AddJob<ServiceExpiryJob>(opts => opts.WithIdentity(jobKey));

    // Trigger every minute for testing
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ServiceExpiryJob-trigger")
        .WithCronSchedule("0 0 8 * * ?"));


   // Every 10 seconds during 18:00
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
