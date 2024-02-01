
using Xzy.KnowledgeBase.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xzy.KnowledgeBase.Domain.Common.DependencyInjection;

namespace Xzy.KnowledgeBase.Domain.Repositories
{
    [ServiceDescription(typeof(IKmss_Repositories), ServiceLifetime.Scoped)]
    public class Kmss_Repositories : Repository<Kmss>, IKmss_Repositories
    {
    }
}
