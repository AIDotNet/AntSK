using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IPermissions_Repositories), ServiceLifetime.Scoped)]
    public class Permissions_Repositories : Repository<Permissions>, IPermissions_Repositories
    {
    }
}
