using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Altimit;
using Altimit.UI;
using Sprite = Altimit.UI.Sprite;

namespace Altimit.UI
{
    public static class BindExtensions
    {
        /*
        public class LinkedBinder<T, P, D>
        {
            Node go;
            T firstTarget;
            P secondTarget;
            Expression<Func<T, P>> firstPropExp;
            Expression<Func<P, D>> secondPropExp;

            // Generated binding data
            Action<object, string> dataAction;
            object modelTarget;
            string modelPropertyName;
            Action<object, string> modelAction;

            public LinkedBinder(Node go, T firstTarget, Expression<Func<T, P>> firstPropExp, Expression<Func<P, D>> secondPropExp, bool invert = false)
            {
                this.go = go;
                // Set first and second target
                this.firstTarget = firstTarget;
                this.firstPropExp = firstPropExp;
                this.secondPropExp = secondPropExp;
                BindSecondProperty(invert);

                firstTarget.BindProperty(firstPropExp, OnFirstPropertyChanged);
            }

            void OnFirstPropertyChanged(object sender, string propertyName)
            {
                // The first property changed--unregister previous bindings
                if (dataAction != null)
                    secondTarget?.UnbindProperty(secondPropExp, dataAction);
                if (modelAction != null)
                    modelTarget?.UnbindProperty(modelPropertyName, modelAction);
                // Set new bindings
                BindSecondProperty();
            }

            void BindSecondProperty(bool invert = false)
            {
                // Set second target
                secondTarget = firstTarget.GetProperty(firstPropExp);
                if (secondTarget == null)
                    return;
                // Auto-generate binding data between data and model according to Node's components
                go.BindProperty(secondTarget, secondPropExp, out dataAction, out modelTarget, out modelPropertyName, out modelAction, invert);
                if (dataAction != null)
                    secondTarget?.BindProperty(secondPropExp, dataAction);
                if (modelAction != null)
                    modelTarget?.BindProperty(modelPropertyName, modelAction);
                if (dataAction != null)
                {
                    // Set model to data according to new target
                    dataAction(secondTarget, BindingExtensions.GetPropertyName(secondPropExp));
                } else
                {
                    modelAction?.Invoke(modelTarget, modelPropertyName);
                }
            }
        }
        

        public class LinkedPropertyObserver<T, P>
        {
            T firstTarget;
            P secondTarget;
            Expression<Func<T, P>> firstPropExp;
            Expression<Func<P, object>> secondPropExp;
            Action<object, string> finalAction;

            public LinkedPropertyObserver(T firstTarget, Expression<Func<T, P>> firstPropExp, Expression<Func<P, object>> secondPropExp, Action<object, string> finalAction)
            {
                this.firstTarget = firstTarget;
                this.firstPropExp = firstPropExp;
                this.secondPropExp = secondPropExp;
                this.finalAction = finalAction;
                firstTarget.BindProperty(firstPropExp, OnFirstPropertyChanged);
                BindSecondProperty();
            }

            void OnFirstPropertyChanged(object sender, string propertyName)
            {
                secondTarget?.UnbindProperty(secondPropExp, finalAction);
                BindSecondProperty();
            }

            void BindSecondProperty()
            {
                secondTarget = (P)firstTarget.GetProperty(firstPropExp);
                secondTarget.BindProperty(secondPropExp, finalAction);
            }
        }
        */

        public static List<KeyValuePair<object, object>> Bindings = new List<KeyValuePair<object, object>>();

        //TODO: Pass ref variable and use bindings list to try binding to component. Include results as actions added to binder get and set, or implement specific list of binded variables.
        //These variables are different than variables currently being binded
        //TODO: May not need to, test code and see. If it fails: pass Expression<Func<T, object>> as func. Convert property name to string and use string as Value in key value pair
        //Add version of TryBind that uses a string as a parameter instead of a lambda expression
        public static KeyValuePair<object, object> Bind<T>(Expression<Func<T, object>> func)
        {
            return new KeyValuePair<object, object>(typeof(T),func);
        }

        /*
        public static Node BindProperty<T, P, D>(this Node go, T target, Expression<Func<T, P>> firstPropExp, Expression<Func<P, D>> secondPropExp, bool invert = false)
        {
            if (target == null)
                return go;
            //string firstPropertyName = GlobeExtensions.GetPropertyName(firstPropExp);
            //string secondPropertyName = GlobeExtensions.GetPropertyName(secondPropExp);
            //var linkedBinder = new LinkedBinder<T, P, D>(go, target, firstPropExp, secondPropExp, invert);
            //var linkedObserver = new LinkedPropertyObserver<T, P>(target, firstPropExp, secondPropExp, (x, y) => { go.Text((string)x.GetProperty(y), true); });
            return go;
        }*/

        // Used for one way binding with specified action: data->model
        public static Control BindProperty<T, P>(this Control go, T target, Expression<Func<T, P>> dataPropExp, Action<Node, P, P> dataAction)
        {
            target.BindProperty(dataPropExp, (x, y, z) => { dataAction(go, (P)target.GetProperty(y), (P)z); });
            dataAction(go, (P)target.GetProperty(dataPropExp), default(P));
            return go;
        }

        public static Control BindInactive<T>(this Control go, T target, Expression<Func<T, bool>> propExp, bool isActive = true)
        {
            go.SetVisibility(target.GetProperty(propExp) ^ isActive);
            target.BindProperty(propExp, (x, y, z) => go.SetVisibility((bool)target.GetProperty(y) ^ isActive));
            return go;
        }

        public static Control BindActive<T>(this Control go, T target, Expression<Func<T, string>> propExp)
        {
            go.SetVisibility(!string.IsNullOrEmpty(target.GetProperty(propExp)));
            target.BindProperty(propExp, (x, y, z) => go.SetVisibility(!string.IsNullOrEmpty((string)target.GetProperty(y))));
            return go;
        }

        public static void BindProperty(this Node go, object dataTarget, Expression dataPropExp,
            out Action<object, string, object> dataAction, out object modelTarget, out Expression modelPropExp, out Action<object, string, object> modelAction, bool invert = false)
        {
            var dataPropertyInfo = dataTarget.GetPropertyInfo(dataPropExp);
            dataAction = null;
            modelTarget = null;
            modelPropExp = null;
            modelAction = null;
            Expression tempModelPropExp = null;

#if TEMP
            if (typeof(AList<>).IsSubclassOfRawGeneric(dataPropertyInfo.PropertyType))
            {
                if (go.Has<ListBinder>())
                {
                    var listBinder = go.Get<ListBinder>();
                    dataAction = (x, y, z) =>
                    {
                        var instance = dataTarget.GetProperty(dataPropExp);
                        listBinder.Instance = instance;
                    };
                    ExecutePropAction(dataTarget, dataPropExp, dataAction);
                }
            }
            // One way: bool property -> Node active status
            else if (dataPropertyInfo.PropertyType == typeof(Sprite))
            {
                /* todo reimplement after UI changes
                if (go.Has<Image>())
                {
                    dataAction = (x, y, z) => go.Get<Image>(x => x.sprite = (Sprite)dataTarget.GetProperty(dataPropExp));
                    ExecutePropAction(dataTarget, dataPropExp, dataAction);

                    modelTarget = go.Get<Image>();
                    tempModelPropExp = BindingExtensions.GetPropertyExp<Image>(x => x.sprite);
                    modelAction = (x, y, z) => {
                        dataTarget.SetProperty(dataPropExp, x.GetProperty(y));
                    };
                }
                */
            }
            else if (dataPropertyInfo.PropertyType == typeof(bool))
            {
                if (go.Has<Toggle>())
                {
                    dataAction = (x, y, z) => go.Get<Toggle>(x=>x.isOn = (bool)dataTarget.GetProperty(dataPropExp) ^ invert);
                    dataAction(dataTarget, dataPropertyInfo.Name, null);

                    modelTarget = go.Get<Toggle>();
                    tempModelPropExp = BindingExtensions.GetPropertyExp<Toggle>(x => x.isOn);
                    modelAction = (x, y, z) => {
                        dataTarget.SetProperty(dataPropExp, (bool)x.GetProperty(tempModelPropExp) ^ invert);
                    };
                }
                else
                {
                    dataAction = (x, y, z) => go.SetVisibility((bool)dataTarget.GetProperty(y) ^ invert);
                    ExecutePropAction(dataTarget, dataPropExp, dataAction);
                    //go.SetActive((bool)dataTarget.GetProperty(dataPropertyInfo.Name));
                    //modelAction =  (x,y) => target.SetProperty(propExp, go.activeSelf);
                }
            }
            else if (dataPropertyInfo.PropertyType.IsEnum)
            {
                if (go.Has<BetterToggleGroup>())
                {
                    var toggleGroupObserver = go.Get<BetterToggleGroup>();
                    modelTarget = toggleGroupObserver;
                    tempModelPropExp = BindingExtensions.GetPropertyExp<BetterToggleGroup>(x => x.Toggle);

                    modelAction = (x, y, z) => {
                        var toggleValue = ((Toggle)x.GetProperty(tempModelPropExp))?.Node.AddOrGet<InstanceBinder>().Instance;
                        if (toggleValue != null)
                        {
                            dataTarget.SetProperty(dataPropExp, toggleValue);
                        }
                    };
                    dataAction = (x, y, z) =>
                    {
                        var toggle = toggleGroupObserver.Toggles.SingleOrDefault(toggle => toggle.Node.AddOrGet<InstanceBinder>().Instance.Equals(dataTarget.GetProperty(dataPropExp)));
                        toggleGroupObserver.SetToggle(toggle);
                    };
                    ExecutePropAction(dataTarget, dataPropExp, dataAction);
                }
                else if (go.Has<TMPro.TextMeshProUGUI>(true))
                {
                    go.BindText(dataTarget, dataPropExp, out dataAction, out modelTarget, out tempModelPropExp, out modelAction);
                }
            }
            else if (dataPropertyInfo.PropertyType == typeof(string))
            {
                // One way: input field -> text property
                if (go.Has<TMPro.TMP_InputField>(true))
                {
                    //dataAction = (x, y) => go.Get<TMPro.TMP_InputField>(true).text = (string)dataTarget.GetProperty(y);
                    var input = go.Get<TMPro.TMP_InputField>(true);
                    modelTarget = input;
                    tempModelPropExp = BindingExtensions.GetPropertyExp<TMPro.TMP_InputField>(x => x.text);
                    modelAction = (x, y, z) => { dataTarget.SetProperty(dataPropExp, x.GetProperty(tempModelPropExp)); };
                    dataAction = (x, y, z) => { input.text = (string)dataTarget.GetProperty(dataPropExp); };
                    ExecutePropAction(dataTarget, dataPropExp, dataAction);
                    // Sets the model's initial value to the data's value
                    //modelTarget.SetProperty(modelPropName, dataTarget.GetProperty(dataPropExp));
                }
                // One way: text property -> text field
                else if (go.Has<TMPro.TextMeshProUGUI>(true))
                {
                    go.BindText(dataTarget, dataPropExp, out dataAction, out modelTarget, out tempModelPropExp, out modelAction);
                }
            }
#endif
            modelPropExp = tempModelPropExp;

            if (modelTarget != null && modelPropExp == null)
                throw new Exception("A model target has been set but no property has been set for this binding!");
        }

        public static void BindText(this Label label, object dataTarget, Expression dataPropExp,
            out Action<object, string, object> dataAction, out object modelTarget, out Expression modelPropExp, out Action<object, string, object> modelAction, bool invert = false)
        {
            dataAction = null;
            modelTarget = null;
            modelPropExp = null;
            modelAction = null;
            dataAction = (x, y, z) =>
            {
                label.Text = dataTarget.GetProperty(dataPropExp)?.ToString();
            };

            ExecutePropAction(dataTarget, dataPropExp, dataAction);
        }

        public static void ExecutePropAction(object dataTarget, Expression dataPropExp, Action<object, string, object> dataAction)
        {
            var lastInstance = dataTarget.GetLastInstance(dataPropExp);
            if (lastInstance != null)
                dataAction(lastInstance, dataTarget.GetPropertyInfo(dataPropExp).Name, null);
        }

        public static Node BindProperty<T, P>(this Node go, T target, Expression<Func<T, P>> propExp, bool invert = false)
        {
            if (target == null)
                OS.LogError("Binding target is null!");

            // Generated binding data
            Action<object, string, object> dataAction;
            object modelTarget;
            Expression modelPropertyExp;
            Action<object, string, object> modelAction;
            go.BindProperty(target, propExp.Body, out dataAction, out modelTarget, out modelPropertyExp, out modelAction, invert);

            if (dataAction != null)
                target.BindProperty(propExp, dataAction, false);
            if (modelAction != null)
                modelTarget?.BindProperty(modelPropertyExp, modelAction, false);
            return go;
        }

        public static bool TryBind(this Node go, Type compType, Type propType, object compExp, object prop, object propExp)
        {
            Type delType = typeof(Func<,>).MakeGenericType(compType, typeof(object));
            Type expType = typeof(Expression<>).MakeGenericType(delType);

            var method = typeof(A).GetMethods().Where(x => x.Name == "TryBind" &&
                x.ContainsGenericParameters &&
                x.GetParameters().Length == 4 &&// !x.GetParameters()[1].ParameterType.Equals(typeof(object)) &&
                !x.GetParameters()[2].ParameterType.Equals(propType)).SingleOrDefault();

            MethodInfo genericMethod = method.MakeGenericMethod(compType, propType);
            var parameters = new[] { go, compExp, prop, propExp };
            var result = genericMethod.Invoke(null, parameters);

            return (bool)result;
        }

        /*
        public static Node Bind(this Node go, IBound value)
        {
            foreach (var binding in Bindings)
            {
                if (go.TryBind((Type)binding.Key, binding.Value, value))
                    break;
            }
            return go;
        }

        public static bool TryBind(this Node go, Type compType, object compExp, IBound value)
        {
            Type delType = typeof(Func<,>).MakeGenericType(compType, typeof(object));
            Type expType = typeof(Expression<>).MakeGenericType(delType);

            var method = typeof(A).GetMethods().Where(x => x.Name == "TryBind" &&
                x.ContainsGenericParameters &&
                x.GetParameters().Length == 3 &&
                x.GetParameters()[2].ParameterType.Equals(typeof(IBound))).SingleOrDefault();

            MethodInfo genericMethod = method.MakeGenericMethod(compType);
            var parameters = new[] { go, compExp, value };
            var result = genericMethod.Invoke(null, parameters);

            return (bool)result;
        }
        */

        // TODO: Add other methods for optionally passing function that includes element index
        public static Node BindList<T>(this Node node, IAList<T> list, Func<T, Node> createChildNodeFunc, bool isInit = true)//Func<T, bool> isBindable = null)
        {
            Action<int, T> onAddedAction = (index, value) =>
            {
                Node childNode = null;

                if (createChildNodeFunc != null)
                {
                    childNode = createChildNodeFunc(value);
                }

                childNode.Parent = node;
            };

            list.BindList<T>(onAddedAction);
            return node;
        }

        /*
        public static Node BindList<TTarget, TElement>(this Node node, TTarget target, Expression<Func<TTarget, AList<TElement>>> propExp, Func<TElement, Node> childGO, Func<TElement, bool> isBindable = null)
        {
            var listBinder = new ListBinder();
            listBinder.Init(node, typeof(AList<TElement>), ConvertFunc(childGO), ConvertFunc(isBindable));
            node.BindProperty(target, propExp);
            return node;
        }
        */

        public static Node BindToggleGroup<T, P>(this Node go, T instance, Expression<Func<T, P>> propExp, Func<P, Node> childGO) where P : Enum
        {
            /* todo reimpliment after UI changes
            foreach (var enumValue in typeof(P).GetEnumValues())
            {
                Toggle toggle;
                var _childGO = childGO((P)enumValue).Toggle().Hold<Toggle>(out toggle).BindInstance(enumValue);
                go.Hold(_childGO);
            }
            go.ToggleGroup().BindProperty(instance, propExp);
            */
            return go;
        }

        public static Node BindToggleShift<T, P>(this Node go, T instance, Expression<Func<T, P>> propExp) where P : Enum
        {
            /* todo reimpliment after UI changes
return go.HList(0).Hold(
    AUI.UI.ImageButton(AUI.GetSprite("Back")).OnClick(()=> {
        var values = Enum.GetValues(typeof(P));
        var index = Array.IndexOf(values, instance.GetProperty(propExp));
        instance.SetProperty(propExp, (P)values.GetValue((int)Mathf.Repeat(index-1, values.Length)));
    }),
    AUI.UI.Text("Hair", AUI.DarkGrey, TextAnchor.MiddleCenter).FlexibleWidth(1).BindProperty(instance, propExp),
    AUI.UI.ImageButton(AUI.GetSprite("Forward")).OnClick(() => {
        var values = Enum.GetValues(typeof(P));
        var index = Array.IndexOf(values, instance.GetProperty(propExp));
        instance.SetProperty(propExp, (P)values.GetValue((int)Mathf.Repeat(index + 1, values.Length)));
    })
);
            */
            return go;
        }

        /* todo: possibly add back in with new UI system
        public static Node BindInstance<T>(this Node go, T instance)
        {
            return go.Hold<InstanceBinder>(x => x.Instance = instance);
        }
        */

        /*
        public static Node BindList<T>(this Node node, AList<T> list, Func<int, T, Node> childGO, Func<T, bool> isBindable = null)
        {
            var listBinder = new ListBinder();
            listBinder.Init(typeof(List<T>), list, new Func<int, object, Node>((index, x) => { return childGO(index, (T)x); }), ConvertFunc(isBindable));
            return node;
        }
        */

        public static Action<Node, object> ConvertAction<Prop>(Action<Node, Prop> action)
        {
            return new Action<Node, object>((x, y) => { action(x, (Prop)y); });
        }

        public static Func<object, D> ConvertFunc<T, D>(Func<T, D> func)
        {
            if (func == null)
                return null;
            return new Func<object, D>((x) => { return func((T)x); });
        }
    }
}