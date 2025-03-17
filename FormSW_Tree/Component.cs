using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FormSW_Tree
{
    public class Component
    {
        public string CubyNumber { get; private set; }
        public string FullPath { get; private set; }
        public string Ext { get; private set; }
        IEdmFile5 _File = null;
        public int CurVersion { get; set; }
        public IEdmState5 State { get; set; }
        public int bFolder { get; set; }
        public int Level { get; set; }
        public Dictionary<string, int> listRefChild;
        public Dictionary<string, string> listRefChildError;
        public List<string> listParent;
        public bool IsRebuild { get; set; }
        public bool isDraw { get; set; }
        

        public Component(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            listRefChild = new Dictionary<string, int>();
            listRefChildError = new Dictionary<string, string>();
            listParent = new List<string>();
            IsRebuild = false;
            Ext = Path.GetExtension(FullPath);
            isDraw = false;
        }

        public IEdmFile5 File
        {
            get { return _File; }
            set
            {
                _File = value;
                CurVersion = _File.CurrentVersion;
                State = _File.CurrentState;

            }
        }

        public void isNeedsRebuld()
        {
            if (listRefChild.Count == 0) return;
            foreach (KeyValuePair<string, int> item in listRefChild)
            {

                int isVers = Tree.Part_IsChild(item.Key, item.Value);
                if (isVers != -1)
                {
                    IsRebuild = true;
                    listRefChildError.Add(item.Key, item.Value.ToString() + "/" + isVers.ToString());
                }

            }
            if (IsRebuild)
            {
                foreach (string item in listParent)
                {
                    Tree.SearchForOldLinks(item);
                }
            }

        }

        public void IsDrawing()
        {
            isDraw = PDM.IsDrawings(this);
        }
    }
}
