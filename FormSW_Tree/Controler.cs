using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EPDM.Interop.epdm;
using System.Reflection.Emit;
using System.ComponentModel;

namespace FormSW_Tree
{

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

    public class Controler : BackgroundWorker
    {

        SW sw;
        InfoF f;
        List<ViewUser> listVU;
        string[] msgInfo;
        public Controler(InfoF _f, ref List<ViewUser> viewUser)
        {
            WorkerReportsProgress = true;
            listVU=viewUser;
            f = _f;
            f.cmdRebuild += F_action;
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

        private void F_action()
        {
            sw.CloseDoc();
            RebuildTree();
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


       

        public void RebuildTree()
        {
 
            List<Drawing> listPartDraw = Tree.listDraw.Where(d => d.condition.stateModel==StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .ToList();

            List<Part> listAss = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .ToList();

            List<Drawing> listAssDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .ToList();

            

            int CountItemToCheckOut = listAssDraw.Count + listAss.Count + listPartDraw.Count;

            msgInfo[0] = "Extract files from storage - CheckOut";
            msgInfo[1] = CountItemToCheckOut.ToString();
            Sw_operationSW(msgInfo);

            PDM.CockSelList(CountItemToCheckOut);
            listPartDraw.ForEach(d => d.AddItemToSelList());
            listAss.ForEach(d => d.AddItemToSelList());
            listAssDraw.ForEach(d => d.AddItemToSelList());
            PDM.BatchGet();

            if(listAssDraw.Count > 0)
            {
                msgInfo[0] = "opening and rebuilding drawings of parts";
                msgInfo[1] = CountItemToCheckOut.ToString();
                Sw_operationSW(msgInfo);
                BatchRefreshFile( listPartDraw);
            }

            if (listAss.Count > 0)
            {
                msgInfo[0] = "opening and rebuilding assemble";
                msgInfo[1] = CountItemToCheckOut.ToString();
                Sw_operationSW(msgInfo);
                listAss.ForEach(d => sw.OpenAndRefresh(d.FullPath));
                PDM.CockSelList(listAss.Count);
                listAss.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
            }
         
            if(listAssDraw.Count > 0)
            {
                msgInfo[0] = "opening and rebuilding drawings of Assemble";
                msgInfo[1] = CountItemToCheckOut.ToString();
                Sw_operationSW(msgInfo);
                BatchRefreshFile( listAssDraw);
            }
            


        }
        private void BatchRefreshFile( List<Drawing> listDraw)
        {
            listDraw.ForEach(d => sw.OpenAndRefresh(d.FullPath));
            PDM.CockSelList(listDraw.Count);
            listDraw.ForEach(d => d.AddItemToSelList());
            PDM.DocBatchUnLock();
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

