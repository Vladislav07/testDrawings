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

        public Controler(InfoF _f)
        {
            WorkerReportsProgress = true;
            f = _f;
            f.Action += F_action;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Init();
        }

        public bool Init()
        {
            sw = new SW();
            sw.connectSw += Sw_connectSw;
            sw.btnConnectSW();
            return true;
        }

        private List<ViewUser> F_action()
        {
            sw.CloseDoc();
            RebuildTree();
            Refresh();
            string p = Tree.listComp.First(c => c.Level == 0).FullPath;
            sw.OpenFile(p);
            List<ViewUser> LV = JoinCompAndDraw();
            return LV;
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
                Tree.SearchParentFromChild();
                Tree.FillCollection();
                GetInfoFromPDM();
            }
        }

        public bool GetInfoFromPDM()
        {
            Tree.GetInfoPDM();
            Tree.CompareVersions();
            return true;
        }


        public bool RebuildTree()
        {
            
            List<IRebuild> listPart = Tree.listComp.Where(d => IsRebuidModel(d))
               .Where(d => d.Ext == ".sldprt" || d.Ext == ".SLDPRT")
               .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPdmParts = new List<PdmID>();
            List<string> listPathParts = new List<string>();
            listPart.ForEach(d =>
            {
                listPdmParts.AddRange(d.GetIDFromPDM());
                listPathParts.Add(d.GetPath());
            });

            List<IRebuild> listPartDraw = Tree.listDraw.Where(d => IsRebuidDraw(d))
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPdmDrawParts = new List<PdmID>();
            List<string> listPathDrawParts = new List<string>();
            listPartDraw.ForEach(d =>
            {
                listPdmDrawParts.AddRange(d.GetIDFromPDM());
                listPathDrawParts.Add(d.GetPath());
            });


            List<IRebuild> listAss = Tree.listComp.Where(c => IsRebuidModel(c))
                .Where(c => IsAsm(c))
                .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPdmAss = new List<PdmID>();
            List<string> listPathAss = new List<string>();
            listAss.ForEach(a =>
            {
                listPdmAss.AddRange(a.GetIDFromPDM());
                listPathAss.Add(a.GetPath());
            });


            List<IRebuild> listAssDraw = Tree.listDraw.Where(d => IsRebuidDraw(d))
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .Select(d => (IRebuild)d).ToList();
            List<PdmID> listPdmDrawAss = new List<PdmID>();
            List<string> listPathDrawAss = new List<string>();
            listAssDraw.ForEach(d =>
            {
                listPdmDrawAss.AddRange(d.GetIDFromPDM());
                listPathDrawAss.Add(d.GetPath());
            });

            if (listPart.Count > 0)
            {
                Update(listPdmParts, listPathParts);
               
            }

            if (listPartDraw.Count > 0)
            {
                Update(listPdmDrawParts, listPathDrawParts);
               
            }

                if (listAss.Count > 0)
            {
                listPathAss.Reverse();
                Update(listPdmAss, listPathAss);
             
            }

            if (listAssDraw.Count > 0)
            {
                Update(listPdmDrawAss, listPathDrawAss);
                
            }
             Tree.listComp.ForEach(c => c.ResetState());
             Tree.listDraw.ForEach(c => c.ResetState());
            return true;
        }

        private bool Refresh()
        {
            Tree.Refresh();
            Tree.CompareVersions();
            return true;
        }


        private void Update(List<PdmID> listToPdm, List<string> listToSw)
        {
            try
            {
                PDM.AddSelItemToList(listToPdm);
                PDM.BatchGet(listToPdm);
                sw.OpenAndRefresh(listToSw);
                PDM.DocBatchUnLock();
            }
            catch (Exception)
            {
                MessageBox.Show("Error updating");

            }

        }
  

      

 

        Predicate<Model> IsCuby = (Model comp) =>
       {
           string regCuby = @"^CUBY-\d{8}$";
           return Regex.IsMatch(comp.CubyNumber, regCuby);
       };

        Predicate<Model> IsRebuidModel = (Model comp) => comp.st == StateModel.OnlyAss || comp.st == StateModel.ExtractPart;
        Predicate<Drawing> IsRebuidDraw = (Drawing comp) => comp.st == StateModel.OnlyDraw || comp.st == StateModel.DrawFromPart;
        Predicate<Model> IsParts = (Model comp) => comp.Ext == ".sldprt" || comp.Ext == ".SLDPRT";
        Predicate<Model> IsAsm = (Model comp) => comp.Ext == ".sldasm" || comp.Ext == ".SLDASM";

        internal  List<ViewUser> JoinCompAndDraw( )
        {
            List<Model> compList = Tree.listComp;
            List<Drawing> drawList = Tree.listDraw;
            List<ViewUser> lv = new List<ViewUser>();
            foreach (Part item in compList)
            {
                Drawing dr = drawList.FirstOrDefault(d => d.CubyNumber == item.CubyNumber);

              lv.Add(  new ViewUser
                {
                    NameComp = item.CubyNumber,
                    TypeComp = item.Section,
                    Ext=item.Ext,
                    Level = item.Level.ToString(),
                    StPDM= item.File.CurrentState.Name.ToString(),
                    State =item.st.ToString(),
                    VersionModel = item.File?.CurrentVersion.ToString() ?? "",
                    IsLocked = item.File?.IsLocked.ToString() ?? "",

                    DrawState = dr != null ? dr.st.ToString() : "",
                    StDrPDM = dr != null ? dr.File.CurrentState.Name : "",
                    // DrawVersRev = result.Draw != null ? result.Draw.Version.ToString() : "none",
                    // DrawNeedRebuild = result.Draw != null ? result.Draw.NeedRebuild.ToString() : "none",
                    DrawIsLocked = dr != null ? dr.File?.IsLocked.ToString() : ""
                });
            }
         

            return lv;
        }
    }

    
}

