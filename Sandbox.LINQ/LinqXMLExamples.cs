using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sandbox.LINQ
{
    /// <summary>
    /// Learning exercise for LINQ to XML queries.
    /// </summary>
    public static class LinqXMLExamples
    {
        public static void Run()
        {
            Console.WriteLine("LINQ XML examples");
            Console.WriteLine("------------------------------");
            Console.WriteLine("");

            XLinq1();
            XLinq2();
            XLinq3();
            XLinq4();
            XLinq5();
            XLinq6();
            XLinq7();
            XLinq8();
            XLinq9();
            XLinq10();
            XLinq11();
            XLinq12();

            Console.WriteLine("Example complete. Press a key to proceed.");
            Console.ReadKey();
            Console.WriteLine("");
        }

        private static string path = Path.GetFullPath(
            Path.Combine(System.Environment.CurrentDirectory, @"..\..\..\Sandbox.LINQ\Data\"));

        private static void XLinq1()
        {
            // Load document from a string
            string xml = "<book price='100' isbn='1002310'>" +
                            "<title>XClarity Samples</title>" +
                            "<author>Matt</author>" +
                         "</book>";
            XDocument doc = XDocument.Parse(xml);
            Console.WriteLine(doc);

            // Save document to a file
            using (var writer = new StreamWriter(System.Environment.CurrentDirectory + @"\xml.txt", false))
            {
                writer.Write(doc.ToString());
            }

            // Load document from a file
            XDocument doc2 = XDocument.Load(System.Environment.CurrentDirectory + @"\xml.txt");
            Console.WriteLine(doc2);

            Console.ReadLine();
        }

        private static void XLinq2()
        {
            // Construct an XElement
            string xml = "<purchaseOrder price='100'>" +
                "<item price='50'>Motor</item>" +
                "<item price='50'>Cable</item>" +
              "</purchaseOrder>";
            XElement po = XElement.Parse(xml);
            Console.WriteLine(po);

            Console.ReadLine();
        }

        private static void XLinq3()
        {
            // Document creation example
            XDocument myDocument =
            new XDocument(
              new XElement("configuration",
                new XElement("system.web",
                  new XElement("membership",
                    new XElement("providers",
                      new XElement("add",
                        new XAttribute("name",
                                       "WebAdminMembershipProvider"),
                        new XAttribute("type",
                                       "System.Web.Administration.WebAdminMembershipProvider")))),
                  new XElement("httpModules",
                    new XElement("add",
                      new XAttribute("name",
                                      "WebAdminModule"),
                      new XAttribute("type",
                                      "System.Web.Administration.WebAdminModule"))),
                  new XElement("authentication",
                    new XAttribute("mode", "Windows")),
                  new XElement("authorization",
                    new XElement("deny",
                      new XAttribute("users", "?"))),
                  new XElement("identity",
                    new XAttribute("impersonate", "true")),
                  new XElement("trust",
                    new XAttribute("level", "full")),
                  new XElement("pages",
                    new XAttribute("validationRequest", "true")))));

            Console.WriteLine(myDocument);

            Console.ReadLine();
        }

        private static void XLinq4()
        {
            // Create a CDATA section
            XElement e = new XElement("Dump",
              new XCData("<dump>this is some xml</dump>"),
              new XText("some other text"));

            Console.WriteLine("Element Value: {0}", e.Value);
            Console.WriteLine("Text nodes collapsed!: {0}", e.Nodes().First());
            Console.WriteLine("CData preserved on serialization: {0}", e);

            Console.ReadLine();
        }

        private static void XLinq5()
        {
            XDocument doc = XDocument.Load(path + "customers.xml");

            // Select all customers
            foreach (XElement result in doc.Elements("Root").Elements("Customers"))
                Console.WriteLine(result);

            // Select all child elements of the first customer
            var query = doc.Element("Root").Element("Customers").Elements();
            foreach (XElement result in query)
                Console.WriteLine(result);

            // Select the first element
            var result2 = doc.Element("Root").Element("Customers");
            Console.WriteLine(result2);

            // Select an attribute from all elements
            var query2 = doc.Element("Root").Elements("Customers").Attributes("CustomerID");
            foreach (XAttribute result in query2)
                Console.WriteLine(result.Name + " = " + result.Value);

            Console.ReadLine();
        }

        private static void XLinq6()
        {
            XDocument doc = XDocument.Load(path + "customers.xml");

            // Filter query results using where clause
            var query =
                    from
                        c in doc.Element("Root")
                                .Elements("Customers")
                    where
                        c.Element("FullAddress")
                         .Element("Country")
                         .Value == "Germany"
                    select c;

            foreach (XElement result in query)
                Console.WriteLine(result);

            Console.ReadLine();
        }

        private static void XLinq7()
        {
            XDocument doc = XDocument.Load(path + "customers.xml");

            // Select all descendants of an element
            var query = doc.Descendants("ContactName");
            foreach (XElement result in query)
                Console.WriteLine(result);

            // Select all descendants of a type
            var query2 = doc.DescendantNodes().OfType<XText>().Select(t => t.Value);
            foreach (string result in query2)
                Console.WriteLine(result);

            Console.ReadLine();
        }

        private static void XLinq8()
        {
            XDocument doc = XDocument.Load(path + "customers.xml");

            // Select all ancestors
            XElement element1 = doc.Element("Root");
            XElement element2 = doc.Descendants("Customers")
                                .ElementAt(3);
            var query = from a in element1.AncestorsAndSelf()
                        from b in element2.AncestorsAndSelf()
                        where a == b
                        select a;

            Console.WriteLine(query.Any());

            Console.ReadLine();
        }

        private static void XLinq9()
        {
            XDocument doc = XDocument.Load(path + "bib.xml");

            // Union of two sequences of nodes
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Peter"));
            var books = b1.Union(b2);

            foreach (var b in books)
                Console.WriteLine(b);

            Console.ReadLine();
        }

        private static void XLinq10()
        {
            XDocument doc = XDocument.Load(path + "bib.xml");

            // Intersect of two sequences of nodes
            var b1 = doc.Descendants("book")
            .Where(b => b.Elements("author")
                            .Elements("first")
                            .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                        .Elements("first")
                                        .Any(f => (string)f == "Peter"));
            var books = b1.Intersect(b2);

            foreach (var b in books)
                Console.WriteLine(b);

            Console.ReadLine();
        }

        private static void XLinq11()
        {
            XElement elem = new XElement("customer",
                             new XElement("name", "jack"),
                             "this is an XElement",
                             new XComment("new customer"),
                             new XAttribute("id", "abc"));
            Console.WriteLine("Original element: {0}", elem);

            // Remove content of an element
            var temp = elem;
            temp.RemoveNodes();
            Console.WriteLine("RemoveNodes result: {0}", temp);

            // Remove all content
            temp = elem;
            temp.RemoveAll();
            Console.WriteLine("RemoveAll result: {0}", temp);

            // Remove attributes
            temp = elem;
            temp.RemoveAttributes();
            Console.WriteLine("RemoveAttributes result: {0}", temp);

            // Set attribute value
            temp = elem;
            temp.SetAttributeValue("id", "xyz");
            Console.WriteLine("SetAttributeValue result: {0}", temp);

            // Set element value
            temp = elem;
            temp.SetElementValue("name", "david");
            Console.WriteLine("SetElementValue result: {0}", temp);

            Console.ReadLine();
        }

        private static void XLinq12()
        {
            XDocument doc = XDocument.Load(path + "customers.xml");

            // Transform document into a table
            var header = new[]{                 
                    new XElement("th","#"),
                    new XElement("th",
                                 "customer id"),
                    new XElement("th",
                                 "contact name")};
            int index = 0;
            var rows =
                from
                    customer in doc.Descendants("Customers")
                select
                    new XElement("tr",
                                 new XElement("td",
                                              ++index),
                                 new XElement("td",
                                              (string)customer.Attribute("CustomerID")),
                                 new XElement("td",
                                              (string)customer.Element("ContactName")));

            XElement html = new XElement("html",
                                  new XElement("body",
                                    new XElement("table",
                                      header,
                                      rows)));
            Console.Write(html);
            Console.WriteLine("");

            Console.ReadLine();
        }
    }
}
