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
  public abstract class Model: IDisplay, IRebuild
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
      
        public List<string> listParent;

        public IEdmFile5 File { get; set; }

        public Model(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            Ext = Path.GetExtension(fn);
            st = StateModel.Clean;
            File = null;
            listParent = new List<string>();
                 
        }
        public virtual  void SetState()
        {
            if (st == StateModel.ModelAndDraw || st == StateModel.DrawFromModel)
            {
                foreach (string item in listParent)
                {
                    NotificationParent.Invoke(item);
                }

            }
        }
        public abstract string[] Print();
   
      

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

