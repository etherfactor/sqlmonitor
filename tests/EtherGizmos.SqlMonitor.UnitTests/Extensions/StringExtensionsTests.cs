namespace EtherGizmos.SqlMonitor.UnitTests.Extensions;

internal class StringExtensionsTests
{
    [TestCase("SomeText", "some_text")]
    [TestCase("some_text", "some_text")]
    [TestCase("ANItem", "an_item")]
    public void ToSnakeCase_WorksAsExpected(string from, string to)
    {
        //Arrange

        //Act
        var test = from.ToSnakeCase();

        //Assert
        Assert.That(test, Is.EqualTo(to));
    }
}
