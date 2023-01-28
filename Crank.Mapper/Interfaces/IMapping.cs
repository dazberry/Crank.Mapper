namespace Crank.Mapper.Interfaces
{
    public interface IMapping
    {

    }

    public interface IMapping<TSource, TDestination> : IMapping
    {
        TDestination Map(TSource source, TDestination destination = default);
    }
}
