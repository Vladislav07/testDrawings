using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace FormSW_Tree
{
    public class SW
    {
        string  TemplateName = "C:\\CUBY_PDM\\library\\templates\\Спецификация.sldbomtbt";
  

        private SldWorks swApp;
        private ModelDoc2 swMainModel;
        private AssemblyDoc swMainAssy;
        private Configuration swMainConfig;
        private Frame pFrame;
        public string PathRootDoc {get;set;}
  
        public event Action<MsgInfo, bool> connectSw;
        public event Action<int,MsgInfo> NotifySW;
  
        public void btnConnectSW()
        {
            string strAttach = swAttach();
            if (strAttach != null)
            {
                    DialogResult dr = MessageBox.Show(strAttach,
                    "Loading SW",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                return;
            }

            string[] strResult = LoadActiveModel();
          
            IsConnect(strResult);
        }

        private void IsConnect(string[] statLabel)
        {   bool isInit = false;
            MsgInfo info = new MsgInfo();
            info.errorMsg = statLabel[0];
            info.numberCuby= statLabel[2];
            if (statLabel[0] == "") {
               isInit = true;
               
               ResolvedLigthWeiht(swMainAssy);
            }
            connectSw?.Invoke(info, isInit);

        }

        string swAttach()
        {

            string strMessage;

            if (System.Diagnostics.Process.GetProcessesByName("sldworks").Length < 1)
            {
                strMessage = "Solidworks instance not detected.";
     
            }
            else if (System.Diagnostics.Process.GetProcessesByName("sldworks").Length > 1)
            {
                strMessage = "Multiple instances of Solidworks detected.";

            }
            else
            {
                strMessage = null;
                //swApp = System.Diagnostics.Process.GetProcessesByName("SldWorks.Application");
                swApp = (SldWorks)System.Runtime.InteropServices.Marshal.GetActiveObject("SldWorks.Application");
                //swApp = (SolidWorks.Interop.sldworks.Application)
            }

            return (strMessage);

        }
        string[] LoadActiveModel()
        {
            ModelDoc2 swDoc;
            swDocumentTypes_e swDocType;

            string strModelFile;
            string strModelName;
            string strConfigName = null;

            string[] strReturn = new string[4];
            strReturn[0] = "";
            int intErrors = 0;
            int intWarnings = 0;

            swDoc = (ModelDoc2)swApp.ActiveDoc;

            if (swDoc == null)
            {
                strReturn[0] = "Could not acquire an active document";
        
                return (strReturn);
            }

            strModelFile = swDoc.GetPathName();
            strModelName = strModelFile.Substring(strModelFile.LastIndexOf("\\") + 1, strModelFile.Length - strModelFile.LastIndexOf("\\") - 1);
            swDocType = (swDocumentTypes_e)swDoc.GetType();


            if (swDocType != swDocumentTypes_e.swDocASSEMBLY)
            {
                strReturn[0] = "This program only works with assemblies";            
                return (strReturn);
            }
            PathRootDoc = strModelFile;

            try
            {
                swMainModel = swApp.OpenDoc6(strModelFile, (int)swDocType, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, strConfigName, ref intErrors, ref intWarnings);
            }
            catch (Exception e)
            {
                strReturn[0] = e.Message;
                return (strReturn);
            }


            if (strConfigName != null)
            {
                swMainConfig = (Configuration)swMainModel.GetConfigurationByName(strConfigName);
            }
            else
            {
                swMainConfig = (Configuration)swMainModel.GetActiveConfiguration();
            }
            swMainAssy = (AssemblyDoc)swMainModel;
            Component2 swRootComp;
            swRootComp = (Component2)swMainConfig.GetRootComponent();
         
            // Write model info to return array
            strReturn[1] = strModelFile;
            strReturn[2] = strModelName;
            strReturn[3] = strConfigName;
            return (strReturn);

        }
        private void ResolvedLigthWeiht(AssemblyDoc ass)
        {
             int res = ass.ResolveAllLightWeightComponents(true);
             string msg;
            switch (res)
            {
                
                case 0:
                    msg = "Components resolved okay";
                    NotifyBeginOperation(0, msg);
                    break;
                case 1:
                    msg = "User aborted resolving the components";
                    NotifyError(msg);
                    break;
                case 2:
                    msg = "Some of the components did not get resolved despite the user requesting it";
                    NotifyError(msg);
                    break;
                case 3:
                    msg = "Not used";
                    NotifyError(msg);
                    break;
                default:
                    break;
            }
           
        }
        private void TraverseComponent(Component2 swComp)
        {
            object[] ChildComps;
            Component2 swChildComp;
            string childName;
            string e;
            ModelDoc2 swModelChild;

            AssemblyDoc swAssyChild;

            ChildComps = (object[])swComp.GetChildren();

            for (int i = 0; i < ChildComps.Length; i++)
            {
                swChildComp = (Component2)ChildComps[i];
                swChildComp.SetSuppression2((int)swComponentSuppressionState_e.swComponentFullyResolved);
                if (swChildComp.IsSuppressed()) continue;
                childName = swChildComp.GetPathName();
                e = Path.GetExtension(childName);

                if (e == ".SLDASM" || e == ".sldasm")
                {
                    swModelChild = (ModelDoc2)swChildComp.GetModelDoc2();
                    swAssyChild = (AssemblyDoc)swModelChild;
                    ResolvedLigthWeiht(swAssyChild);
                    TraverseComponent(swChildComp);
                }
            }


        }
        public void BuildTree()
        {
            GetRootComponent();
            GetBomTable();
        }
        private void GetRootComponent()
        {
       
            string rootPath = swMainModel.GetPathName();
            string nameRoot = Path.GetFileNameWithoutExtension(rootPath);        
            Tree.AddNode("0", nameRoot, rootPath);
        }
        private void GetBomTable()
        {
            ModelDocExtension Ext;
            BomFeature swBOMFeature;
            BomTableAnnotation swBOMAnnotation;
            string Configuration;
            TableAnnotation swTableAnn;
            ExtractomTable(out Ext, out swBOMFeature, out swBOMAnnotation, out Configuration, out swTableAnn);
            if (swBOMFeature == null) return;
            int nNumRow = 0;
            int J = 0;

            nNumRow = swTableAnn.RowCount;
            NotifyBeginOperation(nNumRow, "Reading TableBOM, total lines");
            for (J = 0; J <= nNumRow - 1; J++)
            {
                ExtractItem(swBOMAnnotation, Configuration, J);
            }
          

            string BomName = swBOMFeature.Name;
            bool boolstatus = TableBomClose(Ext, BomName);
            NotifyBeginOperation(0, "TableBOM");
        }
        private void ExtractItem(BomTableAnnotation swBOMAnnotation, string Configuration, int J)
        {
            string ItemNumber;
            string PartNumber;
            string PathName;
            string e;
            string designation;
            string[] result = new string[2];
            swBOMAnnotation.GetComponentsCount2(J, Configuration, out ItemNumber, out PartNumber);

            if (PartNumber == null) return;
            string PartNumberTrim = PartNumber.Trim();
            if (PartNumberTrim == "") return;

            string[] str = (string[])swBOMAnnotation.GetModelPathNames(J, out ItemNumber, out PartNumber);

            PathName = str[0];
            designation = Path.GetFileNameWithoutExtension(PathName);
            string regCuby = @"^CUBY-\d{8}$";
            bool IsCUBY = Regex.IsMatch(PartNumberTrim, regCuby);

            if (!IsCUBY)
            {
                PartNumberTrim = designation;
            }

            e = Path.GetExtension(PathName);
            string AddextendedNumber = "0." + ItemNumber;
            if (e == ".SLDPRT" || e == ".sldprt" || e == ".SLDASM" || e == ".sldasm")
            {

                Tree.AddNode(AddextendedNumber, PartNumberTrim, PathName);
                NotifyStepOperation(PathName);
            }
        }
        private bool TableBomClose(ModelDocExtension Ext, string BomName)
        {
            bool boolstatus;
            string numberTable = BomName.Substring(17);
            boolstatus = Ext.SelectByID2("DetailItem" + numberTable + "@Annotations", "ANNOTATIONTABLES", 0, 0, 0, false, 0, null, 0);
            swMainModel.EditDelete();
            swMainModel.ClearSelection2(true);
            return boolstatus;
        }
        private void ExtractomTable(out ModelDocExtension Ext, out BomFeature swBOMFeature,
            out BomTableAnnotation swBOMAnnotation, out string Configuration, out TableAnnotation swTableAnn)
        {
            Ext = default(ModelDocExtension);
            Ext = swMainModel.Extension;

            swBOMFeature = default(BomFeature);
            swBOMAnnotation = default(BomTableAnnotation);
            Configuration = swMainConfig.Name;
            int nbrType = (int)swNumberingType_e.swNumberingType_Detailed;
            int BomType = (int)swBomType_e.swBomType_Indented;
            try
            {
                swBOMAnnotation = Ext.InsertBomTable3(TemplateName, 0, 0, BomType, Configuration, true, nbrType, false);
                swBOMFeature = swBOMAnnotation.BomFeature;
                swTableAnn = (TableAnnotation)swBOMAnnotation;
                NotifyBeginOperation(0, "Extraction TableBOM");
            }
            catch (Exception e)
            {
                swTableAnn = null;
                NotifyError(e.Message,e.TargetSite.ToString(), e.Source.ToString());
            }
          
        }
        public void CloseDoc()
        {
            swApp.CloseAllDocuments(true);
        }
    

        public void loopFilesToRebuild(List<string>listFiles)
        {
             string nameOper = "Opening and rebuilding files";
             NotifyBeginOperation(listFiles.Count, nameOper);
             listFiles.ForEach(file => {
                 ModelDoc2 swModelDoc = OpenFile(file);
                 if (swModelDoc != null)
                 {
                      RefreshFile(swModelDoc);
                     swApp.CloseDoc(file);
                     swModelDoc = null;
                 }
              
             });
        }
        public ModelDoc2 OpenFile(string item)
        {
            ModelDoc2 swModelDoc = null;
            int errors = 0;
            int warnings = 0;
            string fileName = null;
            int type = -1;

            try
            {
                fileName = item;  
                NotifyStepOperation(fileName);
                string ext = Path.GetExtension(fileName);

                if (ext == ".sldprt" || ext == ".SLDPRT")
                {
                    type = (int)swDocumentTypes_e.swDocPART;
                }
                else if (ext == ".sldasm" || ext == ".SLDASM")
                {
                    type = (int)swDocumentTypes_e.swDocASSEMBLY;
                }
                else if (ext == ".slddrw" || ext == ".SLDDRW")
                {
                    type = (int)swDocumentTypes_e.swDocDRAWING;
                }

                swModelDoc = (ModelDoc2)swApp.OpenDoc6(fileName, type, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
                swModelDoc = (ModelDoc2)swApp.ActivateDoc2(fileName, true, ref errors);
                if (swModelDoc == null)
                {
                    NotifyError("Error", "Open file", fileName);
                }
            }
            catch (Exception error)
            {
                NotifyError(error.GetType().ToString(), error.Message, fileName);
            }
            return swModelDoc;
        }

        private void NotifyError(string msg, string typeOper="", string fileName="")
        {
            MsgInfo msgInfo = new MsgInfo();
            msgInfo.typeError=typeOper;
            msgInfo.errorMsg = msg;
            if (fileName != "")
            {
                msgInfo.numberCuby = Path.GetFileName(fileName);
            }        
            NotifySW?.Invoke(0, msgInfo);
        }

        private void RefreshFile(ModelDoc2 swModelDoc)
        {
            int lErrors = 0;
            int lWarnings = 0;
            ModelDocExtension extMod = swModelDoc.Extension;
            extMod.ForceRebuildAll();
            swModelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, ref lErrors, ref lWarnings);
          
        }

        private void NotifyBeginOperation(int count, string nameOper)
        {
            y = 0;
            MsgInfo msgInfo = new MsgInfo();
            msgInfo.typeOperation = nameOper;
            msgInfo.countStep = count;
            NotifySW?.Invoke(2, msgInfo);

        }
        int y;
        private void NotifyStepOperation(string file)
        {
            y++;
            MsgInfo msgInfo = new MsgInfo();
            string numberCuby=Path.GetFileName(file);
            msgInfo.numberCuby=numberCuby;
            msgInfo.currentStep = y;
            NotifySW?.Invoke(3, msgInfo);

        }
      

        public void InvisibleApp( bool isVisible)
        {
            if (!isVisible) return;
            try
            {
                swApp.UserControl = false;
                swApp.Visible = false;
                pFrame = (Frame)swApp.Frame();
                pFrame.KeepInvisible = true;
            }
            catch (Exception)
            {

                MessageBox.Show("Invisible");
            }        
         
        }

        public void UnInvisibleApp(bool isVisible)
        {
            if (!isVisible) return;
            pFrame.KeepInvisible = false;
            swApp.Visible = true;
        }
    }
}

