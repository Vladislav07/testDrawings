using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    public class Drawing: IRebuild
    {

        public string path { get; set; }
        public int FileID { get; set; }
        public int FolderID { get; set; }
        public IEdmFile7 bFile { get; set; }
        public string CubyNumber { get; set; }
        string msgRefVers;
        public StateModel st { get; set; }
        public Model model { get; set; }
        public bool isPart { get; set; }

        public Drawing(string _path, Model _m, IEdmFile7 _bFile, int _bFolder)
        {
            path = _path;
            model = _m;
            bFile = _bFile;
            FileID = _bFile.ID;
            FolderID = _bFolder;
            st = StateModel.Clean;
            CubyNumber = model.CubyNumber;
            if(model.Ext == ".SLDPRT" || model.Ext == ".sldprt")
            {
                isPart = true;
            }
            else
            {
                isPart = false;
            }
        }

  

        public void SetState()
        {
            bool rf = RevVersion(ref msgRefVers);
             
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

                if (model.st == StateModel.Clean)
                {
                    model.st = StateModel.DrawFromModel;
                }
               
            }

            if (model.st == StateModel.ModelAndDraw)
            {
                st = StateModel.OnlyDraw;
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
            get { return (bFile.CurrentState.Name == "In work") ? true : false; }
        }

         bool NeedsRebuild
        {
            get { return (bFile.NeedsRegeneration(bFile.CurrentVersion, FolderID)) ? true : false; }
        }

         bool RevVersion(ref string msgRefVers)
        {
            int refDrToModel = this.GetRefVersion();
            msgRefVers = model.File.CurrentVersion.ToString() + "/" + refDrToModel.ToString();
            return (refDrToModel == model.File.CurrentVersion) ? false:true;
        }


        public List<PdmID> GetIDFromPDM()
        {
            List<PdmID> list = new List<PdmID>();
            
            list.Add(new PdmID(FileID, FolderID));
            if (st == StateModel.DrawFromModel)
            {
                list.Add(new PdmID(model.bFile, model.bFolder));
            }
            return list;
        }

        public string GetPath()
        {
            return path;
        }

        public void RefreshPdmFile()
        {
            bFile.Refresh();
        }

    }
}
