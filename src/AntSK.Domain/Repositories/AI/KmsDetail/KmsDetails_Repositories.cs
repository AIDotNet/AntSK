
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IKmsDetails_Repositories), ServiceLifetime.Scoped)]
    public class KmsDetails_Repositories : Repository<KmsDetails>, IKmsDetails_Repositories
    {
    }
}
