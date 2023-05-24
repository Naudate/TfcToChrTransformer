using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NLog;
using NLog.Web;
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
                string xlsxFilePath = Path.Combine(outputFolder, "output.xlsx");

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlFilePath);

                var program = new Program();

                // Get all distinct tag names from the XML document
                HashSet<string> tagNames = program.GetDistinctTagNames(xmlDocument);

                HashSet<string> list = program.GetDistinct(xmlDocument);

                // Create the XLSX document
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(xlsxFilePath, SpreadsheetDocumentType.Workbook))
                {
                    // Add a workbook part to the document
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Add a worksheet part to the workbook part
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add the worksheet part to the workbook
                    Sheets sheets = document.WorkbookPart!.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                    sheets.Append(sheet);

                    // Get the sheet data
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

                    // Create the header row with XML tags
                    Row headerRow = new Row();

                    // Add tag names as headers
                    foreach (string tagName in tagNames)
                    {
                        headerRow.Append(CreateCell(tagName));
                    }

                    // Add the header row to the sheet data
                    sheetData.Append(headerRow);

                    logger.Info("Headers have been created");

                    var rootNode = xmlDocument.DocumentElement;
                    var shop = rootNode?.SelectNodes("//SHOPITEM");
                    uint count = 2;
                    foreach (XmlNode? shopItemNode in shop!)
                    {
                        Row valueRow = new Row() { RowIndex = count };
                        foreach (var item in list)
                        {
                            var index = tagNames.ToList().IndexOf(item);
                            var columnName = GetColumnName(index + 1);
                            columnName += count;
                            logger.Info(columnName);
                            valueRow.Append(program.CreateCellWithColumn(shopItemNode?.SelectSingleNode(item)?.InnerText!, columnName));
                        }

                        var paramNode = shopItemNode?.SelectSingleNode("PARAMETERS");
                        foreach (XmlNode parameter in paramNode?.SelectNodes("Parameter")!)
                        {
                            string paramName = parameter.SelectSingleNode("ParamName")!.InnerText;
                            string paramValue = parameter.SelectSingleNode("ParamValue")!.InnerText;
                            string paramUnit = parameter.SelectSingleNode("ParamUnit")!.InnerText;
                            var index = tagNames.ToList().IndexOf(paramName);
                            var columnName = GetColumnName(index);
                            columnName += count;
                            valueRow.Append(program.CreateCellWithColumn($"{paramValue} {paramUnit}", columnName));
                        }
                        
                        sheetData.Append(valueRow);
                        count++;
                    }                    

                    // Save the workbook
                    workbookPart.Workbook.Save();

                    // Close the document
                    document.Dispose();
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
                LogManager.Shutdown();
            }
        }

        private HashSet<string> GetDistinctTagNames(XmlDocument xmlDocument)
        {
            HashSet<string> tagNames = new HashSet<string>();

            // Get all elements in the XML document
            XmlNodeList elements = xmlDocument.GetElementsByTagName("*");

            // Iterate over the elements and add the tag names to the set
            foreach (XmlNode element in elements)
            {
                
                if (element.Name.Equals("ParamName"))
                {
                    tagNames.Add(element.InnerText);
                }
                else
                {
                    if (element.Name != "Parameter" && element.Name != "PARAMETERS" && element.Name != "ParamValue" && element.Name != "ParamUnit" && element.Name != "SHOP" && element.Name != "SHOPITEM")
                    {
                        tagNames.Add(element.Name);
                    }
                }
            }

            return tagNames;
        }

        private HashSet<string> GetDistinct(XmlDocument xmlDocument)
        {
            HashSet<string> tagNames = new HashSet<string>();

            // Get all elements in the XML document
            XmlNodeList elements = xmlDocument.GetElementsByTagName("*");

            // Iterate over the elements and add the tag names to the set
            foreach (XmlNode element in elements)
            {
                if (element.Name != "Parameter" && element.Name != "PARAMETERS" && element.Name != "ParamValue" && element.Name != "ParamName" && element.Name != "ParamUnit" && element.Name != "SHOP" && element.Name != "SHOPITEM")
                {
                    tagNames.Add(element.Name);
                }
            }

            return tagNames;
        }

        private static Cell CreateCell(string xmlTag)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;

            // Add the XML tag as the value of the cell
            cell.CellValue = new CellValue(xmlTag);

            return cell;
        }

        // Helper method to get the column name from the column index
        private static string GetColumnName(int columnIndex)
        {
            string columnName = string.Empty;
            int dividend = columnIndex;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }

        private Cell CreateCellWithColumn(string tagName, string columnName)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellReference = columnName;

            // Add the XML tag as the value of the cell
            cell.CellValue = new CellValue(tagName);

            return cell;
        }
    }
}