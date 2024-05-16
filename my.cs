using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPDMLibLib;

namespace TestDrawings
{
   public class my:IEPDM
    {
        public void IsVaulted(string bsPathname, out bool pbCheckedOut)
        {
            throw new NotImplementedException();
        }

        public void CheckOut(object oFileNames, int LParentWnd, object poVaultPtr)
        {
            throw new NotImplementedException();
        }

        public void ExploreInWindows(string bsPathname)
        {
            throw new NotImplementedException();
        }

        public void CheckIn(object oFileNames, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void UndoCheckOut(object oFileNames, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void GetLatestVersion(object oFileNames, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public Array GetAllVersionData(string bsPathname)
        {

            throw new NotImplementedException();
        }

        public void GetSpecificVersion(string bsPathname, int lVersion, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void StartSearch(string bsPathname, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void ShowCard(string bsPathname, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public Array GetAllTransitions(object oFileNames, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void ChangeState(object oFileNames, string bsTransitionState, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void SilentMode(bool oFlag)
        {
            throw new NotImplementedException();
        }

        public int GetRefFileVersion(string bsPathname, string bsRefFile, int lVersion)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string bsSourceFile, string bsDestination)
        {
            throw new NotImplementedException();
        }

        public void RenameFile(string bsSourceFile, string bsNewname)
        {
            throw new NotImplementedException();
        }

        public void CopyFile(string bsSourceFile, string bsDestination)
        {
            throw new NotImplementedException();
        }

        public void ChangeState2(object oFileNames, int lTransitionState, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public Array SearchFileInVault(string bsSearchArgs, string bsModelPath, string bsExtension)
        {
            throw new NotImplementedException();
        }

        public void GetFileInfo(string bsSourceFile, ref string bsVersionInfo, ref string bsRevisionInfo, ref string bsCurrentState, ref string bsUsername, ref string bsCheckoutpath)
        {
            throw new NotImplementedException();
        }

        public void IsFolderVaulted(string bsSourceFile, out bool pbFolderVaulted)
        {
            throw new NotImplementedException();
        }

        public void HasCheckOutRight(string bsSourceFile, out bool pbCheckedOutRight)
        {
            throw new NotImplementedException();
        }

        public void AddFile(string bsPathname)
        {
            throw new NotImplementedException();
        }

        public void GetLicenseType(string bsSourceFile, out int plLicenseType)
        {
            throw new NotImplementedException();
        }

        public void IsFileCheckedOutByCurrentUser(string bsSourceFile, out bool pbCheckedOutByCurrentUser)
        {
            throw new NotImplementedException();
        }

        public void IsAlwaysGetLatest(string bsSourceFile, out bool pbAlwaysGetLatest)
        {
            throw new NotImplementedException();
        }

        public void GetFileInfo2(string bsSourceFile, ref string bsVersionInfo, ref string bsRevisionInfo, ref string bsCurrentState, ref string bsUsername, ref string bsCheckoutpath, ref bool bpIsLatest)
        {
            throw new NotImplementedException();
        }

        public void SetAddIn(object poIntegration)
        {
            throw new NotImplementedException();
        }

        public Array GetAllTransitions2(object oFileNames, int LParentWnd)
        {
            throw new NotImplementedException();
        }

        public void ChangeState3(object oFileNames, int lTransitionState, int LParentWnd, bool bIsRevoke)
        {
            throw new NotImplementedException();
        }

        public void ChangeState4(object oFileNames, string bsTransitionState, int LParentWnd, bool bIsRevoke)
        {
            throw new NotImplementedException();
        }

        public dynamic GetVaultPtr(string bsSourceFile)
        {
            throw new NotImplementedException();
        }

        public void IsFileCheckedOutByCurrentUser2(string bsSourceFile, string bsLockedUser, out bool pbCheckedOutByCurrentUser)
        {
            throw new NotImplementedException();
        }

        public void GetFileInfo3(string bsSourceFile, ref string bsVersionInfo, ref string bsRevisionInfo, ref string bsCurrentState, ref string bsUsername, ref string bsCheckoutpath, ref string bsCheckoutComputer, ref bool bpIsLatest)
        {
            throw new NotImplementedException();
        }

        public void IsShowFullUsernameInExplorer(string bsSourceFile, out bool pbShowFullUsername)
        {
            throw new NotImplementedException();
        }

        public void GetFileInfo4(string bsSourceFile, ref string bsVersionInfo, ref string bsRevisionInfo, ref string bsCurrentState, ref string bsUsername, ref string bsCheckoutpath, ref string bsCheckoutComputer, ref bool bpIsLatest, ref int lFileID, ref int lFolder, int bUpdateFileInfo)
        {
            throw new NotImplementedException();
        }

        public void GetFileNameFromID(string bsSourceFile, int lFileID, int lFolderID, ref string bsFullPath)
        {
            throw new NotImplementedException();
        }

        public void IsVaultCreated(ref bool pbVaultCreated)
        {
            throw new NotImplementedException();
        }

        public void DisplayExistingDataCard()
        {
            throw new NotImplementedException();
        }

        public void CheckConnection(string bsPath, out object oVaultPtr)
        {
            throw new NotImplementedException();
        }

        public void ShowError(int LParentWnd, int hError)
        {
            throw new NotImplementedException();
        }
    }
}
