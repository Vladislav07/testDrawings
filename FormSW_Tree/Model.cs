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
        Init=7,
        Stand=8,
        Initiated=9,
        UpdateDrawing=10,
        ChildCannotBeUpdated=11,

    }
    [Flags]
   
    public enum StateMode : ushort
    {
        None = 0,
        OnlyDraw = 1 << 0,          // 0b0000000000000001
        DrawFromPart = 1 << 1,      // 0b0000000000000010
        OnlyAss = 1 << 2,           // 0b0000000000000100
        ExtractPart = 1 << 3,       // 0b0000000000001000
        Clean = 1 << 4,             // 0b0000000000010000
        Blocked = 1 << 5,           // 0b0000000000100000
        ImpossibleRebuild = 1 << 6, // 0b0000000001000000
        Init = 1 << 7,              // 0b0000000010000000
        Stand = 1 << 8,             // 0b0000000100000000
        Initiated = 1 << 9,         // 0b0000001000000000
        UpdateDrawing = 1 << 10,    // 0b0000010000000000
        ChildCannotBeUpdated = 1 << 11 // 0b0000100000000000
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

        public virtual void SetMode()
        {

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

