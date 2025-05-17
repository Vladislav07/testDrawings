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
        List<ViewUser> listVU;
        string[] msgInfo;
        public ReadControler(InfoF _f, ref List<ViewUser> viewUser)
        {
            WorkerReportsProgress = true;
            listVU=viewUser;
            f = _f;
            Tree.msgDataOperation += Tree_msgDataOperation;
            Tree.msgNameOperation += Tree_msgNameOperation;
            msgInfo=new string[2];
        }

        private void Tree_msgNameOperation(string[] obj)
        { 
            ReportProgress(3, obj);
        }

        private void Tree_msgDataOperation(string[] obj)
        {
            ReportProgress(4, obj);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Init();
        }

        public bool Init()
        {
            sw = new SW();
            sw.connectSw += Sw_connectSw;
            sw.readedTree += Sw_readedTree;
            sw.operationSW += Sw_operationSW;
            sw.loadTree += Sw_loadTree;
            sw.btnConnectSW();
            return true;
        }

       

        private void Sw_operationSW(string[] obj)
        {
            ReportProgress(3, obj);
        }

        private void Sw_loadTree(string[] msg)
        {
            ReportProgress(4, msg);
        }

     
        private void Sw_connectSw(string[] msg, bool arg)
        {
            if (!arg)
            {
                 ReportProgress(0, msg);
               
            }
            else
            {
 
                ReportProgress(1, msg);
                sw.BuildTree();

               
            }
        }
        private void Sw_readedTree(bool arg2)
        {
            if (arg2)
            {
                msgInfo[0] = "List formation";
                ReportProgress(2, msgInfo);
                Tree.SearchParentFromChild();
                Tree.FillCollection();
                msgInfo[0] = "Load EdmFile from PDM";
                ReportProgress(2, msgInfo);
                Tree.GetInfoPDM();
                Tree.CompareVersions();
                JoinCompAndDraw(listVU);
            }
        }
  
        internal  List<ViewUser> JoinCompAndDraw(List<ViewUser> lv )
        {
            List<Part> compList = Tree.listComp;
            List<Drawing> drawList = Tree.listDraw;
           // List<ViewUser> lv = new List<ViewUser>();
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

            msgInfo[0] = "----------------------";
            ReportProgress(5, msgInfo);
            return lv;
        }
    }

    
}

