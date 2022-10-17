using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ratings.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<IdentityContext>(opts => 
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:IdentityConnection"]);
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();
builder.Services.AddDbContext<RatingsContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:RatingsConnection"]);
});
builder.Services.AddScoped<RatingsRepository>();
builder.Services.Configure<MvcOptions>(opts => {
    opts.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((value) => "Proszê wprowadziæ wartoœæ");
    opts.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, type) => $"Wartoœæ '{value}' nie jest poprawna dla pola typu {type}");
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IdentitySeeder.SeedDatabase(
        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(), 
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(), 
        builder.Configuration, 
        scope.ServiceProvider.GetRequiredService<ILogger<IdentitySeeder>>()
        ).Wait();
    RatingsSeeder.SeedDatabase2(scope.ServiceProvider.GetRequiredService<RatingsRepository>(), scope.ServiceProvider.GetRequiredService<ILogger<RatingsSeeder>>()).Wait();
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
    endpoints.MapControllerRoute("default", "/{controller=Ratings}/{action=Index}/{id?}");
});

app.Run();
