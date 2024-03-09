
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IApis_Repositories), ServiceLifetime.Scoped)]
    public class Apis_Repositories : Repository<Apis>, IApis_Repositories
    {
    }
}
