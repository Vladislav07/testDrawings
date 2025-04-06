using System;
using System.Collections.Generic;

namespace FormSW_Tree
{
    internal class Part : Model
    {
        public List<string> listParent;
        
        internal Part(string cn, string fn) : base(cn, fn)
        {
            listParent = new List<string>();
        }
      

        public override void SetState()
        {
        
            if (st == StateModel.ExtractPart)
            {
                foreach (string item in listParent)
                {
                    Notification(item, st);
                }
            }
            base.SetState();

        }
    }
}
