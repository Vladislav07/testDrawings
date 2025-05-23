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
        private DataGridView dataGridView;
        public List<ViewUser> userView { get; set; }
        bool isClean = false;
        bool isDispleyRebuild = true;
        bool isImpossible = false;
        bool isBlocked = false;

        public InfoF()
        {
            InitializeComponent();
            dt = new DataTable();
        }

        private void InfoF_Load(object sender, EventArgs e)
        {
            this.Width = 500;
            this.Height = 250;
            GenerateLabelsAndProgressBar();
            c = new ReadControler(this);
            c.RunWorkerCompleted += C_RunWorkerCompleted;
            c.ProgressChanged += C_ProgressChanged;
            c.RunWorkerAsync();

        }

        private void C_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MsgInfo msg = (MsgInfo)e.UserState;
            int t = e.ProgressPercentage;
            Notifacation(t, msg);

        }

        private void C_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
                if (userView == null) return;
                DestroyLabelsAndProgressBar();
               
                GenerateDataGridView();
                GenerateNamedCheckBoxes();
                GeneratedButton();
                RefreshForm();
                actionControler = new ActionControler();
                actionControler.RunWorkerCompleted += ActionControler_RunWorkerCompleted;
                actionControler.ProgressChanged += ActionControler_ProgressChanged;
        }

        private void ActionControler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MsgInfo msg = (MsgInfo)e.UserState;
            int t = e.ProgressPercentage;
            Notifacation(t, msg);
        }

        private void ActionControler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lbMsg.Text = "Update complete, close the program";
            lbCount.Text = "";
            lbStart.Text = "";
            lbNumber.Text = "";
            StatusCheckControler scc=new StatusCheckControler(this);
            scc.ProgressChanged += Scc_ProgressChanged;
            scc.RunWorkerCompleted += Scc_RunWorkerCompleted;
            scc.RunWorkerAsync();
        }

        private void Scc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (userView == null) return;
            DestroyLabelsAndProgressBar();
            GenerateDataGridView();
            GenerateNamedCheckBoxes();
            GeneratedButton();
            RefreshForm();
        }

        private void Scc_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MsgInfo msg = (MsgInfo)e.UserState;
            int t = e.ProgressPercentage;
            Notifacation(t, msg);
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

        private Button button1;

        private void GeneratedButton()
        {
            button1 = new Button();
            button1.Text = "&Rebuild";
            button1.Location = new Point(this.Width - button1.Width - 50, 10);
            button1.Click += Button1_Click;
            this.Controls.Add(button1);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            dataGridView.Dispose();
            chB_ToRebuild.Dispose();
            chB_Clean.Dispose();
            checkBox1.Dispose();
            chB_Impossible.Dispose();
            button1.Dispose();
            this.Width = 500;
            this.Height = 250;
            GenerateLabelsAndProgressBar();
            actionControler.RunWorkerAsync();
        }


        private void FillToListIsRebuild()
        {
            dt.Clear();

            if (userView.Count == 0) return;
            foreach (ViewUser v in userView)
            {
                if (v.State == "Rebuild" && !isDispleyRebuild) continue;
                if (v.State == "Clean" && !isClean) continue;
                if (v.State == "Blocked" && !isBlocked) continue;
                if (v.State == "Manufacturing" && !isImpossible) continue;

                DataRow dr = dt.NewRow();
                dr[0] = v.NameComp;
                if (v.Ext == ".sldprt" || v.Ext == ".SLDPRT")
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


        private CheckBox chB_ToRebuild;
        private CheckBox chB_Clean;
        private CheckBox checkBox1;
        private CheckBox chB_Impossible;

        private void GenerateNamedCheckBoxes()
        {
            chB_ToRebuild = new CheckBox();
            chB_ToRebuild.Checked = true;
            chB_ToRebuild.Text = "To Rebuild";
            chB_ToRebuild.Name = "chB_ToRebuild";
            chB_ToRebuild.Location = new Point(50, 30);
            chB_ToRebuild.Size = new Size(100, 20);
            chB_ToRebuild.CheckedChanged += ChB_ToRebuild_CheckedChanged;
            this.Controls.Add(chB_ToRebuild);

            chB_Clean = new CheckBox();
            chB_Clean.Text = "Clean";
            chB_Clean.Name = "chB_Clean";
            chB_Clean.Location = new Point(170, 30);
            chB_Clean.Size = new Size(100, 20);
            chB_Clean.CheckedChanged += ChB_Clean_CheckedChanged;
            this.Controls.Add(chB_Clean);

            checkBox1 = new CheckBox();
            checkBox1.Text = "Blocked";
            checkBox1.Name = "checkBox1";
            checkBox1.Location = new Point(290, 30);
            checkBox1.Size = new Size(100, 20);
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            this.Controls.Add(checkBox1);

            chB_Impossible = new CheckBox();
            chB_Impossible.Text = "Manufacturing";
            chB_Impossible.Name = "chB_Impossible";
            chB_Impossible.Location = new Point(410, 30);
            chB_Impossible.Size = new Size(100, 20);
            chB_Impossible.CheckedChanged += ChB_Impossible_CheckedChanged;
            this.Controls.Add(chB_Impossible);
        }

        private void ChB_Impossible_CheckedChanged(object sender, EventArgs e)
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

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
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

        private void ChB_Clean_CheckedChanged(object sender, EventArgs e)
        {
            if (userView == null) return;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isClean = true;
            }
            else
            {
                isClean = false;
            }
            RefreshForm();
        }

        private void ChB_ToRebuild_CheckedChanged(object sender, EventArgs e)
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

        private void SetStateForm()
        {
            if (userView == null) return;
            if (userView.Any(v => v.State == "Rebuild"))
            {
                chB_ToRebuild.Enabled = true;
                button1.Enabled = true;
            }
            else
            {
                chB_ToRebuild.Enabled = false;
                button1.Enabled = false;
            }
            if (userView.Any(v => v.State == "Clean"))
            {
                chB_Clean.Enabled = true;
            }
            else
            {
                chB_Clean.Enabled = false;
            }
            if (userView.Any(v => v.State == "Blocked"))
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



        private void Notifacation(int typeEvent, MsgInfo msg)
        {
            switch (typeEvent)
            {
                case 0:       //Error
                    this.lbMsg.Text = msg.errorMsg;
                    this.lbNumber.Text = msg.numberCuby;
                    lbMsg.ForeColor = Color.Red;

                    break;
                case 1:      //LoadActiveModel
                    this.Text = msg.numberCuby;

                    break;
                case 2:      //BeginOperation
                    this.lbMsg.Text = msg.typeOperation;
                    this.progressBar1.Maximum = msg.countStep;
                    this.progressBar1.Minimum = 0;
                    this.lbCount.Text = msg.countStep.ToString();

                    break;
                case 3:      //StepOperation
                    this.lbStart.Text = msg.currentStep.ToString();
                    this.progressBar1.Value = msg.currentStep;
                    this.lbNumber.Text = msg.numberCuby;
                    break;
                default:
                    break;
            }
        }
        private void GenerateDataGridView()
        {
            dataGridView = new DataGridView();
            this.Width = 1000;
            
            dataGridView.Location = new Point(0, 50);
            dataGridView.BackgroundColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.Width = this.Width;
            dataGridView.AutoGenerateColumns = true;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.BackgroundColor = Color.White; 
            dataGridView.BorderStyle = BorderStyle.None; 
            dataGridView.GridColor = Color.White;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.DataBindingComplete += (sender, e) =>
            {
                int rowHeight1 = dataGridView.RowTemplate.Height;
                int headerHeight1 = dataGridView.ColumnHeadersHeight;
                int rowsHeight1 = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                int totalHeight1 = rowsHeight1 + headerHeight1;
                int maxAllowedHeight1 = 500;
                int desiredHeight1 = Math.Min(totalHeight1, maxAllowedHeight1);
                int minHeight1 = rowHeight1 + headerHeight1;

                dataGridView.Height = Math.Max(desiredHeight1, minHeight1);
                this.Height = 100 + dataGridView.Height;
            };
            Controls.Add(dataGridView);

            dt = new DataTable();
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

            dataGridView.DataSource = dt;

            SetPropertiesGrig();
        }

        private Label lbMsg;
        private ProgressBar progressBar1;
        private Label lbStart;
        private Label lbCount;
        private Label lbNumber;
        private void GenerateLabelsAndProgressBar()
        {
            lbMsg = new Label();
            lbMsg.Text = "...";
            lbMsg.Location = new Point(20, 10);
            lbMsg.Width = 300;
            this.Controls.Add(lbMsg);

            progressBar1 = new System.Windows.Forms.ProgressBar();
            progressBar1.Location = new Point(100, 100);
            progressBar1.Width = 250;
            progressBar1.Height = 15;
            this.Controls.Add(progressBar1);

            lbStart = new Label();
            lbStart.Text = "0";
            lbStart.Location = new Point(50, 100);
            this.Controls.Add(lbStart);

            lbCount = new Label();
           
            lbCount.Text = "0";
            lbCount.Location = new Point(370, 100);
            this.Controls.Add(lbCount);

            lbNumber = new Label();
            lbNumber.Text = "...";
            lbNumber.Width = 200;
            lbNumber.Location = new Point(100, 80);
            this.Controls.Add(lbNumber);
        }
        public void DestroyLabelsAndProgressBar()
        {
            lbMsg.Dispose();
            progressBar1.Dispose();
            lbStart.Dispose();
            lbCount.Dispose();
            lbNumber.Dispose();
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
