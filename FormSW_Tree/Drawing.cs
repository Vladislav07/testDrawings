using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    public class Drawing: Model
    {

        public string msgRefVers;
        public Model model { get; set; }
        public bool isPart { get; set; }

        public Drawing(string cn, string fn, Model _m,
            IEdmFile7 _bFile, int _bFolder):base(cn,fn)
        {
       
            model = _m;
            bFile = _bFile.ID;
            File = _bFile;
            bFolder = _bFolder;
         
         
            if(model.Ext == ".SLDPRT" || model.Ext == ".sldprt")
            {
                isPart = true;
            }
            else
            {
                isPart = false;
            }
        }


        public override void SetState()
        {
            try
            {
                base.SetState();
                bool rf = RevVersion(ref msgRefVers);
                bool isRebuldDraw = (NeedsRebuild || rf) ? true:false;               
                condition = condition.GetState(isRebuldDraw);
                model.condition = model.condition.GetState(isRebuldDraw);
            }
            catch (Exception)
            {

               
            }
          
        }

        public void CompareStateFromModel()
        {
            StateModel st = model.condition.stateModel;
           // StateModel stDr = condition.stateModel;
            if (st.ToString()== "Rebuild")
            {
                condition=condition.GetState(true);
            }
        }
        

        public bool NeedsRebuild
        {
            get { return (File.NeedsRegeneration(File.CurrentVersion, bFolder)) ? true : false; }
        }

         bool RevVersion(ref string msgRefVers)
        {
            int refDrToModel = this.GetRefVersion();
            msgRefVers = model.File.CurrentVersion.ToString() + "/" + refDrToModel.ToString();
            return (refDrToModel == model.File.CurrentVersion) ? false:true;
        }


    }
}
