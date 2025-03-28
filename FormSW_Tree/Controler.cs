using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

   public static class Controler
    {
        public static event Action<string> NumberModel;
        public static event Action<string> MsgState;
        public static event Action<string, List<IDisplay>> ActionRebuild;
        static SW sw;
        private static List<Model> models { get; set; }

 

         static Controler()
        {
            models = new List<Model>();
        }

        public static bool Init()
        {
            sw = new SW();
            sw.numberModel += Sw_numberModel;
            sw.action += Sw_action;
            sw.btnConnectSW();
            sw.BuildTree();
            Tree.SearchParentFromChild();
            Tree.FillCollection();
            return true;
        }

        private static void Sw_action(string msg)
        {
            MsgState.Invoke(msg);
        }

        private static void Sw_numberModel(string obj)
        {
            if (NumberModel!=null)
            {
                string number = Path.GetFileName(obj);
                NumberModel.Invoke(number);
            }
        }

        public static bool GetInfoFromPDM()
        {
            Tree.GetInfoPDM();
            Tree.CompareVersions();
            FilteringList();
            return true;
        }

        private static void FilteringList()
        {
            List<IDisplay> list = Tree.listComp.Where(comp => IsRebuidModel(comp)).Select(comp => (IDisplay)comp).ToList();
            List<IDisplay> listDraw = Tree.listDraw.Where(comp => IsRebuidDraw(comp)).Select(comp => (IDisplay)comp).ToList();

            ActionRebuild.Invoke("list", list.Concat(listDraw).ToList());
        }
   
        public static bool RebuildTree()
        {
            List<IRebuild> listPartDraw = Tree.listDraw.Where(d => IsRebuidDraw(d))
                .Where(d=>d.model.Ext == ".sldprt"|| d.model.Ext == ".SLDPRT")
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
            List<PdmID> listPdmAss= new List<PdmID>();
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



            if (listPartDraw.Count > 0) UpdateDraw(listPdmDrawParts, listPathDrawParts);

            if (listAss.Count > 0)
            {
                listPathAss.Reverse();
                Update(listPdmAss, listPathAss);
            }

            if (listAssDraw.Count>0) UpdateDraw(listPdmDrawAss, listPathDrawAss);

            Refresh();
            string p = Tree.listComp.First(c => c.Level == 0).FullPath;
            sw.OpenFile(p);
            return true;
        } 

        private static bool Refresh()
        {
            Tree.Refresh();
            Tree.CompareVersions();
            FilteringList();
            return true;
        }


        private static void Update(List<PdmID> listToPdm, List<string> listToSw)
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
        private static void UpdateDraw(List<PdmID> listToPdm, List<string> listToSw)
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

        static Predicate<Model> IsCuby = (Model comp) =>
        {
            string regCuby = @"^CUBY-\d{8}$";
            return Regex.IsMatch(comp.CubyNumber, regCuby);
        };

        static Predicate<Model> IsRebuidModel = (Model comp) => comp.st == StateModel.ModelAndDraw|| comp.st == StateModel.DrawFromModel;
        static Predicate<Drawing> IsRebuidDraw = (Drawing comp) => comp.st == StateModel.OnlyDraw || comp.st == StateModel.DrawFromModel;
        static Predicate<Model> IsParts = (Model comp) => comp.Ext == ".sldprt" || comp.Ext == ".SLDPRT";
        static Predicate<Model> IsAsm = (Model comp) => comp.Ext == ".sldasm" || comp.Ext == ".SLDASM";
      
    }
}

