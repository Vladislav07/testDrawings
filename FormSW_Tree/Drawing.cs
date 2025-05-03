using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    public class Drawing: Model, IRebuild
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
            st = StateModel.Init;
         
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

            bool rf = RevVersion(ref msgRefVers);
            if (isPart)
            {
                if (model.st == StateModel.ExtractPart)
                {
                    st = StateModel.DrawFromPart;
                }

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
                    st = StateModel.DrawFromPart;
                    model.st = StateModel.ExtractPart;
                }
                
              
            }
            else
            {
                if (NeedsRebuild || rf)
                {
                    st = StateModel.OnlyDraw;
                }
                else
                {
                    st = StateModel.Clean;
                }

                if (model.st == StateModel.OnlyAss)
                {
                    st = StateModel.OnlyDraw;
                }
            }

            if (File.CurrentState.Name != "In work" && (st == StateModel.OnlyDraw || st == StateModel.DrawFromPart))
            {
                st = StateModel.Blocked;
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


        public override List<PdmID> GetIDFromPDM()
        {
            List<PdmID> list = new List<PdmID>();
            
            list.Add(new PdmID(bFile, bFolder));
            if (st == StateModel.DrawFromPart)
            {
                list.Add(new PdmID(model.bFile, model.bFolder));
            }
            return list;
        }

        public override void ResetState()
        {
            model.st = StateModel.Init;
            base.ResetState();
        }
    

    }
}
