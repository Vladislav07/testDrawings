using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{

    internal class Assemble : Part
    {
        public Dictionary<string, int> listRefChild;
        public Dictionary<string, string> listRefChildError;
        internal Assemble(string cn, string fn) : base(cn, fn)
        {
            listRefChild = new Dictionary<string, int>();
            listRefChildError = new Dictionary<string, string>();
        }

        public override void SetState()
        {
            this.GetReferenceFromAssemble();

            bool isRebuildAsm = isNeedsRebuld();
  
            if (isRebuildAsm && st != StateModel.ChildCannotBeUpdated)
            {
                st = StateModel.OnlyAss;
            }
        
            if (File.CurrentState.Name != "In work" && st == StateModel.OnlyAss)
            {
                st = StateModel.Blocked;
                NotificationState();
            }
            else if(File.CurrentState.Name == "In work" && st == StateModel.OnlyAss)
            {
                NotificationState();
            }
            if (st == StateModel.Init) st = StateModel.Clean;

        }
        public void CascadingUpdate(Model child)
        {
            switch (child.st)
            {               
                case StateModel.OnlyAss:
                case StateModel.ExtractPart:
                    st = StateModel.OnlyAss;
                    break;

           
                case StateModel.Blocked:
                case StateModel.Initiated:
                case StateModel.ChildCannotBeUpdated:
                    st = StateModel.ChildCannotBeUpdated;

                    break;
                
                default:
                    break;
            }
        }

        bool isNeedsRebuld()
        {
            if (listRefChild.Count == 0) return false;
          
            foreach (KeyValuePair<string, int> item in listRefChild)
            {

                int isVers = Tree.Part_IsChild(item.Key, item.Value);

                if (isVers != -1)
                {
                    listRefChildError.Add(item.Key, item.Value.ToString() + "/" + isVers.ToString());
                }


            }
            return (listRefChildError.Count > 0) ? true : false;

        }
      
    }
}