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
using Microsoft.Xrm.Sdk.Metadata;

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
                    action.RunFetchXmlQuery(e.Argument.ToString());
                    e.Result = action;

                },
                PostWorkCallBack = e =>
                {
                    if (e.Error == null)
                    {
                        ProcessResponse((FetchXmlAction)e.Result);
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

        private void SetupGridColumns(IDictionary<string, AttributeMetadata> attributes)
        {
            foreach (KeyValuePair< string, AttributeMetadata > a in attributes)
            {
                AddColumn(a.Key);
                if (a.Value.AttributeType == AttributeTypeCode.Lookup)
                {
                    AddColumn(GetLookupColumnName(a.Key));
                }
            }
        }

        private string GetLookupColumnName(string key)
        {
            return string.Format("{0}.Name", key);
        }

        private void AddColumn(string name)
        {
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = name;
            grid.Columns.Add(column);
        }

        private void ProcessResponse(FetchXmlAction result)
        {
            ClearTheGrid();
            SetupGridColumns(result.Attributes);
            foreach (var entity in result.Entities)
            {
                grid.Rows.Add(EnumerateEntityValues(entity, result).ToArray());
            }
            tcMain.SelectedTab = pageDatatable;
        }

        private IEnumerable<string> EnumerateEntityValues(Entity entity, FetchXmlAction fetcher)
        {
            //foreach (string key in entity.Attributes.Keys)
            foreach (KeyValuePair<string, AttributeMetadata> a in fetcher.Attributes)
            {
                if (!entity.Contains(a.Key))
                {
                    yield return null;
                    continue;
                }
                object value = entity[a.Key];
                yield return fetcher.FormatObject(value, a.Key);
                if (a.Value.AttributeType == AttributeTypeCode.Lookup)
                {
                    yield return ((EntityReference)value).Name;
                }
            }
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
            using (StreamWriter writer = new StreamWriter(fileName,false, Encoding.UTF8))
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