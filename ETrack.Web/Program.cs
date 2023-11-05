using Blazored.LocalStorage;
using ETrack.Web.Authentication;
using ETrack.Web.Services;
using ETrack.Web.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        //Address for the api server, to be replaced if the API gets hosted to a permanent domain

       builder.Services.AddBlazoredLocalStorage();
       builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5292") });
       builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
       builder.Services.AddScoped<IUserService, UserService>();
       builder.Services.AddAuthenticationCore();

       var app = builder.Build();

       if (!app.Environment.IsDevelopment())
       {
           app.UseExceptionHandler("/Error");
           app.UseHsts();
       }

       app.UseHttpsRedirection();

       app.UseStaticFiles();
       app.UseAuthorization();
       app.UseAuthentication();

       app.UseRouting();

       app.MapBlazorHub();
       app.MapFallbackToPage("/_Host");

       app.Run();
    }
}