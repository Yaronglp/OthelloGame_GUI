using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ex05.OthelloGameLogic;

namespace Ex05.OthelloGameGUI
{
    public class FormGameSettings : Form
    {
        private const string k_SizeButtonText = "Board Size : {0}:{0} (click to increase)";
        private const byte k_DefaultBoarderSize = 6;
        private static byte s_SizeButton = k_DefaultBoarderSize;
        private Button m_GameBorderSizeButton;
        private Button m_ComputerOpponentButton;
        private Button m_HumanOpponentButton;

        public FormGameSettings()
        {
            this.Size = new Size(370, 270);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Text = "Othello - Game Settings";
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            m_GameBorderSizeButton = new Button();
            m_GameBorderSizeButton.Size = new Size(320, 120);
            m_GameBorderSizeButton.Text = string.Format(k_SizeButtonText, s_SizeButton);
            m_GameBorderSizeButton.Location = new Point(18, 15);
            this.Controls.Add(m_GameBorderSizeButton);
            m_GameBorderSizeButton.Click += new EventHandler(m_SizeButton_Click);

            m_ComputerOpponentButton = new Button();
            m_ComputerOpponentButton.Size = new Size(154, 70);
            m_ComputerOpponentButton.Location = new Point(18, 147);
            m_ComputerOpponentButton.Text = "Play against the computer";
            this.Controls.Add(m_ComputerOpponentButton);
            m_ComputerOpponentButton.Click += new EventHandler(m_ComputerOpponentButton_Click);

            m_HumanOpponentButton = new Button();
            m_HumanOpponentButton.Size = new Size(154, 70);
            m_HumanOpponentButton.Location = new Point(184, 147);
            m_HumanOpponentButton.Text = "Play against your friend";
            this.Controls.Add(m_HumanOpponentButton);
            m_HumanOpponentButton.Click += new EventHandler(m_HumanOpponentButton_Click);
        }

        private void m_HumanOpponentButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormOthelloGame game = new FormOthelloGame(s_SizeButton, OthelloGame.eGameMode.Human);
            this.Close();
        }

        private void m_ComputerOpponentButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormOthelloGame game = new FormOthelloGame(s_SizeButton, OthelloGame.eGameMode.Computer);
            this.Close();
        }

        private void m_SizeButton_Click(object sender, EventArgs e)
        {
            s_SizeButton += 2;
            if (!OthelloGame.IsBoardSizeValid(s_SizeButton))
            {
                s_SizeButton = k_DefaultBoarderSize;
            }

            m_GameBorderSizeButton.Text = string.Format(k_SizeButtonText, s_SizeButton);
        }
    }
}