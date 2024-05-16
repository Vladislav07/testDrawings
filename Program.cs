using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EPDM.Interop.epdm;
using EPDMLibLib;



namespace TestDrawings
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"A:\My\10-тонный электрический подъемный кран\ramka-1.SLDDRW";
            string pathA = @"A:\My\10-тонный электрический подъемный кран\ramka-1.SLDASM";

            IEdmFile5 file = null;
            IEdmFile8 f8 = null;
            IEdmFolder5 folder = null;
            EdmViewInfo[] Views = null;
            try
            {
                IEdmVault5 vault= new EdmVault5();
                
                if (!vault.IsLoggedIn)
                {
                    //Log into selected vault as the current user
                    vault.LoginAuto("My", 0);
                }
                 IEdmVault8 vault8 = (IEdmVault8)vault;

                IEdmClearLocalCache3 ClearLocalCache = default(IEdmClearLocalCache3);
                ClearLocalCache = (IEdmClearLocalCache3)vault8.CreateUtility(EdmUtility.EdmUtil_ClearLocalCache);
                ClearLocalCache.IgnoreToolboxFiles = true;
                ClearLocalCache.UseAutoClearCacheOption = true;
               
                vault8.GetVaultViews(out Views, false);
                foreach (EdmViewInfo View in Views)
                {
                    Console.Write(View.mbsVaultName + "\r\n");
                 
                }

                file = vault.GetFileFromPath(path, out folder);
                Console.Write(file.Name + "\r\n");
               
                f8 = (IEdmFile8)file;
                int version = file.CurrentVersion;
                Console.Write(version.ToString() + "\r\n");
              
                bool isRebuild = f8.NeedsRegeneration(version);
                Console.Write(isRebuild.ToString() + "\r\n");

                IEPDM e = new EPDMClass();
                Array array = e.GetAllVersionData(path);
                foreach (EPDMVersionInfo item in array)
                {
                    Console.Write(item.mlVersionNo.ToString() + "\r\n");
                }

                int testVersion = e.GetRefFileVersion(path, pathA, 4);
                Console.Write(testVersion.ToString() + "\r\n");

                IEdmReference5 ref5 = file.GetReferenceTree(folder.ID);
                IEdmPos5 pos = default(IEdmPos5);
                IEdmReference10 ref10 = (IEdmReference10)ref5;
                pos = ref10.GetFirstChildPosition3("A", true, true, (int)EdmRefFlags.EdmRef_File, "", 0);
                IEdmReference10 @ref = default(IEdmReference10);
                string pathAsm = "";
                while ((!pos.IsNull))
                {
                    @ref = (IEdmReference10)ref5.GetNextChild(pos);
                    Console.Write(@ref.FoundPath + "\r\n");
                    Console.Write("VersionRef - " + @ref.VersionRef + "\r\n");
                    Console.Write("ReferencedAs - " + @ref.ReferencedAs + "\r\n");
                    pathAsm = @ref.ReferencedAs;
                }
                IEdmFolder5 folderAsm = null;
                IEdmFile5 fAsm = vault.GetFileFromPath(pathAsm, out folderAsm);
                Console.Write(fAsm.CurrentVersion + "\r\n");

                IEdmBatchListing4 BatchListing = default(IEdmBatchListing4);
                BatchListing = (IEdmBatchListing4)vault8.CreateUtility(EdmUtility.EdmUtil_BatchList);
                // ((IEdmBatchListing4)BatchListing).AddFileCfg(KvPair.Key, DateTime.Now, (Convert.ToInt32(KvPair.Value)), "@", Convert.ToInt32(EdmListFileFlags.EdmList_Nothing));
                BatchListing.AddFile(path, DateTime.Now, 1);
                EdmListCol[] BatchListCols = null;
                BatchListing.CreateList("\n\nDescription\nNumber", ref BatchListCols);
                EdmListFile2[] BatchListFiles = null;
                BatchListing.GetFiles2(ref BatchListFiles);
                EdmListFile2 curListFile = BatchListFiles[0];
                Console.Write(curListFile.mlFileID + "\r\n");
                Console.Write(curListFile.mlLatestVersion + "\r\n");

            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                MessageBox.Show("HRESULT = 0x" + ex.ErrorCode.ToString("X") + " " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            Console.Read();
        }


    }
}
