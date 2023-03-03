using System;
using System.Collections.Generic;
using System.Text;

namespace Crank.Mapper
{
    public class MapDestinationNullResultException : Exception
    {
        public MapDestinationNullResultException()
            : base("The MapDestination.Result Map value is null. Invoking the MapDestination.Map delegate will pass a null value in the mapAction delegate.")
        {

        }
    }
}
