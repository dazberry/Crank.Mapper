namespace Crank.Mapper.Interfaces
{
    public interface IMapping
    {

    }

    public interface IMapping<TSource, TDestination> : IMapping
    {
        TDestination Map(TSource source, TDestination destination = default);
    }

    public interface IMapping<TSource, TSource2, TDestination> : IMapping
    {
        TDestination Map(TSource source, TSource2 source2, TDestination destination = default);
    }
}
