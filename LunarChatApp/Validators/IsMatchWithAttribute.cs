using System;
using System.ComponentModel.DataAnnotations;

namespace LunarChatApp.Validators;

public sealed class IsMatchWithAttribute(string matchProperty, string errorMessage = "Not match")
    : ValidationAttribute(errorMessage)
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var instance = validationContext.ObjectInstance;
        var otherValue = instance.GetType().GetProperty(matchProperty)?.GetValue(instance);

        return ((IComparable)value!).CompareTo(otherValue) == 0
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }
}
