using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DB2_Mid_term
{
    public partial class ADDB : Form
    {

        Allbooks BBB = new Allbooks();
        DataSet DS = new DataSet();
        public ADDB()
        {
            InitializeComponent();
            DS.ReadXml("XMLFile.xml");
        }

        private void ADDB_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            BBB.dataGridView1.Refresh();
            this.Close();
        }
    }
}
