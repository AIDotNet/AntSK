using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IUserRoles_Repositories), ServiceLifetime.Scoped)]
    public class UserRoles_Repositories : Repository<UserRoles>, IUserRoles_Repositories
    {
    }
}
