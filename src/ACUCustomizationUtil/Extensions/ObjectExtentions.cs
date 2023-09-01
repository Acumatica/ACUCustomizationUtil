namespace ACUCustomizationUtils.Extensions;

public static class ObjectExtensions
{
    public static T CopyValues<T>(this T target, T source)
    {
        var targetType = target?.GetType();
        if (targetType == null) return target;

        var properties = targetType.GetProperties().Where(prop => prop is { CanRead: true, CanWrite: true });
        foreach (var prop in properties)
        {
            if (prop.PropertyType.Assembly == targetType.Assembly)
            {
                var targetObject = prop.GetValue(target);
                var sourceObject = prop.GetValue(source);
                targetObject.CopyValues(sourceObject);
            }
            else
            {
                var value = prop.GetValue(source, null);
                if (value != null) prop.SetValue(target, value, null);
            }
        }

        return target;
    }
}