using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JukeboxWeb;
using JukeboxWeb.Services;
using CurrieTechnologies.Razor.SweetAlert2;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var baseAddress = builder.Configuration.GetValue<string>("BaseUrl");
Console.WriteLine($"base address: {baseAddress}");

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(baseAddress ?? "")
    });

builder.Services.AddSweetAlert2();

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped(typeof(JukeboxService));
builder.Services.AddScoped(typeof(JukeboxSocketService));

await builder.Build().RunAsync();