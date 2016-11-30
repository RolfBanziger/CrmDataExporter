using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
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

        public IList<Entity> Entities { get; private set; }

        public void RunFetchXmlQuery(string fetchXmlQuery)
        {
            List<Entity> entities = new List<Entity>();
            // Define the fetch attributes.
            int fetchCount = 500;
            // Initialize the page number.
            int pageNumber = 1;
            // Specify the current paging cookie. For retrieving the first page, 
            // pagingCookie should be null.
            string pagingCookie = null;
            XmlDocument doc = CreateXml(fetchXmlQuery, pagingCookie, pageNumber, fetchCount);
            FillEntityMetadata(doc);
            while (true)
            {
                RetrieveMultipleRequest fetchRequest = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(ConvertXmlDocumentToString(doc))
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
                    doc = CreateXml(fetchXmlQuery, pagingCookie, pageNumber, fetchCount);
                }
                else
                {
                    Message = string.Format("{0} records", entities.Count);
                    // If no more records in the result nodes, exit the loop.
                    break;
                }
            }
            Entities = entities;
        }

        /// <summary>
        /// Contains all attributes. For entity attributes, contains all names, for linked entity, contains alias.Attribute name.
        /// </summary>
        public IDictionary<string, AttributeMetadata> Attributes { get; private set; }

        public IDictionary<string, IDictionary<int, string>> OptionSetLabels { get; private set; }

        private void FillEntityMetadata(XmlDocument doc)
        {
            Attributes = new Dictionary<string, AttributeMetadata>();
            OptionSetLabels = new Dictionary<string, IDictionary<int, string>>();
            XmlNode node = doc.SelectSingleNode("/fetch/entity");
            ExtractAttributes(node);
            foreach ( XmlNode node2 in doc.SelectNodes("//link-entity"))
            {
                ExtractAttributes(node2);
            }
        }

        private void ExtractAttributes(XmlNode node)
        {
            string entity = node.GetAttributeValue("name");
            string alias = node.GetAttributeValue("alias");
            foreach (XmlNode attribute in node.SelectNodes("./attribute/@name"))
            {
                string attributeName = attribute.Value;
                string qualifiedAttribute = (string.IsNullOrEmpty(alias)) ? attributeName : string.Format("{0}.{1}", alias, attributeName);
                var attReq = new RetrieveAttributeRequest();
                attReq.EntityLogicalName = entity;
                attReq.LogicalName = attributeName;
                attReq.RetrieveAsIfPublished = true;

                var attResponse = (RetrieveAttributeResponse)Service.Execute(attReq);

                Attributes.Add(qualifiedAttribute, attResponse.AttributeMetadata);
                if (attResponse.AttributeMetadata is EnumAttributeMetadata)
                {
                    var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;
                    var labels = new Dictionary<int, string>();
                    foreach (var option in attMetadata.OptionSet.Options)
                    {
                        labels.Add(option.Value.GetValueOrDefault(), option.Label.UserLocalizedLabel.Label);
                    }
                    OptionSetLabels.Add(qualifiedAttribute, labels);
                }
            }
        }

        private XmlDocument CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        private XmlDocument CreateXml(XmlDocument doc, string cookie, int page, int count)
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

            return doc;
        }

        private string ConvertXmlDocumentToString(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        public string FormatObject(object value, string attribute)
        {
            if (value is EntityReference)
                return FormatObject((EntityReference)value);
            else if (value is EntityCollection)
                return FormatObject((EntityCollection)value);
            else if (value is Guid)
                return FormatObject((Guid)value);
            else if (value is OptionSetValue)
                return FormatObject((OptionSetValue)value, attribute);
            else if (value is Money)
                return ((Money)value).Value.ToString();
            else if (value is DateTime)
                return ((DateTime)value).ToString("s");
            else if (value is AliasedValue)
                return FormatObject((AliasedValue)value, attribute);
            else
                return value.ToString();
        }

        private string FormatObject(OptionSetValue value, string attribute)
        {
            return OptionSetLabels[attribute][value.Value];
        }

        private string FormatObject(AliasedValue value, string attribute)
        {
            return FormatObject(value.Value, attribute);
        }

        private string FormatObject(EntityCollection value)
        {
            StringBuilder formattedValue = new StringBuilder();
            for (int i = 0; i < value.Entities.Count; i++)
            {
                var e = value.Entities[i];
                formattedValue.Append(e.Id.ToString("B"));
                if (i < value.Entities.Count - 1)
                {
                    formattedValue.Append(", ");
                }
            }
            return formattedValue.ToString();
        }

        private string FormatObject(Guid value)
        {
            return value.ToString("B");
        }

        private string FormatObject(EntityReference value)
        {
            if ((value.LogicalName == "systemuser") || (value.LogicalName == "team")) return value.Name;
            return FormatObject(value.Id);
        }


    }
}
