﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class CoreComponentOutputFile : BinaryOutputFile
    {
        public CoreComponentOutputFile(BuildOutputFileDefinition fileDefinition) 
            : base(fileDefinition)
        {
        }
    }
}
