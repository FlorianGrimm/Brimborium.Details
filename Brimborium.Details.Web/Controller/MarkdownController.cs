namespace Brimborium.Details.Controller;

[Brimborium.Registrator.Singleton]
public class MarkdownController : IMinimalAPIController {
    public MarkdownController() {
    }

    public void MapMinimalAPIController(WebApplication app) {
        app.MapGet("/hugo", async (context) => {
            var result = await this.HugoGetAsync();
            await context.Response.WriteAsync(result);
        });

        app.MapGet("/hugo/{name}", async (HttpContext context, string name) => {
            //var name = context.Request.RouteValues["name"]?.ToString();
            var result = await this.HugoGetAsync(name);
            await context.Response.WriteAsync(result);
        });
    }

    public Task<string> HugoGetAsync(string? name = default) {
        if (name is null) { name = "Hugo"; }

        return Task.FromResult($"Hello {name}!");
    }
}