using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Details;
public static class ParallelUtility {
    
    public static async Task ForEachAsync<TSource>(
        IEnumerable<TSource> source, 
        CancellationToken cancellationToken, 
        Func<TSource, CancellationToken, ValueTask> body
        ) {
#if false
        await System.Threading.Tasks.Parallel.ForEachAsync(
            source,
            cancellationToken,
            async (item, cancellationToken) => await body(item, cancellationToken));
#else
        foreach (var item in source) {
            await body(item, cancellationToken);
        }
#endif
    }
    
}
