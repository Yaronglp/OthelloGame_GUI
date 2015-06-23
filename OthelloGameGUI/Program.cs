using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ex05.OthelloGameGUI
{
    public static class Program
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new FormGameSettings());
        }
    }
}
