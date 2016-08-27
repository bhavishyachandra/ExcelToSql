using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelToSql
{
    public partial class MainPage : Form
    {
        private readonly string _connectionString;
        private string _selectedPath;

        public MainPage()
        {
            InitializeComponent();
            _connectionString = ConfigurationManager.ConnectionStrings["destinationDatabaseConnectionString"].ConnectionString;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void browserButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };

            DialogResult result = fbd.ShowDialog();

            _selectedPath = fbd.SelectedPath;

            folderSelectedLabel.Text = _selectedPath;
        }

        private void StatusUpdate(string message)
        {
            statusTextBox.AppendText(DateTime.Now + " : " + message + Environment.NewLine);
        }

        private string SaveFileToDatabase(string filePath)
        {

            try
            {
                string excelConnectionString =
                        $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 12.0\"";
                using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
                {
                    excelConnection.Open();
                    var dtSchema = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    var sheet1 = dtSchema.Rows[0].Field<string>("TABLE_NAME");
                    using (OleDbCommand cmd = new OleDbCommand(ConfigurationManager.AppSettings["excelQuery"] + "[" + sheet1 + "]", excelConnection))
                    {
                        using (OleDbDataReader dReader = cmd.ExecuteReader())
                        {
                            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(_connectionString))
                            {
                                sqlBulk.DestinationTableName = ConfigurationManager.AppSettings["destinationTable"];
                                if (dReader != null) sqlBulk.WriteToServer(dReader);
                            }
                        }
                    }
                }

                return "Successfully inserted " + Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_selectedPath))
                {
                    string[] files = Directory.GetFiles(_selectedPath);

                    foreach (string file in files)
                    {
                        if (Path.GetExtension(file) == null) continue;
                        if (Path.GetExtension(file).Equals(".xls") || Path.GetExtension(file).Equals(".xlsx"))
                        {
                            StatusUpdate(SaveFileToDatabase(file));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.StatusUpdate(ex.Message);
            }

        }

        private void MainPage_Load(object sender, EventArgs e)
        {

        }
    }
}
