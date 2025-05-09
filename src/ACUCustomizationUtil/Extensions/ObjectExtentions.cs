namespace ACUCustomizationUtils.Extensions;

public static class ObjectExtensions
{
    public static T CopyValues<T>(this T target, T source)
    {
        Type? targetType = target?.GetType();
        if (targetType == null)
            return target;

        IEnumerable<System.Reflection.PropertyInfo> properties = targetType
            .GetProperties()
            .Where(prop => prop is { CanRead: true, CanWrite: true });
        foreach (System.Reflection.PropertyInfo? prop in properties)
        {
            if (prop.PropertyType.Assembly == targetType.Assembly)
            {
                object? targetObject = prop.GetValue(target);
                object? sourceObject = prop.GetValue(source);
                targetObject.CopyValues(sourceObject);
            }
            else
            {
                object? value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }

        return target;
    }
}
