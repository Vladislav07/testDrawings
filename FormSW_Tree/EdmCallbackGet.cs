﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPDM.Interop.epdm;

namespace FormSW_Tree
{
    public class EdmCallbackGet: IEdmGetOpCallback
    {
        public void ProgressBegin(EdmProgressType eType, int lSteps)
        {
            MsgInfo info = new MsgInfo("", true, "", "CheckIn files", lSteps);
            PDM.NotifyOperation(2, info);
        }

        public bool ProgressStep(EdmProgressType eType, string bsMessage, int lProgressPos)
        {
            MsgInfo info = new MsgInfo();
            string nameFile=Path.GetFileName(bsMessage);
            info.numberCuby = nameFile;
            info.currentStep = lProgressPos + 1;
            PDM.NotifyOperation(3, info);
            return true;
        }

        public void ProgressEnd(EdmProgressType eType)
        {
            MsgInfo info = new MsgInfo();
            info.typeOperation = "Finish CheckIn";
            info.countStep = 0;
            PDM.NotifyOperation(2, info);
        }

        public void LogMessage(EdmGetOpMsg eMsgID, int hCode, string bsDetails)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmReplace(EdmGetConfirmReason eReason, string bsPath)
        {
            throw new NotImplementedException();
        }

        public bool ReportFailure(int lFileID, string bsPath, int hError, string bsDetails)
        {
            throw new NotImplementedException();
        }
    }
}
