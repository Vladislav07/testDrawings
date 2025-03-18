using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;

namespace FormSW_Tree
{
    public static class Tree
    {

        static Dictionary<string, Component> ModelTree;
        public static List<Component> listComp;
        public static List<Drawing> listDraw;
        static Dictionary<string, string> structuralNumbers;
        static Tree()
        {
           
            ModelTree = new Dictionary<string, Component>();
            listComp = new List<Component>();
            listDraw = new List<Drawing>();
            structuralNumbers = new Dictionary<string, string>();
        }
        public static void AddNode(string NodeNumber, string cubyNumber, string pathNode)
        {

            ModelTree.Add(NodeNumber, GetComponentFromNumber(cubyNumber, pathNode));
            structuralNumbers.Add(NodeNumber, cubyNumber);
        }

        public static Component GetComponentFromNumber(string numberCuby, string path)
        {
            Component comp = null;
            foreach (KeyValuePair<string, Component> item in ModelTree)
            {
                comp = item.Value;
                if (numberCuby == comp.CubyNumber) return comp; 
            }
            comp = new Component(numberCuby, path);
            return comp;
        }

        public static void CompareVersions()
        {
            listComp.Reverse();
            foreach (Component item in listComp)
            {
                item.isNeedsRebuld();
            }
            listComp.Reverse();
        }

        public static bool IsDrawings()
        {
            foreach (Component item in listComp)
            {
                item.IsDrawing();
            }
            if (listDraw.Count>0)
            {
                return true;
            }
            return false;
           
        }
  
        public  static int Part_IsChild(string cubyNumber, int VersChild)
          {
     
            Component comp = listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
              if (comp == null) return -1;
              if (comp.CurVersion != VersChild) return comp.CurVersion;
              return -1;
          }
      
        public static void SearchParentFromChild()
        {

            string StructureNumberChild;
            string ParentStructurenumber;
            int index = 0;
            char separate = new char[] { '.' }[0];
            Component child = null;
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
                child.listParent.Add(parentNumber);

            }
      
        }
      
        public static void FillCollection()
        {
            
            char s = new char[] { '.' }[0];
            int level_ = 0;
            var uniqueComponentByGroup = ModelTree
            .GroupBy(pair => pair.Key.Count(o => o == s))
            .Select(group => group.Select(g => g.Value).Distinct().ToList())
            .ToList();
            foreach (var item in uniqueComponentByGroup)
            {
                foreach (Component comp in item)
                {
                    comp.Level = level_;
                    comp.GetEdmFile();
                    comp.GetReferenceFromAssemble();
                    listComp.Add(comp);
                }
                level_++;
            }
        }

        public static void SearchForOldLinks(string cubyNumber)
        {
            Component comp = listComp.FirstOrDefault(p => p.CubyNumber == cubyNumber);
            if (comp == null) return;
            comp.IsRebuild = true;
        }

        public static void FillToListIsRebuild(ref DataTable table)
        {

            table.Columns.Add("Level", typeof(string));
            table.Columns.Add("Cuby Number", typeof(string));
            table.Columns.Add("Current Version", typeof(string));
            table.Columns.Add("List of Ref Child Errors", typeof(string));
            table.Columns.Add("Child", typeof(string));
            table.Columns.Add("Child info", typeof(string));
            table.Columns.Add("State", typeof(string));
           
           // int level = 0;

            foreach (Component comp in listComp)
            {
               // DataRow workRow = table.NewRow();
                
                table.Rows.Add(comp.Level.ToString(), comp.CubyNumber, comp.CurVersion.ToString(), comp.IsRebuild.ToString(), "", "", "comp.State.Name.ToString()");
              
               if (comp.listRefChildError.Count != 0)
              {
                   foreach (KeyValuePair<string, string> i in comp.listRefChildError)

                    {
                        table.Rows.Add("", "", "", "", i.Key, i.Value, "");
                    }
                }
              
              //  level++;
            }
        }

        public static void PossibilityOfUpdating()
        {

        }
    }
}
