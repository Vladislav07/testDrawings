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
            
            base.SetState();

            if (st == StateModel.Clean || st == StateModel.Blocked) return;

            foreach (string item in listParent)
            {
                Notification(item, st);
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
