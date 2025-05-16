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
      
        ReadControler c;
        ActionControler actionControler;
        DataTable dt;
        List<ViewUser> userView;
        bool isClean=false;
        bool isDispleyRebuild = true;
        bool isImpossible = false;
        bool isBlocked=false;

        public InfoF()
        {
            InitializeComponent();
            dt= new DataTable();
            InitDT();
            this.dataGridView.AutoGenerateColumns = true;
            chB_ToRebuild.Checked = true;
            button1.Enabled = false;
            userView=new List<ViewUser>();
        }

        private void InfoF_Load(object sender, EventArgs e)
        {
            c = new ReadControler(this, ref userView);
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
                    int count =Convert.ToInt16( msg[1]);
                    this.lbMsg.Text = msg[0];
                    this.progressBar1.Maximum = 0;
                    this.progressBar1.Maximum = count;
                    this.lbCount.Text = count.ToString();
                    break;
                case 4:
                    int i = Convert.ToInt16(msg[1]);
                    this.lbNumber.Text = msg[0];
                    this.progressBar1.Value = i;
                    this.lbStart.Text = i.ToString();
                    break;
                case 5:
                    this.lbMsg.Text = msg[0];
                    
                    break;
                default:
                    break;
            }
            
            
        }

        private void C_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshForm();
            button1.Enabled = true;
            actionControler = new ActionControler(this);
            actionControler.RunWorkerCompleted += ActionControler_RunWorkerCompleted;
            actionControler.ProgressChanged += ActionControler_ProgressChanged;

        }

        private void ActionControler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ActionControler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
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
        private void FillDataGridView()
        {

            dataGridView.Cursor = Cursors.WaitCursor;
            this.bindingSource1.DataSource = dt;
            SetPropertiesGrig();
            dataGridView.Cursor = Cursors.Default;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            actionControler.RunWorkerAsync();
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
        private void FillToListIsRebuild()
        {
            dt.Clear();

            if (userView.Count == 0) return;
            foreach (ViewUser v in userView)
            {
                if (v.State == "Rebuild" && !isDispleyRebuild) continue;
                if (v.State  == "Clean"  && !isClean) continue;
                if (v.State  == "Blocked" && !isBlocked) continue;
                if (v.State  == "Manufacturing" && !isImpossible) continue;

                DataRow dr = dt.NewRow();
                dr[0] = v.NameComp;
                if(v.Ext == ".sldprt"|| v.Ext== ".SLDPRT")
                {
                    dr[1] = GetImageData(0);
                }
                else
                {
                    dr[1] = GetImageData(1);
                }

                dr[2] = v.Level;
                dr[3] = v.StPDM;
                dr[4] = v.State;
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
                dr[10] = v.DrawState;

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
            if (userView.Any(v => v.State == "Rebuild"))
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
            if (userView.Any(v => (v.State == "Manufacturing")))
            {
                chB_Impossible.Enabled = true;
            }
            else
            {
                chB_Impossible.Enabled = false;
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
   
    }

    public struct ViewUser
    {
        internal string NameComp { get; set; }
        internal string TypeComp { get; set; }
        internal string Ext { get; set; }
        internal string Level { get; set; }
        internal string State { get; set; }
        internal string StPDM { get; set; }
        internal string VersionModel { get; set; }
        internal string IsLocked { get; set; }
        internal string IsChildRefError { get; set; }
        internal string DrawState { get; set; }
        internal string StDrPDM { get; set; }
        internal string DrawVersRev { get; set; }
        internal string DrawNeedRebuild { get; set; }
        internal string DrawIsLocked { get; set; }
    }
}
