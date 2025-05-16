using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace FormSW_Tree

{
   
    public static class Tree
    {

        static Dictionary<string, Part> ModelTree;
        public static List<Part> listComp;
        public static List<Drawing> listDraw;

        static Dictionary<string, string> structuralNumbers;
        internal static event Action<string[]> msgDataOperation;
        internal static event Action<string[]> msgNameOperation;
        static Tree()
        {
           
            ModelTree = new Dictionary<string, Part>();
            listComp = new List<Part>();
            listDraw = new List<Drawing>();
            structuralNumbers = new Dictionary<string, string>();
        }
        public static void AddNode(string NodeNumber, string cubyNumber, string pathNode)
        {

            ModelTree.Add(NodeNumber, GetModelFromNumber(cubyNumber, pathNode));

            structuralNumbers.Add(NodeNumber, cubyNumber);
        }

        private static Part GetModelFromNumber(string numberCuby, string path)
        {
            Part comp = null;
            foreach (KeyValuePair<string, Part> item in ModelTree)
            {
                comp = item.Value;
                if (numberCuby == comp.CubyNumber) return comp; 
            }
            string e = Path.GetExtension(path);
            if (e == ".SLDPRT" || e == ".sldprt")
            {
                comp = new Part(numberCuby, path);

            }
            else if (e == ".SLDASM" || e == ".sldasm")
            {
                comp = new Assemble(numberCuby, path);
                
            }
            else { 
                return null;
            }
              comp.NotificationParent += Comp_NotificationParent;
             
            return comp;
        }

        private static void Comp_NotificationParent(string cubyNumber, StateModel isBlocedchild)
        {
            Assemble comp =(Assemble) listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
            if (comp == null) return;
            comp.CascadingUpdate(isBlocedchild);
   
        }

       

        public  static int Part_IsChild(string cubyNumber, int VersChild)
          {
     
            Model comp = listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
            if (comp == null) return -1;
            if (comp.File.CurrentVersion != VersChild) return comp.File.CurrentVersion;
            return -1;
        }
      
        public static void SearchParentFromChild()
        {

            string StructureNumberChild;
            string ParentStructurenumber;
            int index = 0;
            char separate = new char[] { '.' }[0];
            Part child = null;
            string parentNumber=null;
            foreach (KeyValuePair<string, string> item in structuralNumbers)
            {
               
                child = ModelTree[item.Key];
                StructureNumberChild = item.Key;
                if (StructureNumberChild == "0") continue;
                if (child == null && child.CubyNumber != item.Value) continue;
                index = StructureNumberChild.LastIndexOf(separate);
                ParentStructurenumber = StructureNumberChild.Substring(0,index);
                if (!structuralNumbers.ContainsKey(ParentStructurenumber)) continue;
                parentNumber = structuralNumbers[ParentStructurenumber];
                if (child.listParent.Contains(parentNumber))continue;
                child.listParent.Add(parentNumber);

            }
      
        }
      
        public static void FillCollection()
        {
            listComp.Clear();
            char s = new char[] { '.' }[0];
            int level_ = 0;
            var uniqueModelByGroup = ModelTree
            .GroupBy(pair => pair.Key.Count(o => o == s))
            .Select(group => group.Select(g => g.Value).Distinct().ToList()).ToList();
                                                                           
            foreach (var item in uniqueModelByGroup)
            {
                foreach (Part comp in item)
                {
                   
                    comp.Level = level_; 
                    if(listComp.Contains(comp))continue;
                    listComp.Add(comp);
                }
                level_++;
            }
        }

     

        public static void GetInfoPDM()
        {
            int i = 1;
            InfoAboutProcessing("Extract from storage PDM", listComp.Count);
            foreach (Model comp in listComp)
             {
                comp.GetEdmFile();
                comp.IsDrawings();
                InfoDataProcessing(comp.CubyNumber, i);
                i++;
             }
            
        }
        public static void CompareVersions()
        {
            listComp.Reverse();
            listDraw.Reverse();

            listDraw.ForEach(dr => dr.SetState());
            listComp.ForEach(prt=>prt.SetState());
            listComp.ForEach(prt => {
                if (prt.condition.stateModel == StateModel.Clean || prt.condition.stateModel == StateModel.Manufacturing) return;
                prt.NotificationState(prt.condition.stateModel);
            });
            listDraw.ForEach(dr => dr.CompareStateFromModel());

        }

     /*   public static void Refresh()
        {
            int i = 1;
            InfoAboutProcessing("Updating part and assembly files from the repository PDM", listComp.Count);
            listComp.ForEach(c => {
                c.RefreshPdmFile();
               // InfoDataProcessing(c.CubyNumber, i);
                i++;
            });
            i = 1;
            InfoAboutProcessing("Updating part and assembly files from the repository PDM", listComp.Count);
            listDraw.ForEach(d =>
            {
                d.RefreshPdmFile();
               // InfoDataProcessing(d.CubyNumber, i);
                i++;
            });
        }*/

        private static void InfoAboutProcessing(string nameOper, int countCycl)
        {
            string[] mno = new string[2];
            mno[0] = nameOper;
            mno[1]=countCycl.ToString();
            msgNameOperation.Invoke(mno);
        }
        private static void InfoDataProcessing(string nameCuby, int i)
        {
           
            string[] mdata = new string[2];
            mdata[0]=nameCuby;
            mdata[1] = i.ToString();
            msgDataOperation.Invoke(mdata);
        }

    }
}
