﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Ex05.OthelloGameLogic
{
    public delegate void MakeMoveEventHandler(object sender, MoveEventArgs e);
    public delegate void LegalSquareEventHandler(object sender, MoveEventArgs e);
    public delegate void IllegalSquareEventHandler(object sender, MoveEventArgs e);
    public delegate void GameOverEventHandler(object sender, GameOverArgs e);

    /// <summary>
    /// Represents OthelloGame Logic class
    /// </summary>
    public class OthelloGame
    {
        private const int k_WaitingTimeUntilPlayingTurnInMiliSec = 2000;
        private eGameMode m_GameMode;
        private Board m_GameBoard;
        private eCurrentTurn m_Turn = eCurrentTurn.BlackOpponentTurn;
        private int m_WhiteWins = 0;
        private int m_BlackWins = 0;

        public event MakeMoveEventHandler MakingMove;
        public event LegalSquareEventHandler LegalMove;
        public event IllegalSquareEventHandler IllegalMove;
        public event GameOverEventHandler GameOver;

        public int WhiteWins
        {
            get { return m_WhiteWins; }
        }

        public int BlackWins
        {
            get { return m_BlackWins; }
        }

        public OthelloGame(eGameMode i_GameMode, byte i_MatrixSize)
        {
            GameBoardSize = i_MatrixSize;
            m_GameMode = i_GameMode;
        }

        private enum eCurrentTurn : byte
        {
            WhiteOpponentTurn = 1,
            BlackOpponentTurn = 2
        }

        public enum eGameMode
        {
            Computer,
            Human
        }

        public enum eGameToken : byte
        {
            WhiteToken = 1,
            BlackToken = 2,
            Empty = 3
        }

        private struct legalMove
        {
            private byte m_Row;
            private byte m_Col;

            internal byte Row
            {
                get { return m_Row; }

                set { m_Row = value; }
            }

            internal byte Col
            {
                get { return m_Col; }

                set { m_Col = value; }
            }

            internal legalMove(byte i_row, byte i_Column)
            {
                m_Row = i_row;
                m_Col = i_Column;
            }
        }

        internal OthelloGame.eGameToken[,] GameStatus
        {
            get { return GameBoard.GameBoard; }

            set { GameBoard.GameBoard = value; }
        }

        internal byte GameBoardSize
        {
            get { return GameBoard.BoardSize; }

            set
            {
                m_GameBoard = new Board(value);
                GameBoard.ClearBoard();
            }
        }

        public Board GameBoard
        {
            get { return m_GameBoard; }
        }

        public static bool IsBoardSizeValid(byte i_BoardSize)
        {
            return Board.IsFittedToBounderiesOfBoardSize(i_BoardSize);
        }

        private void changePlayerTurn()
        {
            if (m_Turn == eCurrentTurn.WhiteOpponentTurn)
            {
                m_Turn = eCurrentTurn.BlackOpponentTurn;
            }
            else
            {
                m_Turn = eCurrentTurn.WhiteOpponentTurn;
            }
        }

        /// <summary>
        /// This method is Used for getting all the valid moves to a new list.
        /// </summary>
        /// <returns>List of legal moves</returns>
        private List<legalMove> getLegalMoves()
        {
            List<legalMove> legalMovesList = new List<legalMove>();

            for (byte i = 0; i < GameBoardSize; ++i)
            {
                for (byte j = 0; j < GameBoardSize; ++j)
                {
                    if (isLegalMovement(i, j))
                    {
                        legalMovesList.Add(new legalMove(i, j));
                    }
                }
            }

            return legalMovesList;
        }

        /// <summary>
        /// Choosing random move from list of legal movements and add it to the board.
        /// </summary>
        private void playComputerTurn()
        {
            List<legalMove> legalMoves = getLegalMoves();
            Random random = new Random();
            byte moveSelection = (byte)random.Next(0, legalMoves.Count - 1);
            byte row = legalMoves[moveSelection].Row;
            byte col = legalMoves[moveSelection].Col;

            Thread.Sleep(k_WaitingTimeUntilPlayingTurnInMiliSec);
            GameStatus.SetValue(this.getCurrentPlayerToken(), row, col);
            OnMakingMove(new MoveEventArgs(row, col, this.getCurrentPlayerToken()));
            this.addWonTokensToBoard(row, col);
        }

        private eGameToken getCurrentPlayerToken()
        {
            eGameToken currentToken = eGameToken.BlackToken;

            if (m_Turn == eCurrentTurn.WhiteOpponentTurn)
            {
                currentToken = eGameToken.WhiteToken;
            }

            return currentToken;
        }

        private eGameToken getOppositeCurrentPlayerToken()
        {
            eGameToken currentToken = this.getCurrentPlayerToken();

            return (currentToken == eGameToken.BlackToken) ? eGameToken.WhiteToken : eGameToken.BlackToken;
        }

        /// <summary>
        /// Responsible for placing the first four tokens on the board.
        /// </summary>
        public void PlacingFirstGameTokens()
        {
            byte middleOfBoard = (byte)((GameBoardSize / 2) - 1);

            GameStatus.SetValue(eGameToken.WhiteToken, middleOfBoard, middleOfBoard);
            OnMakingMove(new MoveEventArgs(middleOfBoard, middleOfBoard, eGameToken.WhiteToken));
            GameStatus.SetValue(eGameToken.WhiteToken, middleOfBoard + 1, middleOfBoard + 1);
            OnMakingMove(new MoveEventArgs(middleOfBoard + 1, middleOfBoard + 1, eGameToken.WhiteToken));
            GameStatus.SetValue(eGameToken.BlackToken, middleOfBoard, middleOfBoard + 1);
            OnMakingMove(new MoveEventArgs(middleOfBoard, middleOfBoard + 1, eGameToken.BlackToken));
            GameStatus.SetValue(eGameToken.BlackToken, middleOfBoard + 1, middleOfBoard);
            OnMakingMove(new MoveEventArgs(middleOfBoard + 1, middleOfBoard, eGameToken.BlackToken));
            this.updateAvailableSquares();
        }

        private void updateAvailableSquares()
        {
            for (byte i = 0; i < GameBoardSize; ++i)
            {
                for (byte j = 0; j < GameBoardSize; ++j)
                {
                    if (isLegalMovement(i, j))
                    {
                        OnLegalMove(new MoveEventArgs(i, j));
                    }
                    else
                    {
                        if (GameStatus[i, j] == eGameToken.Empty)
                        {
                            OnIllegalMove(new MoveEventArgs(i, j));
                        }
                    }
                }
            }
        }

        protected virtual void OnMakingMove(MoveEventArgs e)
        {
            if (MakingMove != null)
            {
                MakingMove.Invoke(this, e);
            }
        }

        protected virtual void OnLegalMove(MoveEventArgs e)
        {
            if (LegalMove != null)
            {
                LegalMove.Invoke(this, e);
            }
        }

        protected virtual void OnIllegalMove(MoveEventArgs e)
        {
            if (IllegalMove != null)
            {
                IllegalMove.Invoke(this, e);
            }
        }

        protected virtual void OnGameOver(GameOverArgs e)
        {
            if (GameOver != null)
            {
                GameOver.Invoke(this, e);
            }
        }

        private bool isLegalMovement(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool isBlockerTokenExist = false;
            eGameToken currentSquare = GameStatus[i_ChosenRow, i_ChosenColumn];

            if (currentSquare == eGameToken.Empty)
            {
                if (availableTokenPlacementFromDownToUp(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementFromUpToDown(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementFromLeftToRight(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementFromRightToLeft(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementDiagonalFromDownToUpFromLeftToRight(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementDiagonalFromDownToUpFromRightToLeft(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementDiagonalFromUpToDownFromLeftToRight(i_ChosenRow, i_ChosenColumn) ||
                    availableTokenPlacementDiagonalFromUpToDownFromRightToLeft(i_ChosenRow, i_ChosenColumn))
                {
                    isBlockerTokenExist = true;
                }
            }

            return isBlockerTokenExist;
        }

        private bool availableTokenPlacementFromDownToUp(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availablePlaceFromDown = false;

            if (i_ChosenRow > 1)
            {
                eGameToken oneSquareUp = this.GameStatus[i_ChosenRow - 1, i_ChosenColumn];
                if (oneSquareUp == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenRow - 1;
                    while ((i >= 0) && (this.GameStatus[i, i_ChosenColumn] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i, i_ChosenColumn] == this.getCurrentPlayerToken())
                        {
                            availablePlaceFromDown = true;
                        }

                        --i;
                    }
                }
            }

            return availablePlaceFromDown;
        }

        private bool availableTokenPlacementFromUpToDown(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availablePlaceFromDown = false;

            if (i_ChosenRow < GameBoardSize - 2)
            {
                eGameToken oneSquareDown = this.GameStatus[i_ChosenRow + 1, i_ChosenColumn];
                if (oneSquareDown == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenRow + 1;
                    while ((i < GameBoardSize) && (this.GameStatus[i, i_ChosenColumn] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i, i_ChosenColumn] == this.getCurrentPlayerToken())
                        {
                            availablePlaceFromDown = true;
                        }

                        ++i;
                    }
                }
            }

            return availablePlaceFromDown;
        }

        private bool availableTokenPlacementFromLeftToRight(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availablePlaceFromDown = false;

            if (i_ChosenColumn < GameBoardSize - 2)
            {
                eGameToken oneSquareRight = this.GameStatus[i_ChosenRow, i_ChosenColumn + 1];
                if (oneSquareRight == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenColumn + 1;
                    while ((i < GameBoardSize) && (this.GameStatus[i_ChosenRow, i] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i_ChosenRow, i] == this.getCurrentPlayerToken())
                        {
                            availablePlaceFromDown = true;
                        }

                        ++i;
                    }
                }
            }

            return availablePlaceFromDown;
        }

        private bool availableTokenPlacementFromRightToLeft(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availablePlaceFromDown = false;

            if (i_ChosenColumn > 1)
            {
                eGameToken oneSquareLeft = this.GameStatus[i_ChosenRow, i_ChosenColumn - 1];
                if (oneSquareLeft == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenColumn - 1;
                    while ((i >= 0) && (this.GameStatus[i_ChosenRow, i] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i_ChosenRow, i] == this.getCurrentPlayerToken())
                        {
                            availablePlaceFromDown = true;
                        }

                        --i;
                    }
                }
            }

            return availablePlaceFromDown;
        }

        private bool availableTokenPlacementDiagonalFromDownToUpFromLeftToRight(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availableTokenPlacement = false;

            if (i_ChosenRow > 1 && i_ChosenColumn < GameBoardSize - 2)
            {
                eGameToken oneSquareRightUp = this.GameStatus[i_ChosenRow - 1, i_ChosenColumn + 1];
                if (oneSquareRightUp == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenRow - 1;
                    int j = i_ChosenColumn + 1;
                    while ((i >= 0) && (j < GameBoardSize) && (this.GameStatus[i, j] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i, j] == this.getCurrentPlayerToken())
                        {
                            availableTokenPlacement = true;
                        }

                        --i;
                        ++j;
                    }
                }
            }

            return availableTokenPlacement;
        }

        private bool availableTokenPlacementDiagonalFromDownToUpFromRightToLeft(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availableTokenPlacement = false;

            if (i_ChosenRow > 1 && i_ChosenColumn > 1)
            {
                eGameToken oneSquareLeftUp = this.GameStatus[i_ChosenRow - 1, i_ChosenColumn - 1];
                if (oneSquareLeftUp == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenRow - 1;
                    int j = i_ChosenColumn - 1;
                    while ((i >= 0) && (j >= 0) && (this.GameStatus[i, j] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i, j] == this.getCurrentPlayerToken())
                        {
                            availableTokenPlacement = true;
                        }

                        --i;
                        --j;
                    }
                }
            }

            return availableTokenPlacement;
        }

        private bool availableTokenPlacementDiagonalFromUpToDownFromLeftToRight(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availableTokenPlacement = false;

            if (i_ChosenRow < GameBoardSize - 2 && i_ChosenColumn < GameBoardSize - 2)
            {
                eGameToken oneSquareRightDown = this.GameStatus[i_ChosenRow + 1, i_ChosenColumn + 1];
                if (oneSquareRightDown == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenRow + 1;
                    int j = i_ChosenColumn + 1;
                    while ((i < GameBoardSize) && (j < GameBoardSize) && (this.GameStatus[i, j] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i, j] == this.getCurrentPlayerToken())
                        {
                            availableTokenPlacement = true;
                        }

                        ++i;
                        ++j;
                    }
                }
            }

            return availableTokenPlacement;
        }

        private bool availableTokenPlacementDiagonalFromUpToDownFromRightToLeft(byte i_ChosenRow, byte i_ChosenColumn)
        {
            bool availableTokenPlacement = false;

            if (i_ChosenRow < GameBoardSize - 2 && i_ChosenColumn > 1)
            {
                eGameToken oneSquareLeftDown = this.GameStatus[i_ChosenRow + 1, i_ChosenColumn - 1];
                if (oneSquareLeftDown == this.getOppositeCurrentPlayerToken())
                {
                    int i = i_ChosenRow + 1;
                    int j = i_ChosenColumn - 1;
                    while ((i < GameBoardSize) && (j >= 0) && (this.GameStatus[i, j] != eGameToken.Empty))
                    {
                        if (this.GameStatus[i, j] == this.getCurrentPlayerToken())
                        {
                            availableTokenPlacement = true;
                        }

                        ++i;
                        --j;
                    }
                }
            }

            return availableTokenPlacement;
        }

        /// <summary>
        /// Perform calculation and add the tokens that the user earned by his move
        /// and then check if there is available turn.
        /// </summary>
        /// <param name="i_Row">Row selected by user</param>
        /// <param name="i_Column">Column Selected by user</param>
        public void AddTokenFromUserToBoard(byte i_Row, byte i_Column)
        {
            if (isLegalMovement(i_Row, i_Column))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), i_Row, i_Column);
                OnMakingMove(new MoveEventArgs(i_Row, i_Column, getCurrentPlayerToken()));
                this.addWonTokensToBoard(i_Row, i_Column);
                this.turnCheckerAndPerform();
            }
        }

        private void turnCheckerAndPerform()
        {
            this.changePlayerTurn();
            if (this.isGameOver())
            {
                this.endGame();
            }
            else
            {
                if (m_GameMode == eGameMode.Computer)
                {
                    this.playComputerMode();
                }

                this.updateAvailableSquares();
            }
        }

        private void playComputerMode()
        {
            if (thereIsAvailableMovementForPlayerOnBoard())
            {
                this.playComputerTurn();
                this.changePlayerTurn();
            }
            else
            {
                this.changePlayerTurn();
            }

            while (!thereIsAvailableMovementForPlayerOnBoard())
            {
                this.changePlayerTurn();
                if (!thereIsAvailableMovementForPlayerOnBoard())
                {
                    this.endGame();
                    break;
                }
                else
                {
                    this.playComputerTurn();
                    this.changePlayerTurn();
                }
            }
        }

        private bool isGameOver()
        {
            bool isGameOver = false;

            if (!thereIsAvailableMovementForPlayerOnBoard())
            {
                this.changePlayerTurn();
                if (!thereIsAvailableMovementForPlayerOnBoard())
                {
                    isGameOver = true;
                }

                if (m_GameMode == eGameMode.Computer)
                {
                    this.changePlayerTurn();
                }
            }

            return isGameOver;
        }

        private void endGame()
        {
            byte amountOfWhiteTokens;
            byte amountOfBlackTokens;

            this.updateAvailableSquares();
            this.countingPlayersTokens(out amountOfWhiteTokens, out amountOfBlackTokens);
            this.OnGameOver(new GameOverArgs(amountOfWhiteTokens, amountOfBlackTokens));
            this.m_Turn = eCurrentTurn.BlackOpponentTurn;
        }

        private void addWonTokensToBoard(byte i_Row, byte i_Column)
        {
            if (availableTokenPlacementFromLeftToRight(i_Row, i_Column))
            {
                addWonTokensFromLeftToRight(i_Row, i_Column);
            }

            if (availableTokenPlacementFromRightToLeft(i_Row, i_Column))
            {
                addWonTokensFromRightToLeft(i_Row, i_Column);
            }

            if (availableTokenPlacementFromUpToDown(i_Row, i_Column))
            {
                addWonTokensFromUpToDown(i_Row, i_Column);
            }

            if (availableTokenPlacementFromDownToUp(i_Row, i_Column))
            {
                addWonTokensFromDownToUp(i_Row, i_Column);
            }

            if (availableTokenPlacementDiagonalFromUpToDownFromLeftToRight(i_Row, i_Column))
            {
                addWonTokensFromUpToDownFromLeftToRight(i_Row, i_Column);
            }

            if (availableTokenPlacementDiagonalFromUpToDownFromRightToLeft(i_Row, i_Column))
            {
                addWonTokensFromUpToDownFromRightToLeft(i_Row, i_Column);
            }

            if (availableTokenPlacementDiagonalFromDownToUpFromLeftToRight(i_Row, i_Column))
            {
                addWonTokensFromDownToUpFromLeftToRight(i_Row, i_Column);
            }

            if (availableTokenPlacementDiagonalFromDownToUpFromRightToLeft(i_Row, i_Column))
            {
                addWonTokensFromDownToUpFromRightToLeft(i_Row, i_Column);
            }
        }

        private void addWonTokensFromUpToDownFromLeftToRight(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenColumn + 1;
            int j = i_ChosenRow + 1;

            while ((i < GameBoardSize) && (j < GameBoardSize) && (this.GameStatus[j, i] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), j, i);
                OnMakingMove(new MoveEventArgs(j, i, getCurrentPlayerToken()));
                ++i;
                ++j;
            }
        }

        private void addWonTokensFromUpToDownFromRightToLeft(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenColumn - 1;
            int j = i_ChosenRow + 1;

            while ((i > 0) && (j < GameBoardSize) && (this.GameStatus[j, i] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), j, i);
                OnMakingMove(new MoveEventArgs(j, i, getCurrentPlayerToken()));
                --i;
                ++j;
            }
        }

        private void addWonTokensFromDownToUpFromRightToLeft(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenColumn - 1;
            int j = i_ChosenRow - 1;

            while ((i > 0) && (j > 0) && (this.GameStatus[j, i] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), j, i);
                OnMakingMove(new MoveEventArgs(j, i, getCurrentPlayerToken()));
                --i;
                --j;
            }
        }

        private void addWonTokensFromDownToUpFromLeftToRight(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenColumn + 1;
            int j = i_ChosenRow - 1;

            while ((i < GameBoardSize) && (j > 0) && (this.GameStatus[j, i] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), j, i);
                OnMakingMove(new MoveEventArgs(j, i, getCurrentPlayerToken()));
                ++i;
                --j;
            }
        }

        private void addWonTokensFromLeftToRight(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenColumn + 1;

            while ((i < GameBoardSize) && (this.GameStatus[i_ChosenRow, i] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), i_ChosenRow, i);
                OnMakingMove(new MoveEventArgs(i_ChosenRow, i, getCurrentPlayerToken()));
                ++i;
            }
        }

        private void addWonTokensFromRightToLeft(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenColumn - 1;

            while ((i > 0) && (this.GameStatus[i_ChosenRow, i] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), i_ChosenRow, i);
                OnMakingMove(new MoveEventArgs(i_ChosenRow, i, getCurrentPlayerToken()));
                --i;
            }
        }

        private void addWonTokensFromUpToDown(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenRow + 1;

            while ((i < GameBoardSize) && (this.GameStatus[i, i_ChosenColumn] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), i, i_ChosenColumn);
                OnMakingMove(new MoveEventArgs(i, i_ChosenColumn, getCurrentPlayerToken()));
                ++i;
            }
        }

        private void addWonTokensFromDownToUp(byte i_ChosenRow, byte i_ChosenColumn)
        {
            int i = i_ChosenRow - 1;

            while ((i > 0) && (this.GameStatus[i, i_ChosenColumn] == getOppositeCurrentPlayerToken()))
            {
                GameStatus.SetValue(this.getCurrentPlayerToken(), i, i_ChosenColumn);
                OnMakingMove(new MoveEventArgs(i, i_ChosenColumn, getCurrentPlayerToken()));
                --i;
            }
        }

        private bool thereIsAvailableMovementForPlayerOnBoard()
        {
            bool availableMove = false;

            for (byte i = 0; i < GameBoardSize; ++i)
            {
                for (byte j = 0; j < GameBoardSize; ++j)
                {
                    if (this.isLegalMovement(i, j))
                    {
                        availableMove = true;
                    }
                }
            }

            return availableMove;
        }

        private void countingPlayersTokens(out byte o_AmountOfWhiteTokens, out byte o_AmountOfBlackTokens)
        {
            o_AmountOfWhiteTokens = 0;
            o_AmountOfBlackTokens = 0;

            foreach (eGameToken tokenType in GameStatus)
            {
                if (eGameToken.BlackToken == tokenType)
                {
                    o_AmountOfBlackTokens++;
                }

                if (eGameToken.WhiteToken == tokenType)
                {
                    o_AmountOfWhiteTokens++;
                }
            }

            this.updateWinsScore(o_AmountOfWhiteTokens, o_AmountOfBlackTokens);
        }

        private void updateWinsScore(byte i_AmountOfWhiteTokens, byte i_AmountOfBlackTokens)
        {
            if (i_AmountOfWhiteTokens < i_AmountOfBlackTokens)
            {
                m_BlackWins++;
            }
            else if (i_AmountOfWhiteTokens > i_AmountOfBlackTokens)
            {
                m_WhiteWins++;
            }
            else
            {
                m_BlackWins++;
                m_WhiteWins++;
            }
        }
    }
}