using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    public class Drawing
    {
        public string path { get; set; }
        public int FileID { get; set; }
        public int FolderID { get; set; }
        public IEdmFile7 bFile { get; set; }
        public string CubyNumber{get;set;}
        string msgRefVers;
        public StateModel st { get; set; }
        Model model;
        public Drawing(string _path, Model _m, IEdmFile7 _bFile, int _bFolder)
        {
            path = _path;
            Model model = _m;
            bFile = _bFile;
            FileID = _bFile.ID;
            FolderID = _bFolder;
            st = StateModel.Init;
        }

      

        public void SetState()
        {
            bool rf = RevVersion(out msgRefVers);
             
            if (!NeedsRebuild && rf)
            {
                st = StateModel.OnlyDraw;
            }          
            else if (!NeedsRebuild && !rf)
            {
                st = StateModel.Clean;
            }
            else 
            {
                st = StateModel.DrawFromModel;
            }
         
        }


         bool IsWork
        {
            get { return (bFile.CurrentState.Name == "In work") ? true : false; }
        }

         bool NeedsRebuild
        {
            get { return (bFile.NeedsRegeneration(bFile.CurrentVersion, FolderID)) ? true : false; }
        }

         bool RevVersion(out string msgRefVers)
        {
            int refDrToModel = this.GetRefVersion();
            msgRefVers = model.File.CurrentVersion.ToString() + "/" + refDrToModel.ToString();
            return (refDrToModel == model.File.CurrentVersion) ? true : false;
        }
       
    }
}
