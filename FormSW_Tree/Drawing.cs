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
        string path;
        public int currentVers { get; set; }
        public bool NeedsRegeneration { get; set; }
        public bool CompareVersRef { get; set; }
        public string VersCompareToModel { get; set; }
        public int FileID { get; set; }
        public int FolderID { get; set; }
        public IEdmState5 State { get; set; }
        public string CubyNumber{get;set;}

        public Drawing(string _path, int _versModel)
        {
            path = _path;
            NeedsRegeneration = false;
            CompareVersRef = false;
        }
       
    }
}
