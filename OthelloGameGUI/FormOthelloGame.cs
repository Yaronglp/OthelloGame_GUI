﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ex05.OthelloGameLogic;

namespace Ex05.OthelloGameGUI
{
    public class FormOthelloGame : Form
    {
        private const string k_White = "White";
        private const string k_Black = "Black";
        private const byte k_WidthDistance = 40;
        private const byte k_HeightDistance = 10;
        private readonly OthelloGame r_Game;
        private GameButton[,] m_ButtonsMatrix;

        public FormOthelloGame(byte i_MatrixSize, OthelloGame.eGameMode i_GameMode)
        {
            this.Size = new Size(k_WidthDistance * i_MatrixSize + k_WidthDistance, k_WidthDistance * i_MatrixSize + k_WidthDistance + k_HeightDistance * 2);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Text = "Othello";
            r_Game = new OthelloGame(i_GameMode, i_MatrixSize);
            this.createMatrix(i_MatrixSize);
            this.createListeners();
            this.startGame();
            this.ShowDialog();
        }

        private void createListeners()
        {
            r_Game.MakingMove += new MakeMoveEventHandler(r_Game_MakingMove);
            r_Game.LegalMove += new LegalSquareEventHandler(r_Game_LegalMove);
            r_Game.IllegalMove += new IllegalSquareEventHandler(r_Game_IllegalMove);
            r_Game.GameOver += new GameOverEventHandler(r_Game_GameOver);
        }

        private void r_Game_GameOver(object sender, GameOverArgs e)
        {
            string winner = "It's a tie";

            if (e.WhiteTokens != e.BlackTokens)
            {
                winner = string.Format("{0} Won", e.WhiteTokens > e.BlackTokens ? k_White : k_Black);
            }

            string msgToShow = string.Format(
@"{0}!! ({1}/{2}) ({3}/{4})
Would you like to play another round?",
winner,
e.BlackTokens,
e.WhiteTokens,
r_Game.BlackWins,
r_Game.WhiteWins);
            DialogResult result = MessageBox.Show(msgToShow, "Game Over", MessageBoxButtons.YesNo);
            this.decideIfToPlayAnotherRoundOrNot(result);
        }

        private void r_Game_IllegalMove(object sender, MoveEventArgs e)
        {
            m_ButtonsMatrix[e.I, e.J].Enabled = false;
            m_ButtonsMatrix[e.I, e.J].BackColor = Button.DefaultBackColor;
        }

        private void r_Game_LegalMove(object sender, MoveEventArgs e)
        {
            m_ButtonsMatrix[e.I, e.J].Enabled = true;
            m_ButtonsMatrix[e.I, e.J].BackColor = Color.Gray;
        }

        private void r_Game_MakingMove(object sender, MoveEventArgs e)
        {
            this.Refresh();
            m_ButtonsMatrix[e.I, e.J].Enabled = true;
            switch (e.Token)
            {
                case OthelloGame.eGameToken.BlackToken:
                    m_ButtonsMatrix[e.I, e.J].BackColor = Color.Black;
                    m_ButtonsMatrix[e.I, e.J].ForeColor = Color.White;
                    break;
                case OthelloGame.eGameToken.WhiteToken:
                    m_ButtonsMatrix[e.I, e.J].BackColor = Color.White;
                    m_ButtonsMatrix[e.I, e.J].ForeColor = Color.Black;
                    break;
            }
            m_ButtonsMatrix[e.I, e.J].Text = "O";
            this.Refresh();
        }

        private void createMatrix(byte i_MatrixSize)
        {
            m_ButtonsMatrix = new GameButton[i_MatrixSize, i_MatrixSize];

            for (byte i = 0; i < i_MatrixSize; ++i)
            {
                for (byte j = 0; j < i_MatrixSize; ++j)
                {
                    m_ButtonsMatrix[i, j] = new GameButton(i, j);
                    m_ButtonsMatrix[i, j].Location = new Point(k_HeightDistance + j * k_WidthDistance, k_HeightDistance + i * k_WidthDistance);
                    m_ButtonsMatrix[i, j].Size = new Size(k_WidthDistance, k_WidthDistance);
                    m_ButtonsMatrix[i, j].Enabled = false;
                    this.Controls.Add(m_ButtonsMatrix[i, j]);
                    m_ButtonsMatrix[i, j].Click += new EventHandler(FormOthelloGame_Click);
                }
            }
        }

        private void FormOthelloGame_Click(object sender, EventArgs e)
        {
            GameButton button = sender as GameButton;
            r_Game.AddTokenFromUserToBoard(button.I, button.J);
        }

        private void startGame()
        {
            r_Game.PlacingFirstGameTokens();
        }

        private void decideIfToPlayAnotherRoundOrNot(DialogResult i_Result)
        {
            if (i_Result == DialogResult.No)
            {
                this.Close();
            }
            else
            {
                this.prepareBoardForAnotherRound();
            }
        }

        private void prepareBoardForAnotherRound()
        {
            foreach (Button button in m_ButtonsMatrix)
            {
                button.Text = null;
                button.BackColor = Button.DefaultBackColor;
            }

            r_Game.GameBoard.ClearBoard();
            this.startGame();
        }
    }
}