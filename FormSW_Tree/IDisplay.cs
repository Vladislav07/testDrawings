using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
  public  interface IDisplay
    {
        void RefreshPdmFile();
        string[] Print();
    }

    public interface IRebuild
    {
        List<PdmID> GetIDFromPDM();
        string GetPath();
    }
}
