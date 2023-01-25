using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    // Custom type info for local types
    [ATypeInfo(typeof(DereferencedObject<>))]
    public class DereferencedObjectInfo : ATypeInfo {

        public DereferencedObjectInfo(Type type) : base(type) {
        }

        public override void InheritMaps()
        {
            base.InheritMaps();

            if (!Type.IsGenericTypeDefinition)
            {
                var innerType = Type.GetGenericArguments()[0];
                foreach (var propertyInfo in innerType.GetATypeInfo().ReplicatedPropertyInfos)
                {
                    AddProperty(TypeExtensions.CreateAPropertyInfo(Type, TypeExtensions.GetPropertyLocalType(propertyInfo.PropertyType), propertyInfo.Name, propertyInfo.Index));
                }

                IsCollection = innerType.IsCollection();
                ElementType = IsCollection ? innerType.GetATypeInfo().ElementType.GetPropertyLocalType() : null;
                IsInstance = false;
            }
        }
       
    }
}

