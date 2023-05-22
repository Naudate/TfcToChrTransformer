using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
            // string invalidCharacters = "~`!@#$%^&*()-+=[]\\{}|;:'\",./<>?\u00A0\u2019 ";
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
                string xlsxFilePath = Path.Combine(outputFolder, "output.xlsx");

                // Create a new XLSX file
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(xlsxFilePath, SpreadsheetDocumentType.Workbook))
                {
                    // Add a WorkbookPart to the document
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add a Sheets object to the Workbook
                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                    // Append a new sheet and associate it with the WorksheetPart
                    Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                    sheets.Append(sheet);

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(xmlFilePath);

                    // Get the root element of the XML document
                    XmlElement rootElement = xmlDocument.DocumentElement;

                    // Traverse the XML and populate the spreadsheet
                    TraverseXmlElement(worksheetPart.Worksheet.GetFirstChild<SheetData>(), rootElement);


                    // Save and close the document
                    workbookPart.Workbook.Save();
                    spreadsheetDocument.Close();
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

        // Recursive method to traverse XML elements and populate the spreadsheet
        private static void TraverseXmlElement(SheetData sheetData, XmlElement xmlElement)
        {
            // Create a new row for each XML element
            Row row = new Row();

            // Create a new cell for the element's tag name
            Cell tagCell = new Cell() { DataType = CellValues.String, CellValue = new CellValue(xmlElement.Name) };
            row.Append(tagCell);

            // Create a new cell for the element's inner text
            Cell valueCell = new Cell() { DataType = CellValues.String, CellValue = new CellValue(xmlElement.InnerText) };
            row.Append(valueCell);

            // Append the row to the sheet data
            sheetData.Append(row);

            // Traverse child elements recursively
            foreach (XmlNode childNode in xmlElement.ChildNodes)
            {
                if (childNode is XmlElement childElement)
                {
                    TraverseXmlElement(sheetData, childElement);
                }
            }
        }
    }
}