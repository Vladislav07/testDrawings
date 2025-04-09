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
        internal event Func<List<ViewUser>> Action;
        Controler c;
        DataTable dt;
        List<ViewUser> userView;
        bool isClean=false;
        bool isDispleyRebuild = false;
        public InfoF()
        {
            InitializeComponent();
            dt= new DataTable();
            InitDT();
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
          
            userView = c.JoinCompAndDraw();
            FillToListIsRebuild();
            FillDataGridView();
            this.Refresh();
        }

        public void FillDataGridView()
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;         
            dataGridView.Cursor = Cursors.Default;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            userView= Action?.Invoke();
            FillToListIsRebuild();
            this.Refresh();
        }

        private void chB_ToRebuild_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender; 
            if (checkBox.Checked == true)
            {
                isDispleyRebuild = true;
                FillToListIsRebuild();
                FillDataGridView();
                this.Refresh();
            }
            else
            {
                isDispleyRebuild = false;
                FillToListIsRebuild();
                FillDataGridView();
                this.Refresh();
            }
        }
        internal void FillToListIsRebuild()
        {
            dt.Clear();
           

            foreach (ViewUser v in userView)
            {
               // if (!IsRebuldViewUser(v) && !isDispleyRebuild) continue;
                if (v.State=="Clean" && !isClean) continue;
                DataRow dr = dt.NewRow();
                dr[0] = v.NameComp;
                dr[1] = v.TypeComp;
                dr[2] = v.Level;
                dr[3] = v.State;
                dr[4] = v.VersionModel;
                dr[5] = v.IsLocked;
                dr[6] = v.DrawState;
                dr[7] = v.DrawVersRev;
                dr[8] = v.DrawNeedRebuild;
                dr[9] = v.DrawIsLocked;

                dt.Rows.Add(dr);
            }

        }

        private void InitDT()
        {
            dt.Columns.Add("Cuby Number", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("State", typeof(string));
            dt.Columns.Add("Current Version", typeof(string));
            dt.Columns.Add("IsLocked", typeof(string));
            dt.Columns.Add("DrawState", typeof(string));
            dt.Columns.Add("DrawVersRev", typeof(string));
            dt.Columns.Add("DrawNeedRebuild", typeof(string));
            dt.Columns.Add("DrawIsLocked", typeof(string));
        }

        bool IsRebuldViewUser(ViewUser v)
        {
             if(v.State=="OnlyAss"||
                v.State== "DrawFromPart"||
                v.State== "ExtractPart"||
                v.DrawState == "OnlyDraw")
            {  return true; }
            else
            {
                return false;
            }
        }

        private void chB_Clean_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isClean= true;
            }
            else
            {
                isClean = false;         
            }
                FillToListIsRebuild();
                FillDataGridView();
                this.Refresh();
        }
    }
}
