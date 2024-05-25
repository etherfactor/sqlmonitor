namespace EtherGizmos.SqlMonitor.UnitTests.Extensions;

internal class DateTimeExtensionTests
{
    [TestCase("2023-09-01T08:01Z", "00:05", "2023-09-01T08:00Z")]
    [TestCase("2023-09-01T08:04Z", "00:05", "2023-09-01T08:00Z")]
    [TestCase("2023-09-01T08:00Z", "00:05", "2023-09-01T08:00Z")]
    [TestCase("2023-09-01T08:00Z", "06:00", "2023-09-01T06:00Z")]
    public void Floor_WorksAsExpected(DateTimeOffset date, TimeSpan interval, DateTimeOffset target)
    {
        //Arrange

        //Act
        var test = date.Floor(interval);

        //Assert
        Assert.That(test, Is.EqualTo(target));
    }

    [TestCase("2023-09-01T08:00Z", "00:00")]
    [TestCase("2023-09-01T08:00Z", "-00:01")]
    public void Floor_IsInvalid_ThrowsInvalidOperationException(DateTimeOffset date, TimeSpan interval)
    {
        //Arrange

        //Act

        //Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            date.Floor(interval);
        });
    }

    [TestCase("2023-09-01T08:01Z", "00:05", "2023-09-01T08:00Z")]
    [TestCase("2023-09-01T08:04Z", "00:05", "2023-09-01T08:05Z")]
    [TestCase("2023-09-01T08:00Z", "00:05", "2023-09-01T08:00Z")]
    [TestCase("2023-09-01T09:00Z", "06:00", "2023-09-01T12:00Z")]
    public void Round_WorksAsExpected(DateTimeOffset date, TimeSpan interval, DateTimeOffset target)
    {
        //Arrange

        //Act
        var test = date.Round(interval);

        //Assert
        Assert.That(test, Is.EqualTo(target));
    }

    [TestCase("2023-09-01T08:00Z", "00:00")]
    [TestCase("2023-09-01T08:00Z", "-00:01")]
    public void Round_IsInvalid_ThrowsInvalidOperationException(DateTimeOffset date, TimeSpan interval)
    {
        //Arrange

        //Act

        //Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            date.Round(interval);
        });
    }

    [TestCase("2023-09-01T08:01Z", "00:05", "2023-09-01T08:05Z")]
    [TestCase("2023-09-01T08:04Z", "00:05", "2023-09-01T08:05Z")]
    [TestCase("2023-09-01T08:00Z", "00:05", "2023-09-01T08:00Z")]
    [TestCase("2023-09-01T08:00Z", "06:00", "2023-09-01T12:00Z")]
    public void Ceiling_WorksAsExpected(DateTimeOffset date, TimeSpan interval, DateTimeOffset target)
    {
        //Arrange

        //Act
        var test = date.Ceiling(interval);

        //Assert
        Assert.That(test, Is.EqualTo(target));
    }

    [TestCase("2023-09-01T08:00Z", "00:00")]
    [TestCase("2023-09-01T08:00Z", "-00:01")]
    public void Ceiling_IsInvalid_ThrowsInvalidOperationException(DateTimeOffset date, TimeSpan interval)
    {
        //Arrange

        //Act

        //Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            date.Ceiling(interval);
        });
    }
}
