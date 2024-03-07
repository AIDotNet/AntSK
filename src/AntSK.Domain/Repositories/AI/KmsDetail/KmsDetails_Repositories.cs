
using AntSK.Domain.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Common.DependencyInjection;

namespace AntSK.Domain.Repositories
{
    [ServiceDescription(typeof(IKmsDetails_Repositories), ServiceLifetime.Scoped)]
    public class KmsDetails_Repositories : Repository<KmsDetails>, IKmsDetails_Repositories
    {
    }
}
