using System;

namespace Ex05.OthelloGameLogic
{
    public class Board
    {
        private const byte k_MinBoardLength = 6;
        private const byte k_MidDownLength = 8;
        private const byte k_MidUpLength = 10;
        private const byte k_MaxBoardLength = 12;
        private byte m_BoardSize;
        private OthelloGame.eGameToken[,] m_GameBoard;

        internal static byte MinBoardLength
        {
            get { return k_MinBoardLength; }
        }

        internal static byte MaxBoardLength
        {
            get { return k_MaxBoardLength; }
        }

        internal byte BoardSize
        {
            get { return m_BoardSize; }

            set { m_BoardSize = value; }
        }

        internal OthelloGame.eGameToken[,] GameBoard
        {
            get { return m_GameBoard; }

            set { m_GameBoard = value; }
        }

        public Board(byte i_BoardSize)
        {
            BoardSize = i_BoardSize;
            GameBoard = new OthelloGame.eGameToken[i_BoardSize, i_BoardSize];
        }

        internal static bool IsFittedToBounderiesOfBoardSize(byte i_BoardSize)
        {
            return i_BoardSize == MinBoardLength || i_BoardSize == MaxBoardLength || i_BoardSize == k_MidDownLength
                || i_BoardSize == k_MidUpLength;
        }

        public void ClearBoard()
        {
            for (int i = 0; i < BoardSize; ++i)
            {
                for (int j = 0; j < BoardSize; ++j)
                {
                    GameBoard[i, j] = OthelloGame.eGameToken.Empty;
                }
            }
        }
    }
}