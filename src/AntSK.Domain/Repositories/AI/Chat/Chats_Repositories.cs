
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IChats_Repositories), ServiceLifetime.Scoped)]
    public class Chats_Repositories : Repository<Chats>, IChats_Repositories
    {
    }
}
