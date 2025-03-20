using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace FormSW_Tree
{
    enum stateApp
    {
        readForUpdate=0,
        updateIsNotPossible=2,
        detailsUpdate=3,
        partsDrawings=4,
        assemble=5,
        asembleDrawings=6,
        errorUpdate=7
    }
   public static class Controler
    {
        public static event Action<string> NumberModel;
        static SW sw;
        public static bool isIncludeDraw { get; set; }
        private static List<Component> models { get; set; }
        private static List<Drawing> drawings { get; set; }

         static Controler()
        {
            models = new List<Component>();
            drawings = new List<Drawing>();
        }
        public static bool Init()
        {
            isIncludeDraw = true;
            sw = new SW();
            sw.numberModel += Sw_numberModel;
            sw.btnConnectSW();
            sw.BuildTree();
            Tree.SearchParentFromChild();

            return true;
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
            Tree.FillCollection();
            Tree.CompareVersions();
            if (isIncludeDraw) Tree.IsDrawings();
            return true;
        }

        public static bool RebuildTree()
        {
            RetrievingParts();
         

            return true;
        }

        public static bool Cancel()
        {
            return true;
        }

        private static void RetrievingParts()
        {
            foreach (Component comp in Tree.listComp)
            {
               if (comp.Section=="Стандартные изделия" && comp.IsRebuild)
                  {
                    models.Add(comp);
                  }           
            }
        }

        private static void Upadate(List<Component> l)
        {
            PDM.AddSelItemToList(l);
            PDM.BatchGet(l);
            sw.OpenAndRefresh(l);
            PDM.DocBatchUnLock();
        }
    }
}
