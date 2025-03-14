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
        int versModel;
        int versRefModel;
        public int currentVers{get;set;}
        public bool isDraw { get; set;}
        public bool NeedsRegeneration { get; set; }
        public bool IsCompareVers { get; set; }
        public string VersCompareToModel { get; set; }

        public Drawing(string _path, int _versModel)
        {

        }
        /*
        void IsDrawingsRebuild(string p, string d)
        {
            int refDrToModel = -1;
            bool NeedsRegeneration = false;

            IEdmFile7 modelFile = null;
            IEdmFile7 bFile = null;

            Drawing draw = null;
            modelFile = (IEdmFile7)v.GetFileFromPath(p, out IEdmFolder5 modelFolder);

            if ((modelFile != null) && (!modelFile.IsLocked))
            {

                refDrToModel = modelFile.CurrentVersion;
            }


            bFile = (IEdmFile7)v.GetFileFromPath(d, out IEdmFolder5 bFolder);

            if ((bFile != null) && (!bFile.IsLocked)) //true если файл не пусто и зачекинен                                           
            {

                try
                {
                    int versionDraiwing = bFile.CurrentVersion;
                    NeedsRegeneration = bFile.NeedsRegeneration(versionDraiwing, bFolder.ID);

                    // Достаем из чертежа версию ссылки на родителя (VersionRef)
                    IEdmReference5 ref5 = bFile.GetReferenceTree(bFolder.ID);
                    IEdmReference10 ref10 = (IEdmReference10)ref5;
                    IEdmPos5 pos = ref10.GetFirstChildPosition3("A", true, true, (int)EdmRefFlags.EdmRef_File, "", 0);
                    while (!pos.IsNull)
                    {

                        IEdmReference10 @ref = (IEdmReference10)ref5.GetNextChild(pos);

                        string extension = Path.GetExtension(@ref.Name);
                        if (extension == ".sldasm" || extension == ".sldprt" || extension == ".SLDASM" || extension == ".SLDPRT")
                        {
                            refDrToModel = @ref.VersionRef;
                            break;
                        }
                        else
                        {
                            ref5.GetNextChild(pos);
                        }
                    }

                    if (!(refDrToModel == modelFile.CurrentVersion) || NeedsRegeneration)
                    {
                        draw = new Drawing(bFile.ID, bFolder.ID, d);
                        Root.drawings.Add(draw);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + bFile.Name + ex.Message);

                }
            }
        }
        */
    }
}
