using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Options
{
    public class FileDirOption
    {
        public static string DirectoryPath { get; set; } = Directory.GetCurrentDirectory();
    }
}
