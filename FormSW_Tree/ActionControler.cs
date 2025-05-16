using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace FormSW_Tree
{
    public partial class ActionControler : BackgroundWorker
    {
        SW sw;
        InfoF f;
        string[] msgInfo;
        public ActionControler(InfoF _f)
        {
            WorkerReportsProgress = true;
            f = _f;
        
        }

    
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Init();
        }

        public bool Init()
        {
            sw = new SW();
            sw.connectSw += Sw_connectSw;
       
            sw.btnConnectSW();
            return true;
        }

        private void Sw_connectSw(string[] msg, bool arg)
        {
            if (!arg)
            {
                ReportProgress(0, msg);
            }
            else
            {
                ReportProgress(1, msg);
                sw.CloseDoc();
                RebuildTree();

            }
        }

        private void RebuildTree()
        {

            List<Drawing> listPartDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .ToList();

            List<Part> listAss = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .ToList();

            List<Drawing> listAssDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .ToList();



            int CountItemToCheckOut = listAssDraw.Count + listAss.Count + listPartDraw.Count;

          /*  msgInfo[0] = "Extract files from storage - CheckOut";
            msgInfo[1] = CountItemToCheckOut.ToString();
            Sw_operationSW(msgInfo);*/

            PDM.CockSelList(CountItemToCheckOut);
            listPartDraw.ForEach(d => d.AddItemToSelList());
            listAss.ForEach(d => d.AddItemToSelList());
            listAssDraw.ForEach(d => d.AddItemToSelList());
            PDM.BatchGet();

            if (listAssDraw.Count > 0)
            {
               /* msgInfo[0] = "opening and rebuilding drawings of parts";
                msgInfo[1] = CountItemToCheckOut.ToString();
                Sw_operationSW(msgInfo);*/
                BatchRefreshFile(listPartDraw);
            }

            if (listAss.Count > 0)
            {
             /*   msgInfo[0] = "opening and rebuilding assemble";
                msgInfo[1] = CountItemToCheckOut.ToString();
                Sw_operationSW(msgInfo)*/;
                listAss.ForEach(d => sw.OpenAndRefresh(d.FullPath));
                PDM.CockSelList(listAss.Count);
                listAss.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
            }

            if (listAssDraw.Count > 0)
            {
             /*   msgInfo[0] = "opening and rebuilding drawings of Assemble";
                msgInfo[1] = CountItemToCheckOut.ToString();
                Sw_operationSW(msgInfo);*/
                BatchRefreshFile(listAssDraw);
            }



        }
        private void BatchRefreshFile(List<Drawing> listDraw)
        {
            listDraw.ForEach(d => sw.OpenAndRefresh(d.FullPath));
            PDM.CockSelList(listDraw.Count);
            listDraw.ForEach(d => d.AddItemToSelList());
            PDM.DocBatchUnLock();
        }
    }
}
