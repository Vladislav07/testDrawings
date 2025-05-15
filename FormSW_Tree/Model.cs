﻿using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;


namespace FormSW_Tree
{
    public enum StateModel
    {
        Rebuild=0,   
        Manufacturing=1,  
        Clean=4,         
        Blocked=5,
      

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

        public List<string> listParent;

        internal ModelCondition condition;
        public IEdmFile7 File { get; set; }

        public Model(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            Ext = Path.GetExtension(fn);
          
            File = null;
        
                 
        }
        public virtual void SetState()
        {
           
            switch (File.CurrentState.Name)
            {
                case "Check library item":
                case "Kanban":
                case "Approved to use":
                case "In work":
                    condition = new ModeClear();
                    break;

                case "Initiated":
                    condition = new ModeBloced();
                    break;

                case "Pending Express Manufacturing":
                case "Express Manufacturing":
                case "Reset to in Work":
                    condition = new ModeManufacturing();
                    break;

                default:
                    condition = new ModeClear();
                    break;
            }

           
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

        public  void ResetState()
        {
            //
        }

   

    }
}

