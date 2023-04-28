using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TfcToChrTransformer.Mapping;

namespace TfcToChrTransformer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            XmlSerializer serializer = new XmlSerializer(typeof(Shop));
            try
            {
                logger.Info("init main - application starting...");
                logger.Info("Enter file path : ");
                var input = Console.ReadLine();
                var doc = new XmlDocument();
                doc.Load(input);
                var rootNode = doc.DocumentElement;
                var mapping = Shop.ToMapping(rootNode);
                // Construct the full path to the output folder
                string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");

                // Create the output folder if it doesn't already exist
                Directory.CreateDirectory(outputFolder);

                // Construct the full path to the file
                string fileName = Path.Combine(outputFolder, "output.xml");

                if (File.Exists(fileName))
                {
                    logger.Info("File already exist");
                    using TextWriter writer = new StreamWriter(fileName);
                    serializer.Serialize(writer, mapping);
                }
                else
                {
                    using FileStream fs = File.Create(fileName);
                    fs.Close();
                    using TextWriter writer = new StreamWriter(fileName);
                    serializer.Serialize(writer, mapping);
                }

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