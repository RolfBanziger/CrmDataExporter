using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;
using XrmToolBox.Extensibility.Args;
using Microsoft.Crm.Sdk.Messages;
using System.IO;
using System.Xml;
using Microsoft.Xrm.Sdk;

namespace UniversityOfWestminster.MsCrm.DataExporter
{
    public partial class MainUserControl : PluginControlBase, IStatusBarMessenger
    {
        public MainUserControl()
        {
            InitializeComponent();
        }

        private string Message { get; set; }

        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void ProcessFetchXmlQuery(string query)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Fetching data...",
                AsyncArgument = query,
                IsCancelable = true,

                Work = (bw, e) =>
                {
                var action = new FetchXmlAction(Service);
                    action.MessageChanged += delegate (object sender, EventArgs eventArgs)
                    {
                        if (SendMessageToStatusBar != null)
                        {
                            SendMessageToStatusBar(this, new StatusBarMessageEventArgs(
                                ((FetchXmlAction)sender).Message) );
                        }
                    };

                    e.Result = action.RunFetchXmlQuery(e.Argument.ToString());

                },
                PostWorkCallBack = e =>
                {
                    if (e.Error == null)
                    {
                        ProcessResponse((IList<Entity>)e.Result);
                    }
                    else
                    {
                        MessageBox.Show(this, "An error occured: " + e.Error.Message, "Error", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }
            });

        }
        
        private void ClearTheGrid()
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
        }

        private void ProcessResponse(IList<Entity> result)
        {
            ClearTheGrid();
            if (result.Count == 0) return;
            List<string> attributes = new List<string>();
            foreach (var entity in result)
            {
                foreach (string a in entity.Attributes.Keys)
                {
                    if (!attributes.Contains(a))
                    {
                        attributes.Add(a);
                        DataGridViewColumn column = new DataGridViewTextBoxColumn();
                        column.Name = a;
                        column.HeaderText = a;
                        grid.Columns.Add(column);
                    }
                }
            }
            foreach (var entity in result)
            {
                grid.Rows.Add(EnumerateEntityValues(entity, attributes).ToArray());
            }
            tcMain.SelectedTab = pageDatatable;
        }

        private IEnumerable<string> EnumerateEntityValues(Entity entity, IEnumerable<string> attributes)
        {
            //foreach (string key in entity.Attributes.Keys)
            foreach (string key in attributes)
            {
                if (!entity.Contains(key))
                {
                    yield return null;
                    continue;
                }
                object value = entity[key];
                yield return FormatObject(value);
                
            }
        }

        private string FormatObject(object value)
        {
            if (value is EntityReference)
                return FormatObject((EntityReference)value);
            else if (value is EntityCollection)
                return FormatObject((EntityCollection)value);
            else if (value is Guid)
                return FormatObject((Guid)value);
            else if (value is OptionSetValue)
                return ((OptionSetValue)value).Value.ToString();
            else if (value is Money)
                return ((Money)value).Value.ToString();
            else if (value is DateTime)
                return ((DateTime)value).ToString("s");
            else if (value is AliasedValue)
                return FormatObject((AliasedValue)value);
            else
                return value.ToString();
        }
        

        private string FormatObject(AliasedValue value)
        {
            return FormatObject(value.Value);
        }

        private string FormatObject(EntityCollection value)
        {
            StringBuilder formattedValue = new StringBuilder();
            for (int i = 0; i< value.Entities.Count; i++)
            {
                var e = value.Entities[i];
                formattedValue.Append(e.Id.ToString("B"));
                if (i < value.Entities.Count-1)
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
            if ((value.LogicalName == "systemuser")||(value.LogicalName == "team")) return value.Name;
            else return FormatObject(value.Id);
        }

        private void tsbWhoAmI_Click(object sender, EventArgs e)
        {
            ProcessFetchXmlQuery(txtRequest.Text);
        }

        private void asCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogCSV.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SaveDGVtoCSV(saveFileDialogCSV.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void SaveDGVtoCSV(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName,false, Encoding.Unicode))
            {
                // Header row
                for (int idx = 0; idx < grid.ColumnCount; idx++)
                {
                    writer.Write(FormatCsvCellValue(grid.Columns[idx].Name));
                    if (idx < grid.ColumnCount - 1)
                    {
                        writer.Write(',');
                    }
                }
                writer.WriteLine();
                // Data rows
                foreach (DataGridViewRow row in grid.Rows)
                {
                    for (int idx = 0; idx < row.Cells.Count; idx++)
                    {
                        writer.Write(FormatCsvCellValue(row.Cells[idx].Value));
                        if (idx < row.Cells.Count - 1)
                        {
                            writer.Write(',');
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Formats a value to be save to written in a CSV file.
        /// </summary>
        /// <param name="value">The value of a cell</param>
        /// <returns>The value with any control chars escaped.</returns>
        private string FormatCsvCellValue(object value)
        {
            if (value == null) return string.Empty;
            string formattedValue = value.ToString();
            // escape double quotes
            formattedValue = formattedValue.Replace("\"", "\"\"");
            if (formattedValue.Contains("\r") ||
                formattedValue.Contains("\n") ||
                formattedValue.Contains("\"") ||
                formattedValue.Contains(",") ||
                formattedValue.Contains(";")
                )
            {
                formattedValue = string.Format("\"{0}\"", formattedValue);
            }
            return formattedValue;
        }


    }
}