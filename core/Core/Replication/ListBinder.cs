﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altimit.UI
{
    public class ListBinder
    {
        public ListBinder(IAList list, Action<IAList, int, object> onAddedAction, Action<IAList, int, object> onRemovedAction, bool isInit = true)
        {
            list.ElementAdded += (list, elementEventArgs) =>
            {
                onAddedAction((IAList)list, elementEventArgs.index, elementEventArgs.element);
            };
            list.ElementRemoved += (list, elementEventArgs) =>
            {
                onRemovedAction((IAList)list, elementEventArgs.index, elementEventArgs.element);
            };

            if (isInit)
            {
                int i = 0;
                foreach (var element in list)
                {
                    onAddedAction(list, i, element);
                    i++;
                }
            }
        }
            /*
            Node listNode;
            public IAList List => Instance as IAList;
            Type type;
            Func<int, object, Node> createBinderFunc;
            Func<object, Node> unorderedCreateBinderFunc;
            Func<object, bool> IsBindable = x => true;
            List<Node> Binders = new List<Node>();

            public void Init(Node listNode, Type type, IAList list, Func<object, Node> unorderedCreateBinderFunc, Func<object, bool> isBindable = null)
            {
                Init(listNode, type, unorderedCreateBinderFunc, isBindable);
                Instance = list;
            }

            public void Init(Node listNode, Type type, Func<object, Node> unorderedCreateBinderFunc, Func<object, bool> isBindable = null)
            {
                this.listNode = listNode;
                this.type = type;
                this.unorderedCreateBinderFunc = unorderedCreateBinderFunc;
                if (isBindable != null)
                    IsBindable = isBindable;
            }

            public void Init(Type type, object list, Func<int, object, Node> childGO, Func<object, bool> isBindable = null)
            {
                this.type = type;
                createBinderFunc = childGO;
                if (isBindable != null)
                    IsBindable = isBindable;
                Instance = list;
            }

            public void Init(Type type, object list, Func<int, object, Node> childGO)
            {
                Init(type, list, childGO);
            }

            protected override void Set(object value)
            {

                if (List == value)
                    return;

                if (List != null)
                {
                    lock (List)
                    {
                        List.ElementAdded -= ItemAdded;
                        List.ElementRemoved -= ItemRemoved;

                        while (Binders.Count > 0)
                            RemoveBinder(Binders[0]);
                    }
                }

                base.Set(value);

                if (List != null)
                {
                    //OS.Log($"Setting list in list binder to list with {List.Count} elements.");

                    lock (List)
                    {
                        foreach (var element in List)
                        {
                            AddBinder(element);
                        }

                        List.ElementAdded += ItemAdded;
                        List.ElementRemoved += ItemRemoved;
                    }
                }
            }

            public void OnDestroy()
            {
                var observableList = Instance as IAList;
                if (observableList != null)
                {
                    observableList.ElementAdded -= ItemAdded;
                    observableList.ElementRemoved -= ItemRemoved;
                }
            }

            public Node CreateBinder(int index, object value)
            {
                if (!IsBindable(value))
                    return null;

                Node childBinder;

                if (createBinderFunc != null)
                {
                    childBinder = createBinderFunc(index, value);
                } else if (unorderedCreateBinderFunc != null)
                {
                    childBinder = unorderedCreateBinderFunc(value);
                } else
                {
                    throw new Exception(string.Format("Create binder function not implemented for list of type {0}!", type));
                }

                childBinder.Parent = listNode;

                return childBinder;
            }

            private void ItemRemoved(object source, ElementEventArgs e)
            {
                RemoveBinder(e.index);
            }

            private void ItemAdded(object source, ElementEventArgs e)
            {
                AddBinder(e.index, e.element);
            }

            void RemoveBinder(Node binder)
            {
                RemoveBinder(Binders.IndexOf(binder));
            }

            void RemoveBinder(int index)
            {
                Node binder = Binders[index];
                binder.Destroy();
                Binders.RemoveAt(index);
            }

            void AddBinder(object data)
            {
                AddBinder(Binders.Count, data);
            }

            void AddBinder(int index, object value)
            {
                Node binder = CreateBinder(index, value);
                Binders.Insert(index, binder);
            }
            */
        }
}