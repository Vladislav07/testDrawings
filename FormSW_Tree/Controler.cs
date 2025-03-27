using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FormSW_Tree
{
    public struct PdmID
    {
        public PdmID(int file, int folder)
        {
            FileId = file;
            FolderId = folder;
        }
        public int FileId { get; set; }
        public int FolderId { get; set; }
 
    }

   public static class Controler
    {
        public static event Action<string> NumberModel;
        public static event Action<string> MsgState;
        public static event Action<string, List<IDisplay>> ActionRebuild;
        static SW sw;
        private static List<Model> models { get; set; }

 

         static Controler()
        {
            models = new List<Model>();
        }

        public static bool Init()
        {
            sw = new SW();
            sw.numberModel += Sw_numberModel;
            sw.action += Sw_action;
            sw.btnConnectSW();
            sw.BuildTree();
            Tree.SearchParentFromChild();
            Tree.FillCollection();
            return true;
        }

        private static void Sw_action(string msg)
        {
            MsgState.Invoke(msg);
        }

        private static void Sw_numberModel(string obj)
        {
            if (NumberModel!=null)
            {
                string number = Path.GetFileName(obj);
                NumberModel.Invoke(number);
            }
        }

        public static bool GetInfoFromPDM()
        {
            Tree.GetInfoPDM();
            Tree.CompareVersions();
            FilteringList();
            return true;
        }

        private static void FilteringList()
        {
            List<IDisplay> list = Tree.listComp.Where(comp => IsRebuidModel(comp)).Select(comp => (IDisplay)comp).ToList();
            List<IDisplay> listDraw = Tree.listDraw.Where(comp => IsRebuidDraw(comp)).Select(comp => (IDisplay)comp).ToList();

            ActionRebuild.Invoke("list", list.Concat(listDraw).ToList());
        }
   


        public static bool RebuildTree()
        {
           //List<IRebuild> listPartDraw = Tree.listDraw.Where(comp => IsRebuidDraw(comp)).Where(d=>d.mode)

            List<PdmID> listPdmParts = new List<PdmID>();
            List<PdmID> listPdmDrawParts = new List<PdmID>();
            List<PdmID> listPdmModelPartsToDraw = new List<PdmID>();
            List<PdmID> listPdmModelAsmToDraw = new List<PdmID>();
            List<PdmID> listPdmAsm = new List<PdmID>();
            List<PdmID> listPdmDrawAsm = new List<PdmID>();

           

            if (listPdmParts.Count > 0)Update(listPdmParts, listPdmParts);

            if (listPdmDrawParts.Count>0) UpdateDraw(listPdmDrawParts.Concat(listPdmModelPartsToDraw).ToList(), listPdmDrawParts);

            if (listPdmAsm.Count>0)Update(listPdmAsm, listPdmAsm);
            
            if(listPdmDrawAsm.Count>0) UpdateDraw(listPdmDrawAsm, listPdmDrawAsm);

           // GetInfoFromPDM();

            return true;
        } 

        public static bool Cancel()
        {
            return true;
        }


        private static void Update(List<PdmID> listToPdm, List<PdmID> listToSw)
        {
            try
            {
                PDM.AddSelItemToList(listToPdm);
                PDM.BatchGet(listToPdm);
                sw.OpenAndRefresh(listToSw);
                PDM.DocBatchUnLock();
            }
            catch (Exception)
            {
                MessageBox.Show("Error updating model");
               
            }
          
        }
        private static void UpdateDraw(List<PdmID> listToPdm, List<PdmID> listToSw)
        {
            try
            {
                PDM.AddSelItemToList(listToPdm);
                PDM.BatchGet(listToPdm);
                sw.OpenAndRefreshDrawings(listToSw);
                PDM.DocBatchUnLock();
            }
            catch (Exception)
            {
                MessageBox.Show("Error updating Draw");

            }

        }

        static Predicate<Model> IsCuby = (Model comp) =>
        {
            string regCuby = @"^CUBY-\d{8}$";
            return Regex.IsMatch(comp.CubyNumber, regCuby);
        };

        static Predicate<Model> IsRebuidModel = (Model comp) => comp.st == StateModel.ModelAndDraw|| comp.st == StateModel.DrawFromModel;
        static Predicate<Drawing> IsRebuidDraw = (Drawing comp) => comp.st == StateModel.OnlyDraw || comp.st == StateModel.DrawFromModel;
        static Predicate<Model> IsParts = (Model comp) => comp.Ext == ".sldprt" || comp.Ext == ".SLDPRT";
        static Predicate<Model> IsAsm = (Model comp) => comp.Ext == ".sldasm" || comp.Ext == ".SLDASM";
      
    }
}

/*
 1. выявить
       все компоненты, которые не дадут обновить дерево
       а. в состоянии не inwork (для CUBY-00...)
       б. библиотечные компоненты, состояние которых init...
  if > 0
 1.1 Состояние - обновление не возможно
 1.2 Вывести список 
 1.3 Вывести сообщение

 2. Состояние - возможно обновление
 2.1 вывести список файлов к обновлению
     а. модели не перестроенные
     б. модели - в иерархии выше
     в. чертежи не перестроенные
     г. чертежи обновляемых моделей
 2.2 вывести сообщение

 3 Состояние - обновление
 3.1 модели деталей
 3.2 Чертежи деталей (модель детали в checkout)
 3.3 Модели сборок снизу вверх
 3.4 Чертежи сборок
 3.5 вывод номера обновляемой детали 
 3.6 обновление списка

 4. состояние - сканирование дерева
       
    
 
 * */