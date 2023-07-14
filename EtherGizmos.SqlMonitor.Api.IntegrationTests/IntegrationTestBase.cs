using System.Text;

namespace EtherGizmos.SqlMonitor.Api.IntegrationTests;

/// <summary>
/// Assuming integration tests do not run in parallel, intercepts the console output and logs it on a synchronous thread,
/// allowing it to be captured by NUnit.
/// </summary>
internal abstract class IntegrationTestBase
{
    private TextWriter OriginalWriter { get; set; }

    private StringBuilder CurrentOutput { get; set; }

    [SetUp]
    public void ConsoleSetUp()
    {
        CurrentOutput = new StringBuilder();
        TextWriter writer = new StringWriter(CurrentOutput);

        OriginalWriter = Console.Out;
        Console.SetOut(writer);
    }

    [TearDown]
    public void ConsoleTearDown()
    {
        string output = CurrentOutput.ToString();

        Console.SetOut(OriginalWriter);
        Console.Out.WriteLine(output);
    }
}
