using NDO;
using NDOInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NETDataObjects.NDOVSPackage
{
    class ProviderLoader
    {

        public static void AddProviderDlls()
        {
            NDOProviderFactory.Instance.AddProviderPlugIns(Path.GetDirectoryName(typeof(ProviderLoader).Assembly.Location));
        }
    }
}
