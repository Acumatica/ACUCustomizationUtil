using ACUCustomizationUtils.Extensions;
using FluentValidation;

namespace ACUCustomizationUtils.Validators;
/// <summary>
/// Base abstract option validator
/// </summary>
/// <remarks>
/// Authored by Aleksej Slusar
/// email: aleksej.slusar@sprinterra.com
/// Copyright Sprinterra(c) 2023
/// </remarks>
public abstract class OptionsValidatorBase
{
    protected static void Validate<T>(T obj, IValidator<T> validator)
    {
        validator.ValidateAndThrowArgumentException(obj);
    }
}