using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormSW_Tree
{
    public partial class InfoF : Form
    {
       
        public InfoF()
        {
            InitializeComponent();
            this.dataGridView.AutoGenerateColumns = true;

        }

        private void InfoF_Load(object sender, EventArgs e)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (senderF, eF) =>
            {
               
                Controler.Init();
            };

            backgroundWorker.RunWorkerCompleted += (senderF, eF) =>
            {
        
                DataTable dt = new DataTable();

                Controler.FillToListIsRebuild(ref dt);
                FillDataGridView(dt);
                this.Refresh();
               // OperationCompleted.Invoke(this, "Operation complete");
            };

            backgroundWorker.RunWorkerAsync();
        }

    

        public void FillDataGridView(DataTable dt)
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;         
            dataGridView.Cursor = Cursors.Default;
        }
    }
}
