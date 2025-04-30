using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FormSW_Tree
{
    internal class Part : Model
    {
        public virtual event Action<string, Model> NotificationParent;
        public List<string> listParent;
        
        internal Part(string cn, string fn) : base(cn, fn)
        {
            listParent = new List<string>();
        }
      

        public override void SetState()
        {
            switch (File.CurrentState.Name)
            {
                case "Check library item":
                case "Kanban":
                    st = StateModel.Stand;
                    break;
                case "Initiated":
                    st = StateModel.Initiated;
                    NotificationState();
                    break;
                case "In work":
                    if (st == StateModel.Init)
                    {
                        st = StateModel.Clean;
                    }
                    else if (st == StateModel.ExtractPart|| st == StateModel.OnlyAss)
                    {
                        NotificationState();
                    }
                    else if(st == StateModel.ChildCannotBeUpdated)
                    {
                        NotificationState();
                    }
                        break;
                case "Pending Express Manufacturing":
                case "Express Manufacturing":
                case "Reset to in Work":
                    if (st == StateModel.Init || st == StateModel.Clean || st == StateModel.UpdateDrawing)
                    {
                        st = StateModel.Blocked;
                    }
                 
                    else
                    {
                        st = StateModel.ImpossibleRebuild;
                        NotificationState();
                    }
                    
                    break;
                default:
                    break;
            }

            void NotificationState()
            {
                foreach (string item in listParent)
                {
                    Notification(item);
                }
            }

        }
        protected void Notification(string item)
        {
            try
            {
                NotificationParent.Invoke(item, this);
            }
            catch (Exception)
            {
                MessageBox.Show("Parent - " + item.ToString());
                
            }
           
        }

    }
}
