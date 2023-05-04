using NLog.Web;
using System.Xml;
using System.Xml.Serialization;

namespace TfcToChrTransformer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Some ivalid characters....
            string invalidCharacters = "~`!@#$%^&*()-+=[]\\{}|;:'\",./<>?\u00A0\u2019 ";
            // NLog: setup the logger first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("init main - application starting...");
                logger.Info("Enter file path : ");
                var input = Console.ReadLine();
                var doc = new XmlDocument();
                doc.Load(input);
                var rootNode = doc.DocumentElement;
                var shop = rootNode?.SelectNodes("//SHOPITEM");
                foreach (XmlNode? shopItemNode in shop)
                {
                    var paramNode = shopItemNode?.SelectSingleNode("PARAMETERS");
                    foreach (XmlNode parameter in paramNode?.SelectNodes("Parameter"))
                    {
                        string paramName = parameter.SelectSingleNode("ParamName").InnerText.Trim();
                        string paramValue = parameter.SelectSingleNode("ParamValue").InnerText.Trim();
                        string paramUnit = parameter.SelectSingleNode("ParamUnit").InnerText.Trim();

                        foreach (char invalidChar in invalidCharacters)
                        {
                            paramName = paramName.Replace(invalidChar, '_');
                        }

                        string elementName = paramName;

                        XmlNode newElement = doc.CreateElement(elementName);
                        newElement.InnerText = paramValue + paramUnit;

                        parameter.ParentNode.ReplaceChild(newElement, parameter);
                    }
                }

                // Construct the full path to the output folder
                string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");

                // Create the output folder if it doesn't already exist
                Directory.CreateDirectory(outputFolder);

                // Construct the full path to the file
                string fileName = Path.Combine(outputFolder, "output.xml");

                //Save the file
                doc.Save(fileName);


                logger.Info("File created !");
            }
            catch (Exception ex)
            {
                // NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
    }
}