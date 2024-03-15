
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IFuns_Repositories), ServiceLifetime.Scoped)]
    public class Funs_Repositories : Repository<Funs>, IFuns_Repositories
    {
    }
}
