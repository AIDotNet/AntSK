
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IUsers_Repositories), ServiceLifetime.Scoped)]
    public class Users_Repositories : Repository<Users>, IUsers_Repositories
    {
    }
}
