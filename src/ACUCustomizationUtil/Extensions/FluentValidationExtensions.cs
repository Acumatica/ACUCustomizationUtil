using FluentValidation;

namespace ACUCustomizationUtils.Extensions;

public static class FluentValidationExtensions
{
    public static void ValidateAndThrowArgumentException<T>(this IValidator<T> validator, T instance)
    {
        var res = validator.Validate(instance);

        if (res.IsValid) return;
        var ex = new ValidationException(res.Errors);
        throw new ArgumentException(ex.Message, ex);
    }
}