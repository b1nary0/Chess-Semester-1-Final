using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPODevChess
{
    /* GAMEMANAGER HANDLES ALL THE FRONT-END PROCESSING SUCH AS GAME
     * LOOP AND INTERFACE */

    public class GameManager
    {
        #region Fields

        ChessBoard gb;
        Player playerOne;
        Player playerTwo;

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public GameManager()
        {
            Console.WindowHeight = 45;
            Console.Title = "Chess - JPODev Final";
            gb = new ChessBoard();
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            /* INITIALIZE SETS UP PLAYER COLOURS AND PLAYER NAMES.
             * INITIALIZE ALSO SETS UP THE COLOUR SCHEME AS PER USER 
             * REQUEST */

            playerOne = new Player("white");
            playerTwo = new Player("black");

            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n                            ");
            Console.Write("Select a colour theme:\n");
            Console.Write("                            [1] - Default\n");
            Console.Write("                            [2] - Christmas!\n");
            Console.Write("                            -> ");
            int colourChoice = int.Parse(Console.ReadLine());

            Console.Clear();
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n                 ");
            Console.Write("Player 1 (white/red) enter a name -> ");
            playerOne.GetName();
            Console.Clear();
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n                 ");
            Console.Write("Player 2 (black/green) enter a name -> ");
            playerTwo.GetName();


            switch (colourChoice)
            {
                case 1:
                    gb.Initialize("default");
                    break;

                case 2:
                    gb.Initialize("christmas");
                    break;

                default:
                    gb.Initialize("default");
                    break;
            }
        }

        public void Run()
        {
            /* RUN IS IS THE GAME LOOP. RUNS UNTIL THE WINDOW IS CLOSED */
            Initialize();

            while (true)
            {
                if (playerOne.IsTurn)
                {
                    Interface();
                    gb.Draw();

                    while (!playerOne.Play(gb))
                    {
                        Interface();
                        gb.Draw();
                    }

                    playerTwo.IsTurn = true;
                }
                else if (playerTwo.IsTurn)
                {
                    Interface();
                    gb.Draw();

                    while (!playerTwo.Play(gb))
                    {
                        Interface();
                        gb.Draw();
                    }

                    playerOne.IsTurn = true;
                }
            }
        }

        public void Interface()
        {
            /* INTERFACE DISPLAYS ALL THE NECESSARY INFORMATION SUCH AS 
             * THE RELATIONSHIP OF COLOUR TO PLAYER, WHAT PIECES THE PLAYER HAS
             * TAKEN, AND WHAT THE LAST MOVE WAS */

            Console.Clear();
            Console.Write(playerOne.Name + ": " + playerOne.Colour + "     ");

            Console.Write("Last Move: " + gb.LastMove);

            Console.Write("       " + playerOne.Name + " has taken: ");

            for (int i = 0; i < gb.P1Takes.Count; ++i)
            {
                Console.Write(gb.P1Takes[i].Identifier + " ");
            }

            Console.Write("\n");

            Console.Write(playerTwo.Name + ": " + playerTwo.Colour + "     ");

            Console.Write("                " + playerTwo.Name + " has taken: ");

            for (int i = 0; i < gb.P2Takes.Count; ++i)
            {
                Console.Write(gb.P2Takes[i].Identifier + " ");
            }

            Console.Write("\n");
        }

        #endregion
    }
}
