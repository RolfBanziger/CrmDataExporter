using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XrmToolBox.Extensibility.Args;

namespace UniversityOfWestminster.MsCrm.DataExporter
{
    /// <summary>
    /// This class contains all the logic to run the Fetch XML query.
    /// </summary>
    class FetchXmlAction
    {

        private IOrganizationService Service { get; set; }
        public FetchXmlAction(IOrganizationService service)
        {
            this.Service = service;
        }

        public event EventHandler<EventArgs> MessageChanged;

        protected void OnMessageChanged(EventArgs e)
        {
            if (MessageChanged != null)
                MessageChanged(this, e);
        }

        private string message = null;
        /// <summary>
        /// Gets the current Status message
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnMessageChanged(EventArgs.Empty);
            }
        }

        public IList<Entity> RunFetchXmlQuery(string fetchXmlQuery)
        {
            List<Entity> entities = new List<Entity>();
            // Define the fetch attributes.
            int fetchCount = 500;
            // Initialize the page number.
            int pageNumber = 1;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;
            while (true)
            {
                // Build fetchXml string with the placeholders.
                string xml = CreateXml(fetchXmlQuery, pagingCookie, pageNumber, fetchCount);

                // Excute the fetch query and get the xml result.
                RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(xml)
                };

                var result = ((RetrieveMultipleResponse)Service.Execute(fetchRequest));
                EntityCollection collection = result.EntityCollection;
                entities.AddRange(collection.Entities);
                // Check for morerecords, if it returns 1.
                if (collection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    pageNumber++;
                    pagingCookie = collection.PagingCookie;
                    Message = string.Format("{0} records and still going...", entities.Count);
                }
                else
                {
                    Message = string.Format("{0} records", entities.Count);
                    // If no more records in the result nodes, exit the loop.
                    break;
                }

            }
            
            return entities;
        }

        public string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }
    }
}
