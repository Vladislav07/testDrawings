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
        public event Func<DataTable> action;
        Controler c;
        DataTable dt;
        public InfoF()
        {
            InitializeComponent();
            this.dataGridView.AutoGenerateColumns = true;
        }

        private void InfoF_Load(object sender, EventArgs e)
        {
            c = new Controler(this);
            c.RunWorkerCompleted += C_RunWorkerCompleted;
            c.ProgressChanged += C_ProgressChanged;
            c.RunWorkerAsync();
        }

        private void C_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] msg = (string[])e.UserState;
            this.lbMsg.Text = msg[0];
            this.Text = msg[2];
        }

        private void C_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable dt = new DataTable();
            c.FillToListIsRebuild(ref dt);
            FillDataGridView(dt);
        }

        public void FillDataGridView(DataTable dt)
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;         
            dataGridView.Cursor = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dt= action?.Invoke();
            FillDataGridView(dt);
            this.Refresh();
        }

        private void chB_ToRebuild_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; 
            if (checkBox.Checked == true)
            {
                
            }
            else
            {
                MessageBox.Show("Флажок " + checkBox.Text + "  теперь не отмечен");
            }
        }
    }
}
