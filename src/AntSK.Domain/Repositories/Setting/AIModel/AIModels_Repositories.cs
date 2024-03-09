
using AntSK.Domain.Common.DependencyInjection;
using AntSK.Domain.Repositories.Base;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IAIModels_Repositories), ServiceLifetime.Scoped)]
    public class AIModels_Repositories : Repository<AIModels>, IAIModels_Repositories
    {
    }
}
