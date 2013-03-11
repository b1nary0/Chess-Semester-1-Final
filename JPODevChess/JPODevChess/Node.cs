using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPODevChess
{
    /// <summary>
    /// EVERYTHING IS A NODE. A NODE IS SIMPLY AN OBJECT THAT CONTAINS A 'SLOT' FOR
    /// PIECES AND EMPTY NODES. NODE ALSO CONTAINS THE BASE METHOD FOR THE VIRTUAL METHOD 
    /// MOVE
    /// </summary>

    public class Node
    {
        #region Fields

        private Node currentPiece;
        protected char identifier;
        private string colour;

        protected int x;
        protected int y;

        #endregion

        #region Properties

        public Node CurrentPiece
        {
            get { return currentPiece; }
            set { currentPiece = value; }
        }

        public char Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        public String Colour
        {
            get { return colour; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        #endregion

        #region Constructors

        public Node()
        {
        }

        public Node(string squareColour)
        {
            colour = squareColour;
        }

        public Node(string squareColour, Node piece)
        {
            colour = squareColour;
            currentPiece = piece;
        }

        #endregion

        #region Methods

        public virtual bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            // MOVE PROCESSES THE MOVEMENTS OF PIECES. RETURNS TRUE FOR SUCCESSFUL OPERATIONS //
            // AND FALSE FOR UNSUCCESSFUL ONES //

            if (dX > 0)
                dX -= 1;

            if (dY > 0)
                dY -= 1;

            // create node objects to hold the current pieces from both sets of coordinates //
            Node piece = gb.NodeArray[this.Y, this.X].CurrentPiece;
            Node dest = gb.NodeArray[dY, dX].CurrentPiece;

            // swap the pieces //
            gb.NodeArray[dY, dX].CurrentPiece = piece;
            gb.NodeArray[this.Y, this.X].CurrentPiece = dest;

            // set the nodes x and y position to the new values //
            this.X = dX;
            this.Y = dY;

            return true;
        }

        #endregion
    }

    public class EmptyNode : Node
    {
        // EMPTYNODES DO NOTHING. THEY ARE JUST PLACEHOLDER OBJECTS //
        public EmptyNode()
        {
            identifier = ' ';
        }
    }
}
