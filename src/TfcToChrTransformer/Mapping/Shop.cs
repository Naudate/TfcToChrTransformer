using NLog.Web;
using Stef.Validation;
using System.Xml;

namespace TfcToChrTransformer.Mapping
{
    public class Shop
    {
        public List<ShopItem>? SHOPITEM { get; set; }


        public static Shop ToMapping(XmlNode? rootNode)
        {
            Guard.NotNull(rootNode);
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var shop = new Shop();
            var ListShopItems = new List<ShopItem>();
            foreach (XmlNode? shopItemNode in rootNode?.SelectNodes("//SHOPITEM"))
            {
                logger.Info($"Getting SHOPITEM with ITEM_ID = {shopItemNode?.SelectSingleNode("ITEM_ID")?.InnerText}");

                var PARAMETERS = new Parameters();
                var PARAMETER = new List<Parameter>();
                var paramNode = shopItemNode?.SelectSingleNode("PARAMETERS");
                foreach (XmlNode parameter in paramNode?.SelectNodes("Parameter"))
                {
                    var param = new Parameter()
                    {
                        ParamName = parameter?.SelectSingleNode("ParamName")?.InnerText,
                        ParamUnit = parameter?.SelectSingleNode("ParamUnit")?.InnerText,
                        ParamValue = parameter?.SelectSingleNode("ParamValue")?.InnerText,
                    };
                    PARAMETER.Add(param);
                }

                PARAMETERS.PARAMETER = PARAMETER;

                var shopItem = new ShopItem()
                {
                    ITEM_ID = shopItemNode?.SelectSingleNode("ITEM_ID")?.InnerText,
                    PRODUCTNAME = shopItemNode?.SelectSingleNode("PRODUCTNAME")?.InnerText,
                    PRODUCT = shopItemNode?.SelectSingleNode("PRODUCT")?.InnerText,
                    SUMMARY = shopItemNode?.SelectSingleNode("SUMMARY")?.InnerText,
                    DESCRIPTION = shopItemNode?.SelectSingleNode("DESCRIPTION")?.InnerText,
                    DESCRIPTION2 = shopItemNode?.SelectSingleNode("DESCRIPTION2")?.InnerText,
                    URL = shopItemNode?.SelectSingleNode("URL")?.InnerText,
                    CATEGORYTEXT1 = shopItemNode?.SelectSingleNode("CATEGORYTEXT1")?.InnerText,
                    ON_STOCK = shopItemNode?.SelectSingleNode("ON_STOCK")?.InnerText,
                    PRICE = shopItemNode?.SelectSingleNode("PRICE")?.InnerText,
                    WARRANTY = shopItemNode?.SelectSingleNode("WARRANTY")?.InnerText,
                    IMGURL1= shopItemNode?.SelectSingleNode("IMGURL1")?.InnerText,
                    IMGURL2= shopItemNode?.SelectSingleNode("IMGURL2")?.InnerText,
                    IMGURL5= shopItemNode?.SelectSingleNode("IMGURL5")?.InnerText,
                    IMGURL6= shopItemNode?.SelectSingleNode("IMGURL6")?.InnerText,
                    ENERGY_ARROW = shopItemNode?.SelectSingleNode("ENERGY_ARROW")?.InnerText,
                    DATA_SHEET = shopItemNode?.SelectSingleNode("DATA_SHEET")?.InnerText,
                    USER_MANUAL = shopItemNode?.SelectSingleNode("USER_MANUAL")?.InnerText,
                    SPARE_PART_LIST = shopItemNode?.SelectSingleNode("SPARE_PART_LIST")?.InnerText,
                    ENERGY_LABEL = shopItemNode?.SelectSingleNode("ENERGY_LABEL")?.InnerText,
                    NEW_ITEM = shopItemNode?.SelectSingleNode("NEW_ITEM")?.InnerText,
                    ACTION = shopItemNode?.SelectSingleNode("ACTION")?.InnerText,
                    PARAMETERS = PARAMETERS,
                    EAN = shopItemNode?.SelectSingleNode("EAN")?.InnerText,
                    PRODUCTNO = shopItemNode?.SelectSingleNode("PRODUCTNO")?.InnerText,
                    DELIVERY_DATE = shopItemNode?.SelectSingleNode("DELIVERY_DATE")?.InnerText,
                };
                ListShopItems.Add(shopItem);
            }
            shop.SHOPITEM = ListShopItems;
            return shop;
        }
    }
}
