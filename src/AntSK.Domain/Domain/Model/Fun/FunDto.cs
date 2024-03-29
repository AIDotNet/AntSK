﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntSK.Domain.Domain.Model.Fun
{
    public class FunDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public FunType FunType { get; set; }
    }

    public enum FunType
    {
        System=1,
        Import=2
    }
}
