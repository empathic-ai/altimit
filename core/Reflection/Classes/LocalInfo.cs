using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    // Custom type info for local types
    [ATypeInfo(typeof(Local<>))]
    public class LocalInfo : ATypeInfo {

        public LocalInfo(Type type) : base(type) {
        }

        public override void InheritMaps()
        {
            base.InheritMaps();

            if (!Type.IsGenericTypeDefinition)
            {
                var innerType = Type.GetGenericArguments()[0];
                foreach (var propertyInfo in innerType.GetATypeInfo().ReplicatedPropertyInfos)
                {
                    AddProperty(TypeExtensions.CreateAPropertyInfo(Type, GetPropertyLocalType(propertyInfo.PropertyType), propertyInfo.Name, propertyInfo.Index));
                }

                IsCollection = innerType.IsCollection();
                ElementType = IsCollection ? GetPropertyLocalType(innerType.GetATypeInfo().ElementType) : null;
                IsInstance = false;
            }
        }
        
        static Type GetPropertyLocalType(Type type)
        {
            if (type.IsInstance())
            {
                return typeof(AID);
            }
            else if (type.IsStructure())
            {
                return type.GetLocalType();
            }
            return type;
        }
    }
}

