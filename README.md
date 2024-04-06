
# Crank.Mapper

**crank**

*verb (used without object)*
to turn a crank, as in starting an automobile engine.

*noun*
Informal. an ill-tempered, grouchy person.

---
**What**
Crank.Mapper is not a mapper, or at least an automatic mapper. It is a set of classes that allow mapping based syntax such as *Mapper.Map<Source, Destination>(source)* but without implementing the mappings themselves. Mappings need to be manually implemented and registered.

**Why**
If you like mapping syntax, but perhaps you're working on a smaller project or API and find it difficult to justify using the more full feature mappers. Mapping are manually implemented, so there is no difficult mapping rules or conventions. 

**How**
The mapping *engine* is simply a list of IMapping<TSource, TDestination> implementations. When invoking a map function, the engine will search for a match based on the source and destination types. If a match is found it will invoke the matching implementation

**IMapping<TSource, TDestination>**
The IMapping interface contracts to a single Map method. 

    public interface IMapping<TSource, TDestination> : IMapping
    {
    	TDestination Map(TSource source, TDestination destination = default);
    }

For each individual mapping you wish to perform an implementation of this interface needs to be created. *As selection is based on source and destination types, each implementation must be a unique pair of type parameters*.

Supplying an instance of TDestination is optional,  however note that it is not generically constrained to *new()*. **This is by design**.* If you wish to create an instance by default, that instance needs to be instantiated in the mapping implementation.

**Mapper**
Instantiating the mapper requires a set of mappings, and optionally a set of options that changed default behaviour. 

    public Mapper(IEnumerable<IMapping> mappings, MapperOptions mapperOptions = default)

Depending on your use case, you may wish to register the mappings and the mapper itself with some form of dependency injection implementation. The mapper is purposely DI agnostic, but (at time of writing) sample code is included for registering with the Microsoft's dotnet core DI implementation. 

**The mapping syntax**
Crank mapper implements two mapping conventions.
- single source to single destination
This is the classic mapper.Map<source, destination> syntax. 

    > var userModel = _mapper.Map<UserEntity, UserModel>(userEntity);

- multiple source to single destination

    > var userEntity = _mapper.MapTo<UserEntity>()
                    .MapFrom(userModel)
                    .MapFrom(sessionModel)
                    .Result;
                    

**Why ideally you should only create a destination instance if required during mapping**
It really depends how you want to use the mapper, but the safest way **is to create if needed, but do not replace if supplied**. If you are doing partial or multiple mappings (mapTo) it might be safer to consider this pattern. If you simply use map for single type to type mappings it likely doesn't matter and you can choose not to do so. 

    public UserEntity Map(UserModel source, UserEntity destination = null)
    {
        destination ??= new UserEntity(); //<----- only create if null

        destination.PartitionId = "user";
        destination.RowKey = $"{source.UserId:n}";
        destination.Username = source.Username;
        destination.UserAccess = (int)source.UserAccess;

        return destination;
    }

**MapNew (introduced in v1.0.3)**
When MapTo is called it creates a MapDestination<TDestination> struct which supports the multiple source to single destination mapping example above.

        public MapDestination<TDestination> MapTo<TDestination>();
        public MapDestination<TDestination> MapTo<TDestination>(TDestination destination);

If MapTo is called with no parameters the initial TDestination value will be null or default. This is likely not an issue if the next method called is a MapFrom and you are following the *create if needed, but do not replace if supplied* mantra.

    public MapDestination<TDestination> MapFrom<TSource>(TSource source, bool throwMapNotFoundException = true);

~~However if you first call Map, the value passed to the mapAction delegate will be null or default.~~  
1.0.4 If a null value is passed to the mapAction delegate, a **MapDestinationNullResultException** exception will be thrown. This behaviour can be changed by setting the **IgnoreNullResultWhenCallingDestinationMap** MapperOptions flag.

    public MapDestination<TDestination> Map(Action<TDestination> mapAction);

This may not be desired behaviour. 

    Mapper.MapTo<EditViewModel>()
    .Map(x => {
       x.SessionId = _session.Id; // <--- null reference exception
       etc.
    });


    
An alternative is to pass an instance of TDestination:

    Mapper.MapTo<EditViewModel>(new EditViewModel())
    .Map(x => {
       x.SessionId = _session.Id;
       etc.
    });

Alternatively if TDestination is generically new-able, the *new* **MapNew** method can be called instead of MapTo.

    Mapper.MapNew<EditViewModel>()
    .Map(x => {
       x.SessionId = _session.Id;
       etc.
    });

**MapBoth (introduced in v1.0.5)**
A new overloaded Map method has been added to the Mapper (*and MapFromBoth to MapDestination*) that allows two source objects to be passed to a mapping together. 

The general idea is to allow some sort of runtime configuration to be passed with the source object for the mapping.

    var userModel = _mapper.Map<UserEntity, EntityToModelMapConfig, UserModel>(userEntity, entityToModelMapConfig);


**MapperOptions**
When creating a Mapper instance, some additional options are available to change the mapper behaviour.

    public struct MapperOptions
    {
        public bool ThrowMappingNotFoundException { get; set; }
        public bool DisallowDuplicateMappingTypes { get; set; }
        public Action<Type, Type> MappingNotFoundEvent { get; set; }
        public bool IgnoreNullResultWhenCallingDestinationMap { get; set; } //new 1.0.4
    }

| Default behaviour  | Changed Behaviour |
|--|--|
| **ThrowMappingNotFoundException**  |  |
|When a mapping is not found: returns false on TryGetMapping or default on Map.|A MappingNotFoundException is thrown instead. May change behaviour of MapTo also ** |
|**DisallowDuplicateMappingTypes**|  |
|Allow duplicate IMapping types, but when attempting to Map, presently only the first one registered in invoked.| A DuplicateMappingException is thrown when attempting to create the mapper. |
|**MappingNotFoundEvent**||
|If set - invoked if a mapping is not found. Invokes before a ThrowMappingNotFoundException if triggered.| n/a |
|**IgnoreNullResultWhenCallingDestinationMap**||
|**new 1.0.4**. After calling MapTo, calling Map where the MapDestination Result value is null - will cause a **MapDestinationNullResultException** to be  thrown.|Set to true, a null value will be passed through the Map delegate.|

** **MapTo and ThrowMappingNotFoundException**
The MapTo method includes a default throwMappingNotFound argument, that by default is set to true. However the ThrowMappingNotFoundException options flag may change this behaviour depending on how the flags are set.


|flag|parameter  |result|
|--|--|--|
|true | true | throws exception |
|false| true | throws exception |
|true| false | **throws exception** |
|false| false | does not do mapping and fails silently |

