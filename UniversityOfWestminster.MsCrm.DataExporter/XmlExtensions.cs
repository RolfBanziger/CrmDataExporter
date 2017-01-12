using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UniversityOfWestminster.MsCrm.DataExporter
{
    public static class XmlExtensions
    {

        public static string GetAttributeValue(this XmlNode node, string attribute)
        {
            var attr = node.Attributes[attribute];
            if (attr == null) return null;
            return attr.Value;
        }
    }
}
