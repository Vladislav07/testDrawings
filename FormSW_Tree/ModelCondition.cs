using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    internal abstract class ModelCondition
    {
        public abstract ModelCondition GetState(bool isMode);
              
    }

 
    internal  class ModeClear : ModelCondition
    {
        public override ModelCondition GetState(bool isMode)
        {
            ModelCondition mode = null;
            if (isMode) {
                mode = new ModeRebuild(); 
            }
            else
            {
                mode=this; 
            } 
            return mode;
        }

    }
    internal  class ModeBloced : ModelCondition
    {
        public override ModelCondition GetState(bool isMode)
        {
            return this;
        }

    }

    internal  class ModeRebuild : ModelCondition
    {
        public override ModelCondition GetState(bool isMode)
        {

        }

    }
    internal class ModeManufacturing : ModelCondition
    {
        public override ModelCondition GetState(bool isMode)
        {
            ModelCondition mode = null;
            if (isMode)
            {
                mode = new ModeBloced();
            }
            else
            {
                mode = this;
            }
            return mode;
        }

    }
}
