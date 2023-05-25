using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NLog;
using NLog.Web;
using System.Text;
using System.Xml;

namespace TfcToChrTransformer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("init main - application starting...");
                // Load the XML data
                string xmlFilePath = "C:\\Users\\n8bm\\Downloads\\GetXmlFeed.xml";
                // Construct the full path to the output folder
                string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");

                // Create the output folder if it doesn't already exist
                Directory.CreateDirectory(outputFolder);

                // Construct the full path to the file
                string csvFilePath = Path.Combine(outputFolder, "output.csv");

                XmlToCsvConverter converter = new XmlToCsvConverter();

                converter.ConvertXmlToCsv(xmlFilePath, csvFilePath);

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
                LogManager.Shutdown();
            }
        }
    }
}