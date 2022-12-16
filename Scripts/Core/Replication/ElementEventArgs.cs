using Altimit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Altimit
{
    public class ElementEventArgs : EventArgs
    {
        public int index;
        public object element;
        public ElementEventArgs(int index, object item)
        {
            this.index = index;
            this.element = item;
        }
    }

    public delegate void ElementAddedEventHandler(object source, ElementEventArgs e);
    public delegate void ElementRemovedEventHandler(object source, ElementEventArgs e);
    public delegate void ListChangedEventHandler(object source, ElementEventArgs e);
    public delegate void ListClearedEventHandler(object source, EventArgs e);

    public delegate void HashsetItemAddedEventHandler<T>(object source, T e);
    public delegate void HashsetItemRemovedEventHandler<T>(object source, T e);
    public delegate void HashsetClearedEventHandler(object source, EventArgs e);
}