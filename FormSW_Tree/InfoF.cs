using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;



namespace FormSW_Tree
{
    public partial class InfoF : Form
    {
        ReadControler c;
        ActionControler actionControler;
        StatusCheckControler scc;
        DataTable dt;
       
        public List<ViewUser> userView { get; set; }
        bool isClean = false;
        bool isDispleyRebuild = true;
        bool isImpossible = false;
        bool isBlocked = false;
        bool isVisible = false;
        bool isAll = false;
        int  numberLogger;

        private enum StateFopm
        {
            Init=0,
            Display=1,
            Error=2
        }
        private StateFopm stateFopm;
        private void SetDispley()
        {
            switch (stateFopm)
            {
                case StateFopm.Init:
                    tabControl.TabPages[0].Enabled = true;
                    tabControl.TabPages[1].Enabled = false;
                    tabControl.TabPages[2].Enabled = false;
                    tabControl.SelectTab(0);
                    lv.Items.Clear();
                    break;
                case StateFopm.Display:
                    tabControl.TabPages[1].Enabled = true;
                    tabControl.TabPages[0].Enabled = false;
                    tabControl.TabPages[2].Enabled = true;
                    tabControl.SelectTab(1);
                   
                    break;
                case StateFopm.Error:

                    break;
                default:
                    break;
            }
        }

        
               
        private void InfoF_Load(object sender, EventArgs e)
        {
            this.FormClosing += InfoF_FormClosing;
            stateFopm=StateFopm.Init;
            tabControl.SelectedIndex = 1;
            tabControl.SelectedIndex = 0;
            SetDispley();
            c = new ReadControler(this);
            c.RunWorkerCompleted += C_RunWorkerCompleted;
            c.ProgressChanged += C_ProgressChanged;
            c.RunWorkerAsync();

        }

        private void InfoF_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(c!=null && c.IsBusy)
            {
                c.CancelAsync();
                c.Dispose();
            }
            if (actionControler != null && actionControler.IsBusy)
            {
                actionControler.CancelAsync();
                actionControler.Dispose();
            }
            if (scc != null && scc.IsBusy)
            {
                scc.CancelAsync();
                scc.Dispose();
            }
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
            
            c.RunWorkerCompleted -= C_RunWorkerCompleted;
            c.ProgressChanged -= C_ProgressChanged;
            stateFopm = StateFopm.Display;
            SetDispley();
            RefreshForm();
           
        }
        private void ActionControler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MsgInfo msg = (MsgInfo)e.UserState;
            int t = e.ProgressPercentage;
            Notifacation(t, msg);
        }
        private void ActionControler_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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
            stateFopm = StateFopm.Display;
            SetDispley();
            RefreshForm();
        }
        private void Scc_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MsgInfo msg = (MsgInfo)e.UserState;
            int t = e.ProgressPercentage;
            Notifacation(t, msg);
        }
       
        private void Button1_Click(object sender, EventArgs e)
        {
            actionControler = new ActionControler(isVisible, isAll);
            actionControler.RunWorkerCompleted += ActionControler_RunWorkerCompleted;
            actionControler.ProgressChanged += ActionControler_ProgressChanged;
            actionControler.RunWorkerAsync();
            stateFopm = StateFopm.Init;
            SetDispley();
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
                if ((v.State == "Standart"|| v.State == "Manufacturing") && !isImpossible) continue;

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
                    case "Standart":
                        dr[4] = GetImageData(8);
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
            if (userView.Any(v => (v.State == "Manufacturing"|| v.State == "Standart")))
            {
                chB_Impossible.Enabled = true;
            }
            else
            {
                chB_Impossible.Enabled = false;
            }


        }
        public void RefreshForm()
        {
            this.SuspendLayout();
            try
            {
               
                FillToListIsRebuild();
                SetStateForm();
                this.Refresh();
            }
            catch (Exception e)
            {
                string msgText = numberLogger.ToString() + " Error refresh Form " + e.Message;
                lbMsg.ForeColor = Color.Red;
                lbMsg.Text = msgText;
                AddItemWithColor(msgText, Color.Red);
                lv.ForeColor = Color.Red;
                lv.Items.Add(msgText);

            }
            finally
            {
                this.ResumeLayout();
            }
           
        }
        private void Notifacation(int typeEvent, MsgInfo msg)
        {
            switch (typeEvent)
            {
                case 0:       //Error
                  
                    string msgText = numberLogger.ToString() + " - " + msg.errorMsg + " - " + 
                                     msg.typeError + " - " +
                                     msg.numberCuby;
                    lbMsg.ForeColor = Color.Red;
                    lbMsg.Text = msgText;
                    AddItemWithColor(msgText, Color.Red);

                    lv.ForeColor = Color.Red;
                    lv.Items.Add(msgText);
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
   
                    AddItemWithColor(msgOper, Color.Green);
                    break;
                case 3:      //StepOperation
              
                    lbStart.Text = msg.currentStep.ToString();
                    progressBar1.Value = msg.currentStep;
                    lbNumber.Text = msg.numberCuby;
                    string msgStep = numberLogger.ToString() + "." + msg.currentStep.ToString() +
                                 " - " + msg.numberCuby + Environment.NewLine;
                    AddItemWithColor(msgStep, Color.Green);
                    break;
                    case 4:
                    string msgWarning = numberLogger.ToString() + " - " + msg.errorMsg;
                              
                    lbMsg.ForeColor = Color.Violet;
                    lbMsg.Text = msgWarning;
                    AddItemWithColor(msgWarning, Color.Violet);

                    lv.ForeColor = Color.Violet;
                    lv.Items.Add(msgWarning);
                    break;
                default:
                    break;
            }
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
