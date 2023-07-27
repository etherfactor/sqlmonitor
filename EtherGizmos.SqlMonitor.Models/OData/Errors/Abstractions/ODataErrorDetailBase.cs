using Microsoft.OData;

namespace EtherGizmos.SqlMonitor.Models.OData.Errors.Abstractions;

/// <summary>
/// A base class upon which to construct OData error details.
/// </summary>
public abstract class ODataErrorDetailBase
{
    /// <summary>
    /// When evaluated, returns the error code.
    /// </summary>
    private Func<string> CodeProvider { get; }

    /// <summary>
    /// When evaluated, returns the target property.
    /// </summary>
    private Func<string?>? TargetProvider { get; }

    /// <summary>
    /// When evaluated, returns the error message.
    /// </summary>
    private Func<string?>? MessageProvider { get; }

    /// <summary>
    /// Construct the error detail.
    /// </summary>
    /// <param name="codeProvider">When evaluated, returns the error code.</param>
    /// <param name="targetProvider">When evaluated, returns the target property.</param>
    /// <param name="messageProvider">When evaluated, returns the error message.</param>
    public ODataErrorDetailBase(Func<string> codeProvider, Func<string?>? targetProvider, Func<string?>? messageProvider)
    {
        CodeProvider = codeProvider;
        TargetProvider = targetProvider;
        MessageProvider = messageProvider;
    }

    /// <summary>
    /// Evaluate this into an error detail.
    /// </summary>
    /// <returns>The resulting error detail.</returns>
    public virtual ODataErrorDetail GetDetail()
    {
        string code = CodeProvider();
        string? target = TargetProvider?.Invoke();
        string? message = MessageProvider?.Invoke();

        return new ODataErrorDetail()
        {
            ErrorCode = code,
            Target = target,
            Message = message
        };
    }
}
