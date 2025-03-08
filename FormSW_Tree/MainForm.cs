using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace FormSW_Tree
{
    public partial class MainForm :Form
    {

        SW sw;
        DataTable dt;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConnectSW_Click(object sender, EventArgs e)
        {
            sw= new SW();
            sw.action += Sw_action;
            sw.btnConnectSW();
            sw.numberModel += Sw_numberModel;
        }

        private void Sw_numberModel(string numberCuby)
        {
          txtActiveDoc.Text = numberCuby;
        }

        private void Sw_action(string message)
        {
           txtStatus.Text = message;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            sw.Refresh();
            this.btnRefresh.Enabled = false;
            //this.cmdCancel.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            sw.Cancel();
            // change button config
            this.btnRefresh.Enabled = true;
            //this.cmdCancel.Enabled = false;
        }


        public void FillDataGridView(DataTable dt)
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            { dataGridView.Columns[i].ReadOnly = true; }

            dataGridView.Cursor = Cursors.Default;
        }








    }

}
