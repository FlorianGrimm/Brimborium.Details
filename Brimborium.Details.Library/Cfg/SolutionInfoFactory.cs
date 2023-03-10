using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Details.Cfg;

public interface ISolutionInfoFactory {
    SolutionData? GetSolutionInfo();
}

[Singleton(ServiceType = typeof(ISolutionInfoFactory))]
public class SolutionInfoFactory : ISolutionInfoFactory {
    private readonly IOptions<AppSettings> _Options;
    private readonly IConfiguration _Configuration;

    public SolutionInfoFactory(
        IOptions<AppSettings> options,
        IConfiguration configuration
        ) {
        this._Options = options;
        this._Configuration = configuration;
    }

    public SolutionData? GetSolutionInfo() {
        var appSettings = this._Options.Value;
        var solutionInfo = appSettings.ValidateConfiguration(this._Configuration);
        return solutionInfo;
    }
}