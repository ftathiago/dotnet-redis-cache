using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace PocCache.Cache.Tests.ObjectCacheFacadeTest;

public class ObjectCacheFacadeBaseTest
{
    public Mock<ILogger<Guid>> Logger { get; } = new Mock<ILogger<Guid>>();
    public Mock<IDistributedCache> DistributedCache { get; } = new Mock<IDistributedCache>();

    public IObjectCache<Guid> BuildCacheFacade() =>
        new ObjectCacheFacade<Guid>(
            Logger.Object,
            DistributedCache.Object,
            new CacheMonitor());
}
