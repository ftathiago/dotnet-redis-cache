namespace PocCache.Cache.Tests.Fixtures;

public static class Fixture
{
    public static readonly Faker _faker = new("pt_BR");
    public static Faker GetFaker() => _faker;
}
