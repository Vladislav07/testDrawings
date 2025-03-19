using System.IO;
using System.Xml.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using EPDM.Interop.epdm;
using System.Text.RegularExpressions;

namespace FormSW_Tree
{
   
   public static class PDM
    {
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

        public static void GetEdmFile(this Component item)
        {
           
            IEdmFile5 File = null;
            IEdmFolder5 ParentFolder = null;
            try
            {
                File = vault1.GetFileFromPath(item.FullPath, out ParentFolder);

                item.File = File;
                item.bFolder = ParentFolder.ID;
                enumVar = File.GetEnumeratorVariable();
                object val = null;
                EdmStrLst5 listConf = File.GetConfigurations(0);

                bool error = enumVar.GetVar("Раздел", ".", out val);
                if (val != null)
                {
                    item.Section = (string)val;
                }
            }
            catch (Exception)
            {

                
            }
         
            
        }

        public static void GetReferenceFromAssemble(this Component ass)
        {
            string e = Path.GetExtension(ass.FullPath);
            if (e == ".SLDPRT" || e == ".sldprt") return;
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
                    //
                    string extension = Path.GetExtension(@ref.Name);
                    if (extension == ".sldasm" || extension == ".sldprt" || extension == ".SLDASM" || extension == ".SLDPRT")
                    {
                        cubyNumber = Path.GetFileNameWithoutExtension(@ref.Name);
                      //  string regCuby = @"^CUBY-\d{8}$";
                     //   bool IsCUBY = Regex.IsMatch(cubyNumber.Trim(), regCuby);
                      //  if (!IsCUBY) continue;
                        verChildRef = @ref.VersionRef;
                        ass.listRefChild.Add(cubyNumber.Trim(), verChildRef);

                    }
       
                    //ref5.GetNextChild(pos);
                }
            }

            catch (Exception p)
            {

                MessageBox.Show("uuu" + p.Message);
            }
           
            
        }

        public static void AddSelItemToList(List<Component>updateList)
        {
            int i = 0;
            try
            {
                ppoSelection = new EdmSelItem[updateList.Count];
                foreach (Component item in updateList)
                {
                    ppoSelection[i] = new EdmSelItem();
                    ppoSelection[i].mlDocID = item.File.ID;
                    ppoSelection[i].mlProjID = item.bFolder;
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

        public static  void BatchGet(List<Component> updateList)
        {
            int i = 0;

            try
            {

                batchGetter = (IEdmBatchGet)vault.CreateUtility(EdmUtility.EdmUtil_BatchGet);
                /*
                foreach (Component item in updateList)
                {
         
                    batchGetter.AddSelectionEx((EdmVault5)vault1, item.File.ID, item.bFolder, item.CurVersion);
                    i++;
                }
                */
                batchGetter.AddSelection((EdmVault5)vault1, ppoSelection);

                if ((batchGetter != null))
                {
                    batchGetter.CreateTree(0, (int)EdmGetCmdFlags.Egcf_Lock + (int)EdmGetCmdFlags.Egcf_SkipOpenFileChecks);// + (int)EdmGetCmdFlags.Egcf_IncludeAutoCacheFiles);  
                    batchGetter.ShowDlg(0);
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
                bool retVal = batchUnlocker.ShowDlg(0);
                if ((retVal))
                {
                    batchUnlocker.UnlockFiles(0, null);
                    EventProcess("Processing completed");
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
                    vault1.LoginAuto("CUBY_PDM", 0);
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

        public static bool IsDrawings(this Component comp)
        {
            int refDrToModel = -1;
            bool NeedsRegeneration = false;
            IEdmFile7 bFile = null;
            string p = Path.Combine(Path.GetDirectoryName(comp.FullPath), comp.CubyNumber + ".SLDDRW");
            bFile = (IEdmFile7)vault.GetFileFromPath(p, out IEdmFolder5 bFolder);

            if (bFile != null)                                         
            {            
                try
                {                 
                    NeedsRegeneration = bFile.NeedsRegeneration(bFile.CurrentVersion, bFolder.ID);

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

                   // if (!(refDrToModel == comp.CurVersion) || NeedsRegeneration)
                   // {
                        Drawing draw = new Drawing(p, comp.CurVersion);
                        draw.FileID = bFile.ID;
                        draw.FolderID = bFolder.ID;
                        draw.CubyNumber = comp.CubyNumber;
                        draw.NeedsRegeneration = NeedsRegeneration;
                        draw.currentVers = bFile.CurrentVersion;
                        draw.State = bFile.CurrentState;
                        draw.CompareVersRef = true;
                        draw.VersCompareToModel = comp.CurVersion.ToString() + "/" + refDrToModel.ToString();
                        Tree.listDraw.Add(draw);
                        return true;
                  //  }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + bFile.Name + ex.Message);
                }
            }
            return false;
        }
    }
}
