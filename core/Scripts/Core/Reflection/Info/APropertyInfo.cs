using System;
using System.Linq;
using System.Reflection;

namespace Altimit
{
    public class APropertyInfo
    {
        public Type ClassType
        {
            get;
        }
        public Type PropertyType
        {
            get;
        }
        public string Name { get; set; }

        public bool CanSet
        {
            get { return Setter != null; }
        }

        public bool CanGet
        {
            get { return Getter != null; }
        }

        public bool IsReplicated = false;
        public SetByRef Setter { get; set; }
        public Func<object,object> Getter { get; set; }
        public Func<object, object, bool> IsEqualFunc { get; set; }
        public int Index { get; set; }
        public bool IsValue
        {
            get { return !PropertyType.IsInstanceType(); }
        }

        public ObserveType ObserveType { get; set; } = ObserveType.Mutable;
        public FormatType FormatType { get; set; } = FormatType.Default;

        public APropertyInfo(Type classType, Type propertyType, string name, bool isReplicated, Func<object,object> getter = null, SetByRef setter = null,
            ObserveType observeType = ObserveType.Mutable, Func<object, object, bool> isEqualFunc = null, FormatType formatType = FormatType.Default)
        {
            ClassType = classType;
            PropertyType = propertyType;
            IsReplicated = isReplicated;
            Name = name;
            Getter = getter;
            Setter = setter;
            if (Setter == null)
                observeType &= ~ObserveType.Mutable;
            ObserveType = observeType;
            FormatType = formatType;
            IsEqualFunc = isEqualFunc;
        }

        public void Set(ref object target, object value)
        {
            if (!CanSet)
                throw new Exception(string.Format("Failed to set property named {0}. A set method is not implemented.", this));

            if (target == null)
                throw new Exception(string.Format("Attempted to set a value for property named {0}, object is null!", this));

            if (OS.LogPropertyChanges)
                OS.Log(string.Format("Setting {0} to {1}.", this, value.ToNestedString()));
            try
            {
                Setter.Invoke(ref target, value);
            } catch (Exception e)
            {
                throw new Exception(string.Format("Failed to set property named {0} to value {1}.", this, value), e);
            }

            if (OS.LogPropertyChanges)
                OS.Log($"Value for {this} was set to {value.ToNestedString()}.");
        }

        public object Get(object target)
        {
            if (!CanGet)
                throw new ArgumentException(string.Format("Failed to get property named {0}. A get method is not implemented.", this));

            if (OS.LogPropertyChanges)
                OS.Log(string.Format("Getting value for {0}.", this));
            try
            {
                return Getter.Invoke(target);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get property named {this} from instance. Target null: {(target == null)}.", e);
            }
        }

        public override string ToString()
        {
            return ClassType.GetTypeName() + "." + Name.ToString();
        }
    }
}



