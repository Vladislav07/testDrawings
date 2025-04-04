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
        internal string Level { get; set; }
        internal string State { get; set; }
        internal string Version { get; set; }
        internal string IsLocked { get; set; }
        internal string DrawState { get; set; }
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
            f.action += F_action;
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

        private void F_action()
        {
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
            sw.CloseDoc();
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

            if (listPart.Count > 0) Update(listPdmParts, listPathParts);

            if (listPartDraw.Count > 0) UpdateDraw(listPdmDrawParts, listPathDrawParts);

            if (listAss.Count > 0)
            {
                listPathAss.Reverse();
                Update(listPdmAss, listPathAss);
            }

            if (listAssDraw.Count > 0) UpdateDraw(listPdmDrawAss, listPathDrawAss);

            Refresh();
            string p = Tree.listComp.First(c => c.Level == 0).FullPath;
            sw.OpenFile(p);
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
                MessageBox.Show("Error updating model");

            }

        }
        private void UpdateDraw(List<PdmID> listToPdm, List<string> listToSw)
        {
            try
            {
                PDM.AddSelItemToList(listToPdm);
                PDM.BatchGet(listToPdm);
                sw.OpenAndRefreshDrawings(listToSw);
                PDM.DocBatchUnLock();
            }
            catch (Exception)
            {
                MessageBox.Show("Error updating Draw");

            }

        }

        internal void FillToListIsRebuild(ref DataTable dt)
        {
           // List<ViewUser> userView=Tree.listComp.Join

            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("Cuby Number", typeof(string));
            dt.Columns.Add("Current Version", typeof(string));
            dt.Columns.Add("List of Ref Child Errors", typeof(string));
            dt.Columns.Add("Child", typeof(string));
            dt.Columns.Add("Child info", typeof(string));
            dt.Columns.Add("State", typeof(string));

            foreach (Model comp in Tree.listComp)
            {
                DataRow dr = dt.NewRow();
                dr[0] = comp.Level.ToString();
                dr[1] = comp.CubyNumber;
                dr[2] = comp.Section;
                dr[3] = comp.st.ToString();
                dr[4] = "";
                dr[5] = "";
                dr[6] = "";


                dt.Rows.Add(dr);
            }

        }

        Predicate<Model> IsCuby = (Model comp) =>
       {
           string regCuby = @"^CUBY-\d{8}$";
           return Regex.IsMatch(comp.CubyNumber, regCuby);
       };

        Predicate<Model> IsRebuidModel = (Model comp) => comp.st == StateModel.ModelAndDraw || comp.st == StateModel.DrawFromModel;
        Predicate<Drawing> IsRebuidDraw = (Drawing comp) => comp.st == StateModel.OnlyDraw || comp.st == StateModel.DrawFromModel;
        Predicate<Model> IsParts = (Model comp) => comp.Ext == ".sldprt" || comp.Ext == ".SLDPRT";
        Predicate<Model> IsAsm = (Model comp) => comp.Ext == ".sldasm" || comp.Ext == ".SLDASM";

    }

    
}

