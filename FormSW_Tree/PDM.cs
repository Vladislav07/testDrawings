using EPDM.Interop.epdm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FormSW_Tree
{

    public static class PDM
    {
       // static string NAME_PDM = "CUBY_PDM";
        static string NAME_PDM = "My";

        static IEdmVault5 vault1 = null;
        static IEdmVault7 vault = null;
        static IEdmBatchGet batchGetter;
        static EdmSelItem[] ppoSelection = null;
        static IEdmBatchUnlock2 batchUnlocker;
        static IEdmEnumeratorVariable5 enumVar = default(IEdmEnumeratorVariable5);
        public static event Action<string> UnLock;

        static PDM()
        {
            vault1 = new EdmVault5();
            ConnectingPDM();
        }

        public static void GetEdmFile(this Model item)
        {
           
            IEdmFile7 File = null;
            IEdmFolder5 ParentFolder = null;

            try
            {
                File = (IEdmFile7)vault1.GetFileFromPath(item.FullPath, out ParentFolder); 
                item.File = File;
                item.bFolder = ParentFolder.ID;
                item.bFile = File.ID;
    
                enumVar = File.GetEnumeratorVariable();
                object val = null;
                EdmStrLst5 listConf = File.GetConfigurations(0);

                bool error = enumVar.GetVar("Раздел", ".", out val);
                if (val != null)
                {
                    item.Section = (string)val;
                }
                else
                {
                     error = enumVar.GetVar("Раздел", "@", out val);
                     item.Section = val != null? (string)val: "none";
                }
            }
            catch (Exception)
            {
                
            }      
            
        }

        internal static void GetReferenceFromAssemble(this Assemble ass)
        {
            ass.listRefChild.Clear();
            IEdmReference5 ref5 = ass.File.GetReferenceTree(ass.bFolder);
            IEdmReference10 ref10 = (IEdmReference10)ref5;
            IEdmPos5 pos = ref10.GetFirstChildPosition3("A", true, true, (int)EdmRefFlags.EdmRef_File, "", 0);
            string cubyNumber = null;
            int verChildRef = -1;
            try
            {
                while (!pos.IsNull)
                {
                    IEdmReference10 @ref = (IEdmReference10)ref5.GetNextChild(pos);
                    string extension = Path.GetExtension(@ref.Name);
                    if (extension == ".sldasm" || extension == ".sldprt" || extension == ".SLDASM" || extension == ".SLDPRT")
                    {
                        cubyNumber = Path.GetFileNameWithoutExtension(@ref.Name);
                       
                        string regCuby = @"^CUBY-\d{8}$";
                        bool IsCUBY = Regex.IsMatch(cubyNumber.Trim(), regCuby);
                        // if (!IsCUBY) continue;

                        verChildRef = @ref.VersionRef;
                        if (ass.listRefChild.ContainsKey(cubyNumber.Trim()))continue;
                        ass.listRefChild.Add(cubyNumber.Trim(), verChildRef);
                    }
                }
            }

            catch (Exception p)
            {

               // MessageBox.Show("uuu" + p.Message);
            }
           
            
        }

    

        public static void AddSelItemToList(List<PdmID>updateList)
        {
            int i = 0;
            try
            {
                ppoSelection = new EdmSelItem[updateList.Count];

                foreach (PdmID item in updateList)
                {
                    ppoSelection[i] = new EdmSelItem();
                    ppoSelection[i].mlDocID = item.FileId;
                    ppoSelection[i].mlProjID = item.FolderId;
           
                    i++;
                }

            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static  void BatchGet(List<PdmID> list)
        {       
            try
            {

                batchGetter = (IEdmBatchGet)vault.CreateUtility(EdmUtility.EdmUtil_BatchGet);
     
                batchGetter.AddSelection((EdmVault5)vault1, ppoSelection);
                if ((batchGetter != null))
                {
                    batchGetter.CreateTree(0, (int)EdmGetCmdFlags.Egcf_Lock + (int)EdmGetCmdFlags.Egcf_SkipOpenFileChecks);// + (int)EdmGetCmdFlags.Egcf_IncludeAutoCacheFiles);  
                   // batchGetter.ShowDlg(0);
                    batchGetter.GetFiles(0, null);
                }

            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DocBatchUnLock()
        {
            try
            {
                batchUnlocker = (IEdmBatchUnlock2)vault.CreateUtility(EdmUtility.EdmUtil_BatchUnlock);
                batchUnlocker.AddSelection((EdmVault5)vault1, ref ppoSelection);
                batchUnlocker.CreateTree(0, (int)EdmUnlockBuildTreeFlags.Eubtf_MayUnlock);
                batchUnlocker.Comment = "Refresh";
                batchUnlocker.UnlockFiles(0, null);
                EventProcess("Processing completed");
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        static void EventProcess(string message)
        {
            if(UnLock != null)
            {
                UnLock.Invoke(message);
            }
        }

        static void  ConnectingPDM()
         {
            try
            {
                if (!vault1.IsLoggedIn)
                {
                    vault1.LoginAuto(NAME_PDM, 0);    
                }
                vault = (IEdmVault7)vault1;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "connecting");
            }

        }

        public static bool IsDrawings(this Model comp)
        {

            IEdmFile7 bFile = null;
            try
            {
                string p = Path.Combine(Path.GetDirectoryName(comp.FullPath), comp.CubyNumber + ".SLDDRW");
                bFile = (IEdmFile7)vault.GetFileFromPath(p, out IEdmFolder5 bFolder);

                if (bFile != null)                                         
                {                
                    Drawing draw = new Drawing(comp.CubyNumber, p, comp, bFile, bFolder.ID);
                    Tree.listDraw.Add(draw);
                    return true;
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "connecting to CUBY_PDM");
            }
           return false; 
        }

        public static int GetRefVersion(this Drawing draw)
        {
            int refDrToModel = -1;
            IEdmReference5 ref5 = draw.File.GetReferenceTree(draw.bFolder);
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
            return refDrToModel;
        }
    }
}
