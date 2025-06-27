using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPDM.Interop.epdm;

namespace FormSW_Tree
{
    public class EdmCallbackGet: IEdmGetOpCallback3
    {
        public void ProgressBegin(EdmProgressType eType, int lSteps)
        {
            if (eType != EdmProgressType.Ept_Operation) return;
            MsgInfo info = new MsgInfo();
            info.numberCuby=eType.ToString();
            info.countStep = lSteps;
            info.typeOperation = "CheckOut files";
            PDM.NotifyOperation(2, info);
        }

        public bool ProgressStep(EdmProgressType eType, string bsMessage, int lProgressPos)
        {
            MsgInfo info = new MsgInfo();
            string nameFile=Path.GetFileName(bsMessage);
            info.numberCuby = nameFile;
            info.currentStep = lProgressPos;
            PDM.NotifyOperation(3, info);
            return true;
        }

        public void ProgressEnd(EdmProgressType eType)
        {
            if (eType != EdmProgressType.Ept_Operation) return;
            MsgInfo info = new MsgInfo();
            info.typeOperation = "Finish CheckOut";
            info.countStep = 0;
            info.currentStep = 0;
            PDM.NotifyOperation(2, info);
        }

        public void LogMessage(EdmGetOpMsg eMsgID, int hCode, string bsDetails)
        {
            Console.WriteLine(eMsgID.ToString() + " - " + bsDetails + " -  " + hCode.ToString());
        }

        public bool ConfirmReplace(EdmGetConfirmReason eReason, string bsPath)
        {
            Console.WriteLine(eReason.ToString() + " - " + bsPath);
            return true;
        }

        public bool ReportFailure(int lFileID, string bsPath, int hError, string bsDetails)
        {
            MsgInfo info = new MsgInfo();
            info.errorMsg = bsDetails;
            info.numberCuby = Path.GetFileName(bsPath);
            PDM.NotifyOperation(0, info);
            return true;
        }

        public EdmGetOpReply ReportFailureEx(int lFileID, int lVersionNo, string bsPath, EdmGetOpError eError, object oArg1, object oArg2, object oArg3)
        {
            MsgInfo info = new MsgInfo();
            info.typeOperation = eError.ToString();
            info.numberCuby = bsPath;
            info.errorMsg = " Error Get File from PDM";
            PDM.NotifyOperation(0, info);
            return EdmGetOpReply.EdmGetRep_Process;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void SetProgressMessage(string bsMessage)
        {
            Console.Write(bsMessage);
        }

        public void IsCancelPressed()
        {
            throw new NotImplementedException();
        }
    }
}
