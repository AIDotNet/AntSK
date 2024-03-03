
using AntSK.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Common.DependencyInjection;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IAIModels_Repositories), ServiceLifetime.Scoped)]
    public class AIModels_Repositories : Repository<AIModels>, IAIModels_Repositories
    {
    }
}
