namespace Altimit
{
    //public class DeltaSnapshot : BiMapper<object>
    //{
    /*
    public static implicit operator DeltaSnapshot(Dictionary<object, object> x)
    {
        var globalValue = new DeltaSnapshot();
        foreach (var instances in x)
        {
            int typeID = Convert.ToInt32(instances.Key);
            globalValue.Add(typeID, new BiMapper<object>());

            var classInfo = ClassExtensions.GetClassInfo(Globe.GetType(typeID));

            var _instances = (Dictionary<object, object>)instances.Value;
            foreach (var properties in _instances)
            {
                int instanceID = Convert.ToInt32(properties.Key);
                globalValue[typeID].Add(instanceID, new UniMapper<object>());

                var _properties = (Dictionary<object, object>)properties.Value;
                foreach (var property in _properties)
                {
                    int propertyID = Convert.ToInt32(property.Key);

                    var propertyType = classInfo.GetPropertyByID(propertyID).PropertyType;
                    globalValue[typeID][instanceID].Add(propertyID, property.Value.To(propertyType));
                }
            }
        }
        return globalValue;
    }*/
    //}
}
