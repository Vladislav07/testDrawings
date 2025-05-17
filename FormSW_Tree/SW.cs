using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
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

        private string[] operSW;
  
        public event Action<string[], bool> connectSw;
        public event Action<bool> readedTree;
        public event Action<string[]> operationSW;
        public event Action<string[]> loadTree;
        public event Action<int,MsgInfo> rebuild;
        public SW()
        {
            operSW = new string[2];
        }
     
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

            // Get active model
            string[] strResult = LoadActiveModel();
          
            IsConnect(strResult);
        }

        private void IsConnect(string[] statLabel)
        {   bool isInit = false;
            if (statLabel[0] == "") {
               isInit = true;
            }
            connectSw?.Invoke(statLabel, isInit);

        }

        string swAttach()
        {

            string strMessage;
            //bool blnStatus = true;

            //Check for the status of existing solidworks apps
            if (System.Diagnostics.Process.GetProcessesByName("sldworks").Length < 1)
            {
                strMessage = "Solidworks instance not detected.";
                //blnStatus = false;
            }
            else if (System.Diagnostics.Process.GetProcessesByName("sldworks").Length > 1)
            {
                strMessage = "Multiple instances of Solidworks detected.";
                //blnStatus = false;
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

            // returns string array
            //   element 0 = error message
            //   element 1 = model name with path
            //   element 2 = model name
            //   element 3 = referenced configuration name
          

            ModelDoc2 swDoc;
            swDocumentTypes_e swDocType;

            string strModelFile;
            string strModelName;
            string strConfigName = null;

            string[] strReturn = new string[4];
            strReturn[0] = "";
            int intErrors = 0;
            int intWarnings = 0;

            // Get the active document
            swDoc = (ModelDoc2)swApp.ActiveDoc;

            if (swDoc == null)
            {
                strReturn[0] = "Could not acquire an active document";
        
                return (strReturn);
            }

            //Check for the correct doc type
            strModelFile = swDoc.GetPathName();
            strModelName = strModelFile.Substring(strModelFile.LastIndexOf("\\") + 1, strModelFile.Length - strModelFile.LastIndexOf("\\") - 1);
            swDocType = (swDocumentTypes_e)swDoc.GetType();


            if (swDocType != swDocumentTypes_e.swDocASSEMBLY)
            {
                strReturn[0] = "This program only works with assemblies";            
                return (strReturn);
            }

            // Try to load the model file
            try
            {
                swMainModel = swApp.OpenDoc6(strModelFile, (int)swDocType, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, strConfigName, ref intErrors, ref intWarnings);
            }
            catch (Exception e)
            {
                strReturn[0] = e.Message;
                return (strReturn);
            }

            // Write model info to shared variables
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
            ResolvedLigthWeiht(swMainAssy);

            // Write model info to return array
            strReturn[1] = strModelFile;
            strReturn[2] = strModelName;
            strReturn[3] = strConfigName;
            return (strReturn);

        }
        private void ResolvedLigthWeiht(AssemblyDoc ass)
        {
            int countLigthWeiht = ass.GetLightWeightComponentCount();
            if (countLigthWeiht > 0)
            {
                ass.ResolveAllLightweight();
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

            ModelDocExtension Ext = default(ModelDocExtension);
            Ext=swMainModel.Extension;
           
            BomFeature swBOMFeature = default(BomFeature);
            BomTableAnnotation swBOMAnnotation = default(BomTableAnnotation);
            string Configuration = swMainConfig.Name;

            int nbrType = (int)swNumberingType_e.swNumberingType_Detailed;
            int BomType = (int)swBomType_e.swBomType_Indented;

            swBOMAnnotation = Ext.InsertBomTable3(TemplateName, 0, 0, BomType, Configuration, true, nbrType, false);
            swBOMFeature = swBOMAnnotation.BomFeature;

            TableAnnotation swTableAnn = (TableAnnotation)swBOMAnnotation;
            int nNumRow = 0;
            int J = 0;
            string ItemNumber = null;
            string PartNumber = null;
            string PathName;
            string e;
            string designation;
            string BomName;
            bool boolstatus = false;
            string[] result = new string[2];

            BomName = swBOMFeature.Name;
            nNumRow = swTableAnn.RowCount;

            operSW[0] = "Read Specification";
            operSW[1]= nNumRow.ToString();
            operationSW?.Invoke(operSW);
            for (J = 0; J <= nNumRow - 1; J++)
            {
                swBOMAnnotation.GetComponentsCount2(J, Configuration, out ItemNumber, out PartNumber);

                if (PartNumber == null) continue;
                string PartNumberTrim = PartNumber.Trim();
                if (PartNumberTrim == "") continue;
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
                    result[0]= PartNumberTrim;
                    result[1]= J.ToString();
                    loadTree?.Invoke(result);
                }

            }
            readedTree?.Invoke(true);
            int i = BomName.Length;
            string numberTable = BomName.Substring(17);
            boolstatus = Ext.SelectByID2("DetailItem" + numberTable + "@Annotations", "ANNOTATIONTABLES", 0, 0, 0, false, 0, null, 0);
            swMainModel.EditDelete();
            swMainModel.ClearSelection2(true);
            
        }
        public void CloseDoc()
        {
            swApp.CloseAllDocuments(true);
        }
    

        public void loopFilesToRebuild(List<string>listFiles)
        {
            NotifyBeginRebuild(listFiles.Count);
            listFiles.ForEach(file => OpenAndRefresh(file));
        }
        public void OpenAndRefresh(string item)
        {
            ModelDoc2 swModelDoc = default(ModelDoc2);
            int errors = 0;
            int warnings = 0;
            int lErrors = 0;
            int lWarnings = 0;
            ModelDocExtension extMod;
            string fileName = null;
            DrawingDoc swDraw = default(DrawingDoc);
            object[] vSheetName = null;
            string sheetName;
            int i = 0;
            bool bRet = false;
            int type = -1;

            try
            {

                fileName = item;
                NotifyStepRebuild(fileName);
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
                if (swModelDoc == null)
                {
                    MsgInfo msgInfo = new MsgInfo();
                    //msgInfo.errorMsg=errors.n
                    msgInfo.numberCuby = fileName;
                    return;
                }
                extMod = swModelDoc.Extension;

         /*       if (ext == ".slddrw" || ext == ".SLDDRW")
                {
                    swDraw = (DrawingDoc)swModelDoc;
                    vSheetName = (object[])swDraw.GetSheetNames();
                    for (i = 0; i < vSheetName.Length; i++)

                    {
                        sheetName = (string)vSheetName[i];
                        bRet = swDraw.ActivateSheet(sheetName);
                        extMod.Rebuild((int)swRebuildOptions_e.swCurrentSheetDisp);
                        swModelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, ref lErrors, ref lWarnings);
                        Sheet swSheet = default(Sheet);
                        swSheet = (Sheet)swDraw.GetCurrentSheet();
                    }
                }*/
                extMod.Rebuild((int)swRebuildOptions_e.swRebuildAll);
                swModelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, ref lErrors, ref lWarnings);
                swApp.CloseDoc(fileName);
                swModelDoc = null;

            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());

            }
        }

        private void NotifyBeginRebuild( int count)
        {
            y = 0;
            MsgInfo msgInfo = new MsgInfo();
            msgInfo.typeOperation = "Opening and rebuilding files";
            msgInfo.countStep = count;
            rebuild?.Invoke(2, msgInfo);

        }
        int y;
        private void NotifyStepRebuild(string file)
        {
            y++;
            MsgInfo msgInfo = new MsgInfo();
            string numberCuby=Path.GetFileName(file);
            msgInfo.numberCuby=numberCuby;
            msgInfo.countStep = y;
            rebuild?.Invoke(3, msgInfo);

        }

    }
}

