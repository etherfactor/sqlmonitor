using EtherGizmos.SqlMonitor.Api.Extensions;

namespace EtherGizmos.SqlMonitor.Api.UnitTests.Extensions;

internal class ScoreExtensionsTests
{
    [TestCase(false, 0)]
    [TestCase(true, 1)]
    public void TryGetScore_IsBoolean_WorksAsExpected(bool value, double expectedScore)
    {
        //Arrange

        //Act
        var score = value.TryGetScore();

        //Assert
        Assert.That(score, Is.EqualTo(expectedScore));
    }

    [TestCase("2023-09-01T00:00:00Z", 6.38291232E+17d)]
    [TestCase("2023-10-01T00:00:00Z", 6.38317152E+17d)]
    public void TryGetScore_IsDateTimeOffset_WorksAsExpected(DateTimeOffset value, double expectedScore)
    {
        //Arrange

        //Act
        var score = value.TryGetScore();

        //Assert
        Assert.That(score, Is.EqualTo(expectedScore));
    }

    [TestCase(123456, 123456)]
    [TestCase(789.123, 789.123)]
    public void TryGetScore_IsDouble_WorksAsExpected(double value, double expectedScore)
    {
        //Arrange

        //Act
        var score = value.TryGetScore();

        //Assert
        Assert.That(score, Is.EqualTo(expectedScore));
    }

    [TestCase(123456, 123456)]
    [TestCase(-789123, -789123)]
    public void TryGetScore_IsInt_WorksAsExpected(int value, double expectedScore)
    {
        //Arrange

        //Act
        var score = value.TryGetScore();

        //Assert
        Assert.That(score, Is.EqualTo(expectedScore));
    }

    [TestCase(123, 123)]
    [TestCase(-789, -789)]
    public void TryGetScore_IsShort_WorksAsExpected(short value, double expectedScore)
    {
        //Arrange

        //Act
        var score = value.TryGetScore();

        //Assert
        Assert.That(score, Is.EqualTo(expectedScore));
    }

    [TestCase("First", 2.1625452640489079E+185d)]
    [TestCase("Second", 7.4773056432507901E+247d)]
    public void TryGetScore_IsString_WorksAsExpected(string value, double expectedScore)
    {
        //Arrange

        //Act
        var score = value.TryGetScore();

        //Assert
        Assert.That(score, Is.EqualTo(expectedScore));
    }
}
