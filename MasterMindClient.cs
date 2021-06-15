using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;
using Bindings;

namespace MasterMind56955_JRN
{
    public partial class MasterMindClient : Form
    {
        TCPClient remoteClient;
        BitArray selectedOvals;             //used to identify which oval has been selected
        List<Panel> gameLevels = new List<Panel>();         //Stores panels with gameovals
        List<OvalShape> gameOvals = new List<OvalShape>();

        public delegate void GameOver();

        public event GameOver GO;

        //Answers
        public int[] colourA1 = new int[3];
        public int[] colourA2 = new int[3];
        public int[] colourA3 = new int[3];
        public int[] colourA4 = new int[3];
        public int[] colourA5 = new int[3];

        //buffers used to evaluate score 
        public int[] tempC0 = new int[3];
        public int[] tempC1 = new int[3];
        public int[] tempC2 = new int[3];
        public int[] tempC3 = new int[3];
        public int[] tempC4 = new int[3];

        int[] bestTime = new int[3] { 0,0,0};

        bool winner = false;
        int currentLevel = 1;
        int iScoreStart = 0;
        int iStop = 5;

        public MasterMindClient()
        {
            InitializeComponent();
        }

        private void MasterMindClient_Load(object sender, EventArgs e)
        {
            //Create necessary instances
            remoteClient = new TCPClient();
            selectedOvals = new BitArray(30);

            //Subscribe to events
            remoteClient.ClientConnected += Remote_Client_Connected;
            remoteClient.ConnectionRefused += Remote_Client_ConnectionRefused;
            GO += DisplayPoints;
            remoteClient.ClientDisconnected += Remote_Client_ClientDisconnected;
            remoteClient.DataReceived += PrintDataReceivedFromServer;

            //Disable all rows except starting row
            //and disable colour panel
            lblPointsTotal.Text = "Points: ";
            PopulateGamePanels();
            PopulateGameOvals();
            InitialiseBoard();

            //Connect client
            remoteClient.ConnectToServer();
        }

        #region Event Handlers
        private void Remote_Client_Connected(TCPClient client)
        {
            lblConnInfo.Text = "You are connected to " + client.ClientSocket.RemoteEndPoint;
        }
        private void Remote_Client_ConnectionRefused(TCPClient client, string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }
        private void Remote_Client_ClientDisconnected(TCPClient client, string message)
        {
            MessageBox.Show("You have been disconnected ! Window will now close.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }
        public void PrintDataReceivedFromServer(string message)
        {
            lblServerData.Text = message;
            remoteClient.onClientConnected(remoteClient);
            remoteClient.Connected = true;
            
        }
        #endregion

        #region Utility functions
        private void PopulateGamePanels()
        {
            gameLevels.Add(panel1);
            gameLevels.Add(panel2);
            gameLevels.Add(panel3);
            gameLevels.Add(panel4);
            gameLevels.Add(panel5);
            gameLevels.Add(panel6);
        }

        private void PopulateGameOvals()
        {
            foreach (Panel panel in gameLevels)
            {
                foreach (ShapeContainer sc in panel.Controls)
                {
                    //Foreach starts with uper index-> Order game ovals to have index equal to selectedOvals 
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>().OrderBy(c => c.Name))
                    {
                        //gameOvals
                        if (gameoval.FillColor == Color.WhiteSmoke)
                        {
                            gameOvals.Add(gameoval);
                        }
                        //scoreOvals
                        /*if(gameoval.FillColor == Color.Gainsboro)
                        {
                            scoreOvals.Add(gameoval);
                        }*/
                    }
                }
            }
            /*gameOvals.AddRange(gameLevels
                       .SelectMany(p => p.Controls.OfType<ShapeContainer>()
                           .SelectMany(sc => sc.Shapes.OfType<OvalShape>())));*/
        }

        private int getClickedOvalIndex()
        {
            int i = 0;
            int resultIndex = 0;
            bool test = false;

            //Avoid getting stuck in loop by checking if an oval has been selected
            if (CountBitArray(selectedOvals) > 0)
            {
                while (test == false)
                {
                    if (selectedOvals.Get(i))
                    {
                        resultIndex = i;
                        test = true;
                    }
                    i++;
                }
            }
            return resultIndex;
        }

        private int CountBitArray(BitArray bitarray)
        {
            int count = 0;
            foreach (bool bit in bitarray)
            {
                if (bit)
                {
                    count++;
                }
            }
            return count;
        }

        private int CheckOvalIndex()
        {
            if (CountBitArray(selectedOvals) == 0)
            {
                MessageBox.Show("Please select a placeholder first.", "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
            return 1;
        }
        #endregion

        #region Game Functions
        //Display point and send to server
        private void DisplayPoints()
        {
            lblPointsTotal.Text = "Points: " + remoteClient.TotalPoints;
            lblBestTime.Text = "Best Time " + bestTime[0] +":"+ bestTime[1] + ":"+ bestTime[2];
            remoteClient.SendPoints(winner);
            winner = false;
        }

        private void InitialiseBoard()
        {
            //Reset global indices (start new game)
            iScoreStart = 0;
            iStop = 5;

            ShowAllGameOvals();
            DisableAllGameOvals();

            //Enable fist row
            EnableNextGameRow(1);
            gameOvals[0].Focus();

            //Timer
            lblTimer.Mode = ISIBToolbox.TimerMode.COUNT_UP;
            lblTimer.StartTimer();


            SetAnswers();
            //RevealAnswers();
        }

        //Disable click usage on all game ovals at start of game
        private void DisableAllGameOvals()
        {
            foreach (Panel panel in gameLevels)
            {
                foreach (ShapeContainer sc in panel.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        if(gameoval.FillColor == Color.WhiteSmoke)
                        {
                            gameoval.Enabled = false;
                        }
                    }
                }
            }
        }

        private bool CheckDuplicates()
        {
            //Remind user that Answer contains no dupplicates of colours if found
            if ((tempC0[0] == tempC1[0] || tempC0[0] == tempC2[0] || tempC0[0] == tempC3[0] || tempC0[0] == tempC4[0]) ||
                (tempC1[0] == tempC2[0] || tempC1[0] == tempC3[0] || tempC1[0] == tempC4[0]) ||
                (tempC2[0] == tempC3[0] || tempC2[0] == tempC4[0]) ||
                (tempC3[0] == tempC4[0]))
            {
                MessageBox.Show("Remember : The answer contains no duplicates of colours.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            return false;
        }

        private bool CheckBlanks()
        {
            //Remind user that Answer contains no dupplicates of colours if found
            if (tempC0[0] == Constants.WhiteSmokeR || tempC1[0] == Constants.WhiteSmokeR || tempC2[0] == Constants.WhiteSmokeR || tempC3[0] == Constants.WhiteSmokeR ||
                tempC3[0] == Constants.WhiteSmokeR || tempC4[0] == Constants.WhiteSmokeR)
            {
                MessageBox.Show("Remember : The answer contains no blanks/White ovals.", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            return false;
        }

        private void ShowAllGameOvals()
        {
            //Display next game levels without default black border -> produces inactive effect
            foreach (Panel panel in gameLevels)
            {
                foreach (ShapeContainer sc in panel.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        if (gameoval.FillColor == Color.WhiteSmoke)
                        {
                            gameoval.BorderColor = Color.FromKnownColor(KnownColor.ActiveBorder);
                        }
                    }
                }
            }
        }

        private void EnableNextGameRow(int i)
        {
            if(i == 1)
            {
                foreach (ShapeContainer sc in panel1.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = true;
                        gameoval.BorderColor = Shape.DefaultBorderColor;
                    }
                }
            }
            if (i == 2)
            {
                foreach (ShapeContainer sc in panel2.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = true;
                        gameoval.BorderColor = Shape.DefaultBorderColor;
                    }
                }
            }
            if (i == 3)
            {
                foreach (ShapeContainer sc in panel3.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = true;
                        gameoval.BorderColor = Shape.DefaultBorderColor;
                    }
                }
            }
            if (i == 4)
            {
                foreach (ShapeContainer sc in panel4.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = true;
                        gameoval.BorderColor = Shape.DefaultBorderColor;
                    }
                }
            }
            if (i == 5)
            {
                foreach (ShapeContainer sc in panel5.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = true;
                        gameoval.BorderColor = Shape.DefaultBorderColor;
                    }
                }
            }
            if (i == 6)
            {
                foreach (ShapeContainer sc in panel6.Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = true;
                        gameoval.BorderColor = Shape.DefaultBorderColor;
                    }
                }
            }

            // Disable previous rows -> avoid cheating from level 2 onwards
            if(currentLevel > 1)
            {
                DisablePreviousGameRows(currentLevel);
            }
        }

        private void DisablePreviousGameRows(int currentlevel)
        {
            int previouslevel = currentlevel - 2;
            for(int i = previouslevel; i >= 0; i--)
            {
                foreach (ShapeContainer sc in gameLevels[i].Controls)
                {
                    foreach (OvalShape gameoval in sc.Shapes.OfType<OvalShape>())
                    {
                        gameoval.Enabled = false;
                    }
                }
            }
        }

        private void SetAnswers()
        {
            Random rand = new Random();

            //so no colors are the same
            int a = rand.Next(1, 8);

            int b = rand.Next(1, 8);
            while (b == a)
                b = rand.Next(1, 8);

            int c = rand.Next(1, 8);
            while (c == a || c == b)
                c = rand.Next(1, 8);

            int d = rand.Next(1, 8);
            while (d == a || d == b || d == c)
                d = rand.Next(1, 8);

            int e = rand.Next(1, 8);
            while (e == a || e == b || e == c || e == d)
                e = rand.Next(1, 8);

            colourA1 = ChooseColor(a);
            colourA2 = ChooseColor(b);
            colourA3 = ChooseColor(c);
            colourA4 = ChooseColor(d);
            colourA5 = ChooseColor(e);
        }

        private void RevealAnswers()
        {
            oAns1.BackgroundImage = null;
            oAns2.BackgroundImage = null;
            oAns3.BackgroundImage = null;
            oAns4.BackgroundImage = null;
            oAns5.BackgroundImage = null;

            oAns1.FillColor = Color.FromArgb(colourA1[0], colourA1[1], colourA1[2]);
            oAns2.FillColor = Color.FromArgb(colourA2[0], colourA2[1], colourA2[2]);
            oAns3.FillColor = Color.FromArgb(colourA3[0], colourA3[1], colourA3[2]);
            oAns4.FillColor = Color.FromArgb(colourA4[0], colourA4[1], colourA4[2]);
            oAns5.FillColor = Color.FromArgb(colourA5[0], colourA5[1], colourA5[2]);

            oAns1.BorderColor = Color.FromArgb(colourA1[0], colourA1[1], colourA1[2]);
            oAns2.BorderColor = Color.FromArgb(colourA2[0], colourA2[1], colourA2[2]);
            oAns3.BorderColor = Color.FromArgb(colourA3[0], colourA3[1], colourA3[2]);
            oAns4.BorderColor = Color.FromArgb(colourA4[0], colourA4[1], colourA5[2]);
            oAns5.BorderColor = Color.FromArgb(colourA5[0], colourA5[1], colourA5[2]);
        }

        //color picker. All colours were chosen so the R values
        //of each colour are never the same
        private int[] ChooseColor(int i)
        {
            int[] colors = new int[3];

            //pink  244; 134; 234
            if (i == 1)
            {
                colors[0] = 244;
                colors[1] = 134;
                colors[2] = 234;
                return colors;
            }
            //green
            else if (i == 2)
            {
                colors[0] = 0;
                colors[1] = 128;
                colors[2] = 0;
                return colors;
            }
            //steelblue
            else if (i == 3)
            {
                colors[0] = 70;
                colors[1] = 130;
                colors[2] = 180;
                return colors;
            }
            //Gold (255,215,0)
            else if (i == 4)
            {
                colors[0] = 255;
                colors[1] = 215;
                colors[2] = 0;
                return colors;
            }
            //Orange 250; 123; 10
            else if (i == 5)
            {
                colors[0] = 250;
                colors[1] = 123;
                colors[2] = 10;
                return colors;
            }
            //Red 219; 13; 29 
            else if (i == 6)
            {
                colors[0] = 219;
                colors[1] = 13;
                colors[2] = 29;
                return colors;
            }
            //DarkOrchid 	(153,50,204)
            else if (i == 7)
            {
                colors[0] = 153;
                colors[1] = 50;
                colors[2] = 204;
                return colors;
            }
            return colors;
        }

        private void ColourOval(int index, int[] colour)
        {
            //Verify oval is not part of scoreboard
            if (gameOvals[index].FillColor != Color.Gray)
            {
                gameOvals[index].FillColor = Color.FromArgb(colour[0], colour[1], colour[2]);
            }

            //Set focus to and enable score button if game row completed
            /*if((index + 1) % 5 == 0 && buttonClickCount > 4)
            {
                buttonScore.Enabled = true;
                buttonScore.Focus();
            }*/
        }

        private void StoreTempColours(int i)
        {
            tempC4[0] = gameOvals[i].FillColor.R;
            tempC4[1] = gameOvals[i].FillColor.G;
            tempC4[2] = gameOvals[i].FillColor.B;

            tempC3[0] = gameOvals[i - 1].FillColor.R;
            tempC3[1] = gameOvals[i - 1].FillColor.G;
            tempC3[2] = gameOvals[i - 1].FillColor.B;

            tempC2[0] = gameOvals[i - 2].FillColor.R;
            tempC2[1] = gameOvals[i - 2].FillColor.G;
            tempC2[2] = gameOvals[i - 2].FillColor.B;

            tempC1[0] = gameOvals[i - 3].FillColor.R;
            tempC1[1] = gameOvals[i - 3].FillColor.G;
            tempC1[2] = gameOvals[i - 3].FillColor.B;

            tempC0[0] = gameOvals[i - 4].FillColor.R;
            tempC0[1] = gameOvals[i - 4].FillColor.G;
            tempC0[2] = gameOvals[i - 4].FillColor.B;

        }

        private void EvaluateScore()
        {
            List<OvalShape> scoreOvals = new List<OvalShape>();

            int blackCount = 0;
            int whiteCount = 0;

            int vFlag1 = 0;
            int vFlag2 = 0;
            int vFlag3 = 0;
            int vFlag4 = 0;
            int vFlag5 = 0;
            int vFlag6 = 0;
            int vFlag7 = 0;
            int vFlag8 = 0;
            int vFlag9 = 0;
            int vFlag10 = 0;

            if(currentLevel >= 2 && currentLevel < Constants.MAX_LEVELS) { iScoreStart += 5; iStop += 5; }

            //Collect all ovals in current level as control objects
            scoreOvals.AddRange(collection: gameLevels
           .SelectMany(p => p.Controls.OfType<ShapeContainer>()
               .SelectMany(sc => sc.Shapes.OfType<OvalShape>().Where(oval => oval.Width is Constants.SCORE_OVAL_WIDTH))));

            //Check answers
            for (int i = iScoreStart; i < iStop; i++)
            {
                //Blackfills
                if (tempC0[0] == colourA1[0] && vFlag1 == 0)
                {
                    //Using i becomes too predictable as 
                    //position in game level = position in score all the time 
                    //Randomise will swap colour fill positions of score pegs 1 and 3
                    scoreOvals[i].FillColor = Color.Black;
                    vFlag1 = 1;
                    blackCount++;
                }
                else if (tempC1[0] == colourA2[0] && vFlag2 == 0)
                {
                    scoreOvals[i].FillColor = Color.Black;
                    vFlag2 = 1;
                    blackCount++;
                }
                else if (tempC2[0] == colourA3[0] && vFlag3 == 0)
                {
                    scoreOvals[i].FillColor = Color.Black;
                    vFlag3 = 1;
                    blackCount++;
                }
                else if (tempC3[0] == colourA4[0] && vFlag4 == 0)
                {
                    scoreOvals[i].FillColor = Color.Black;
                    vFlag4 = 1;
                    blackCount++;
                }
                else if (tempC4[0] == colourA5[0] && vFlag5 == 0)
                {
                    scoreOvals[i].FillColor = Color.Black;
                    vFlag5 = 1;
                    blackCount++;
                }

                //for white fills (right colour but wrong position)
                else if ((tempC0[0] == colourA2[0] || tempC0[0] == colourA3[0] || tempC0[0] == colourA4[0] || tempC0[0] == colourA5[0]) && vFlag6 == 0)
                {
                    scoreOvals[i].FillColor = Color.White;
                    vFlag6 = 1;
                    whiteCount++;
                }
                else if ((tempC1[0] == colourA1[0] || tempC1[0] == colourA3[0] || tempC1[0] == colourA4[0] || tempC1[0] == colourA5[0]) && vFlag7 == 0)
                {
                    scoreOvals[i].FillColor = Color.White;
                    vFlag7 = 1;
                    whiteCount++;
                }
                else if ((tempC2[0] == colourA1[0] || tempC2[0] == colourA2[0] || tempC2[0] == colourA4[0] || tempC2[0] == colourA5[0]) && vFlag8 == 0)
                {
                    scoreOvals[i].FillColor = Color.White;
                    vFlag8 = 1;
                    whiteCount++;
                }
                else if ((tempC3[0] == colourA1[0] || tempC3[0] == colourA2[0] || tempC3[0] == colourA3[0] || tempC3[0] == colourA5[0]) && vFlag9 == 0)
                {
                    scoreOvals[i].FillColor = Color.White;
                    vFlag9 = 1;
                    whiteCount++;
                }
                else if ((tempC4[0] == colourA1[0] || tempC4[0] == colourA2[0] || tempC4[0] == colourA3[0] || tempC4[0] == colourA4[0]) && vFlag10 == 0)
                {
                    scoreOvals[i].FillColor = Color.White;
                    vFlag10 = 1;
                    whiteCount++;
                }

                //Else keep same colour (not white nor black)
                else
                {
                    scoreOvals[i].FillColor = Color.Gray;
                }
                //break;
            }

            CheckWin(blackCount);
            currentLevel++;
            EnableNextGameRow(currentLevel);

        }

        private void CheckWin(int numberOfBlackpins)
        {
            if(numberOfBlackpins == 5)
            {
                RevealAnswers();
                remoteClient.TotalPoints += Constants.WIN_POINTS;
                remoteClient.AddBonus(currentLevel);

                if(MessageBox.Show("We have a winner!", "Congratulations", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    winner = true;
                    NewGame();
                }
                else
                {
                    CloseGame();
                }
            }
        }

        private void HandleLoss()
        {
            RevealAnswers();
            if(MessageBox.Show("Game Over! Would you like to try again ?", "Unlucky", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk) == DialogResult.Retry)
            {
                winner = false;
                NewGame();
            }
            else
            {
                CloseGame();
            }
        }

        private void NewGame()
        {
            currentLevel = 0;
            List<OvalShape> scoreOvals = new List<OvalShape>();

            scoreOvals.AddRange(collection: gameLevels
                            .SelectMany(p => p.Controls.OfType<ShapeContainer>()
                                .SelectMany(sc => sc.Shapes.OfType<OvalShape>().Where(oval => oval.Width is Constants.SCORE_OVAL_WIDTH))));
            //Reset game ovals
            foreach (OvalShape oval in gameOvals)
            {
                oval.FillColor = Color.WhiteSmoke;
            }
            foreach (OvalShape oval in scoreOvals)
            {
                oval.FillColor = Color.Gray;
            }

            //Hide and reset Answer
            oAns1.BackgroundImage = Properties.Resources.a1;
            oAns2.BackgroundImage = Properties.Resources.a1;
            oAns3.BackgroundImage = Properties.Resources.a1;
            oAns4.BackgroundImage = Properties.Resources.a1;
            oAns5.BackgroundImage = Properties.Resources.a1;
            oAns1.BorderColor = Color.Black;
            oAns2.BorderColor = Color.Black;
            oAns3.BorderColor = Color.Black;
            oAns4.BorderColor = Color.Black;

            //Make sure no ovals are selected in new game
            selectedOvals.SetAll(false);
            InitialiseBoard();

            //Check if time is better than last best time recorded
            if ((lblTimer.Hours < bestTime[0] || lblTimer.Minutes < bestTime[1] || lblTimer.Seconds < bestTime[2]) ||
                winner == true)
            {
                bestTime[0] = lblTimer.Hours;
                bestTime[1] = lblTimer.Minutes;
                bestTime[2] = lblTimer.Seconds;
            }

            //Reset timer
            lblTimer.StopTimer();
            lblTimer.Hours = 0;
            lblTimer.Minutes = 0;
            lblTimer.Seconds = 0;
            lblTimer.StartTimer();

            //winner = false;

            //Launch gameover Event to calculate client points
            Invoke(GO); 
        }

        private void CloseGame()
        {
            //winner = false;
            currentLevel = 0;
            //Reset timer
            lblTimer.Hours = 0;
            lblTimer.Minutes = 0;
            lblTimer.Seconds = 0;
            lblTimer.StopTimer();
            this.Close();
        }
        #endregion

        #region Colour Clicks Events

        private void oColour1_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            //Colour ovals only if game oval has been selected first
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(1);
                ColourOval(clickedOvalIndex, colour);
            }
        }

        private void oColour2_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(2);
                ColourOval(clickedOvalIndex, colour);
            }
        }

        private void oColour3_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(3);
                ColourOval(clickedOvalIndex, colour);
            }
        }

        private void oColour4_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(4);
                ColourOval(clickedOvalIndex, colour);
            }
        }

        private void oColour5_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(5);
                ColourOval(clickedOvalIndex, colour);
            }
        }

        private void oColour6_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(6);
                ColourOval(clickedOvalIndex, colour);
            }
        }

        private void oColour7_Click(object sender, EventArgs e)
        {
            int[] colour;
            int clickedOvalIndex = getClickedOvalIndex();
            if (CheckOvalIndex() == 1)
            {
                colour = ChooseColor(7);
                ColourOval(clickedOvalIndex, colour);
            }
        }
        #endregion

        #region Oval Clicks Events
        private void oval0_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(0, true);
        }
        private void oval1_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(1, true);
        }

        private void oval2_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(2, true);
        }

        private void oval3_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(3, true);
        }

        private void oval4_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(4, true);
        }

        private void oval5_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(5, true);
        }

        private void oval6_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(6, true);
        }

        private void oval7_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(7, true);
        }

        private void oval8_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(8, true);
        }

        private void oval9_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(9, true);
        }

        private void oval10_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(10, true);
        }

        private void oval11_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(11, true);
        }

        private void oval12_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(12, true);
        }

        private void oval13_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(13, true);
        }

        private void oval14_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(14, true);
        }

        private void oval15_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(15, true);
        }

        private void oval16_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(16, true);
        }

        private void oval17_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(17, true);
        }

        private void oval18_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(18, true);
        }

        private void oval19_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(19, true);
        }

        private void oval20_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(20, true);
        }

        private void oval21_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(21, true);
        }

        private void oval22_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(22, true);
        }

        private void oval23_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(23, true);
        }

        private void oval24_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(24, true);
        }

        private void oval25_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(25, true);
        }

        private void oval26_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(26, true);
        }

        private void oval27_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(27, true);
        }

        private void oval28_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(28, true);
        }

        private void oval29_Click(object sender, EventArgs e)
        {
            selectedOvals.SetAll(false);            //Make sure only one oval can be changed at a time
            selectedOvals.Set(29, true);
        }

        #endregion

        #region Button Clicks Events
        private void buttonScore_Click(object sender, EventArgs e)
        {
            int ilastOvalInRow;

            switch (currentLevel)
            {
                case 2:
                    ilastOvalInRow = 9;
                    break;
                case 3:
                    ilastOvalInRow = 14;
                    break;
                case 4:
                    ilastOvalInRow = 19;
                    break;
                case 5:
                    ilastOvalInRow = 24;
                    break;
                case 6:
                    ilastOvalInRow = 29;
                    break;
                default:
                    //Default to First level
                    ilastOvalInRow = 4;
                    break;
            }
            StoreTempColours(ilastOvalInRow);
            CheckDuplicates();
            if (!CheckBlanks())
            {
                EvaluateScore();

                if (currentLevel >= Constants.MAX_LEVELS)
                {
                    //Loser case
                    HandleLoss();
                }
            }
        }
        #endregion

        #region Extra Form Events
        private void MasterMindClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            remoteClient.ClientDisconnected -= Remote_Client_ClientDisconnected;
            remoteClient.Disconnect();

        }

        /*private void MasterMindClient_MouseClick(object sender, MouseEventArgs e)
        {
            //Check if ACK has been received from server, if not close client
            if (!lblServerData.Visible)
            {
                remoteClient.onClientConnected(remoteClient);
            }
            else
            {
                MessageBox.Show("Connection error: Number of players has been reached", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }*/
        #endregion

    }
}
