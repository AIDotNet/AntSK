using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Model.Fun
{
    public class FunDto
    {
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

        public FunType FunType { get; set; }

        // 函数参数信息（用于前端展示）
        public List<FunParameterDto> Parameters { get; set; } = new();
    }

    public class FunParameterDto
    {
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    }

    public enum FunType
    {
        System=1,
        Import=2
    }
}
