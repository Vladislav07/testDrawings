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
        StatusCheckControler scc;
        DataTable dt;
        private DataGridView dataGridView;
        public List<ViewUser> userView { get; set; }
        bool isClean = false;
        bool isDispleyRebuild = true;
        bool isImpossible = false;
        bool isBlocked = false;
        bool isVisible = false;
        TabControl tabControl;
        TabPage tab1;
        TabPage tab2;
        TextBox lbLogger;
        int numberLogger;
        
        public InfoF()
        {
            InitializeComponent();
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tab1 = new TabPage("Main");
            tabControl.TabPages.Add(tab1);

            tab2 = new TabPage("Loger");
            lbLogger = new TextBox();
            lbLogger.Text = "...";
            lbLogger.Dock = DockStyle.Fill;
            lbLogger.Multiline = true;
            lbLogger.ScrollBars = ScrollBars.Vertical;
            lbLogger.ForeColor = Color.Green;
            lbLogger.AutoSize = true;
            tab2.Controls.Add(lbLogger);
            tabControl.TabPages.Add(tab2);
            this.Controls.Add(tabControl);
            dt = new DataTable();
            numberLogger = 0;
         
        }

        private void InfoF_Load(object sender, EventArgs e)
        {
            tab1.Width = 500;
            tab1.Height = 250;
            GenerateLblMsg();
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
           // GeneratedButtonInfo();
            RefreshForm();
            c.RunWorkerCompleted -= C_RunWorkerCompleted;
            c.ProgressChanged -= C_ProgressChanged;

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
           
            actionControler.RunWorkerCompleted-= ActionControler_RunWorkerCompleted;
            actionControler.ProgressChanged-= ActionControler_ProgressChanged;
            
            scc = new StatusCheckControler(this);
            scc.ProgressChanged += Scc_ProgressChanged;
            scc.RunWorkerCompleted += Scc_RunWorkerCompleted;
            scc.RunWorkerAsync();

        }

        private void Scc_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (userView == null) return;
            scc.RunWorkerCompleted-= Scc_RunWorkerCompleted;
            scc.ProgressChanged -= Scc_ProgressChanged;
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
                    image = Properties.Resources.part_bmp;
                    break;
                case 1:
                    image = Properties.Resources.assembly_bmp;
                    break;
                case 2:
                    image = Properties.Resources.Drawings;
                    break;
                case 3:
                    image = Properties.Resources.free_icon_ok;
                    break;
                case 4:
                    image = Properties.Resources.free_icon_repairing;
                    break;
                case 5:
                    image = Properties.Resources.closed;
                    break;
                case 6:
                    image = Properties.Resources.Warning;
                    break;
                case 7:
                    image = Properties.Resources.x1;
                    break;
                default:
                    image = Properties.Resources.empty;
                    break;
            }
        
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private Button button1;
        private Button button2;
        private void GeneratedButton()
        {
            button1 = new Button();
            button1.Text = "&Rebuild";
            button1.Location = new Point(this.Width - button1.Width - 50, lbMsg.Bottom+10);
            button1.Click += Button1_Click;
            tab1.Controls.Add(button1);
        }

        private void GeneratedButtonInfo()
        {
            button2 = new Button();
            button2.Text = "&Info";
            button2.Location = new Point(this.Width - button1.Width - 150, lbMsg.Bottom + 10);
            button2.Click += Button2_Click;
            tab1.Controls.Add(button2);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
       /*     StatusCheckControler scc;
            scc = new StatusCheckControler(this);
            scc.ProgressChanged += Scc_ProgressChanged;
            scc.RunWorkerCompleted += Scc_RunWorkerCompleted;
            scc.RunWorkerAsync();*/
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            dataGridView.Dispose();
            chB_ToRebuild.Dispose();
            chB_Clean.Dispose();
            checkBox1.Dispose();
            chB_Impossible.Dispose();
            button1.Dispose();
            tab1.Width = 500;
            tab1.Height = 250;
            GenerateLabelsAndProgressBar();
            actionControler = new ActionControler(isVisible);
            actionControler.RunWorkerCompleted += ActionControler_RunWorkerCompleted;
            actionControler.ProgressChanged += ActionControler_ProgressChanged;
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
                switch (v.State)
                {
                    case "Clean":
                        dr[4] = GetImageData(3);
                        break;
                    case "Manufacturing":
                        dr[4] = GetImageData(5);
                        break;
                    case "Blocked":
                        dr[4] = GetImageData(6);
                        break;
                    case "Rebuild":
                        dr[4] = GetImageData(4);
                        break;
                    default:
                        dr[4] = GetImageData(10);
                        break;
                }
                if (v.IsLocked == "True")
                {
                    dr[5] = "CheckIn";
                }
                else
                {
                    dr[5] = "";
                }
              
                if (v.DrawState != "")
                {
                    dr[6] = GetImageData(2);
                }
                else
                {
                    dr[6] = GetImageData(10);
                }
                dr[7] = v.StDrPDM;
                dr[8] = v.DrawVersRev;
                if (v.DrawNeedRebuild == "")
                {
                    dr[9] = "";
                }
                else if (v.DrawNeedRebuild == "True")
                {
                   dr[9] = "Yes";
                }
                else
                {
                   dr[9] = "No";
                }
          
               
                switch (v.DrawState)
                {
                    case "Clean":
                        dr[10] = GetImageData(3);
                        break;
                    case "Manufacturing":
                        dr[10] = GetImageData(5);
                        break;
                    case "Blocked":
                        dr[10] = GetImageData(6);
                        break;
                    case "Rebuild":
                        dr[10] = GetImageData(4);
                        break;
                    default:
                        dr[10] = GetImageData(10);
                        break;
                }
                if (v.IsLocked == "True")
                {
                    dr[11] = "CheckIn";
                }
                else
                {
                    dr[11] = "";
                }

                    dt.Rows.Add(dr);
            }

        }


        private CheckBox chB_ToRebuild;
        private CheckBox chB_Clean;
        private CheckBox checkBox1;
        private CheckBox chB_Impossible;
        private CheckBox chB_IsVisible;
        private void GenerateLblMsg()
        {
            lbMsg = new Label();
            lbMsg.Text = "...";
            lbMsg.Location = new Point(20, 10);
            lbMsg.Width = 300;
            lbMsg.ForeColor = Color.Green;
            lbMsg.AutoSize = true;
            tab1.Controls.Add(lbMsg);
        }
        private void GenerateNamedCheckBoxes()
        {
            chB_IsVisible = new CheckBox();
            chB_IsVisible.Checked = false;
            chB_IsVisible.Text = "IsVisible";
            chB_IsVisible.Name = "chB_IsVisible";
            chB_IsVisible.Location = new Point(250, lbMsg.Bottom + 10);
            chB_IsVisible.Size = new Size(100, 20);
            chB_IsVisible.CheckedChanged += ChB_IsVisible_CheckedChanged;
            tab1.Controls.Add(chB_IsVisible);

            chB_ToRebuild = new CheckBox();
            chB_ToRebuild.Checked = true;
            chB_ToRebuild.Text = "To Rebuild";
            chB_ToRebuild.Name = "chB_ToRebuild";
            chB_ToRebuild.Location = new Point(50, lbMsg.Bottom+50);
            chB_ToRebuild.Size = new Size(100, 20);
            chB_ToRebuild.CheckedChanged += ChB_ToRebuild_CheckedChanged;
            tab1.Controls.Add(chB_ToRebuild);

            chB_Clean = new CheckBox();
            chB_Clean.Text = "Clean";
            chB_Clean.Name = "chB_Clean";
            chB_Clean.Location = new Point(170, lbMsg.Bottom + 50);
            chB_Clean.Size = new Size(100, 20);
            chB_Clean.CheckedChanged += ChB_Clean_CheckedChanged;
            tab1.Controls.Add(chB_Clean);

            checkBox1 = new CheckBox();
            checkBox1.Text = "Blocked";
            checkBox1.Name = "checkBox1";
            checkBox1.Location = new Point(290, lbMsg.Bottom + 50);
            checkBox1.Size = new Size(100, 20);
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            tab1.Controls.Add(checkBox1);

            chB_Impossible = new CheckBox();
            chB_Impossible.Text = "Manufacturing";
            chB_Impossible.Name = "chB_Impossible";
            chB_Impossible.Location = new Point(410, lbMsg.Bottom + 50);
            chB_Impossible.Size = new Size(100, 20);
            chB_Impossible.CheckedChanged += ChB_Impossible_CheckedChanged;
            tab1.Controls.Add(chB_Impossible);
        }

        private void ChB_IsVisible_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                isVisible = true;
            }
            else
            {
                isVisible = false;
            }
            RefreshForm();
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
            this.SuspendLayout();
            try
            {
                SetStateForm();
                FillToListIsRebuild();
                this.Refresh();
            }
            catch (Exception)
            {

                
            }
            finally
            {
                this.ResumeLayout();
            }
           
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
                    numberLogger++;
                    string msgText = numberLogger.ToString() + " - " + msg.errorMsg + Environment.NewLine + 
                                     msg.typeError + Environment.NewLine +
                                     msg.numberCuby + Environment.NewLine;
                    lbMsg.ForeColor = Color.Red;
                    lbMsg.Text = msgText;                            
                    lbLogger.ForeColor = Color.Red; 
                    lbLogger.Text = lbLogger.Text + msgText;
                   
                    break;
                case 1:      //LoadActiveModel
                    Text = msg.numberCuby;

                    break;
                case 2:      //BeginOperation
                    numberLogger++;
                    lbMsg.ForeColor = Color.Green;
                    lbMsg.Text =  msg.typeOperation;
                    progressBar1.Maximum = msg.countStep;
                    progressBar1.Minimum = 0;
                    lbCount.Text = msg.countStep.ToString();
                    string msgOper = numberLogger.ToString() + " - " + msg.typeOperation + 
                                  ": " + msg.countStep + Environment.NewLine;
                    lbLogger.Text = lbLogger.Text + msgOper;
                   
                    break;
                case 3:      //StepOperation
              
                    lbStart.Text = msg.currentStep.ToString();
                    progressBar1.Value = msg.currentStep;
                    lbNumber.Text = msg.numberCuby;
                    string msgStep = numberLogger.ToString() + "." + msg.currentStep.ToString() +
                                 " - " + msg.numberCuby + Environment.NewLine;
                    lbLogger.Text = lbLogger.Text + msgStep;
                    break;
                default:
                    break;
            }
        }
        private void GenerateDataGridView()
        {
            dataGridView = new DataGridView();
           // this.Width = 1200;
            
            dataGridView.Location = new Point(0, lbMsg.Bottom + 75); ;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            
            dataGridView.AutoGenerateColumns = true;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView.BackgroundColor = Color.White; 
            dataGridView.BorderStyle = BorderStyle.None; 
            dataGridView.GridColor = Color.White;

           // dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
           
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
                this.Height = 200 + dataGridView.Height;
            };
            tab1.Controls.Add(dataGridView);

            dt = new DataTable();
            dt.Columns.Add("Cuby_Number", typeof(string));
            dt.Columns.Add("Type", typeof(byte[]));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("PDM", typeof(string));
            dt.Columns.Add("IsState", typeof(byte[]));
            dt.Columns.Add("IsLocked", typeof(string));
            dt.Columns.Add("DrawType", typeof(byte[]));
            dt.Columns.Add("DrawState", typeof(string));
            dt.Columns.Add("DrawVersRev", typeof(string));
            dt.Columns.Add("DrawingNeedsRebuild", typeof(string));
            dt.Columns.Add("IsStateDraw", typeof(byte[]));
            dt.Columns.Add("DrawIsLocked", typeof(string));

            dataGridView.DataSource = dt;

           // SetPropertiesGrig();
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            int totalColumnsWidth = 0;
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                totalColumnsWidth += column.Width;
            }

            dataGridView.Width = totalColumnsWidth + 50;
            this.Width = totalColumnsWidth + 75;
        }

        private Label lbMsg;
        private ProgressBar progressBar1;
        private Label lbStart;
        private Label lbCount;
        private Label lbNumber;
        private void GenerateLabelsAndProgressBar()
        {
            progressBar1 = new System.Windows.Forms.ProgressBar();
            progressBar1.Location = new Point(100, 100);
            progressBar1.Width = 250;
            progressBar1.Height = 15;
            tab1.Controls.Add(progressBar1);

            lbStart = new Label();
            lbStart.Text = "0";
            lbStart.Location = new Point(50, 100);
            tab1.Controls.Add(lbStart);

            lbCount = new Label();
           
            lbCount.Text = "0";
            lbCount.Location = new Point(370, 100);
            tab1.Controls.Add(lbCount);

            lbNumber = new Label();
            lbNumber.Text = "...";
            lbNumber.Width = 200;
            lbNumber.Location = new Point(100, 80);
            tab1.Controls.Add(lbNumber);
        }
        public void DestroyLabelsAndProgressBar()
        {
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
        internal string DrawIsState { get; set; }
        internal string DrawIsLocked { get; set; }
    }

 


}
