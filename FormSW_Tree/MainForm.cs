using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace FormSW_Tree
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private String statLabel;
        private Thread thdScan;
        private SldWorks swApp;
        private ModelDoc2 swMainModel;
        private AssemblyDoc swMainAssy;
        private Configuration swMainConfig;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConnectSW_Click(object sender, EventArgs e)
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
            txtActiveDoc.Text = strResult[2];
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // check for lightweight components
            if (swMainAssy.GetLightWeightComponentCount() > 0)
            {
                DialogResult dr = MessageBox.Show("All lightweight components will be fully resolved.  Continue?",
                                                  "Lightweight Components",
                                                  MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    this.txtStatus.Text = "Resolving lightweight components...";
                    swMainAssy.ResolveAllLightWeightComponents(false);
                    this.txtStatus.Text = "Proceding with scan";
                }
                else
                {
                    this.txtStatus.Text = "Some components still lightweight";
                    return;
                }
            }

            // scan once
            //DoScan();

            // create and start the scanning thread
            this.thdScan = new Thread(new ThreadStart(ScanControl));
            thdScan.Start();

            // change button config
            this.btnRefresh.Visible = false;
            //this.cmdCancel.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // stop scanning thread
            thdScan.Interrupt();

            // change button config
            this.btnRefresh.Visible = true;
            //this.cmdCancel.Enabled = false;
        }
        void ScanControl()
        {

            MethodInvoker WriteLabelDelegate = new MethodInvoker(WriteLabel);

            try
            {
                DoScan();
            }
            catch (ThreadInterruptedException e)
            {
                this.statLabel = "Interupted \n " + e.ToString();
                Invoke(WriteLabelDelegate);
                return;
            }
            this.statLabel = "Scanning Completed Successfully";
            Invoke(WriteLabelDelegate);

        }


        ThreadInterruptedException DoScan()
        {

            try
            {
                Thread.Sleep(0);
            }
            catch (ThreadInterruptedException e)
            {
                return (e);
            }


            // Set the initial BOM level
            int intLevel = 0;

            // Find the root node.  This is only done ONCE for the entire assembly structure
            Component2 swRootComp = (Component2)swMainConfig.GetRootComponent();

            // Get name of configuration containing properties
            string strMainConfig = swMainConfig.Name;
            string strMainPath = swMainModel.GetPathName();

            // write custom properties for main model
            //DbWriteConfigs(swMainModel, strMainPath, true);
            DbWriteConfig(swMainModel, strMainPath, strMainConfig);

            // Recursively traverse the assembly
            if (swRootComp != null)
            {
                TraverseAssy(swRootComp, strMainPath, strMainConfig, intLevel);
            }

            return (null);

        }

        void WriteLabel()
        {

            this.txtStatus.Text = this.statLabel.ToString();

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
            DrawingDoc swDrawDoc;
            SolidWorks.Interop.sldworks.View swDrawView;
            swDocumentTypes_e swDocType;

            string strModelFile;
            string strModelName;
            //string strFileExt;
            string strConfigName = null;

            string[] strReturn = new string[4];

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

            // get model document from drawing document
            if (swDocType == swDocumentTypes_e.swDocDRAWING)
            {

                swDrawDoc = (DrawingDoc)swDoc;
                swDrawView = (SolidWorks.Interop.sldworks.View)swDrawDoc.GetFirstView();
                swDrawView = (SolidWorks.Interop.sldworks.View)swDrawView.GetNextView();

                strModelFile = swDrawView.GetReferencedModelName();
                strConfigName = swDrawView.ReferencedConfiguration;
                strModelName = strModelFile.Substring(strModelFile.LastIndexOf("\\") + 1, strModelFile.Length - strModelFile.LastIndexOf("\\") - 1);
                swDocType = (swDocumentTypes_e)GetTypeFromString(strModelFile);

                if (swDocType != swDocumentTypes_e.swDocASSEMBLY & swDocType != swDocumentTypes_e.swDocPART)
                {
                    strReturn[0] = "error getting file type from drawing view's referenced model";
                    return (strReturn);
                }

            }

            // if the document is not an assembly then send
            // an error message to the user.
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

            // check for lightweight components
            if (swMainAssy.GetLightWeightComponentCount() > 0)
            {
                DialogResult dr = MessageBox.Show("All lightweight components will be fully resolved.  Continue?",
                                                  "Lightweight Components",
                                                  MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    this.txtStatus.Text = "Resolving lightweight components...";
                    swMainAssy.ResolveAllLightWeightComponents(false);
                    this.txtStatus.Text = "Proceding with scan";
                }
                else
                {
                    this.txtStatus.Text = "Some components still lightweight";
                    return (strReturn);
                }
            }

            // Write model info to return array
            strReturn[1] = strModelFile;
            strReturn[2] = strModelName;
            strReturn[3] = strConfigName;
            return (strReturn);

        }
        ThreadInterruptedException TraverseAssy(Component2 swParentComp, string strParentPath, string strParentConfig, int intStartLevel)
        {

            // Check for cancel button
            try
            {
                Thread.Sleep(0);
            }
            catch (ThreadInterruptedException e)
            {
                return (e);
            }

            // If no component, then exit
            if (swParentComp == null)
            {
                return (null);
            }

            // Prepare to write status label
            MethodInvoker WriteLabelDelegate = new MethodInvoker(WriteLabel);

            // increment BOM level
            int intNextLevel = intStartLevel + 1;

            // Get the list of children (if any)
            object oChildren = swParentComp.GetChildren();
            System.Array aChildren = (Array)oChildren;
            //Component2[] swChildren = (Component2[])aChildren;
            //Component2[] swChildren = (Component2[])swParentComp.GetChildren();

            // die if array contains no children
            if (aChildren == null)
            {
                return (null);
            }

            // get children for each Child in this subassembly
            foreach (Component2 swChildComp in aChildren)
            {

                // Skip suppressed/excluded parts
                if ((swComponentSuppressionState_e)swChildComp.GetSuppression() != swComponentSuppressionState_e.swComponentSuppressed && !swChildComp.ExcludeFromBOM)
                {

                    // Get the model doc and info of the component
                    ModelDoc2 swChildDoc = (ModelDoc2)swChildComp.GetModelDoc();
                    string strChildPath = swChildComp.GetPathName();
                    string strChildConfig = swChildComp.ReferencedConfiguration;

                    // write status to form label
                    string strModelName = strChildPath.Substring(strChildPath.LastIndexOf("\\") + 1,
                                                                 strChildPath.Length - strChildPath.LastIndexOf("\\") - 1);
                    this.statLabel = strModelName;
                    Invoke(WriteLabelDelegate);

                    // Write custom properties for this config of this child 
                    //DbWriteConfig( swChildDoc, strChildPath, strChildConfig );
                    DbWriteConfig(swChildComp, strChildPath, strChildConfig);

                    // Write BOM adjacency
                    DbWriteAdjacency(strParentPath, strParentConfig, strChildPath, strChildConfig);

                    // If this component not already traversed
                    cmdCheckAdjacency.Parameters[0].Value = strChildPath;
                    cmdCheckAdjacency.Parameters[1].Value = strChildConfig;
                    long intExists = Convert.ToInt64(cmdCheckAdjacency.ExecuteScalar());
                    if (intExists == 0)
                    {

                        // If components not hidden from BOM
                        cmdShowChildren.Parameters[0].Value = strChildPath;
                        cmdShowChildren.Parameters[1].Value = strChildConfig;
                        long intShow = Convert.ToInt64(cmdShowChildren.ExecuteScalar());
                        if (intShow != 0)
                        {

                            // Recurse into this child
                            TraverseAssy(swChildComp, strChildPath, strChildConfig, intNextLevel);

                        }
                    }

                }

            }

            return (null);

        }
        swDocumentTypes_e GetTypeFromString(string strModelPathName)
        {
            
            string strModelName;
            string strFileExt;
            swDocumentTypes_e swDocType;

            strModelName = strModelPathName.Substring(strModelPathName.LastIndexOf("\\") + 1, strModelPathName.Length - strModelPathName.LastIndexOf("\\") - 1);
            strFileExt = strModelPathName.Substring(strModelPathName.LastIndexOf(".") + 1, strModelPathName.Length - strModelPathName.LastIndexOf(".") - 1);

            switch (strFileExt)
            {
                case "SLDASM":
                    swDocType = swDocumentTypes_e.swDocASSEMBLY;
                    break;
                case "SLDPRT":
                    swDocType = swDocumentTypes_e.swDocPART;
                    break;
                case "SLDDRW":
                    swDocType = swDocumentTypes_e.swDocDRAWING;
                    break;
                default:
                    swDocType = swDocumentTypes_e.swDocNONE;
                    break;
            }

            return (swDocType);

        }

    }

}
