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
        public static List<Model> listComp;
        public static List<Drawing> listDraw;
        static Dictionary<string, string> structuralNumbers;
        static Tree()
        {
           
            ModelTree = new Dictionary<string, Part>();
            listComp = new List<Model>();
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

        private static void Comp_NotificationParent(string cubyNumber, StateModel st)
        {
            Assemble comp =(Assemble) listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
            if (comp == null) return;
            comp.CascadingUpdate(st);
   
        }

        public static void CompareVersions()
        {
            foreach (Drawing item in listDraw)
            {
                if (!item.isPart) continue;
                item.SetState();
            }

            listComp.Reverse();
            foreach (Model item in listComp)
            {
               if( item is Part part)
                    {
                     part.SetState();
                    }
               else if( item is Assemble ass)
                    {
                     ass.SetState();
                    } 
                
            }
            listComp.Reverse();
       

            foreach (Drawing item in listDraw)
            {
                if (item.isPart) continue;
                item.SetState();
            }
          
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
            .Select(group => group.Select(g => g.Value).Distinct().ToList())
            .ToList();
            foreach (var item in uniqueModelByGroup)
            {
                foreach (Model comp in item)
                {
                   
                    comp.Level = level_;                  
                    listComp.Add(comp);
                }
                level_++;
            }
        }

     

        public static void GetInfoPDM()
        {
            foreach (Model comp in listComp)
                {
                    comp.GetEdmFile();
                    comp.IsDrawings();
                }
            
        }

        public static void Refresh()
        {
            listComp.ForEach(c => c.RefreshPdmFile());
            listDraw.ForEach(c => c.RefreshPdmFile());
        }

    }
}
