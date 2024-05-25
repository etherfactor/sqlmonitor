using EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.RegularExpressions;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors;

public class ODataModelStateInvalidError : ODataErrorBase
{
    private const string Code = "test-0000";

    public ODataModelStateInvalidError(IEdmModel model, Type entityType, ModelStateDictionary state)
        : base(codeProvider: () => Code,
            targetProvider: () => null,
            messageProvider: () => $"Provided model is invalid for type '{GetTypeDisplayName(model, entityType)}'.")
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

                    var propertyDisplayName = propertyName;

                    AddDetail(new ODataModelStateInvalidErrorDetail(Code, propertyDisplayName, error.ErrorMessage));
                }
            }
        }
    }

    /// <summary>
    /// Loads the entity external name, throwing an error if it cannot be found.
    /// </summary>
    /// <param name="model">The OData model.</param>
    /// <param name="entityType">The external entity type.</param>
    /// <returns>The external display name.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static string GetTypeDisplayName(IEdmModel model, Type entityType)
    {
        var edmEntityType = model.SchemaElements.OfType<IEdmEntityType>()
            .FirstOrDefault(e => model.GetAnnotationValue<ClrTypeAnnotation>(e).ClrType == entityType);

        return edmEntityType?.Name ?? entityType.Name.Replace("DTO", "");
    }
}
