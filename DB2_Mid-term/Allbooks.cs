using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace DB2_Mid_term
{
    public partial class Allbooks : Form
    {

        DataSet DS = new DataSet();
        public Allbooks()
        {
            
            InitializeComponent();
            DS.ReadXml("XMLFile.xml");
            this.dataGridView1.DataSource = DS.Tables[0];
            this.btnAdd.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow R = DS.Tables[0].NewRow();
            R[3] = txtISBN.Text;
            R[4] = txtAuthorID.Text;
            R[0] = txtTitle.Text;
            R[2] = txtPublishr.Text;
            R[1] = this.dateTimePicker1.Value;
            DS.Tables[0].Rows.Add(R);
            DS.WriteXml("XMLFile.xml");
            MessageBox.Show("Added Successfully!", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.btnAdd.Enabled = false;
            this.button2.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.btnAdd.Enabled = true;
                 txtISBN.Clear();
                 txtAuthorID.Clear();
                 txtTitle.Clear(); ;
                 txtPublishr.Clear();
                 this.button2.Enabled = false;

        }

        private void btndel_Click(object sender, EventArgs e)
        {
            for (int i = 0 ; i < DS.Tables[0].Rows.Count ; i++)
            {
                if (txtISBN.Text==DS.Tables[0].Rows[i]["ISBN"].ToString())
                {
                    DS.Tables[0].Rows[i].Delete();
                    DS.WriteXml("XMLFile.xml");
                }
            }
            MessageBox.Show("Deleted Successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            txtISBN.Text = this.dataGridView1.CurrentRow.Cells[3].Value.ToString();
            txtAuthorID.Text = this.dataGridView1.CurrentRow.Cells[4].Value.ToString();
            txtTitle.Text = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtPublishr.Text = this.dataGridView1.CurrentRow.Cells[2].Value.ToString();
         dateTimePicker1.Text = this.dataGridView1.CurrentRow.Cells[1].Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xls)|*.xls";
            sfd.FileName = "Book.xls";
            if (sfd.ShowDialog() == DialogResult.OK)
            {

                this.Cursor = Cursors.WaitCursor;
                // Copy DataGridView results to clipboard
                copyAlltoClipboard();

                object misValue = System.Reflection.Missing.Value;
                Excel.Application xlexcel = new Excel.Application();

                xlexcel.DisplayAlerts = false; // Without this you will get two confirm overwrite prompts
                Excel.Workbook xlWorkBook = xlexcel.Workbooks.Add(misValue);
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // Format column D as text before pasting results, this was required for my data
                Excel.Range rng = xlWorkSheet.get_Range("D:D").Cells;
                rng.NumberFormat = "@";

                // Paste clipboard results to worksheet range
                Excel.Range CR = (Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

                // For some reason column A is always blank in the worksheet. ¯\_(ツ)_/¯
                // Delete blank column A and select cell A1
                Excel.Range delRng = xlWorkSheet.get_Range("A:A").Cells;
                delRng.Delete(Type.Missing);
                xlWorkSheet.get_Range("A1").Select();

                // Save the excel file under the captured location from the SaveFileDialog
                xlWorkBook.SaveAs(sfd.FileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlexcel.DisplayAlerts = true;
                xlWorkBook.Close(true, misValue, misValue);
                xlexcel.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlexcel);

                // Clear Clipboard and DataGridView selection
                Clipboard.Clear();
                dataGridView1.ClearSelection();
                this.Cursor = Cursors.Default;
                MessageBox.Show("Exported Successfully!", "Excel Exported");

            }
        }
        private void copyAlltoClipboard()
        {
            dataGridView1.SelectAll();
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
