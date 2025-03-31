using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    internal class Assemble : Model
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
            bool isRebuildAsm = false;
            isRebuildAsm = isNeedsRebuld();

            if (isRebuildAsm)
            {
                st = StateModel.ModelAndDraw;
            }
            else
            {
                st = StateModel.Clean;
            }

            base.SetState();

        }
        bool isNeedsRebuld()
        {
            if (listRefChild.Count == 0) return false;
            listRefChildError.Clear();
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

        public override string[] Print()
        {
            string[] listDisplay = new string[4];
            listDisplay[0] = CubyNumber;
            listDisplay[1] = "Assemble";
            listDisplay[2] = st.ToString();
            listDisplay[3] = Level.ToString();
            listDisplay[4] = File.CurrentState.Name.ToString();
            return listDisplay;
        }

     
    }
}
