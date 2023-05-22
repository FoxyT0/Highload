
using System.Runtime.Serialization;
using CoreWCF.OpenApi.Attributes;

[DataContract(Name = "PropertiesContract", Namespace = "http://gateway.spacebattle.ru")]
public class PropertiesContract
{
		[DataMember(Name = "property", Order = 1)]
        [OpenApiProperty(Description = "Property for command contract")]
        public string property { get; set; }
}

