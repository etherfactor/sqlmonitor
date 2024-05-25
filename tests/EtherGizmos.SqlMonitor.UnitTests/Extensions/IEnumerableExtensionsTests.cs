namespace EtherGizmos.SqlMonitor.UnitTests.Extensions;

internal class IEnumerableExtensionsTests
{
    [Test]
    public void CrossJoin_WorksAsExpected()
    {
        //Arrange
        var first = new int[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
        };

        var second = new int[]
        {
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30
        };

        //Act
        var joined = first.CrossJoin(second);

        //Assert
        Assert.That(joined.Count(), Is.EqualTo(first.Count() * second.Count()));
    }
}
