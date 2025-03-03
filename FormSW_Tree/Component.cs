using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    public class Component
    {
        public string CubyNumber { get; private set; }
        public string FullPath { get; private set; }
   
        public int CurVersion { get; set; }
        public IEdmState5 State { get; set; }
        public int bFolder { get; set; }
        public int Level { get; set; }
        public Dictionary<string, int> listRefChild;
        public Dictionary<string, string> listRefChildError;
        public List<string> listParent;
        public bool IsRebuild { get; set; }

        public Component(string cn, string fn)
        {
            CubyNumber = cn;
            FullPath = fn;
            listRefChild = new Dictionary<string, int>();
            listRefChildError = new Dictionary<string, string>();
            listParent = new List<string>();
            IsRebuild = false;
        }
    }
}
