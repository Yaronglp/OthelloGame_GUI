using System;

namespace Ex05.OthelloGameLogic
{
    public class GameOverArgs : EventArgs
    {
        private int m_BlackTokens;
        private int m_WhiteTokens;

        public int BlackTokens
        {
            get { return m_BlackTokens; }

            set { m_BlackTokens = value; }
        }

        public int WhiteTokens
        {
            get { return m_WhiteTokens; }

            set { m_WhiteTokens = value; }
        }

        public GameOverArgs(int i_WhiteTokens, int i_BlackTokens)
        {
            m_WhiteTokens = i_WhiteTokens;
            m_BlackTokens = i_BlackTokens;
        }
    }
}
