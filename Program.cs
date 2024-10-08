using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TWAB.Areas.Identity;
using TWAB.Areas.Identity.Data;
using TWAB.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TWABIdentityContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("TWAB"))
);

//identity
builder.Services.AddDefaultIdentity<DBUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TWABIdentityContext>();

//http clinet
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=ListaOgloszen}/{id?}");
    pattern: "{controller=Home}/{action=Admin}/{id?}");

//do obs³ugi widoków m.in. Identity
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

//Seedowanie:
//Tworzenie ról u¿ytkownika (je¿eli nie istniej¹)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Recruiter", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

//tworzenie domyœlnego admina
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<DBUser>>();

    string email = "test123@gmail.com";
    string password = "Test123#";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new DBUser();
        user.UserName = email;
        user.Email = email;
        user.FirstName = "admin";
        user.LastName = "admin";
        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
    }

}

app.Run();