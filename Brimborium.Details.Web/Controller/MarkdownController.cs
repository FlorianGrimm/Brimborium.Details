namespace Brimborium.Details.Controller;

[Brimborium.Registrator.Singleton]
public partial class MarkdownController : IMinimalAPIController {
    private readonly ILogger<MarkdownController> _Logger;

    public MarkdownController(
        ILogger<MarkdownController> logger
        ) {
        this._Logger = logger;
    }

    public void MapMinimalAPIController(WebApplication app) {
        //app.MapGroup()
        app.MapGet("/detailsdoc", async (context) => {
            var result = await this.DetailsdocGetIndexAsync();
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(result);
        });

        app.MapGet("/detailsdoc/{name}", async (HttpContext context, string name) => {
            //var name = context.Request.RouteValues["name"]?.ToString();
            var result = await this.DetailsdocGetPageAsync(name);
            await context.Response.WriteAsync(result);
        });
    }

    public Task<string> DetailsdocGetIndexAsync() {

        /*
         var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var result = Markdown.ToHtml("This is a text with some *emphasis*", pipeline);
         */
        return Task.FromResult($"Hello !");
    }

    public Task<string> DetailsdocGetPageAsync(string? name = default) {
        if (name is null) { name = "Hugo"; }

        return Task.FromResult($"Hello {name}!");
    }
}
