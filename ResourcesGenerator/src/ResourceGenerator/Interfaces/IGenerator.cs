using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResourceGenerator.Generator;

namespace ResourceGenerator.Interfaces
{
    internal interface IGenerator
    {
        public enum ClientTypeEnum
        {
            Net,
            Java,
        };

        public void Generate(string jsonResourceFilePath, string outputDirectoryPath, ClientTypeEnum clientType);
    }
}
