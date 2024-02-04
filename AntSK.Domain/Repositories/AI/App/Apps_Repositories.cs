
using AntSK.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Common.DependencyInjection;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IApps_Repositories), ServiceLifetime.Scoped)]
    public class Apps_Repositories : Repository<Apps>, IApps_Repositories
    {
    }
}
