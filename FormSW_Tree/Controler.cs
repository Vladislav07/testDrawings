using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace FormSW_Tree
{
   public static class Controler
    {
       public static event Action<string> NumberModel;
       static SW sw;
        public static bool isIncludeDraw { get; set; }

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
            return true;
        }

        public static bool Cancel()
        {
            return true;
        }
    }
}
