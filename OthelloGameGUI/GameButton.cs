using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ex05.OthelloGameGUI
{
    internal class GameButton : Button
    {
        private readonly byte r_I;
        private readonly byte r_J;

        internal byte I
        {
            get { return r_I; }
        }

        internal byte J
        {
            get { return r_J; }
        }

        internal GameButton(byte i_I, byte i_J)
        {
            r_I = i_I;
            r_J = i_J;
        }
    }
}