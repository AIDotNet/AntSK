
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IDics_Repositories), ServiceLifetime.Scoped)]
    public class Dics_Repositories : Repository<Dics>, IDics_Repositories
    {
    }
}
