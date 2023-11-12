using EtherGizmos.SqlMonitor.Models.OData.Errors.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Models.OData.Errors;

public class ODataModelStateInvalidError : ODataErrorBase
{
    private const string Code = "test-0000";

    public ODataModelStateInvalidError(Type entityType, ModelStateDictionary state)
        : base(codeProvider: () => Code,
            targetProvider: () => null,
            messageProvider: () => $"Provided model is invalid for type '{GetTypeDisplayName(entityType)}'.")
    {
        //If the model contains an empty key, errors are listed under that node
        if (state.ContainsKey(""))
        {
            var regexExtraProperty = new Regex(@"property.+?'(?'PROP'[^']+)'.+?does not exist.+?type.+?'(?'TYPE'[^']+')");

            foreach (var error in state[""]!.Errors)
            {
                string propertyDisplayName;

                //Prefer the error message, but fall back on the exception if necessary
                var errorMessage = error.ErrorMessage;
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = error.Exception?.Message;
                }
                errorMessage ??= "";

                switch (error.ErrorMessage)
                {
                    case var _ when regexExtraProperty.IsMatch(errorMessage):
                        var match = regexExtraProperty.Match(errorMessage);
                        propertyDisplayName = match.Groups["PROP"].Value;
                        break;

                    default:
                        propertyDisplayName = null!;
                        break;
                }

                AddDetail(new ODataModelStateInvalidErrorDetail(Code, propertyDisplayName, errorMessage));
            }
        }
        //If the model does not contain an empty key, errors are in the root
        else
        {
            foreach (var item in state)
            {
                var propertyName = item.Key;
                foreach (var error in item.Value.Errors)
                {
                    var property = entityType.GetProperty(propertyName)
                        ?? throw new InvalidOperationException(string.Format("Unrecognized property: {0}", propertyName));

                    var propertyDisplayName = property.GetCustomAttribute<DisplayAttribute>()?.Name
                        ?? throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' must be annotated with a '{2}' and specify the '{3}' property",
                            property.Name, property.DeclaringType?.Name, nameof(DisplayAttribute), nameof(DisplayAttribute.Name)));

                    AddDetail(new ODataModelStateInvalidErrorDetail(Code, propertyDisplayName, error.ErrorMessage));
                }
            }
        }
    }

    /// <summary>
    /// Loads the entity external name, throwing an error if it cannot be found.
    /// </summary>
    /// <param name="entityType">The external entity type.</param>
    /// <returns>The external display name.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GetTypeDisplayName(Type entityType)
    {
        return entityType.GetCustomAttribute<DisplayAttribute>()?.Name
            ?? throw new InvalidOperationException(string.Format("Type '{0}' must be annotated with a '{1}' and specify the '{2}' property",
                entityType.Name, nameof(DisplayAttribute), nameof(DisplayAttribute.Name)));
    }
}
