namespace Brimborium.Details.Repository;

public interface ISolutionDataRepository {
    SolutionData? GetSolutionData();
}

[Brimborium.Registrator.Singleton]
public class SolutionDataRepository : ISolutionDataRepository {
    private readonly ISolutionInfoFactory _SolutionInfoFactory;
    private SolutionData? _SolutionData;

    public SolutionDataRepository(
        ISolutionInfoFactory solutionInfoFactory
        ) {
        this._SolutionInfoFactory = solutionInfoFactory;
    }

    public SolutionData? GetSolutionData() {
        var result = this._SolutionData;
        if (result is null) {
            result = this._SolutionInfoFactory.GetSolutionInfo();
            this._SolutionData = result;
        }
        return result;
    }
}
