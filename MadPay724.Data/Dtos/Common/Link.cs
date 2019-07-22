using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MadPay724.Data.Dtos.Common
{
   public class Link
   {
       public const string GetMethod = "GET";

       public static Link To(string routeName, object RoutValues = null)
           => new Link
           {
               RouteName = routeName,
               RouteValues = RoutValues,
               Method = GetMethod,
               Relations = null
           };

        [JsonProperty(Order = -4)]
        public string Href { get; set; }


        [JsonProperty(Order = -3,NullValueHandling = NullValueHandling.Ignore,DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(GetMethod)]
        public string Method { get; set; }


        [JsonProperty(Order = -2,PropertyName = "rel", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Relations { get; set; }

        [JsonIgnore]
        public string RouteName { get; set; }
        [JsonIgnore]
        public object RouteValues { get; set; }

   }
}
