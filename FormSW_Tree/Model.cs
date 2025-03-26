using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    public enum StateModel
    {
        Init=0,
        Clean=1,
        Blocked=2,
        NotRebuild=3,
        OnlyDraw=4,
        DrawFromModel=5,
        ModelAndDraw=6
    }
  public  class Model
    {
        public string CubyNumber { get; private set; }
        public string FullPath { get; private set; }
        public string Ext { get; private set; }
        public string Section { get; set; }
        public int bFolder { get; set; }
        public int bFile { get; set; }
        public int Level { get; set; }

        public StateModel st { get; set; }
        public Dictionary<string, int> listRefChild;
        public Dictionary<string, string> listRefChildError;
        public List<string> listParent;
        public Drawing draw { get; set; }

        public IEdmFile5 File { get; set; }

        public Model(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            st = StateModel.Init;
            File = null;
            Ext = Path.GetExtension(FullPath);
            if(Ext == ".sldasm" ||Ext == ".SLDASM")
                {
                    listRefChild = new Dictionary<string, int>();
                    listRefChildError = new Dictionary<string, string>();
                }
          
            listParent = new List<string>();
                 
        }
        public void IsState()
        {
            bool isRebuildAsm = false;

            if (Ext == ".sldasm" || Ext == ".SLDASM")
            {
                isRebuildAsm = isNeedsRebuld();
            }

            bool isDraw = PDM.IsDrawings(this);

            if(isRebuildAsm || (isDraw && draw.st == StateModel.DrawFromModel))
            {
                foreach (string item in listParent)
                {
                    Tree.SearchForOldLinks(item);
                }
            }
            
        }

        bool isNeedsRebuld()
        {
            if (listRefChild.Count == 0) return false;
            listRefChildError.Clear(); 
            foreach (KeyValuePair<string, int> item in listRefChild)
            {

                int isVers = Tree.Part_IsChild(item.Key, item.Value);

                if (isVers != -1)
                {
                    listRefChildError.Add(item.Key, item.Value.ToString() + "/" + isVers.ToString());
                }

            }
           return (listRefChildError.Count>0) ? true: false;
           
        }
       

   }
}

