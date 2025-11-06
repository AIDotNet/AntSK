using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IRoles_Repositories), ServiceLifetime.Scoped)]
    public class Roles_Repositories : Repository<Roles>, IRoles_Repositories
    {
    }
}
