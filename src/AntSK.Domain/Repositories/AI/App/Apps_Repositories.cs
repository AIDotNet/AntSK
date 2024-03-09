
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IApps_Repositories), ServiceLifetime.Scoped)]
    public class Apps_Repositories : Repository<Apps>, IApps_Repositories
    {
    }
}
