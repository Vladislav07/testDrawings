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
        OnlyDraw=0,
        ModelAndDraw=1,
        DrawFromModel=2,
        Clean=3,
        Blocked=4,
        NotRebuild=5,
    }
  public  class Model: IDisplay, IRebuild
    {
        public event Action<string> NotificationParent;
    
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

        public IEdmFile5 File { get; set; }

        public Model(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            st = StateModel.Clean;
            File = null;
            Ext = Path.GetExtension(FullPath);
            if(Ext == ".sldasm" ||Ext == ".SLDASM")
                {
                    listRefChild = new Dictionary<string, int>();
                    listRefChildError = new Dictionary<string, string>();
                }
          
            listParent = new List<string>();
                 
        }
        public void SetState()
        {
            if (Ext == ".sldprt" || Ext == ".SLDPRT") return;
            this.GetReferenceFromAssemble();

            bool isRebuildAsm = false;
            isRebuildAsm = isNeedsRebuld();
        
            if (isRebuildAsm )
            {
                st = StateModel.ModelAndDraw;
            }

            if (st == StateModel.ModelAndDraw || st == StateModel.DrawFromModel)
            {
                    foreach (string item in listParent)
                {
                    NotificationParent.Invoke(item);
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

        public string[] Print()
        {
            string[] listDisplay = new string[4];
            listDisplay[0] = CubyNumber;
            if(Ext == ".sldprt" || Ext == ".SLDPRT")
            {
                listDisplay[1] = "Part";
            }
            else if (Ext == ".sldasm" || Ext == ".SLDASM")
            {
                listDisplay[1] = "Assemble";
            }
            else
            {
                listDisplay[1] = "Other";
            }
            listDisplay[2] = st.ToString();
            listDisplay[3] = Level.ToString();
            return listDisplay;
        }

        public List<PdmID> GetIDFromPDM()
        {
            List<PdmID> list = new List<PdmID>();
            list.Add(new PdmID(bFile, bFolder));
            return list;
        }

        public string GetPath()
        {
            return FullPath;
        }

       public void RefreshPdmFile()
        {
            File.Refresh();
        }
    }
}

