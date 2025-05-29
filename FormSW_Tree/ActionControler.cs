using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Linq;

namespace FormSW_Tree
{
    public partial class ActionControler : BackgroundWorker
    {
        SW sw;
        public ActionControler()
        {
            WorkerReportsProgress = true;
        }

    
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Init();
        }

        public bool Init()
        {
            sw = new SW();
            sw.connectSw += Sw_connectSw;
            sw.NotifySW += Sw_rebuild;
            PDM.NotifyPDM += PDM_NotifyPDM;
            sw.btnConnectSW();
            
            return true;
        }

        private void Sw_rebuild(int stage, MsgInfo info)
        {
            ReportProgress(stage, info);
        }

        private void Sw_connectSw(MsgInfo info, bool arg)
        {
          
            if (!arg)
            {
       
                ReportProgress(0, info);
      
            }
            else
            {
         
                ReportProgress(1, info);
                sw.CloseDoc();
                // RebuildTree();
                RebuildTreeLoopLevel();
                sw.connectSw-= Sw_connectSw;
                sw.NotifySW-= Sw_rebuild;
                PDM.NotifyPDM-= PDM_NotifyPDM;
                this.Dispose();
            }
        }

        private void PDM_NotifyPDM(int stage, MsgInfo msg)
        {
           ReportProgress(stage, msg);
        }

        /*private void RebuildTree()
        {
            List<Part> listPart = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.Ext == ".sldprt" || d.Ext == ".SLDPRT")
                .ToList();

            List<Drawing> listPartDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .ToList();

            List<Part> listAss = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.Ext == ".sldasm" || d.Ext == ".SLDASM")
                .ToList();

            List<Drawing> listAssDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .ToList();



            int CountItemToCheckOut = listAssDraw.Count + listAss.Count + listPartDraw.Count+ listPart.Count;



            PDM.CockSelList(CountItemToCheckOut);
            listPartDraw.ForEach(d => d.AddItemToSelList());
            listAss.ForEach(d => d.AddItemToSelList());
            listAssDraw.ForEach(d => d.AddItemToSelList());
            PDM.BatchGet();

            if (listPartDraw.Count > 0)
            {
          
                List<string> list =listPartDraw.Select(d => d.FullPath).ToList();
                sw.loopFilesToRebuild(list);
                PDM.CockSelList(listPartDraw.Count + listPart.Count);
                listPart.ForEach(d => d.AddItemToSelList());
                listPartDraw.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
            }

            if (listAss.Count > 0)
            {
                List<string> list = listAss.Select(d => d.FullPath).ToList();
                sw.loopFilesToRebuild(list);
                PDM.CockSelList(listAss.Count);
                listAss.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
            }

            if (listAssDraw.Count > 0)
            {
                List<string> list = listAssDraw.Select(d => d.FullPath).ToList();
                sw.loopFilesToRebuild(list);
                PDM.CockSelList(listAssDraw.Count);
                listAssDraw.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
                
            }

            

        }*/
        private void RebuildTreeLoopLevel()
        {
            List<Part> listPart = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.Ext == ".sldprt" || d.Ext == ".SLDPRT")
                .ToList();

            List<Drawing> listPartDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldprt" || d.model.Ext == ".SLDPRT")
                .ToList();

            List<Part> listAss = Tree.listComp.Where(c => c.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.Ext == ".sldasm" || d.Ext == ".SLDASM")
                .ToList();

            List<Drawing> listAssDraw = Tree.listDraw.Where(d => d.condition.stateModel == StateModel.Rebuild)
                .Where(d => d.model.Ext == ".sldasm" || d.model.Ext == ".SLDASM")
                .ToList();

            List<Model> models = listAss.Cast<Model>().Concat(listAssDraw).ToList();
            var groupedModels = models.GroupBy(m => m.Level);

            int CountItemToCheckOut = listAssDraw.Count + listAss.Count + listPartDraw.Count + listPart.Count;



            PDM.CockSelList(CountItemToCheckOut);
            listPart.ForEach(d => d.AddItemToSelList());
            listPartDraw.ForEach(d => d.AddItemToSelList());
            listAss.ForEach(d => d.AddItemToSelList());
            listAssDraw.ForEach(d => d.AddItemToSelList());
            PDM.BatchGet();

            if (listPartDraw.Count > 0)
            {

                List<string> list = listPartDraw.Select(d => d.FullPath).ToList();
                sw.loopFilesToRebuild(list);
                PDM.CockSelList(listPartDraw.Count + listPart.Count);
                listPart.ForEach(d => d.AddItemToSelList());
                listPartDraw.ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();
            }

            foreach (var group in groupedModels)
            {
                List<string> list = group.Select(d => d.FullPath).ToList();

                sw.loopFilesToRebuild(list);
                PDM.CockSelList(list.Count);
                group.ToList().ForEach(d => d.AddItemToSelList());
                PDM.DocBatchUnLock();

            }

        }


    }
}
