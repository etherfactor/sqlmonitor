using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.SqlMonitor.Api.Services.Data.Validation;

/// <summary>
/// Provides <see cref="DisplayMetadata"/> based on <see cref="DisplayAttribute"/> and similar attributes.
/// </summary>
public class AttributeDisplayMetadataProvider : IDisplayMetadataProvider
{
    /// <inheritdoc/>
    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var property = context.Key.PropertyInfo;
        var attributes = context.Attributes;
        var displayMetadata = context.DisplayMetadata;

        //Only process properties
        if (property != null)
        {
            //Only process properties with a DisplayAttribute
            var attribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                displayMetadata.DisplayName = () => attribute.Name;
            }
        }
    }
}
