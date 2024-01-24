using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Platformer
{
    public partial class Form1 : Form
    {
        // create boolean values for each in game mechanic

        // movement
        bool movLeft, movRight, jumping, isEnd, onPlatform;
        //
        // physics & mechanics
        int jumpingSpeed;
        int force;
        int playerScore = 0;
        int playerMovSpeed = 7;
        int verticalSpeed = 3;
        int horizontalSpeed = 5;
        int obstacleSpeed1 = 2;
        int obstacleSpeed2 = 5;
        int obstacleSpeed3 = 3;
        int mobilePlatSpeed = 3;
        int slidingDoorSpeed = 2;

        public Form1()
        {
            InitializeComponent();
        }
        // function to reset game parameters for new game
        private void ResetGame()
        {
            jumping = false;
            
            movLeft = false;
            movRight = false;
            isEnd = false;

            playerScore = 0;
            PlayerScoreLabel.Text = "0";

            // foreach loop to identify all coins and revert them to the visible state
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false || x.Tag == "coinUsed")
                {
                    x.Visible = true;
                    x.Tag = "coin";
                    x.Enabled = true;

                }
            }

            //reset players
            PlayerSprite1.Left = 7;
            PlayerSprite1.Top = 807;
            //reset obstacles
            obstacle1.Left = 255;
            obstacle1.Top = 763;
            obstacle2.Left = 130;
            obstacle2.Top = 616;
            obstacle3.Left = 212;
            obstacle3.Top = 297;
            //reset platforms
            movingPlatform1.Top = 550;
            slidingDoor.Top = 65;

            //intiate the timer
            gameTimer.Start();
        }

        private void EndMessage(int i)
        {
            // function to display a message box pertaining to the situation
            if (i == 0)
            {
                // replay if died
                gameTimer.Stop();
                string message = "Do you want to replay?";
                string title = "You Died!";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    ResetGame();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            if (i == 1)
            {
                // replay if completed
                gameTimer.Stop();
                string message = "Do you want to replay?";
                string title = "Level Complete :)";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    ResetGame();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            if (i == 2)
            {
                // replay if hit
                gameTimer.Stop();
                string message = "Do you want to replay?";
                string title = "You touched an enemy :(";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    ResetGame();
                }
                else
                {
                    Environment.Exit(0);
                }

            }
            
        }

        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            PlayerScoreLabel.Text = playerScore.ToString();

            // listener for player movement - left or right by updating coordinates of player sprite
            PlayerSprite1.Top += jumpingSpeed;
            if (movLeft == true)
            {
                PlayerSprite1.Left -= playerMovSpeed;
            }
            if (movRight == true)
            {
                PlayerSprite1.Left += playerMovSpeed;
            }
            if (jumping == true && force < 0)
            {
                jumping = false;
                onPlatform = true;
            }
            if (jumping == true)
            {
                jumpingSpeed = -8;
                force -= 1;
                onPlatform = false;
            }
            else
            {
                jumpingSpeed = 10;
            }

            // obstacle 1 movement
            obstacle1.Left += obstacleSpeed1;
            if (obstacle1.Left > pictureBox2.Right - obstacle1.Width || obstacle1.Left < pictureBox2.Left)
            {
                obstacleSpeed1 = -obstacleSpeed1;
            }
            // obstacle 2 movement
            obstacle2.Left += obstacleSpeed2;
            if (obstacle2.Left > pictureBox5.Right - obstacle2.Width || obstacle2.Left < pictureBox5.Left)
            {
                obstacleSpeed2 = -obstacleSpeed2;
            }
            // obstacle 3 movement
            obstacle3.Left += obstacleSpeed3;
            if (obstacle3.Left > pictureBox12.Right - obstacle3.Width || obstacle3.Left < pictureBox12.Left)
            {
                obstacleSpeed3 = -obstacleSpeed3;
            }
            // platform movement
            movingPlatform1.Top -= mobilePlatSpeed;
            if (movingPlatform1.Top > pictureBox9.Bottom - movingPlatform1.Height || movingPlatform1.Top < pictureBox9.Top)
            {
                mobilePlatSpeed = -mobilePlatSpeed;
            }
            // sliding door movement
            slidingDoor.Top  -= slidingDoorSpeed;
            if (slidingDoor.Bottom < pictureBox8.Top - slidingDoor.Height || slidingDoor.Bottom > pictureBox8.Top)
            {
                slidingDoorSpeed = -slidingDoorSpeed;
            }


            foreach (Control x in this.Controls)
            {
                // summary
                // this if statement is used to scan all components in the form, where x represents all PictureBox items. 
                // using this if statement allows the handling of interaction events between the playerSprite and all PictureBoxes.

                if (x is PictureBox)
                {
                    // check if out of bounds
                    if (PlayerSprite1.Top > 1883)
                    {
                        EndMessage(0);
                    }
                    // check if game completed
                    if ((string)x.Tag == "FinishPoint")
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && playerScore == 380)
                        {
                            EndMessage(1);
                        }
                    // check if player hit enemy
                    if ((string)x.Tag == "enemy")
                    {
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds))
                        {
                            EndMessage(2);
                        }
                    }
                    // check if coin has been picked up
                    if ((string)x.Tag == "coin")
                    {
                        if(PlayerSprite1.Bounds.IntersectsWith(x.Bounds))
                        {
                            x.Tag = "coinUsed";
                            x.Hide();

                            playerScore += 10;
                        }
                    }
                    // block of code to make walls inpenetrable
                    if ((string)x.Tag == "wall")
                    {
                        if(PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && movRight == true && PlayerSprite1.Top > x.Top)
                        {
                            PlayerSprite1.Left = x.Left - PlayerSprite1.Width;
                        }
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && movLeft == true && PlayerSprite1.Top > x.Top)
                        {
                            PlayerSprite1.Left = x.Left + PlayerSprite1.Width;
                        }
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && jumping == true && PlayerSprite1.Top < x.Bottom)
                        {
                            PlayerSprite1.Top = x.Bottom + PlayerSprite1.Height;
                        }
                    }
                    // block of code to allow player to stand on platforms.
                    if ((string)x.Tag == "platform")
                    {
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && onPlatform == false && PlayerSprite1.Top > x.Top)
                        {
                            PlayerSprite1.Top = x.Bottom + PlayerSprite1.Height;
                        }
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && movLeft == true && PlayerSprite1.Top > x.Top)
                        {
                            PlayerSprite1.Left = x.Left + PlayerSprite1.Width;
                        }
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds) && movRight == true && PlayerSprite1.Top > x.Top)
                        {
                            PlayerSprite1.Left = x.Left - PlayerSprite1.Width;
                        }
                        if (PlayerSprite1.Bounds.IntersectsWith(x.Bounds))
                        {
                            force = 7;
                            PlayerSprite1.Top = x.Top - PlayerSprite1.Height;
                            if ((string)x.Name == "horizontalPlatform" && movLeft == false || (string)x.Name == "horizontalPlatform" && movRight == false)
                            {
                                PlayerSprite1.Left -= horizontalSpeed;
                            }
                        }
                        x.BringToFront();
                    }
                }
            }

        }

        // function to identify keypresses and trigger events
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                movLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                movRight = true;
            }
            if (e.KeyCode == Keys.Up && jumping != true)
            {
                jumping = true;
            }
        }
        // function to identify when key has been released and stop events
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                movLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                movRight = false;
            }
            if (e.KeyCode == Keys.Up && jumping == true)
            {
                jumping = false;
            }
            // enter key to reset game on game over
            if (e.KeyCode == Keys.Enter && isEnd == true) { ResetGame(); }
        }
    }
}