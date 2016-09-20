using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Media;
using Excel = Microsoft.Office.Interop.Excel;
namespace DB2_Mid_term
{
    public partial class Form1 : Form
    {
        DataSet DS;
        Listofbooks F;
        public Form1()
        {
            InitializeComponent();
            DS = new DataSet();
            DS.ReadXml("XMLFile.xml");
            this.dataGridView1.Columns.Add("bookISBN", "ISBN");
            this.dataGridView1.Columns.Add("bookauthorsID", "authors");
            this.dataGridView1.Columns.Add("bookTitle", "Title");
            this.dataGridView1.Columns.Add("bookyear", "Year");
            this.dataGridView1.Columns.Add("bookpublisher", "Publisher");
            this.dataGridView1.Columns.Add("authorfirstName", "First Name");
            this.dataGridView1.Columns.Add("authormiddleName", "Middle Name");
            this.dataGridView1.Columns.Add("authorlastName", "Last Name");
            this.dataGridView1.Columns.Add("authornationality", "Nationality");
            this.textBox1.Text = "";
            this.comboBox1.Items.Add("ISBN");
            this.comboBox1.Items.Add("Part of Book title");
            this.comboBox1.Items.Add("Author ID");
            this.comboBox1.Items.Add("Part of Author Last Name");
            this.comboBox1.Text = "ISBN";
            F = new Listofbooks();
            playSimpleSound();
        }

        private void playSimpleSound()
        {

            SoundPlayer simpleSound = new SoundPlayer("Sound.wav");


            simpleSound.Play();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.dataGridView1.Rows.Clear();
                var doc = XDocument.Parse(DS.GetXml());
                if (this.comboBox1.Text == "ISBN")
                {

                    var book = doc.Root.Elements("book")
                              .FirstOrDefault(b => (string)b.Attribute("ISBN") == this.textBox1.Text.ToString());
                    var authorId = book.Attribute("authors").Value.ToString();
                    var author = doc.Root.Elements("author")
                                   .FirstOrDefault(b => (string)b.Attribute("id") == authorId);
                    this.dataGridView1.Rows.Add
                                 (

                                               book.Attribute("ISBN").Value.ToString(),
                                               book.Attribute("authors").Value.ToString(),
                                               book.Element("title").Value.ToString(),
                                               book.Element("year").Value.ToString(),
                                               book.Element("publisher").Value.ToString(),
                                               author.Element("firstName").Value.ToString(),
                                               author.Element("middleName").Value.ToString(),
                                               author.Element("lastName").Value.ToString(),
                                               author.Element("nationality").Value.ToString()


                                );

                }
                else if (this.comboBox1.Text == "Part of Book title")
                {


                    string IDau = "", ISBN = "";
                    var query = from bookauthors in doc.Element("library").Elements("book")
                                where (bool)bookauthors.Element("title").Value.ToLower().StartsWith(this.textBox1.Text.ToLower())
                                select bookauthors.Attribute("authors").Value;
                    var query1 = from bookauthors in doc.Element("library").Elements("book")
                                 where (bool)bookauthors.Element("title").Value.ToLower().StartsWith(this.textBox1.Text.ToLower())
                                 select bookauthors.Attribute("ISBN").Value;

                    foreach (var item in query)
                    {
                        IDau = item;

                    }
                    foreach (var item in query1)
                    {
                        ISBN = item;

                    }

                    var book = doc.Root.Elements("book")
                              .FirstOrDefault(b => (string)b.Attribute("ISBN") == ISBN.ToString());
                    var authorId = book.Attribute("authors").Value.ToString();
                    var author = doc.Root.Elements("author")
                                   .FirstOrDefault(b => (string)b.Attribute("id") == IDau);
                    this.dataGridView1.Rows.Add
                                 (


                                               book.Attribute("ISBN").Value.ToString(),
                                               book.Attribute("authors").Value.ToString(),
                                               book.Element("title").Value.ToString(),
                                               book.Element("year").Value.ToString(),
                                               book.Element("publisher").Value.ToString(),
                                               author.Element("firstName").Value.ToString(),
                                               author.Element("middleName").Value.ToString(),
                                               author.Element("lastName").Value.ToString(),
                                               author.Element("nationality").Value.ToString()


                                );



                }
                else if (this.comboBox1.Text == "Author ID")
                {


                    F.label1.Text = null;

                    IEnumerable<string> BKTitle = from BK in doc.Descendants("book")
                                                  where (string)BK.Attribute("authors") == this.textBox1.Text
                                                  select BK.Element("title").Value.ToString();
                    foreach (var item in BKTitle)
                    {
                        // MessageBox.Show(item);
                        F.label1.Text += item;
                        F.label1.Text += "\n";
                    }
                    F.groupBox1.Text = "List books by AuthorId";
                    F.ShowDialog();

                }
                else if (this.comboBox1.Text == "Part of Author Last Name")
                {
                    F.label1.Text = null;

                    IEnumerable<string> authorname = from AN in doc.Element("library").Elements("author")

                                                     where (bool)AN.Element("lastName").Value.ToLower().StartsWith(this.textBox1.Text.ToLower())
                                                     select AN.Attribute("id").Value;
                    string ID = "";
                    foreach (var item in authorname)
                    {
                        ID = item;

                    }

                    IEnumerable<string> booktitle = from BT in doc.Element("library").Elements("book")
                                                    where (string)BT.Attribute("authors") == ID
                                                    select BT.Element("title").Value.ToString();
                    foreach (var item in booktitle)
                    {
                        F.label1.Text += item;
                        F.label1.Text += "\n";
                    }
                    F.groupBox1.Text = "List books by Part of Author Last Name";
                    F.ShowDialog();

                }

            }
            catch
            { return; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Documents (*.xls)|*.xls";
            sfd.FileName = "Books";
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

        private void btnbooks_Click(object sender, EventArgs e)
        {
            Allbooks A = new Allbooks();
            A.ShowDialog();
        }
    }
}
