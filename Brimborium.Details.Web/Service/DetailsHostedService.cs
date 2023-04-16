namespace Brimborium.Details.Service;

public partial class DetailsHostedService
    : BackgroundService {
    private readonly IRootRepositoryFactory _DetailsRepositoryFactory;
    private readonly DetailsRunnerService _DetailsLogicService;
    private readonly ISolutionDataRepository _SolutionDataRepository;
    private readonly ILogger<DetailsHostedService> _Logger;

    public DetailsHostedService(
        IRootRepositoryFactory detailsRepository,
        DetailsRunnerService detailsLogicService,
        ISolutionDataRepository solutionDataRepository,
        ILogger<DetailsHostedService> logger) {
        this._DetailsRepositoryFactory = detailsRepository;
        this._DetailsLogicService = detailsLogicService;
        this._SolutionDataRepository = solutionDataRepository;
        this._Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        var solutionData = this._SolutionDataRepository.GetSolutionData();        
        if (solutionData is null) {
            this._Logger.LogError("SolutionInfo is not valid");
            System.Console.Error.WriteLine("SolutionInfo is not valid");
            return;
        }
        var rootRepository = this._DetailsRepositoryFactory.Get(solutionData);
        await this._DetailsLogicService.ExecuteAsync(rootRepository, stoppingToken);
    }
}
