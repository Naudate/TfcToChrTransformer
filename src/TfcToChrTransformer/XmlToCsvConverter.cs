using System.Xml;
using System.Text;

public class XmlToCsvConverter
{
    public void ConvertXmlToCsv(string xmlFilePath, string csvFilePath)
    {
        // Load the XML document
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(xmlFilePath);

        // Get the root element
        XmlElement root = xmlDocument.DocumentElement;

        HashSet<string> tagNames = GetDistinctTagNames(xmlDocument);

        // Create a StringBuilder to store the CSV content
        StringBuilder csvContent = new StringBuilder();

        // Get the column headers from the XML
        foreach (string tagName in tagNames)
        {
            csvContent.Append(tagName + ";");
        }
        csvContent.AppendLine();

        XmlNodeList dataRows = root.ChildNodes;
        // Get the data rows from the XML
        foreach (XmlNode row in dataRows)
        {
            string[] Line = new string[tagNames.Count];
            for (int i = 0; i < Line.Length; i++)
            {
                Line[i] = ";";
            }
            int count = 0;
            XmlNodeList rowData = row.ChildNodes;
            foreach (XmlNode data in rowData)
            {
                if (data.Name != "PARAMETERS")
                {
                    if (count == tagNames.ToList().IndexOf(data.Name))
                    {
                        Line[count] = data.InnerXml.Replace(";",",") + ";";
                    }
                }
                count++;
            }

            var paramNode = row?.SelectSingleNode("PARAMETERS");
            foreach (XmlNode parameter in paramNode?.SelectNodes("Parameter")!)
            {
                string paramName = parameter.SelectSingleNode("ParamName")!.InnerText;
                string paramValue = parameter.SelectSingleNode("ParamValue")!.InnerText;
                string paramUnit = parameter.SelectSingleNode("ParamUnit")!.InnerText;
                var index = tagNames.ToList().IndexOf(paramName.Replace(";", ","));
                Line[index] = paramValue.Replace(";", ",") + paramUnit.Replace(";", ",") + ";";
            }

            csvContent.Append(string.Join("" , Line));
            csvContent.AppendLine();
        }

        ClearCsv(csvFilePath);

        // Write the CSV content to a file
        File.WriteAllText(csvFilePath, csvContent.ToString(), Encoding.UTF8);
    }

    private static HashSet<string> GetDistinctTagNames(XmlDocument xmlDocument)
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

    public static void ClearCsv(string csvFilePath)
    {
        // Clear the contents of the CSV file by overwriting it with an empty string
        File.WriteAllText(csvFilePath, string.Empty);
    }
}
