using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;


namespace FormSW_Tree
{
    public enum StateModel
    {
        OnlyDraw=0,   
        DrawFromPart=1,
        OnlyAss=2,
        ExtractPart = 3,
        Clean=4,
        Blocked=5,
        ImpossibleRebuild = 6,
        Init=7
    }
  public abstract class Model: IRebuild
    {
        //public virtual event Action<string, StateModel> NotificationParent;
        public string CubyNumber { get; private set; }
        public string FullPath { get; private set; }
        public string Ext { get; private set; }
        public string Section { get; set; }
        public int bFolder { get; set; }
        public int bFile { get; set; }
        public int Level { get; set; }

        public StateModel st { get; set; }

        public IEdmFile7 File { get; set; }

        public Model(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            Ext = Path.GetExtension(fn);
            st = StateModel.Init;
            File = null;
        
                 
        }
        public virtual void SetState()
        {
            if (st == StateModel.Init) st = StateModel.Clean;
            if (IsWork) return;
         
            if (st == StateModel.Clean)
            {
                st = StateModel.Blocked;
            }
            else
            {
                st = StateModel.ImpossibleRebuild;
            }
           
        }

        bool IsWork
        {
            get { return (File.CurrentState.Name == "In work") ? true : false; }
        }

        public virtual  List<PdmID> GetIDFromPDM()
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

        public virtual void ResetState()
        {
            st = StateModel.Init;
        }

    }
}

