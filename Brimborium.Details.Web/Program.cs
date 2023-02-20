using Brimborium.Details.Controller;

namespace Brimborium.Details;

public class Program {
    public static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddUserSecrets("Brimborium.Details");
        var appSettings = new AppSettings();
        builder.Configuration.Bind(appSettings);
        appSettings.Configure(builder.Configuration);
        if (!string.IsNullOrEmpty(appSettings.DetailsConfiguration)) {
            builder.Configuration.AddJsonFile(appSettings.DetailsConfiguration, false, true);
        }
        var solutionInfo = appSettings.ValidateConfiguration(builder.Configuration);
        if (solutionInfo is null) { return; }
        builder.Services.AddSingleton(solutionInfo);

        builder.Services.AddServicesWithRegistrator(
            (a) => {
                a.FromDependencyContext(
                    Microsoft.Extensions.DependencyModel.DependencyContext.Default ?? throw new Exception("DependencyContext.Default is null"),
                    (assName) => (assName.Name is string name) && name.StartsWith("Brimborium.")
                    )
                    .AddClasses()
                    .UsingAttributes();
                /*
                a.FromAssembliesOf(typeof(AppSettings))
                    .AddClasses()
                    .UsingAttributes();
                */
            });
        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseAuthorization();

        foreach (var c in app.Services.GetServices<IMinimalAPIController>()) {
            c.MapMinimalAPIController(app);
        }

        app.MapRazorPages();

        await app.RunAsync();
    }
}
