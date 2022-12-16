using System;
using System.Collections;
using System.Collections.Generic;
public interface INotifyPropertyChanged
{
    event PropertyChangedEventHandler PropertyChanged;
}

public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

public class PropertyChangedEventArgs : EventArgs
{
    public virtual string PropertyName { get; }
    public virtual object OldProperty { get; }

    public PropertyChangedEventArgs(string propertyName, object oldProperty)
    {
        PropertyName = propertyName;
        OldProperty = oldProperty;
    }
}