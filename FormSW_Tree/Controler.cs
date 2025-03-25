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
        public PdmID(int file, int folder, string pathFile)
        {
            FileId = file;
            FolderId = folder;
            PathFile = pathFile;
        }
        public int FileId { get; set; }
        public int FolderId { get; set; }
        public string PathFile { get; set; }
    }
    enum StateApp
    {
        readForUpdate=0,
        updateIsNotPossible=2,
        detailsUpdate=3,
        partsDrawings=4,
        assemble=5,
        asembleDrawings=6,
        errorUpdate=7
    }
   public static class Controler
    {
        public static event Action<string> NumberModel;
        public static event Action<string> MsgState;
        public static event Action<string, List<Component>> ActionRebuild;
        static SW sw;
        private static List<Component> models { get; set; }
        private static List<Drawing> drawings { get; set; }
        private static StateApp stateApp;

         static Controler()
        {
            models = new List<Component>();
            drawings = new List<Drawing>();
            stateApp = StateApp.readForUpdate;
        }

        public static bool Init()
        {
            sw = new SW();
            sw.numberModel += Sw_numberModel;
            sw.action += Sw_action;
            sw.btnConnectSW();
            sw.BuildTree();
            Tree.SearchParentFromChild();
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
            Tree.FillCollection();
            Tree.CompareVersions();
            Tree.IsDrawings();
            return true;
        }

   

        public static bool RebuildTree()
        {
            List<Component> list = Tree.listComp.Where(comp => IsCuby(comp)).ToList();
            List<Component> listNot = list.Where(comp => IsNotRebuidCuby(comp)).ToList();

            if (listNot.Count > 0)
            {
                ActionRebuild.Invoke("Перестроение невозможно", listNot);
                return false;
            }

            List<Component> listParts = list.Where(comp => IsRebuidCuby(comp)).Where(comp => IsParts(comp)).ToList();
            List<Component> listAsm = list.Where(comp => IsRebuidCuby(comp)).Where(comp => IsAsm(comp)).ToList();
            List<Component> listPartsDraw = list.Where(comp => IsParts(comp)).Where(c => IsDraw(c)).Where(comp => IsRebuidCubyDraw(comp)).ToList();
            List<Component> listAsmDraw = list.Where(comp => IsAsm(comp)).Where(c => IsDraw(c)).Where(comp => IsRebuidCubyDraw(comp)).ToList();


            List<PdmID> listPdmParts = new List<PdmID>();
            List<PdmID> listPdmDrawParts = new List<PdmID>();
            List<PdmID> listPdmModelPartsToDraw = new List<PdmID>();
            List<PdmID> listPdmModelAsmToDraw = new List<PdmID>();
            List<PdmID> listPdmAsm = new List<PdmID>();
            List<PdmID> listPdmDrawAsm = new List<PdmID>();

            listParts.ForEach(item => listPdmParts.Add(new PdmID(item.bFile, item.bFolder, item.FullPath)));
            listAsm.ForEach(item =>
                listPdmAsm.Add(new PdmID(item.bFile, item.bFolder, item.FullPath)));

            listPartsDraw.ForEach(item =>
                {
                    listPdmDrawParts.Add(new PdmID(item.draw.FileID, item.draw.FolderID, item.draw.path));
                    listPdmModelPartsToDraw.Add(new PdmID(item.bFile, item.bFolder, item.FullPath));
                });

            listAsmDraw.ForEach(item =>
             {
                listPdmDrawAsm.Add(new PdmID(item.draw.FileID, item.draw.FolderID, item.draw.path));
                listPdmModelAsmToDraw.Add(new PdmID(item.bFile, item.bFolder, item.FullPath));
             });

            if (listPdmParts.Count > 0)Update(listPdmParts, listPdmParts);

            if (listPdmDrawParts.Count>0) UpdateDraw(listPdmDrawParts.Concat(listPdmModelPartsToDraw).ToList(), listPdmDrawParts);

            if (listPdmAsm.Count>0)Update(listPdmAsm, listPdmAsm);
            
            if(listPdmDrawAsm.Count>0) UpdateDraw(listPdmDrawAsm.Concat(listPdmModelAsmToDraw).ToList(), listPdmDrawAsm);

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

        static Predicate<Component> IsCuby = (Component comp) =>
        {
            string regCuby = @"^CUBY-\d{8}$";
            return Regex.IsMatch(comp.CubyNumber, regCuby);
        };

        static Predicate<Component> IsRebuidCuby = (comp) => (comp.State.Name == "In work" && comp.IsRebuild == true);

        static Predicate<Component> IsRebuidCubyDraw = (comp) =>
          ((comp.draw.State.Name == "In work" && (comp.draw.CompareVersRef == true || comp.draw.NeedsRegeneration == true))); 
     
        
    
        static Predicate<Component> IsNotRebuidCuby = (comp) => (comp.IsRebuild == true && comp.State.Name != "In work"  );
        static Predicate<Component> IsParts = (Component comp) => comp.Ext == ".sldprt" || comp.Ext == ".SLDPRT";
        static Predicate<Component> IsAsm = (Component comp) => comp.Ext == ".sldasm" || comp.Ext == ".SLDASM";
        static Predicate<Component> IsDraw = (Component comp) => comp.isDraw;
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