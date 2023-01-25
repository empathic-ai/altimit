using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Altimit
{
    [AType]
    public class AssetMetadata
    {
        [AProperty]
        public string TypeName;
        [AProperty]
        public string Extension;

        public AssetMetadata()
        {

        }
        public AssetMetadata(string typeName, string extension)
        {
            TypeName = typeName;
            Extension = extension;
        }
    }
}
