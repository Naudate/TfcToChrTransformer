using System.Xml.Serialization;

namespace TfcToChrTransformer.Mapping
{
    public class Parameter
    {
        public string? ParamName { get; set; }
        public string? ParamUnit { get; set; }
        public string? ParamValue { get; set; }
    }
}