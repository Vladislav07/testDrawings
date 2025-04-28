using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        bool isDispleyRebuild = true;
        bool isImpossible = false;
        bool isBlocked=false;
        bool isStand = false;
        bool isInit = false;
        public InfoF()
        {
            InitializeComponent();
            dt= new DataTable();
            InitDT();
            this.dataGridView.AutoGenerateColumns = true;
            
        }

        private void InfoF_Load(object sender, EventArgs e)
        {

            chB_ToRebuild.Checked = true;
  
            c = new Controler(this);
            c.RunWorkerCompleted += C_RunWorkerCompleted;
            c.ProgressChanged += C_ProgressChanged;
            c.RunWorkerAsync();
        }

        private void C_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] msg = (string[])e.UserState;
            int t = e.ProgressPercentage;
            switch (t)
            {
                case 0:
                    this.lbMsg.Text = msg[0];
                   // lbMsg.;
                    break;
                case 1:
                    this.Text = msg[2];
                    this.lbMsg.Text = "Загрузка спецификации";
                    //lbMsg.BackColor = Color.Green;  
                    break;
                case 2:          
                    this.lbMsg.Text = msg[0];
                    break;
                case 3:
                    int count = e.UserState.
                    this.lbMsg.Text = msg[0];
                    break;
                default:
                    break;
            }
            
            
        }

        private void C_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
            userView = c.JoinCompAndDraw();
            RefreshForm();
        }
        private byte[] GetImageData(int i)
        {

            Image image;
            switch (i)
            {
                case 0:
                    image = Properties.Resources.part;
                    break;
                case 1:
                    image = Properties.Resources.assembly;
                    break;
                case 2:
                    image = Properties.Resources.SWXUiSWV1Drawings;
                    break;
                default:
                    image = Properties.Resources.x;
                    break;
            }
            using (MemoryStream ms = new MemoryStream())
             {
                 image.Save(ms, ImageFormat.Jpeg);
                 return ms.ToArray();
             }
        }
        public void FillDataGridView()
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;
            SetPropertiesGrig();
            dataGridView.Cursor = Cursors.Default;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            userView= Action?.Invoke();
            SetStateForm();
            FillToListIsRebuild();
            this.Refresh();
        }

        private void chB_ToRebuild_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender; 
            if (checkBox.Checked == true)
            {
                isDispleyRebuild = true;
            }
            else
            {
                isDispleyRebuild = false;
              
            }
            RefreshForm();
        }
        internal void FillToListIsRebuild()
        {
            dt.Clear();

            if (userView.Count == 0) return;
            foreach (ViewUser v in userView)
            {
                if (IsRebuldViewUser(v) && !isDispleyRebuild) continue;

                if (v.State  == "Clean" && !isClean) continue;
                if (v.State  == "Blocked" && !isBlocked) continue;
                if (v.State  == "ImpossibleRebuild" && !isImpossible) continue;
                if (v.State  == "Stand" && !isStand) continue;
                if (v.State  == "Initiated" && !isInit) continue;
                DataRow dr = dt.NewRow();
                dr[0] = v.NameComp;
                if(v.Ext == ".sldprt"|| v.TypeComp == ".SLDPRT")
                {
                    dr[1] = GetImageData(0);
                }
                else
                {
                    dr[1] = GetImageData(1);
                }

                dr[2] = v.Level;
                dr[3] = v.StPDM;
                dr[4] = v.VersionModel;
                dr[5] = v.IsLocked;
                if (v.DrawState != "")
                {
                  dr[6] = GetImageData(2);
                }
                else
                {
                  dr[6] = GetImageData(3);
                }
                dr[7] = v.StDrPDM;
                dr[8] = v.DrawVersRev;
                dr[9] = v.DrawNeedRebuild;
                dr[10] = v.DrawIsLocked;

                dt.Rows.Add(dr);
            }

        }

        private void InitDT()
        {
            dt.Columns.Add("Cuby_Number", typeof(string));
            
            dt.Columns.Add("Type", typeof(byte[]));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("State", typeof(string));
            dt.Columns.Add("Current Version", typeof(string));
            dt.Columns.Add("IsLocked", typeof(string));

            dt.Columns.Add("DrawType", typeof(byte[]));
            dt.Columns.Add("DrawState", typeof(string));         
            dt.Columns.Add("DrawVersRev", typeof(string));
            dt.Columns.Add("DrawNeedRebuild", typeof(string));
            dt.Columns.Add("DrawIsLocked", typeof(string));
        }

        bool IsRebuldViewUser(ViewUser v)
        {
             if(v.State=="OnlyAss"||
                v.DrawState== "DrawFromPart"||
                v.State== "ExtractPart"||
                v.DrawState == "OnlyDraw"||
                v.DrawState == "UpdateDrawing")
            {  return true; }
            else
            {
                return false;
            }
        }

        private void chB_Clean_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isClean= true;
            }
            else
            {
                isClean = false;         
            }
            RefreshForm();
        }

        private void chB_Impossible_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isImpossible = true;
            }
            else
            {
                isImpossible = false;
            }
            RefreshForm();
        }

        private void SetStateForm()
        {
            if (userView == null) return;
            if (userView.Any(v => IsRebuldViewUser(v) == true))
            {
                chB_ToRebuild.Enabled = true;
            }
            else
            {
                chB_ToRebuild.Enabled = false;
            }
            if (userView.Any(v => v.State == "Clean"))
            {
                chB_Clean.Enabled = true;
            }
            else
            {
                chB_Clean.Enabled = false;
            }
            if (userView.Any(v =>  v.State == "Blocked"))
            {
                checkBox1.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = false;
            }
            if (userView.Any(v => (v.State == "ImpossibleRebuild")))
            {
                chB_Impossible.Enabled = true;
            }
            else
            {
                chB_Impossible.Enabled = false;
            }
            if (userView.Any(v => v.State == "Stand"))
            {
                chB_Stand.Enabled = true;
            }
            else
            {
                chB_Stand.Enabled = false;
            }
            if (userView.Any(v => v.State == "Initiated"))
            {
                chB_Init.Enabled = true;
            }
            else
            {
                chB_Init.Enabled = false;
            }

        }
        private void RefreshForm()
        {
            SetStateForm();
            FillToListIsRebuild();
            FillDataGridView();
            this.Refresh();
        }
        private void SetPropertiesGrig()
        {
            dataGridView.Columns[0].Width = 150;
            dataGridView.Columns[1].Width = 50;
            dataGridView.Columns[2].Width = 40;
            dataGridView.Columns[3].Width = 100;
            dataGridView.Columns[4].Width = 40;
            dataGridView.Columns[5].Width = 70;
            dataGridView.Columns[6].Width = 50;
            dataGridView.Columns[7].Width = 100;
            dataGridView.Columns[8].Width = 70;
            dataGridView.Columns[9].Width = 70;
            dataGridView.Columns[10].Width = 70;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isBlocked = true;
            }
            else
            {
                isBlocked = false;
            }
            RefreshForm();
        }

        private void chB_Stand_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isStand = true;
            }
            else
            {
                isStand = false;
            }
            RefreshForm();
        }

        private void chB_Init_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isInit= true;
            }
            else
            {
                isInit= false;
            }
            RefreshForm();
        }
    }
}
