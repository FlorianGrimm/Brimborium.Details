namespace Brimborium.Details.Service;

public partial class DetailsHostedService
    : BackgroundService {
    private readonly RootRepository _DetailsRepository;
    private readonly DetailsRunnerService _DetailsLogicService;
    private readonly ILogger<DetailsHostedService> _Logger;

    public DetailsHostedService(
        RootRepository detailsRepository,
        DetailsRunnerService detailsLogicService,
        ILogger<DetailsHostedService> logger) {
        this._DetailsRepository = detailsRepository;
        this._DetailsLogicService = detailsLogicService;
        this._Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        var solutionData = this._DetailsRepository.GetSolutionData();
        if (solutionData is null) {
            this._Logger.LogError("SolutionData is not valid.");
            return; 
        }

        await this._DetailsLogicService.ExecuteAsync(this._DetailsRepository, stoppingToken);
    }
}
