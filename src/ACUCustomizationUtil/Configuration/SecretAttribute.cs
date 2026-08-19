namespace ACUCustomizationUtils.Configuration;

/// <summary>
/// Marks a configuration property whose value is a credential (login, password, or a
/// connection string that embeds one) and must never be written to the console or log file.
/// The configuration printer (<see cref="Helpers.ConfigurationHelper"/>) substitutes a mask
/// for any property carrying this attribute. Declaring secrecy here — right on the property —
/// keeps it impossible to miss when new credential fields are added.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SecretAttribute : Attribute { }
