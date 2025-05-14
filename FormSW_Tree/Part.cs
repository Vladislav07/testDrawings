using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FormSW_Tree
{
    internal class Part : Model
    {
        public virtual event Action<string, bool> NotificationParent;
        public List<string> listParent;
        
        internal Part(string cn, string fn) : base(cn, fn)
        {
            listParent = new List<string>();
        }

        
   
        public void NotificationState()
        {
            bool isBloced=false;
            if (condition is ModeBloced)
            {
                isBloced = true;

            }
            else if (condition is ModeRebuild) {
                isBloced = false;
            }
            else
            {
                return;
            }

            foreach (string item in listParent)
            {
                Notification(item, isBloced);
            }
        }


        protected void Notification(string item, bool isBloced)
        {
            try
            {
                NotificationParent.Invoke(item, isBloced);
            }
            catch (Exception)
            {
                MessageBox.Show("Parent - " + item.ToString());
                
            }
           
        }
        public override void SetState()
        {



            bool isRebuildPart = File.NeedsRegeneration((int)File.CurrentVersion, bFolder);

            if (isRebuildPart) condition = condition.GetState(true);


        }


    }
}
