﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormSW_Tree
{
    internal class Part : Model
    {
        internal Part(string cn, string fn) : base(cn, fn)
        {
        }
        public override string[] Print()
        {
            string[] listDisplay = new string[4];
            listDisplay[0] = CubyNumber;
            listDisplay[1] = "Part";
            listDisplay[2] = st.ToString();
            listDisplay[3] = Level.ToString();
            return listDisplay;
        }
   
    }
}
