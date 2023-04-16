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
        var fileExtensionContentTypeProvider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        app.MapGet("/app", async (HttpContext httpContext) => {
            var filePathIndex = Path.Combine(distFolder, "index.html");

            if (File.Exists(filePathIndex)) {
                httpContext.Response.ContentType = "text/html";
                await httpContext.Response.SendFileAsync(filePathIndex);
            } else {
                httpContext.Response.StatusCode = 404;
            }
        });
        app.MapGet("/app/{*path}", async (string path, HttpContext httpContext) => {
            var filePath = Path.Combine(distFolder, path);
            if (File.Exists(filePath)) {
                if (fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType)){
                    httpContext.Response.ContentType = contentType;                
                }
                await httpContext.Response.SendFileAsync(filePath);
            } else {
                var filePathIndex = Path.Combine(distFolder, "index.html");
                if (File.Exists(filePath)) {
                    httpContext.Response.ContentType = "text/html";
                    await httpContext.Response.SendFileAsync(filePathIndex);
                } else {
                    httpContext.Response.StatusCode = 404;
                }
            }
        });

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
