using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ratings.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<IdentityContext>(opts => 
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]);
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IdentitySeeder.SeedDatabase(
        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(), 
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(), 
        builder.Configuration, 
        scope.ServiceProvider.GetRequiredService<ILogger<IdentitySeeder>>()
    ).Wait();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
