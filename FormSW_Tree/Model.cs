using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;


namespace FormSW_Tree
{
    public enum StateModel
    {
        OnlyDraw=0,   //u drawing
        DrawFromPart=1,  //u part,u drawing
        OnlyAss=2,
        ExtractPart = 3,  //extract
        Clean=4,         
        Blocked=5,
        Init=7,   
        Stand=8,
        Initiated=9,
        ChildCannotBeUpdated=10,

    }

    public abstract class Model: IRebuild
    {
       
        public string CubyNumber { get; private set; }
        public string FullPath { get; private set; }
        public string Ext { get; private set; }
        public string Section { get; set; }
        public int bFolder { get; set; }
        public int bFile { get; set; }
        public int Level { get; set; }

        internal ModelCondition condition;

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
          
        }
        public virtual void SetMode()
        {
            condition = CreateModeCondition();
        }

        internal ModelCondition CreateModeCondition()
        {
            ModelCondition mode = null;
            switch (File.CurrentState.Name)
            {
                case "Check library item":
                case "Kanban":
                case "Approved to use":
                case "In work":
                    mode = new ModeClear();
                    break;

                case "Initiated":
                    mode = new ModeBloced();
                    break;

                case "Pending Express Manufacturing":
                case "Express Manufacturing":
                case "Reset to in Work":
                    mode = new ModeManufacturing();
                    break;

                default:
                    break;
            }

            return mode;
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

