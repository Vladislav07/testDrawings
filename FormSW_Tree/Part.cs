﻿using System;
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
                case "Approved to use":
                    st = StateModel.Stand;
                    break;

                case "Initiated":
                    st = StateModel.Initiated;
                    NotificationState();
                    break;

                case "In work":
                    if (st == StateModel.Init) st = StateModel.Clean;                   
                    if (st == StateModel.ExtractPart)
                    {                     
                        NotificationState();
                    }
                    break;

                case "Pending Express Manufacturing":
                case "Express Manufacturing":
                case "Reset to in Work":

                    if (st == StateModel.Init) st = StateModel.Clean;
                    if (st == StateModel.ExtractPart)
                    {
                        st = StateModel.Blocked;
                        NotificationState();
                    }
                    break;

                default:
                    break;
            }
        }
        protected void NotificationState()
        {
            foreach (string item in listParent)
            {
                Notification(item);
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
