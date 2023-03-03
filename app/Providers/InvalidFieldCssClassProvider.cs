using Microsoft.AspNetCore.Components.Forms;

namespace Gymr.Providers;

internal class InvalidFieldCssClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier) =>
        editContext.GetValidationMessages(fieldIdentifier).Any() ? "is-invalid" : string.Empty;
}