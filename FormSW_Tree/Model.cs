using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace FormSW_Tree
{
    public enum StateModel
    {
        Rebuild=0,   
        Manufacturing=1,  
        Standart,
        Clean=4,         
        Blocked=5,
      

    }

    public abstract class Model
    {      
        public string CubyNumber { get; private set; }
        public string FullPath { get; private set; }
        public string Ext { get; private set; }
        public string Section { get; set; }
        public int bFolder { get; set; }
        public int bFile { get; set; }
        public int Level { get; set; }
        public bool IsVirtual { get; set; } = false;

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
                case "In work":
                    condition = new ModeClear();
                    break;

                case "Initiated":
                case "Use is forbidden":
                    condition = new ModeBloced();
                    break;
              
                case "Pending Express Manufacturing":
                case "Express Manufacturing":
                case "Reset to in Work":
                case "Approved to use":
                    condition = new ModeManufacturing();
                    break;
 
                case "Check library item":
                case "Kanban":            
                    condition = new ModeStandart();
                    break;

                default:
                    condition = new ModeClear();
                    break;
            }

           
        }

    }
}

