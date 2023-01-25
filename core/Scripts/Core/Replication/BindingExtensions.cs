using Altimit.UI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Altimit
{
    public static class BindingExtensions
    {
        public static IAList<TElement> BindList<TElement>(this IAList<TElement> list, Action<TElement> onAdded, bool isInit = true)
        {
            new ListBinder(list, (list, index, element) => onAdded((TElement)element), null, isInit);
            return list;
        }

        public static IAList<TElement> BindList<TElement>(this IAList<TElement> list, Action<TElement> onAdded, Action<TElement> onRemoved, bool isInit = true)
        {

            return list;
        }

        public static IAList<TElement> BindList<TElement>(this IAList<TElement> list, Action<int, TElement> onAdded, bool isInit = true)
        {
            new ListBinder(list, (list, index, element) => onAdded(index, (TElement)element), null, isInit);
            return list;
        }

        public static IAList<TElement> BindList<TElement>(this IAList<TElement> list, Action<int, TElement> onAdded, Action<int, TElement> onRemoved, bool isInit = true)
        {

            return list;
        }

        public static T BindProperty<T, P>(this T instance, Expression<Func<T, P>> propExp, Action<object, string, object> action, bool isInit = true)
        {
            instance.BindProperty(propExp.Body, action, isInit);
            return instance;
        }

        public static TSource BindProperty<TSource, TProperty, TTarget>(this TSource instance, Expression<Func<TSource, TProperty>> propExp, TTarget targetInstance, Expression<Func<TTarget, TProperty>> targetPropExp)
        {
            return instance.BindProperty(propExp.Body, (x,y,z)=>
            {
                targetInstance.SetProperty(targetPropExp.Body, instance.GetProperty(propExp.Body));
            });
        }

        // TODO: Create static dictionary of property binders associated with instances and empty dictionary entries when instances are destroyed
        public static T BindProperty<T>(this T instance, Expression propExp, Action<object, string, object> action, bool isInit = true)
        {
            if (instance == null)
                OS.Logger.LogError("Binding target is null!");

            var propertyObserver = new PropertyBinder(instance, propExp, action, isInit);
            return instance;
        }

        public static void UnbindProperty(this object target, string propertyName, Action<object, string, object> action)
        {
            target.GetObserver().UnbindProperty(propertyName, action);
        }
        public static void UnbindProperty<T, P>(this T target, Expression<Func<T, P>> propExp, Action<object, string, object> action)
        {
            target.GetObserver().UnbindProperty(GetPropertyName(propExp), action);
        }

        public static void BindProperty(this object instance, string propertyName, Action<object, string, object> action)
        {
            instance.GetObserver().BindProperty(propertyName, action);
        }

        public static T BindProperty<T, P>(this T target, Expression<Func<T, P>> propExp, Action<P> action, bool isInit = true)
        {
            target.BindProperty(propExp, (x,y,z)=>action((P)x.GetProperty(y)), isInit);
            return target;
        }

        public static bool TryGetPropertyInfo(this Type type, string propertyName, out APropertyInfo propertyInfo)
        {
            return type.GetATypeInfo().TryGetPropertyInfo(propertyName, out propertyInfo);
        }
        public static APropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            return type.GetATypeInfo().GetPropertyInfo(propertyName);
        }
        public static APropertyInfo GetPropertyInfo<T>(this Type type, Expression<Func<T, object>> propExp)
        {
            return type.GetPropertyInfo(GetPropertyPath(propExp));
        }
        public static APropertyInfo GetPropertyInfo<T>(this T target, Expression<Func<T, object>> propExp)
        {
            return target.GetPropertyInfo(GetPropertyPath(propExp));
        }
        public static APropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource target, Expression<Func<TSource, TProperty>> propExp)
        {
            return target.GetPropertyInfo(GetPropertyPath(propExp));
        }
        public static APropertyInfo GetPropertyInfo<TSource, TProperty>(this Type type, Expression<Func<TSource, TProperty>> propExp)
        {
            return type.GetPropertyInfo(GetPropertyPath(propExp.Body));
        }

        public static APropertyInfo GetPropertyInfo<T>(this T target, Expression propExp)
        {
            return target.GetPropertyInfo(GetPropertyPath(propExp));
        }

        public static APropertyInfo GetPropertyInfo<T>(this T instance, List<string> propertyPath)
        {
            return instance.GetType().GetPropertyInfo(propertyPath);
        }

        public static APropertyInfo GetPropertyInfo(this Type type, List<string> propertyPath)
        {
            ATypeInfo typeInfo = type.GetATypeInfo();
            for (int i = 0; i < propertyPath.Count - 1; i++)
            {
                var propertyName = propertyPath[i];
                typeInfo = typeInfo.GetPropertyInfo(propertyName).PropertyType.GetATypeInfo();
            }
            return typeInfo.GetPropertyInfo(propertyPath[propertyPath.Count - 1]);
        }

        public static object GetLastInstance<T>(this T instance, Expression propExp)
        {
            return instance.GetLastInstance(GetPropertyPath(propExp));
        }
        
        public static object GetLastInstance<T>(this T instance, List<string> propertyPath)
        {
            object pathInstance = instance;
            for (int i = 0; i < propertyPath.Count - 1; i++)
            {
                var propertyName = propertyPath[i];
                pathInstance = pathInstance.GetProperty(propertyName);
                if (pathInstance == null)
                    return null;
            }
            return pathInstance;
        }

        public static APropertyInfo GetPropertyInfo(this object obj, string propertyName)
        {
            return obj.GetATypeInfo().GetPropertyInfo(propertyName);
        }

        public static T GetProperty<T>(this object obj, string propertyName)
        {
            return (T)obj.GetPropertyInfo(propertyName).Get(obj);
        }

        public static object GetProperty(this object obj, string propertyName, bool isFilteredByDB = false)
        {
            if (isFilteredByDB && obj.IsReplicatedInstance())
                return obj.GetDB().GetInstanceProperty(obj, propertyName);

            return obj.GetPropertyInfo(propertyName).Get(obj);
        }

        public static P GetProperty<T, P>(this T target, Expression<Func<T, P>> propExp)
        {
            return (P)target.GetProperty(propExp.Body);
        }
        
        public static object GetProperty<T>(this T target, Expression propExp)
        {
            return target.GetProperty(GetPropertyPath(propExp));
        }

        public static object GetProperty(this object instance, List<string> propertyPath)
        {
            object pathInstance = instance;
            for (int i = 0; i < propertyPath.Count - 1; i++)
            {
                var propertyName = propertyPath[i];
                pathInstance = pathInstance.GetProperty(propertyName);
                if (pathInstance == null)
                    return null;
            }
            return pathInstance.GetProperty(propertyPath[propertyPath.Count - 1]);
        }


        public static void SetProperty<T, P>(this T instance, Expression<Func<T, P>> propExp, P value)
        {
            instance.SetProperty(propExp.Body, value);
        }
        public static void SetProperty<T, P>(this T instance, Expression propExp, P value)
        {
            instance.SetProperty(GetPropertyPath(propExp), value);
        }
        public static void SetProperty(this object instance, List<string> propertyPath, object value)
        {
            object pathInstance = instance;
            for (int i = 0; i < propertyPath.Count - 1; i++)
            {
                var propertyName = propertyPath[i];
                pathInstance = pathInstance.GetProperty(propertyName);
                if (pathInstance == null)
                    return;
            }
            if (propertyPath.Count == 0)
                throw new Exception("Tried to set property from an empty path! Make sure a body of a generic expression is passed to this function.");
            pathInstance.SetProperty(propertyPath[propertyPath.Count - 1], value);
        }

        public static void SetProperty(ref object instance, string propertyName, object value)
        {
            var targetInstance = instance;
            instance.GetPropertyInfo(propertyName).Set(ref targetInstance, value);
            instance = targetInstance;
        }


        public static void SetProperty(this object target, string propertyName, object value)
        {
            target.GetPropertyInfo(propertyName).Set(ref target, value);
        }

        public static string GetPropertyName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetPropertyName(expression);
        }

        public static string GetPropertyName<T, P>(
            Expression<Func<T, P>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            return GetPropertyName(expression.Body);
        }

        public static string GetPropertyName<T, P>(
            this T instance,
            Expression<Action<T>> expression)
        {
            return GetPropertyName(expression);
        }

        public static string GetPropertyName<T>(
            Expression<Action<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            return GetPropertyName(expression.Body);
        }

        private static string GetPropertyName(
            Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            if (expression is MemberExpression)
            {
                // PrimitiveToGlobal type property or field
                var memberExpression =
                    (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // PrimitiveToGlobal type method
                var methodCallExpression =
                    (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetPropertyName(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string GetPropertyName(
            UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression =
                    (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand)
                .Member.Name;
        }

        public static Expression GetPropertyExp<T>(Expression<Func<T, object>> expression)
        {
            return expression.Body;
        }

        public static List<string> GetPropertyPath<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertyPath(expression.Body);
        }

        public static List<string> GetPropertyPath(Expression expression)
        {
            MemberExpression me;
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expression as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expression as MemberExpression;
                    break;
            }
            List<string> propertyPath = new List<string>();
            while (me != null)
            {
                string propertyName = me.Member.Name;
                Type propertyType = me.Type;

                propertyPath.Add(propertyName);
                me = me.Expression as MemberExpression;
            }
            propertyPath.Reverse();
            return propertyPath;
        }
    }

}
