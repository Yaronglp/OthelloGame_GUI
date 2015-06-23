using System;

namespace Ex05.OthelloGameLogic
{
    /// <summary>
    /// Represents the row and column arguments for a game move.
    /// </summary>
    public class MoveEventArgs : EventArgs
    {
        private int m_I;
        private int m_J;
        private OthelloGame.eGameToken m_Token;

        public int I
        {
            get { return m_I; }

            set { m_I = value; }
        }

        public int J
        {
            get { return m_J; }

            set { m_J = value; }
        }

        public OthelloGame.eGameToken Token
        {
            get { return m_Token; }

            set { m_Token = value; }
        }

        public MoveEventArgs(int i_I, int i_J, OthelloGame.eGameToken i_Token)
        {
            m_I = i_I;
            m_J = i_J;
            m_Token = i_Token;
        }

        public MoveEventArgs(int i_I, int i_J)
            : this(i_I, i_J, OthelloGame.eGameToken.Empty)
        {
        }
    }
}