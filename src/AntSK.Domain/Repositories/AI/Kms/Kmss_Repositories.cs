
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IKmss_Repositories), ServiceLifetime.Scoped)]
    public class Kmss_Repositories : Repository<Kmss>, IKmss_Repositories
    {
    }
}
