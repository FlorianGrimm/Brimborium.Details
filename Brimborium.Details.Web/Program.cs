using Brimborium.Details.Cfg;
using Brimborium.Details.Service;

namespace Brimborium.Details;

public class Program {
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

#if DEBUG
        builder.Configuration.AddUserSecrets("Brimborium.Details");
#endif

        builder.Services.AddOptions<AppSettings>().Configure(
            (appSettings) => {
                builder.Configuration.Bind(appSettings);
                //appSettings.Configure(builder.Configuration);
                AppSettings.ConfigureAppSettings(builder.Configuration, builder.Configuration, appSettings);
            });
        builder.Services.AddServicesWithRegistrator(
            (a) => {
                a.FromDependencyContext(
                    Microsoft.Extensions.DependencyModel.DependencyContext.Default ?? throw new Exception("DependencyContext.Default is null"),
                    (assName) => (assName.Name is string name) && name.StartsWith("Brimborium.")
                    )
                    .AddClasses()
                    .UsingAttributes();
            });

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddHostedService<DetailsHostedService>();
        // builder.Services.AddSpaStaticFiles(configuration => {
        //     configuration.RootPath = "wwwroot";
        // });
        // builder.UseSpaStaticFiles();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        foreach (var c in app.Services.GetServices<IMinimalAPIController>()) {
            c.MapMinimalAPIController(app);
        }

        var distFolder = @"C:\github.com\FlorianGrimm\Brimborium.Details\Brimborium.Details.WebClient\dist\brimborium.details.web-client";
        // app.UseStaticFiles(new StaticFileOptions {
        //     // Requires the following import:
        //     // using System.IO;
        //     // using Microsoft.Extensions.FileProviders;
        //     FileProvider = new PhysicalFileProvider(
        //             distFolder
        //             //Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")
        //             ),
        //     RequestPath = "/app",

        //     // OnPrepareResponse = ctx =>
        //     // {
        //     //     ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
        //     // }
        // });
        app.Map("/app", (appApp) => {
             appApp.UseSpaStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(
                        distFolder
                        //Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")
                        ),
                RequestPath = "/app",

                // OnPrepareResponse = ctx =>
                // {
                //     ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                // }
            });
            appApp.UseSpa(
                (spa) => {
                    //spa.Options.DefaultPage = "/app/index.html";
                    spa.Options.DefaultPage = "/app/index.html";
                    spa.Options.SourcePath = distFolder;
                    //spa.Options.SourcePath = @"/app";
                    // spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions() {
                    //     FileProvider = new PhysicalFileProvider(distFolder),
                    //     RequestPath = "/app"
                    // };
                    // if (app.Environment.IsDevelopment())
                    // {
                    //     spa.UseAngularCliServer("start");
                    // }
                }
            );
        });
        // app.UseSpa(
        //     (spa) => {
        //         spa.Options.DefaultPage = "/app/index.html";
        //         spa.Options.SourcePath = distFolder;
        //         //spa.Options.SourcePath = @"/app";
        //         spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions() {
        //             FileProvider = new PhysicalFileProvider(distFolder),
        //             RequestPath = "/app"
        //         };
        //         // if (app.Environment.IsDevelopment())
        //         // {
        //         //     spa.UseAngularCliServer("start");
        //         // }
        //     }
        // );

        app.MapRazorPages();


        var ctsMain = new CancellationTokenSource();
        System.Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
            e.Cancel = true;
            ctsMain.Cancel();

            IHostApplicationLifetime applicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.StopApplication();
        };

        await app.RunAsync(ctsMain.Token);
    }
}
