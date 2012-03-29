using System.Collections.Generic;
using System.Dynamic;
using System.Web.Routing;

namespace Ideastrike.Helpers
{
    public static class ObjectExtensions
    {
        //http://stackoverflow.com/questions/5120317/dynamic-anonymous-type-in-razor-causes-runtimebinderexception/5670899#5670899
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}