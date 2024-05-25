using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherGizmos.SqlMonitor.UnitTests.Services.Caching;

internal class CacheEntitySetFilterTests
{
    private IServiceProvider _serviceProvider;
    private ICacheEntitySet<FakeEntity> _cache;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = Global.CreateScope();
        _cache = _serviceProvider.GetRequiredService<IDistributedRecordCache>().EntitySet<FakeEntity>();
    }

    [Test]
    public void IsBetween_WorksAsExpected()
    {
        //Arrange

        //Act
        var filterProperty = _cache.Where(e => e.Name);
        filterProperty.IsBetween("First", "Second");

        var filterPropertyCast = (ICacheEntitySetFilter<FakeEntity>)filterProperty;
        var startInclusivity = filterPropertyCast.GetStartInclusivity();
        var startScore = filterPropertyCast.GetStartScore();
        var endInclusivity = filterPropertyCast.GetEndInclusivity();
        var endScore = filterPropertyCast.GetEndScore();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(startInclusivity, Is.True);
            Assert.That(startScore, Is.EqualTo("First".TryGetScore()));
            Assert.That(endInclusivity, Is.True);
            Assert.That(endScore, Is.EqualTo("Second".TryGetScore()));
        });
    }

    [Test]
    public void IsEqualTo_WorksAsExpected()
    {
        //Arrange

        //Act
        var filterProperty = _cache.Where(e => e.Name);
        filterProperty.IsEqualTo("Test");

        var filterPropertyCast = (ICacheEntitySetFilter<FakeEntity>)filterProperty;
        var startInclusivity = filterPropertyCast.GetStartInclusivity();
        var startScore = filterPropertyCast.GetStartScore();
        var endInclusivity = filterPropertyCast.GetEndInclusivity();
        var endScore = filterPropertyCast.GetEndScore();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(startInclusivity, Is.True);
            Assert.That(startScore, Is.EqualTo("Test".TryGetScore()));
            Assert.That(endInclusivity, Is.True);
            Assert.That(endScore, Is.EqualTo("Test".TryGetScore()));
        });
    }

    [Test]
    public void IsGreaterThan_WorksAsExpected()
    {
        //Arrange

        //Act
        var filterProperty = _cache.Where(e => e.Name);
        filterProperty.IsGreaterThan("Test");

        var filterPropertyCast = (ICacheEntitySetFilter<FakeEntity>)filterProperty;
        var startInclusivity = filterPropertyCast.GetStartInclusivity();
        var startScore = filterPropertyCast.GetStartScore();
        var endInclusivity = filterPropertyCast.GetEndInclusivity();
        var endScore = filterPropertyCast.GetEndScore();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(startInclusivity, Is.False);
            Assert.That(startScore, Is.EqualTo("Test".TryGetScore()));
            Assert.That(endInclusivity, Is.True);
            Assert.That(endScore, Is.EqualTo(double.MaxValue));
        });
    }

    [Test]
    public void IsGreaterThanOrEqualTo_WorksAsExpected()
    {
        //Arrange

        //Act
        var filterProperty = _cache.Where(e => e.Name);
        filterProperty.IsGreaterThanOrEqualTo("Test");

        var filterPropertyCast = (ICacheEntitySetFilter<FakeEntity>)filterProperty;
        var startInclusivity = filterPropertyCast.GetStartInclusivity();
        var startScore = filterPropertyCast.GetStartScore();
        var endInclusivity = filterPropertyCast.GetEndInclusivity();
        var endScore = filterPropertyCast.GetEndScore();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(startInclusivity, Is.True);
            Assert.That(startScore, Is.EqualTo("Test".TryGetScore()));
            Assert.That(endInclusivity, Is.True);
            Assert.That(endScore, Is.EqualTo(double.MaxValue));
        });
    }

    [Test]
    public void IsLessThan_WorksAsExpected()
    {
        //Arrange

        //Act
        var filterProperty = _cache.Where(e => e.Name);
        filterProperty.IsLessThan("Test");

        var filterPropertyCast = (ICacheEntitySetFilter<FakeEntity>)filterProperty;
        var startInclusivity = filterPropertyCast.GetStartInclusivity();
        var startScore = filterPropertyCast.GetStartScore();
        var endInclusivity = filterPropertyCast.GetEndInclusivity();
        var endScore = filterPropertyCast.GetEndScore();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(startInclusivity, Is.True);
            Assert.That(startScore, Is.EqualTo(double.MinValue));
            Assert.That(endInclusivity, Is.False);
            Assert.That(endScore, Is.EqualTo("Test".TryGetScore()));
        });
    }

    [Test]
    public void IsLessThanOrEqualTo_WorksAsExpected()
    {
        //Arrange

        //Act
        var filterProperty = _cache.Where(e => e.Name);
        filterProperty.IsLessThanOrEqualTo("Test");

        var filterPropertyCast = (ICacheEntitySetFilter<FakeEntity>)filterProperty;
        var startInclusivity = filterPropertyCast.GetStartInclusivity();
        var startScore = filterPropertyCast.GetStartScore();
        var endInclusivity = filterPropertyCast.GetEndInclusivity();
        var endScore = filterPropertyCast.GetEndScore();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(startInclusivity, Is.True);
            Assert.That(startScore, Is.EqualTo(double.MinValue));
            Assert.That(endInclusivity, Is.True);
            Assert.That(endScore, Is.EqualTo("Test".TryGetScore()));
        });
    }

    [Test]
    public void Where_NotIndexedProperty_ThrowsInvalidOperationException()
    {
        //Arrange

        //Act

        //Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            _cache.Where(e => e.Description).IsEqualTo("None");
        });
    }

    public class FakeEntity
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Indexed]
        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
