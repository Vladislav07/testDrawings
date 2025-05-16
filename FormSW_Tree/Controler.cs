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
    public struct PdmID
    {
        public PdmID(int file, int folder)
        {
            FileId = file;
            FolderId = folder;
        }
        public int FileId { get; set; }
        public int FolderId { get; set; }
    }

    internal struct ViewUser
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
        string[] msgInfo;
        public Controler(InfoF _f)
        {
            WorkerReportsProgress = true;
            f = _f;
            f.cmdRebuild += F_action;
            Tree.msgDataOperation += Tree_msgDataOperation;
            Tree.msgNameOperation += Tree_msgNameOperation;
            msgInfo=new string[1];
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
            }
        }

      

        public bool RebuildTree()
        {
            List<PdmID> listExstactPDM = new List<PdmID>();

            List<IRebuild> listPartDraw = Tree.listDraw.Where(d => d.condition.stateModel==StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPDMDrawPart = new List<PdmID>();
            List<string> listPathDrawParts = new List<string>();
            listPartDraw.ForEach(d =>
            {
                listPDMDrawPart.AddRange(d.GetIDFromPDM());
                listExstactPDM.AddRange(d.GetIDFromPDM());
                listPathDrawParts.Add(d.GetPath());
 
            });
         

            List<IRebuild> listAss = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPDMAss = new List<PdmID>();
            List<string> listPathAss = new List<string>();
            listAss.ForEach(a =>
            {
                listExstactPDM.AddRange(a.GetIDFromPDM());
                listPDMAss.AddRange(a.GetIDFromPDM());
                listPathAss.Add(a.GetPath());
               
             
            });
         

            List<IRebuild> listAssDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPDMDrawAss = new List<PdmID>();

            List<string> listPathDrawAss = new List<string>();
            listAssDraw.ForEach(d =>
            {
                listExstactPDM.AddRange(d.GetIDFromPDM());
                listPDMDrawAss.AddRange(d.GetIDFromPDM());
                listPathDrawAss.Add(d.GetPath());
               
              
            });
      
            if (listExstactPDM.Count > 0){
                try
                {
                    PDM.AddSelItemToList(listExstactPDM);
                    PDM.BatchGet();
                }
                catch (Exception)
                {

                    MessageBox.Show("Error extactFilePDM");
                }
            }
        
            if (listPartDraw.Count > 0) Update(listPathDrawParts, listPDMDrawPart);
         
            if (listAss.Count > 0)
            {
              
                Update(listPathAss, listPDMAss);            
            }

            if (listAssDraw.Count > 0) Update(listPathDrawAss, listPDMDrawAss);

           
             
            return true;
        }

      /*  private bool Refresh()
        {
            Tree.Refresh();
            Tree.listComp.ForEach(c => c.ResetState());
            Tree.listDraw.ForEach(c => c.ResetState());         
            Tree.CompareVersions();
            return true;
        }*/


        private void Update(List<string> listToSw, List<PdmID> listPdm)
        {
            try
            {     
                sw.OpenAndRefresh(listToSw);
            }
            catch (Exception)
            {
                MessageBox.Show("Error updating SW");

            }
            try
            {
                PDM.AddSelItemToList(listPdm);
                PDM.DocBatchUnLock();
             
            }
            catch (Exception)
            {
                MessageBox.Show("Error BatchUnLockPDM");

            }

        }
  

        internal  List<ViewUser> JoinCompAndDraw( )
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

