using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPODevChess
{
    public class Player
    {
        #region Fields

        private string name;
        private bool currentTurn;
        private string colour;

        #endregion

        #region Properties

        public bool IsTurn
        {
            get { return currentTurn; }
            set { currentTurn = value; }
        }

        public string Colour
        {
            get { return colour; }
        }

        public string Name
        {
            get { return name; }
        }

        #endregion

        #region Constructors

        public Player()
        {
        }

        public Player(string colour)
        {
            if (colour == "white")
            {
                currentTurn = true;
                this.colour = "Red";
            }
            else if (colour == "black")
            {
                currentTurn = false;
                this.colour = "Green";
            }
        }

        #endregion

        #region Methods

        public bool Play(ChessBoard gb)
        {
            /* PLAY TAKES CARE OF ALL NECESSARY INFORMATION NEEDED TO MOVE PIECES. TAKES THE 
             * PLAYERS CHOSEN FROM AND TO COORDINATES AND ENSURES THEY ARE THE PROPER COLOUR
             * TO MOVE THE PIECE BEFORE SENDING THE INFORMATION TO THE PIECE CLASS FOR LOGIC 
             * PROCESSING. THE BOARD IS ZERO-BASED AT ARRAY LEVEL, BUT FOR USER FAMILIARITY,
             * ALL MOVEMENTS ARE INPUT FROM 1 - 8. THEREFORE, THESE VALUES NEED TO BE DECREASED
             * BEFORE BEING USED TO ACCESS NODEARRAY. */

            Console.Write(this.Name + " from X: ");

            char fromXc;

            if (!char.TryParse(Console.ReadLine(), out fromXc))
            {
                return false;
            }

            Console.Write(this.Name + " from Y: ");

            int fromY;

            if (!int.TryParse(Console.ReadLine(), out fromY))
            {
                return false;
            }

            Console.Write(this.Name + " to X: ");

            char toXc;
            if (!char.TryParse(Console.ReadLine(), out toXc))
            {
                return false;
            }

            Console.Write(this.Name + " to Y: ");

            int toY;
            if (!int.TryParse(Console.ReadLine(), out toY))
            {
                return false;
            }

            int fromX = CharToInt(fromXc);
            int toX = CharToInt(toXc);

            // ENSURE NOT OUT OF BOUNDS
            if (fromX < 1)
                fromX = 1;

            if (fromY < 1)
                fromY = 1;

            if (fromY > 8)
                fromY = 8;

            if (fromX > 8)
                fromX = 8;


            gb.SetLastMove(fromXc, toXc, fromY, toY);

            fromX -= 1;
            fromY -= 1;

            // IF PLAYER IS RED (WHITE) AND THE PIECE THEY WANT TO MOVE IS CONTAINED IN THE 
            // WHITE PIECES ARRAY, MOVE.
            if (this.colour == "Red" &&
                gb.WhitePieces.Contains<Piece>(gb.NodeArray[fromY, fromX].CurrentPiece as Piece))
            {
                if (!gb.NodeArray[fromY, fromX].CurrentPiece.Move(gb, this, toX, toY))
                    return false;

                currentTurn = false;

                return true;
            }
            // SAME CHECK AS ABOVE BUT FOR BLACK.
            else if (this.colour == "Green" &&
                     gb.BlackPieces.Contains<Piece>(gb.NodeArray[fromY, fromX].CurrentPiece as Piece))
            {
                if (!gb.NodeArray[fromY, fromX].CurrentPiece.Move(gb, this, toX, toY))
                    return false;

                currentTurn = false;

                return true;
            }

            return false;
        }

        public int CharToInt(char c)
        {
            // THIS IS A HOMEMADE FUNCTION TO CONVERT THE ALPHA-BASED X COORDS TO A NUMBER THAT
            // CAN BE USED IN AN ARRAY INDEX. //

            // SAFEGUARD CASE OF LETTER. IF LOWER, CHANGE TO UPPER
            switch (c)
            {
                case 'a':
                    c = 'A';
                    break;

                case 'b':
                    c = 'B';
                    break;

                case 'c':
                    c = 'C';
                    break;

                case 'd':
                    c = 'D';
                    break;

                case 'e':
                    c = 'E';
                    break;

                case 'f':
                    c = 'F';
                    break;

                case 'g':
                    c = 'G';
                    break;

                case 'h':
                    c = 'H';
                    break;
            }

            int j = 1;

            // ITERATE THROUGH CHARACTERS A - H. IF c IS EQUAL TO THE VALUE HELD IN i
            // THEN RETURN j, WHICH IS KEEPING TRACK OF THE COORESPONDING INTEGER VALUE FROM
            // 1-8. THIS IS NECESSARY BECAUSE CHARACTER VALUES ARE DIFFERENT THAN THE VALUES WE SEEK.

            for (char i = 'A'; i <= 'H'; ++i, ++j)
            {
                if (c == i)
                {
                    return j;
                }
            }

            return 0;
        }

        public void GetName()
        {
            name = Console.ReadLine();
        }

        #endregion
    }
}
