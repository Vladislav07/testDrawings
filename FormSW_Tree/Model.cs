using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;


namespace FormSW_Tree
{
    public enum StateModel
    {
        OnlyDraw=0,
        ModelAndDraw=1,
        DrawFromModel=2,
        Clean=3,
        Blocked=4,
        ImpossibleRebuild = 5,
        Model=6
    }
  public abstract class Model: IRebuild
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

        public IEdmFile7 File { get; set; }

        public Model(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            Ext = Path.GetExtension(fn);
            st = StateModel.Clean;
            File = null;
            listParent = new List<string>();
                 
        }
        public virtual void SetState()
        {
            if (st == StateModel.Clean && File.NeedsRegeneration(File.CurrentVersion, bFolder))
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

            if (!IsWork)
            {
                if (st == StateModel.Clean)
                {
                    st = StateModel.Blocked;
                }
                else
                {
                    st = StateModel.ImpossibleRebuild;
                }
            }


        }

        bool IsWork
        {
            get { return (File.CurrentState.Name == "In work") ? true : false; }
        }

        public  List<PdmID> GetIDFromPDM()
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

