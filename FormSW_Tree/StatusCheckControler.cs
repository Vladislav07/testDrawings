using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Linq;

namespace FormSW_Tree
{
    public partial class StatusCheckControler : BackgroundWorker
    {
        InfoF f;
        public StatusCheckControler(InfoF _f)
        {
            f = _f;
            Tree.msgNameOperation += Tree_msgNameOperation;
            Tree.msgDataOperation += Tree_msgDataOperation;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }

        private void Tree_msgDataOperation(MsgInfo obj)
        {
            ReportProgress(3, obj);
        }

        private void Tree_msgNameOperation(MsgInfo obj)
        {
            ReportProgress(2, obj);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Tree.RefreshFileFromPDM();
            Tree.CompareVersions();
            f.userView.Clear();
            f.userView = Tree.JoinCompAndDraw();
           // f.RefreshForm();
            Tree.msgDataOperation-= Tree_msgDataOperation;
            Tree.msgNameOperation-= Tree_msgNameOperation;
            this.Dispose();
            
        }
    
    }
}