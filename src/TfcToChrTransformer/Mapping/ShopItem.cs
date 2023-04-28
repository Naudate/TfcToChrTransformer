using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TfcToChrTransformer.Mapping
{
    public class ShopItem
    {
        public string? ITEM_ID { get; set; }
        public string? PRODUCTNAME { get; set; }
        public string? PRODUCT { get; set; }
        public string? SUMMARY { get; set; }
        public string? DESCRIPTION { get; set; }
        public string? DESCRIPTION2 { get; set; }
        public string? URL { get; set; }
        public string? CATEGORYTEXT1 { get; set; }
        public string? ON_STOCK { get; set; }
        public string? PRICE { get; set; }
        public string? WARRANTY { get; set; }
        public string? DELIVERY_DATE { get; set; }
        public string? IMGURL1 { get; set; }
        public string? IMGURL2 { get; set; }
        public string? IMGURL5 { get; set; }
        public string? IMGURL6 { get; set; }
        public string? ENERGY_ARROW { get; set; }
        public string? DATA_SHEET { get; set; }
        public string? USER_MANUAL { get; set; }
        public string? SPARE_PART_LIST { get; set; }
        public string? ENERGY_LABEL { get; set; }
        public string? NEW_ITEM { get; set; }
        public string? ACTION { get; set; }
        public Parameters? PARAMETERS { get; set; }
        public string? EAN { get; set; }
        public string? PRODUCTNO { get; set; }
    }
}
