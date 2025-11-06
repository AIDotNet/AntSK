using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IRolePermissions_Repositories), ServiceLifetime.Scoped)]
    public class RolePermissions_Repositories : Repository<RolePermissions>, IRolePermissions_Repositories
    {
    }
}
