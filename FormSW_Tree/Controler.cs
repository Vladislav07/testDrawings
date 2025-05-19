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
                Tree.GetInfoPDM();
                Tree.CompareVersions();
                f.userView= JoinCompAndDraw();
            }
        }
     
  
        internal  List<ViewUser> JoinCompAndDraw()
        {
            List<Part> compList = Tree.listComp;
            List<Drawing> drawList = Tree.listDraw;
            List<ViewUser> lv = new List<ViewUser>();
            foreach (Part item in compList)
            {
                Drawing dr = drawList.FirstOrDefault(d => d.CubyNumber == item.CubyNumber);

                lv.Add(new ViewUser
                {
                    NameComp = item.CubyNumber,
                    TypeComp = item.Section,
                    Ext = item.Ext,
                    Level = item.Level.ToString(),
                    StPDM = item.File.CurrentState.Name.ToString(),
                    State = item.condition.stateModel.ToString(),
                    VersionModel = item.File?.CurrentVersion.ToString() ?? "",
                    IsLocked = item.File?.IsLocked.ToString() ?? "",
                    IsChildRefError = item is Assemble ? (item as Assemble).listRefChildError.Count.ToString() : "",

                    DrawState = dr != null ? dr.condition.stateModel.ToString() : "",
                    StDrPDM = dr != null ? dr.File.CurrentState.Name : "",
                    DrawNeedRebuild = dr != null ? dr.NeedsRebuild.ToString() : "",
                    DrawVersRev = dr != null ? dr.msgRefVers : "",
                    DrawIsLocked = dr != null ? dr.File?.IsLocked.ToString() : ""
                });
            }

     
            return lv;
        }
    }

    
}

