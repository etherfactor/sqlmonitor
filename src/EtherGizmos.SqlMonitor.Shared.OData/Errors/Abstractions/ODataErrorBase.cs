using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;

namespace EtherGizmos.SqlMonitor.Shared.OData.Errors.Abstractions;

/// <summary>
/// A base class upon which to construct OData errors.
/// </summary>
public abstract class ODataErrorBase
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
    /// The list of all detail messages.
    /// </summary>
    private List<ODataErrorDetailBase> DetailsList { get; } = new List<ODataErrorDetailBase>();

    /// <summary>
    /// The set of all detail messages.
    /// </summary>
    public IEnumerable<ODataErrorDetailBase> Details => DetailsList.Select(e => e);

    /// <summary>
    /// Construct the error.
    /// </summary>
    /// <param name="codeProvider">When evaluated, returns the error code.</param>
    /// <param name="targetProvider">When evaluated, returns the target property.</param>
    /// <param name="messageProvider">When evaluated, returns the error message.</param>
    public ODataErrorBase(Func<string> codeProvider, Func<string?>? targetProvider, Func<string?>? messageProvider)
    {
        CodeProvider = codeProvider;
        TargetProvider = targetProvider;
        MessageProvider = messageProvider;
    }

    /// <summary>
    /// Adds a detail message.
    /// </summary>
    /// <param name="detail">The detail message.</param>
    protected void AddDetail(ODataErrorDetailBase detail)
    {
        DetailsList.Add(detail);
    }

    /// <summary>
    /// Evaluate this into an error.
    /// </summary>
    /// <returns>The resulting error.</returns>
    public virtual ODataError GetError()
    {
        string code = CodeProvider();
        string? target = TargetProvider?.Invoke();
        string? message = MessageProvider?.Invoke();

        return new ODataError()
        {
            ErrorCode = code,
            Target = target,
            Message = message,
            Details = Details.Select(e => e.GetDetail()).ToList()
        };
    }

    /// <summary>
    /// Evaluate this into a response, containing an error.
    /// </summary>
    /// <returns>The resulting response.</returns>
    public virtual IActionResult GetResponse()
    {
        return new UnprocessableEntityODataResult(GetError());
    }
}
