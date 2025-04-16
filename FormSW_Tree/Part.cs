using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FormSW_Tree
{
    internal class Part : Model
    {
        public virtual event Action<string, StateModel> NotificationParent;
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
                    if (st == StateModel.Init|| st == StateModel.Clean)
                    {
                        st = StateModel.Clean;
                    }
                    else 
                    {
                        NotificationState();
                    }
                    break;
                case "Pending Express Manufacturing":
                case "Express Manufacturing":
                case "Reset to in Work":
                    if (st == StateModel.Init || st == StateModel.Clean)
                    {
                        st = StateModel.Blocked;
                    }
                    else
                    {
                        st = StateModel.ImpossibleRebuild;

                    }
                    NotificationState();
                    break;
                default:
                    break;
            }

            void NotificationState()
            {
                foreach (string item in listParent)
                {
                    Notification(item, st);
                }
            }

        }
        protected void Notification(string item, StateModel st)
        {
            try
            {
                NotificationParent.Invoke(item, st);
            }
            catch (Exception)
            {
                MessageBox.Show("Parent - " + item.ToString());
                
            }
           
        }
    }
}
