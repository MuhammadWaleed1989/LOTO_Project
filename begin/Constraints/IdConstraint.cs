using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace ExploreCalifornia.Constraints
{
    public class IdConstraint : IHttpRouteConstraint
    {
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, 
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            object value;

            if (!values.TryGetValue(parameterName, out value) || value == null) 
                return false;

            try
            {
                return Convert.ToInt32(value) > 0;
            }
            catch
            {
                return false;
            }

        }
    }
}