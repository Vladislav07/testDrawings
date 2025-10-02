using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace FormSW_Tree
{

    public class ReadControler : BackgroundWorker
    {

        SW sw;
        InfoF f;
  
        public ReadControler(InfoF _f)
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            f = _f;
            sw = new SW();
            sw.connectSw += Sw_connectSw;
            sw.NotifySW += Sw_NotifySW;  
            Tree.msgDataOperation += Tree_msgDataOperation;
            Tree.msgNameOperation += Tree_msgNameOperation;
            Tree.msgWarnings += Tree_msgWarnings;
            PDM.NotifyPDM += PDM_NotifyPDM;
        }

        private void PDM_NotifyPDM(int stage, MsgInfo msg)
        {
            ReportProgress(stage, msg);
        }

        private void Sw_NotifySW(int arg1, MsgInfo arg2)
        {
            ReportProgress(arg1, arg2);
        }

        private void Tree_msgWarnings(MsgInfo obj)
        {
            ReportProgress(4, obj);
        }

        private void Tree_msgNameOperation(MsgInfo obj)
        {   
            ReportProgress(2, obj);
        }

        private void Tree_msgDataOperation(MsgInfo obj)
        {
            ReportProgress(3, obj);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            sw.btnConnectSW();
            sw.connectSw-= Sw_connectSw;
            Tree.msgDataOperation-=Tree_msgDataOperation;
            Tree.msgNameOperation-=Tree_msgNameOperation;
            Tree.msgWarnings-=Tree_msgWarnings;
            PDM.NotifyPDM -= PDM_NotifyPDM;
            this.Dispose();
        }

       
        private void Sw_connectSw(MsgInfo info, bool arg)
        {
          
            if (!arg)
            {
                
                 ReportProgress(0, info);              
            }
            else
            {
              
                ReportProgress(1,info);
                sw.BuildTree();
                Tree.SearchParentFromChild();
                Tree.FillCollection();
                Tree.ReverseTree();
                Tree.GetInfoPDM();
                Tree.CompareVersions();
                Tree.isCheckOut();
                f.userView= Tree.JoinCompAndDraw();
            }
        }
     
  
 
    }

    
}

