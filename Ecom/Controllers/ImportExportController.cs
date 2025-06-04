using Ecom.DataAccess;
using Ecom.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
namespace Ecom.Controllers
{
    public class ImportExportController : Controller
    {
        private readonly string _xmlPath;
        private readonly string _xsdPath;
        private readonly string _dtdPath;
        private readonly DatabaseHelper _databaseHelper;
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "Admin";
        }
        public ImportExportController(IWebHostEnvironment webHostEnvironment)
        {
            _xmlPath = Path.Combine(webHostEnvironment.WebRootPath, "Schemas", "Products.xml");
            _xsdPath = Path.Combine(webHostEnvironment.WebRootPath, "Schemas", "Products.xsd");
            _dtdPath = Path.Combine(webHostEnvironment.WebRootPath, "Schemas", "Products.dtd");

            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_xmlPath));

            string connstring = "Server=localhost;Database=ecom;User Id=root;Password='';Port=3306;";
            _databaseHelper = new DatabaseHelper(connstring);

        }
         



            public IActionResult Index()
        {

            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }
            string query = @"SELECT p.*, c.CategoryName 
                           FROM Products p 
                           LEFT JOIN Categories c ON p.CategoryID = c.CategoryID 
                           WHERE p.Visible = 1
                           ORDER BY p.CreatedAt DESC";

            List<Product> products = new List<Product>();
            DataTable dt = _databaseHelper.SelectQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    ProductID = int.Parse(row["ProductID"].ToString()),
                    ProductName = row["ProductName"].ToString(),
                    Description = row["Description"]?.ToString(),
                    Price = decimal.Parse(row["Price"].ToString()),
                    Stock = int.Parse(row["Stock"].ToString()),
                    CategoryID = row["CategoryID"] != DBNull.Value ? int.Parse(row["CategoryID"].ToString()) : null,
                    Image = row["Image"]?.ToString(),
                    Visible = bool.Parse(row["Visible"].ToString()),
                    CreatedAt = DateTime.Parse(row["CreatedAt"].ToString()),
                    CategoryName = row["CategoryName"]?.ToString()
                });
            }
            return View(products);
        }

        // Enhanced Export to XML with better error handling
        public IActionResult ExportToXML()
        {
            try
            {
                string query = @"SELECT p.*, c.CategoryName 
                               FROM Products p 
                               LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                               ORDER BY p.ProductID";

                DataTable dt = _databaseHelper.SelectQuery(query);

                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No products found to export" });
                }

                XmlDocument doc = new XmlDocument();
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(declaration);

                // Add DTD reference
                XmlDocumentType docType = doc.CreateDocumentType("Products", null, "Products.dtd", null);
                doc.AppendChild(docType);

                XmlElement root = doc.CreateElement("Products");
                doc.AppendChild(root);

                foreach (DataRow row in dt.Rows)
                {
                    XmlElement product = doc.CreateElement("Product");

                    // Helper method to create elements safely
                    AppendElement(doc, product, "ProductID", row["ProductID"].ToString());
                    AppendElement(doc, product, "ProductName", SanitizeXmlContent(row["ProductName"].ToString()));
                    AppendElement(doc, product, "Description", SanitizeXmlContent(row["Description"]?.ToString() ?? ""));
                    AppendElement(doc, product, "Price", row["Price"].ToString());
                    AppendElement(doc, product, "Stock", row["Stock"].ToString());
                    AppendElement(doc, product, "CategoryID", row["CategoryID"]?.ToString() ?? "");
                    AppendElement(doc, product, "CategoryName", SanitizeXmlContent(row["CategoryName"]?.ToString() ?? ""));
                    AppendElement(doc, product, "Image", SanitizeXmlContent(row["Image"]?.ToString() ?? ""));
                    AppendElement(doc, product, "Visible", row["Visible"].ToString().ToLower());
                    AppendElement(doc, product, "CreatedAt", DateTime.Parse(row["CreatedAt"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss"));

                    root.AppendChild(product);
                }

                doc.Save(_xmlPath);

                return Json(new
                {
                    success = true,
                    message = $"Successfully exported {dt.Rows.Count} products to XML",
                    fileName = "Products.xml"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Export error: {ex.Message}" });
            }
        }

        // Enhanced Import from XML with integrated validation
        [HttpPost]
        public IActionResult ImportFromXML(IFormFile xmlFile)
        {
            try
            {
                string xmlPath = _xmlPath;
                bool isTemporaryFile = false;

                // If a file is uploaded, use it instead of the default XML file
                if (xmlFile != null && xmlFile.Length > 0)
                {
                    // Validate file extension
                    if (!Path.GetExtension(xmlFile.FileName).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                    {
                        return Json(new { success = false, message = "Please select a valid XML file." });
                    }

                    // Create temporary file path
                    string tempFileName = $"temp_import_{DateTime.Now:yyyyMMddHHmmss}.xml";
                    xmlPath = Path.Combine(Path.GetDirectoryName(_xmlPath), tempFileName);
                    isTemporaryFile = true;

                    // Save uploaded file
                    using (var stream = new FileStream(xmlPath, FileMode.Create))
                    {
                        xmlFile.CopyTo(stream);
                    }
                }
                else if (!System.IO.File.Exists(_xmlPath))
                {
                    return Json(new { success = false, message = "No file selected and no existing XML file found. Please select a file or export data first." });
                }

                // INTEGRATED VALIDATION - Validate XML against both DTD and XSD
                var dtdValidation = ValidateXMLWithDTD(xmlPath);
                var xsdValidation = ValidateXMLWithXSD(xmlPath);

                // Create validation summary
                var validationSummary = new
                {
                    DTDValid = dtdValidation.IsValid,
                    DTDError = dtdValidation.ErrorMessage,
                    XSDValid = xsdValidation.IsValid,
                    XSDError = xsdValidation.ErrorMessage,
                    OverallValid = dtdValidation.IsValid || xsdValidation.IsValid
                };

                // If both validations fail, return detailed error information
                if (!validationSummary.OverallValid)
                {
                    // Clean up temp file if it was created
                    if (isTemporaryFile && System.IO.File.Exists(xmlPath))
                    {
                        System.IO.File.Delete(xmlPath);
                    }

                    return Json(new
                    {
                        success = false,
                        message = "XML validation failed. Please check the file format and try again.",
                        validation = validationSummary,
                        details = new List<string>
                {
                    !string.IsNullOrEmpty(dtdValidation.ErrorMessage) ? $"DTD Validation: {dtdValidation.ErrorMessage}" : null,
                    !string.IsNullOrEmpty(xsdValidation.ErrorMessage) ? $"XSD Validation: {xsdValidation.ErrorMessage}" : null
                }.Where(x => x != null).ToList()
                    });
                }

                // Proceed with import if validation passes
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                XmlNodeList products = doc.SelectNodes("/Products/Product");
                int importedCount = 0;
                int skippedCount = 0;
                int totalCount = products?.Count ?? 0;
                List<string> errors = new List<string>();
                List<string> warnings = new List<string>();

                if (totalCount == 0)
                {
                    // Clean up temp file if it was created
                    if (isTemporaryFile && System.IO.File.Exists(xmlPath))
                    {
                        System.IO.File.Delete(xmlPath);
                    }

                    return Json(new
                    {
                        success = false,
                        message = "No products found in the XML file.",
                        validation = validationSummary
                    });
                }

                foreach (XmlNode node in products)
                {
                    try
                    {
                        // Validate required fields
                        string productName = node["ProductName"]?.InnerText?.Trim();
                        if (string.IsNullOrWhiteSpace(productName))
                        {
                            skippedCount++;
                            errors.Add("Skipped product with empty or missing ProductName");
                            continue;
                        }

                        string priceText = node["Price"]?.InnerText?.Trim();
                        if (string.IsNullOrWhiteSpace(priceText) || !decimal.TryParse(priceText, out decimal price) || price < 0)
                        {
                            skippedCount++;
                            errors.Add($"Skipped product '{productName}': Invalid or negative price");
                            continue;
                        }

                        string stockText = node["Stock"]?.InnerText?.Trim();
                        if (string.IsNullOrWhiteSpace(stockText) || !int.TryParse(stockText, out int stock) || stock < 0)
                        {
                            skippedCount++;
                            errors.Add($"Skipped product '{productName}': Invalid or negative stock");
                            continue;
                        }

                        // Check if product already exists
                        string existsQuery = $"SELECT COUNT(*) FROM Products WHERE ProductName = '{productName.Replace("'", "''")}'";
                        DataTable existsResult = _databaseHelper.SelectQuery(existsQuery);

                        if (existsResult.Rows.Count > 0 && int.Parse(existsResult.Rows[0][0].ToString()) > 0)
                        {
                            skippedCount++;
                            warnings.Add($"Product '{productName}' already exists and was skipped");
                            continue;
                        }

                        // Parse optional fields
                        string description = node["Description"]?.InnerText?.Trim() ?? "";

                        int? categoryId = null;
                        string categoryIdText = node["CategoryID"]?.InnerText?.Trim();
                        if (!string.IsNullOrEmpty(categoryIdText) && int.TryParse(categoryIdText, out int parsedCategoryId))
                        {
                            categoryId = parsedCategoryId;
                        }

                        string image = node["Image"]?.InnerText?.Trim() ?? "";

                        bool visible = true; // Default to true
                        string visibleText = node["Visible"]?.InnerText?.Trim();
                        if (!string.IsNullOrEmpty(visibleText))
                        {
                            bool.TryParse(visibleText, out visible);
                        }

                        // Insert the product
                        string insertQuery = $@"INSERT INTO Products (ProductName, Description, Price, Stock, CategoryID, Image, Visible, CreatedAt) 
                                      VALUES ('{productName.Replace("'", "''")}', 
                                             '{description.Replace("'", "''")}', 
                                             {price}, 
                                             {stock}, 
                                             {(categoryId.HasValue ? categoryId.Value.ToString() : "NULL")}, 
                                             '{image.Replace("'", "''")}', 
                                             {(visible ? 1 : 0)}, 
                                             NOW())";

                        _databaseHelper.ExecuteQuery(insertQuery);
                        importedCount++;
                    }
                    catch (Exception ex)
                    {
                        skippedCount++;
                        string productName = node["ProductName"]?.InnerText ?? "Unknown";
                        errors.Add($"Error importing product '{productName}': {ex.Message}");
                    }
                }

                // Clean up temporary file if it was created
                if (isTemporaryFile && System.IO.File.Exists(xmlPath))
                {
                    System.IO.File.Delete(xmlPath);
                }

                // Prepare detailed response
                var allMessages = new List<string>();
                if (warnings.Any()) allMessages.AddRange(warnings.Take(5));
                if (errors.Any()) allMessages.AddRange(errors.Take(5));

                string successMessage = $"Import completed successfully! {importedCount} of {totalCount} products imported";
                if (skippedCount > 0)
                {
                    successMessage += $", {skippedCount} skipped";
                }

                // Add validation info to success message
                var validationInfo = new List<string>();
                if (validationSummary.DTDValid) validationInfo.Add("DTD validation passed");
                if (validationSummary.XSDValid) validationInfo.Add("XSD validation passed");
                if (validationInfo.Any())
                {
                    successMessage += $" ({string.Join(", ", validationInfo)})";
                }

                return Json(new
                {
                    success = true,
                    message = successMessage,
                    validation = validationSummary,
                    statistics = new
                    {
                        total = totalCount,
                        imported = importedCount,
                        skipped = skippedCount,
                        errors = errors.Count,
                        warnings = warnings.Count
                    },
                    details = allMessages.Take(10).ToList() // Limit details to prevent overwhelming UI
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Import failed: {ex.Message}",
                    validation = new
                    {
                        DTDValid = false,
                        XSDValid = false,
                        OverallValid = false,
                        DTDError = "",
                        XSDError = ""
                    }
                });
            }
        }

        // Enhanced XSD Validation with optional file path
        private (bool IsValid, string ErrorMessage) ValidateXMLWithXSD(string xmlFilePath = null)
        {
            try
            {
                string pathToValidate = xmlFilePath ?? _xmlPath;

                if (!System.IO.File.Exists(pathToValidate))
                    return (false, "XML file not found");

                if (!System.IO.File.Exists(_xsdPath))
                {
                    CreateXSDFile();
                }

                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("", _xsdPath);

                XmlDocument doc = new XmlDocument();
                doc.Load(pathToValidate);
                doc.Schemas = schemas;

                bool isValid = true;
                string errorMessage = "";

                doc.Validate((sender, e) => {
                    isValid = false;
                    errorMessage = e.Message;
                });

                return (isValid, errorMessage);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // DTD Validation with optional file path
        private (bool IsValid, string ErrorMessage) ValidateXMLWithDTD(string xmlFilePath = null)
        {
            try
            {
                string pathToValidate = xmlFilePath ?? _xmlPath;

                if (!System.IO.File.Exists(pathToValidate))
                    return (false, "XML file not found");

                if (!System.IO.File.Exists(_dtdPath))
                {
                    CreateDTD();
                }

                XmlDocument doc = new XmlDocument();

                // Enable DTD processing
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.ValidationType = ValidationType.DTD;

                bool isValid = true;
                string errorMessage = "";

                settings.ValidationEventHandler += (sender, e) => {
                    isValid = false;
                    errorMessage = e.Message;
                };

                using (XmlReader reader = XmlReader.Create(pathToValidate, settings))
                {
                    while (reader.Read()) { }
                }

                return (isValid, errorMessage);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Enhanced XSD Creation
        private void CreateXSDFile()
        {
            string xsdContent = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<xs:schema xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""Products"">
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""Product"" maxOccurs=""unbounded"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""ProductID"" type=""xs:positiveInteger""/>
              <xs:element name=""ProductName"" type=""xs:string""/>
              <xs:element name=""Description"" type=""xs:string"" minOccurs=""0""/>
              <xs:element name=""Price"" type=""xs:decimal""/>
              <xs:element name=""Stock"" type=""xs:nonNegativeInteger""/>
              <xs:element name=""CategoryID"" type=""xs:positiveInteger"" minOccurs=""0""/>
              <xs:element name=""CategoryName"" type=""xs:string"" minOccurs=""0""/>
              <xs:element name=""Image"" type=""xs:string"" minOccurs=""0""/>
              <xs:element name=""Visible"" type=""xs:boolean""/>
              <xs:element name=""CreatedAt"" type=""xs:dateTime""/>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";

            System.IO.File.WriteAllText(_xsdPath, xsdContent);
        }

        // Enhanced DTD Creation
        public IActionResult CreateDTD()
        {
            try
            {
                string dtdContent = @"<!ELEMENT Products (Product*)>
<!ELEMENT Product (ProductID, ProductName, Description?, Price, Stock, CategoryID?, CategoryName?, Image?, Visible, CreatedAt)>
<!ELEMENT ProductID (#PCDATA)>
<!ELEMENT ProductName (#PCDATA)>
<!ELEMENT Description (#PCDATA)>
<!ELEMENT Price (#PCDATA)>
<!ELEMENT Stock (#PCDATA)>
<!ELEMENT CategoryID (#PCDATA)>
<!ELEMENT CategoryName (#PCDATA)>
<!ELEMENT Image (#PCDATA)>
<!ELEMENT Visible (#PCDATA)>
<!ELEMENT CreatedAt (#PCDATA)>";

                System.IO.File.WriteAllText(_dtdPath, dtdContent);
                return Json(new { success = true, message = "DTD file created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error creating DTD: {ex.Message}" });
            }
        }

        // Comprehensive XML Validation endpoint
        public IActionResult ValidateXML()
        {
            try
            {
                var xsdValidation = ValidateXMLWithXSD();
                var dtdValidation = ValidateXMLWithDTD();

                return Json(new
                {
                    XSDValid = xsdValidation.IsValid,
                    XSDError = xsdValidation.ErrorMessage,
                    DTDValid = dtdValidation.IsValid,
                    DTDError = dtdValidation.ErrorMessage,
                    OverallValid = xsdValidation.IsValid || dtdValidation.IsValid
                });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        // Helper methods
        private void AppendElement(XmlDocument doc, XmlElement parent, string elementName, string value)
        {
            XmlElement element = doc.CreateElement(elementName);
            element.InnerText = value ?? "";
            parent.AppendChild(element);
        }

        private string SanitizeXmlContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return "";

            return content.Replace("&", "&amp;")
                         .Replace("<", "&lt;")
                         .Replace(">", "&gt;")
                         .Replace("\"", "&quot;")
                         .Replace("'", "&apos;");
        }
    }
}