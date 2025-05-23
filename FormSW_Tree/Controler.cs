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
 
            f = _f;
            sw = new SW();
            sw.connectSw += Sw_connectSw;
              
            Tree.msgDataOperation += Tree_msgDataOperation;
            Tree.msgNameOperation += Tree_msgNameOperation;
    
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
                f.userView= Tree.JoinCompAndDraw();
            }
        }
     
  
 
    }

    
}

