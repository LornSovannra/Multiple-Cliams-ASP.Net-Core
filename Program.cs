using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Multi_Claims.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Db Connection
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreatePolicy", 
                    policy => policy.RequireClaim("Create Role")
    );

    options.AddPolicy("EditPolicy",
                    policy => policy.RequireClaim("Edit Role")
                                    .RequireClaim("Create Role")
    );

    options.AddPolicy("DeletePolicy",
                    policy => policy.RequireClaim("Delete Role")
                                    .RequireClaim("Create Role")
    );
});

// Add and Configure Identity System
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationDbContext>();

// Redirect User
builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = new PathString("/Auth/Login");
    config.LogoutPath = new PathString("/Auth/Login");
    config.AccessDeniedPath = new PathString("/Administration/AccessDenied");
});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
