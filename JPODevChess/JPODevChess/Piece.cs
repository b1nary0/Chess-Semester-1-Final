using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPODevChess
{
    // THE PIECE CLASS IS THE LARGEST CLASS. EACH PIECE IS A NODE, FOR USE OF POLYMORPHISM.
    // THE PIECE CLASS HANDLES ALL MOVE LOGIC AND CHECKS VALID MOVES AND MOVE PATTERNS.

    public class Piece : Node
    {
        #region Fields

        protected string movePattern;
        protected string colour;

        #endregion

        #region Properties

        public char Identifier
        {
            get { return identifier; }
        }

        #endregion

        #region Constructors

        public Piece()
        {
        }

        public Piece(string colour)
        {
            this.colour = colour;
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            // This portion of move just passes control to the base and returns true //
            base.Move(gb, player, dX, dY);

            return true;
        }

        public void Take(ChessBoard gb, Piece piece, int player)
        {
            // Take just adds the piece being taken into the appropriate container //
            if (player == 1)
            {
                gb.P1Takes.Add(piece);
            }
            else if (player == 2)
            {
                gb.P2Takes.Add(piece);
            }
        }

        public bool IsWhite(ChessBoard gb)
        {
            // A simple function that checks if the current piece object is contained //
            // in the whitePieces array within the ChessBoard. Returns true if it is //
            // a white piece, false if it is a black piece. //

            if (gb.WhitePieces.Contains(this))
            {
                return true;
            }
            else
                return false;
        }

        public virtual bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            /* This base version of ScanForPiece() is the last called. It checks to see if 
             * the piece encountered is a piece of the other colour. If it is, it takes the
             * piece. Returns true for a failed operation, and false for a successful one */

            // If the current object in use is a Pawn, a separate branch of logic is needed //
            if (this is Pawn)
            {
                // If the pawn is trying to move forward, it cannot do anything with the piece //
                // ahead of it. //
                if (destX == this.X)
                {
                    if (gb.NodeArray[destY, destX].CurrentPiece is Piece)
                    {
                        // So return //
                        return true;
                    }
                }

                // Otherwise, if the pawn is trying to move diagonally, it can only do this //
                // if it can take the piece at the destination. //
                if (destX > this.X || destX < this.X)
                {
                    if (gb.NodeArray[destY, destX].CurrentPiece is Piece)
                    {
                        // If the piece at the destination coordinates is a white piece, and the //
                        // player colour is green (black), allow. //
                        if (gb.WhitePieces.Contains<Piece>(gb.NodeArray[destY, destX].CurrentPiece as Piece)
                            && player.Colour == "Green")
                        {
                            // Ensure the pawn is moving in the right direction according to piece //
                            // colour orientation //
                            if (destY < this.Y)
                            {
                                // Take the piece //
                                Take(gb, gb.NodeArray[destY, destX].CurrentPiece as Piece, 2);
                                // Replace it with a new EmptyNode
                                gb.NodeArray[destY, destX].CurrentPiece = new EmptyNode();
                                return false;
                            }
                            else
                                return true;
                        }
                        // If the piece at the destination coords is a black piece and the player //
                        // colour is red (white), allow. //
                        else if (gb.BlackPieces.Contains<Piece>(gb.NodeArray[destY, destX].CurrentPiece as Piece)
                                && player.Colour == "Red")
                        {
                            if (destY > this.Y)
                            {
                                Take(gb, gb.NodeArray[destY, destX].CurrentPiece as Piece, 1);
                                gb.NodeArray[destY, destX].CurrentPiece = new EmptyNode();
                                return false;
                            }
                            else
                                return true;
                        }
                    }
                    else
                        return true;
                }
                else
                    return false;
            }

            // Do the same thing but without the pawn rule check: //
            if (gb.WhitePieces.Contains<Piece>(gb.NodeArray[destY, destX].CurrentPiece as Piece)
                && player.Colour == "Green")
            {
                Take(gb, gb.NodeArray[destY, destX].CurrentPiece as Piece, 2);
                gb.NodeArray[destY, destX].CurrentPiece = new EmptyNode();
                return false;
            }
            else if (gb.BlackPieces.Contains<Piece>(gb.NodeArray[destY, destX].CurrentPiece as Piece)
                && player.Colour == "Red")
            {
                Take(gb, gb.NodeArray[destY, destX].CurrentPiece as Piece, 1);
                gb.NodeArray[destY, destX].CurrentPiece = new EmptyNode();
                return false;
            }

            return true;
        }

        #endregion
    }

    public class Pawn : Piece
    {
        #region Fields

        const int VALUE = 10;
        const int MAX_MOVE = 1;

        bool firstMove = true;

        #endregion

        #region Properties
        #endregion

        #region Constructors

        public Pawn(string colour)
            : base(colour)
        {
            this.colour = colour;
            movePattern = "FAD";
            identifier = 'P';
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            // Move is overidden for the rules of the Pawn piece //

            // Guard against improper move. If the pawn is trying to move diagonally //
            // and there is no piece, return false //
            if ((dX > (this.X + 1) || dX < (this.X + 1))
                  && !(gb.NodeArray[dY - 1, dX - 1].CurrentPiece is Piece))
                return false;

            // If ScanForPiece returns false, no piece is in the way and moves can happen //
            if (!ScanForPiece(gb, player, dX, dY))
            {
                // Depending on the colour of the piece being moved, different operations //
                // are needed //
                if (IsWhite(gb))
                {
                    // If the pawn is on the first move, it's allowed to move 2 squares at a time. //
                    if (firstMove)
                    {
                        if (dY - (this.Y + 1) > 0 && dY - (this.Y + 1) <= 2)
                        {
                            firstMove = false;
                            return base.Move(gb, player, dX, dY);
                        }
                        else
                            return false;
                    }
                    else
                    {
                        // Otherwise the pawn is only allowed to move one //
                        if (dY - (this.Y + 1) == 1)
                            return base.Move(gb, player, dX, dY);
                        else
                            return false;
                    }
                }
                else
                {
                    if (firstMove)
                    {
                        if ((this.Y + 1) - dY > 0 && (this.Y + 1) - dY <= 2)
                        {
                            firstMove = false;
                            return base.Move(gb, player, dX, dY);
                        }
                        else
                            return false;
                    }
                    else
                    {
                        if ((this.Y + 1) - dY == 1)
                            return base.Move(gb, player, dX, dY);
                        else
                            return false;
                    }
                }
            }
            else
                return false;
        }

        public override bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            // Overidden version of ScanForPiece // 
            destX -= 1;
            destY -= 1;

            // If the pawn is trying to move diagonally, call the base ScanForPiece to check //
            // if it is a piece that can be taken. Base ScanForPiece ultimately returns true //
            // unless otherwise stated //
            if (destX > this.X || destX < this.X)
            {
                return base.ScanForPiece(gb, player, destX, destY);
            }

            // If moving forward and there is a piece in the way, return the base //
            // ScanForPiece, which will return true and not allow a move //
            if (gb.NodeArray[destY, this.X].CurrentPiece is Piece)
            {
                return base.ScanForPiece(gb, player, destX, destY);
            }
            else
                return false;
        }

        #endregion
    }

    public class Rook : Piece
    {
        #region Constructors

        public Rook(string colour)
            : base(colour)
        {
            movePattern = "cross";
            identifier = 'R';
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            // If ScanForPiece returns false //
            if (!ScanForPiece(gb, player, dX, dY))
            {
                // Allow a move //
                if (dX == (this.X + 1) && dY > 0)
                    return base.Move(gb, player, dX, dY);
                else if (dX > 0 && dY == (this.Y + 1))
                    return base.Move(gb, player, dX, dY);
                else
                    return false;
            }
            else
                return false;
        }

        public override bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            // Rook ScanForPiece checks all vertical and horizontal movements along the destination //
            // axis' //

            destX -= 1;
            destY -= 1;

            // If destX is the same as the current x position, the rook is moving vertically //
            if (destX == this.X)
            {
                if ((this.Y + 1) == 8)
                {
                    // Check vertical up
                    for (int i = this.Y - 1; i >= destY; --i)
                    {
                        if (gb.NodeArray[i, this.X].CurrentPiece is Piece)
                        {
                            return base.ScanForPiece(gb, player, destX, destY);
                        }
                        else
                            continue; // If a piece hasn't been found, move to next iteration //
                    }
                }
                else
                {
                    // Check vertical down
                    for (int i = (this.Y + 1); i <= destY; ++i)
                    {
                        if (gb.NodeArray[i, this.X].CurrentPiece is Piece)
                        {
                            return base.ScanForPiece(gb, player, destX, destY);
                        }
                        else
                            continue;
                    }
                }
            }
            else if (destY == this.Y)
            {
                if ((this.X + 1) == 8)
                {
                    // Check horizontal left
                    for (int i = (this.X - 1); i >= destX; --i)
                    {
                        if (gb.NodeArray[this.Y, i].CurrentPiece is Piece)
                        {
                            return base.ScanForPiece(gb, player, destX, destY);
                        }
                        else
                            continue;
                    }
                }
                else
                {
                    // Check horizontal right
                    for (int i = (this.X + 1); i <= destX; ++i)
                    {
                        if (gb.NodeArray[this.Y, i].CurrentPiece is Piece)
                        {
                            return base.ScanForPiece(gb, player, destX, destY);
                        }
                        else
                            continue;
                    }
                }
            }

            return false;
        }

        #endregion
    }

    public class Bishop : Piece
    {
        #region Constructors

        public Bishop(string colour)
            : base(colour)
        {
            movePattern = "diagonal";
            identifier = 'B';
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            if (!ScanForPiece(gb, player, dX, dY))
            {
                // Check to see if the destination coords are diagonal movements //
                if ((dX - (this.X + 1) == dY - (this.Y + 1) ||
                     (this.X + 1) - dX == dY - (this.Y + 1)))
                {
                    return base.Move(gb, player, dX, dY);
                }
                else
                    return false;
            }
            else
                return false;
        }

        public override bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            // ScanForPiece for Bishop checks diagonal lines for pieces //

            destX -= 1;
            destY -= 1;

            // If moving diagonally right and down
            if (destX > this.X && destY > this.Y)
            {
                // Iterate through the squares diagonally to find a piece.
                for (int i = (this.Y + 1), j = (this.X + 1); (i <= destY || j <= destX); ++i, ++j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                    {
                        return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
            }
            // If moving left and up //
            else if (destX < this.X && destY < this.Y)
            {
                for (int i = (this.Y - 1), j = (this.X - 1); (i >= destY || j >= destX); --i, --j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                    {
                        return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
            }
            // If moving right and up //
            else if (destX > this.X && destY < this.Y)
            {
                for (int i = (this.Y - 1), j = (this.X + 1); (i >= destY || j <= destX); --i, ++j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                        return base.ScanForPiece(gb, player, destX, destY);
                }
            }
            // If moving left and down //
            else if (destX < this.X && destY > this.Y)
            {
                for (int i = (this.Y + 1), j = (this.X - 1); (i <= destY || j >= destX); ++i, --j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                        return base.ScanForPiece(gb, player, destX, destY);
                }
            }

            return false;
        }

        #endregion
    }

    public class Knight : Piece
    {
        #region Fields

        const int MAX_MOVE = 4;

        #endregion

        #region Constructors

        public Knight(string colour)
            : base(colour)
        {
            movePattern = "L";
            identifier = 'H';
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            if (!ScanForPiece(gb, player, dX, dY))
            {
                // Check for valid move coordinates for horse. 2 down, one across //
                if ((dY - (this.Y + 1) == 2 || (this.Y + 1) - dY == 2) &&
                    (dX - (this.X + 1) == 1 || (this.X + 1) - dX == 1))
                {
                    return base.Move(gb, player, dX, dY);
                }
                // Check for 2 across, one down //
                else if ((dX - (this.X + 1) == 2 || (this.X + 1) - dX == 2) &&
                         (dY - (this.Y + 1) == 1 || (this.Y + 1) - dY == 1))
                {
                    return base.Move(gb, player, dX, dY);
                }

                return false;
            }
            else
                return false;
        }

        public override bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            // Override for Horse is easy. If the destination coords contains a piece //
            // pass control to the base method and determine if it can be taken //
            destX -= 1;
            destY -= 1;

            if (gb.NodeArray[destY, destX].CurrentPiece is Piece)
                return base.ScanForPiece(gb, player, destX, destY);
            else
                return false;
        }

        #endregion
    }

    public class King : Piece
    {
        #region Constructors

        public King(string colour)
            : base(colour)
        {
            movePattern = "free";
            identifier = 'K';
            this.colour = colour;
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            if (!Check(gb, dX, dY))
            {
                if (!ScanForPiece(gb, player, dX, dY))
                {
                    // Check for legal moves. One square in either direction //
                    if ((dX - (this.X + 1) <= 1 && dX - (this.X + 1) > 0) ||
                            ((this.X + 1) - dX <= 1 && (this.X + 1) - dX > 0))
                        return base.Move(gb, player, dX, dY);
                    else if ((dY - (this.Y + 1) <= 1 && dY - (this.Y + 1) > 0) ||
                                ((this.Y + 1) - dY <= 1 && (this.Y + 1) - dY > 0))
                        return base.Move(gb, player, dX, dY);
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public override bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            // Override for King checks destination square for a piece and passes control //
            // to the base method to determine if it can be taken //
            destX -= 1;
            destY -= 1;

            if (gb.NodeArray[destY, destX].CurrentPiece is Piece)
                return base.ScanForPiece(gb, player, destX, destY);
            else
                return false;
        }

        public bool Check(ChessBoard gb, int dX, int dY)
        {
            dX -= 1;
            dY -= 1;

            //Check Down
            for (int i = dY; i < gb.Board_Width; ++i)
            {
                if (gb.NodeArray[i, dX].CurrentPiece is Piece)
                {
                    // Save the piece into a variable, mostly to keep the code easily readable.
                    Piece p = gb.NodeArray[i, dX].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (!(p is Rook) || !(p is Queen))
                    {
                        break;
                    }
                    else if (p is Rook || p is Queen)
                    {
                        if (gb.WhitePieces.Contains<Piece>(this))
                        {
                            if (gb.BlackPieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                        }
                        else if (gb.BlackPieces.Contains<Piece>(this))
                        {
                            if (gb.WhitePieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Check Up
            for (int i = dY; i >= 0; --i)
            {
                if (gb.NodeArray[i, dX].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[i, dX].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (!(p is Rook) || !(p is Queen))
                    {
                        break;
                    }
                    else if (p is Rook || p is Queen)
                    {
                        if (gb.BlackPieces.Contains<Piece>(this))
                        {
                            if (gb.WhitePieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                            else
                                break;
                        }
                        else if (gb.WhitePieces.Contains<Piece>(this))
                        {
                            if (gb.BlackPieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                            else
                                break;
                        }
                    }
                }
            }

            // Check left
            for (int i = dX; i >= 0; --i)
            {
                if (gb.NodeArray[dY, i].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[dY, i].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (!(p is Rook) || !(p is Queen))
                    {
                        break;
                    }
                    else if (p is Rook || p is Queen)
                    {
                        if (gb.WhitePieces.Contains<Piece>(this))
                        {
                            if (gb.BlackPieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                        }
                        else if (gb.BlackPieces.Contains<Piece>(this))
                        {
                            if (gb.WhitePieces.Contains<Piece>(this))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            //Check right
            for (int i = 0; i < gb.Board_Width; ++i)
            {
                if (gb.NodeArray[dY, i].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[dY, i].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (!(p is Rook) || !(p is Queen))
                    {
                        break;
                    }
                    else if (p is Rook || p is Queen)
                    {
                        if (gb.WhitePieces.Contains<Piece>(this))
                        {
                            if (gb.BlackPieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                        }
                        else if (gb.BlackPieces.Contains<Piece>(this))
                        {
                            if (gb.WhitePieces.Contains<Piece>(p))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Check right and down
            for (int posY = dY, posX = dX; ; ++posY, ++posX)
            {
                if (posY > gb.Board_Width - 1 || posX > gb.Board_Width - 1)
                    break;

                if (gb.NodeArray[posY, posX].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[posY, posX].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            if ((p is Pawn && (p.X - dX) == 1) || (p is Pawn && (dX - p.X) == 1))
                            {
                                return true;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            if (p is Pawn)
                            {
                                break;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            //Check right and up
            for (int negY = dY, posX = dX; ; --negY, ++posX)
            {
                if (negY < 0 || posX > gb.Board_Width - 1)
                    break;

                if (gb.NodeArray[negY, posX].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[negY, posX].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            if (p is Pawn)
                            {
                                continue;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                    if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            if ((p is Pawn && (p.X - dX) == 1) || (p is Pawn && (dX - p.X) == 1))
                            {
                                return true;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Check left and down
            for (int posY = dY, negX = dX; ; ++posY, --negX)
            {
                if (posY < gb.Board_Width || negX > 0)
                    break;

                if (gb.NodeArray[posY, negX].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[posY, negX].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            if ((p is Pawn && (p.X - dX) == 1) || (p is Pawn && (dX - p.X) == 1))
                            {
                                return true;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            if (p is Pawn)
                            {
                                continue;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Check left and up
            for (int negY = dY, negX = dX; ; --negY, --negX)
            {
                if (negY < 0 || negX < 0)
                    break;

                if (gb.NodeArray[negY, negX].CurrentPiece is Piece)
                {
                    Piece p = gb.NodeArray[negY, negX].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            break;
                        }
                    }

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            if (p is Pawn)
                            {
                                continue;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            if ((p is Pawn && (p.X - dX) == 1) || (p is Pawn && (dX - p.X) == 1))
                            {
                                return true;
                            }
                            else if (p is Bishop || p is Queen)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // Check for Horse
            if (dY + 1 < gb.Board_Width && dX + 2 < gb.Board_Length)
            {
                if (gb.NodeArray[dY + 1, dX + 2].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY + 1, dX + 2].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY - 1 >= 0 && dX < gb.Board_Length)
            {
                if (gb.NodeArray[dY - 1, dX + 2].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY - 1, dX + 2].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY + 1 < gb.Board_Width && dX - 2 >= 0)
            {
                if (gb.NodeArray[dY + 1, dX - 2].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY + 1, dX - 2].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY - 1 >= 0 && dX - 2 >= 0)
            {
                if (gb.NodeArray[dY - 1, dX - 2].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY - 1, dX - 2].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY + 2 < gb.Board_Width && dX + 1 < gb.Board_Length)
            {
                if (gb.NodeArray[dY + 2, dX + 1].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY + 2, dX + 1].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY - 2 >= 0 && dX + 1 < gb.Board_Length)
            {
                if (gb.NodeArray[dY - 2, dX + 1].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY - 2, dX + 1].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY + 2 < gb.Board_Width && dX - 1 >= 0)
            {
                if (gb.NodeArray[dY + 2, dX - 1].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY + 2, dX - 1].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }
            if (dY - 2 >= 0 && dX - 1 >= 0)
            {
                if (gb.NodeArray[dY - 2, dX - 1].CurrentPiece is Knight)
                {
                    Piece p = gb.NodeArray[dY - 2, dX - 1].CurrentPiece as Piece;

                    if (gb.WhitePieces.Contains<Piece>(this))
                    {
                        if (gb.BlackPieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                    else if (gb.BlackPieces.Contains<Piece>(this))
                    {
                        if (gb.WhitePieces.Contains<Piece>(p))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion
    }

    public class Queen : Piece
    {
        #region Constructors

        public Queen(string colour)
            : base(colour)
        {
            movePattern = "free";
            identifier = 'Q';
        }

        #endregion

        #region Methods

        public override bool Move(ChessBoard gb, Player player, int dX, int dY)
        {
            if (!ScanForPiece(gb, player, dX, dY))
            {
                // Queen can move freely throughout the board unless obstructed by a peice //

                // Check for valid moves //
                if ((dX - (this.X + 1) == dY - (this.Y + 1)) ||
                     ((this.X + 1) - dX == (this.Y + 1) - dY))
                    return base.Move(gb, player, dX, dY);
                else if ((dX - (this.X + 1) > 0 || dY - (this.Y + 1) > 0) ||
                          ((this.X + 1) - dX > 0 || (this.Y + 1) - dY > 0))
                    return base.Move(gb, player, dX, dY);
                else
                    return false;
            }
            else
                return false;
        }

        public override bool ScanForPiece(ChessBoard gb, Player player, int destX, int destY)
        {
            destX -= 1;
            destY -= 1;

            // If moving vertically //
            if (destX == (this.X))
            {
                // If moving down //
                if (destY > this.Y)
                {
                    // Check squares along path //
                    for (int i = (this.Y + 1); i <= destY; ++i)
                    {
                        if (gb.NodeArray[i, this.X].CurrentPiece is Piece)
                            return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
                // If moving up //
                else if (destY < (this.Y))
                {
                    for (int i = (this.Y - 1); i >= destY; --i)
                    {
                        if (gb.NodeArray[i, this.X].CurrentPiece is Piece)
                            return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
            }
            // If moving horizontally //
            else if (destY == this.Y)
            {
                // If moving right //
                if (destX > this.X)
                {
                    // Check squares along path //
                    for (int i = (this.X + 1); i <= destX; ++i)
                    {
                        if (gb.NodeArray[this.Y, i].CurrentPiece is Piece)
                            return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
                // If moving left //
                else if (destX < this.X)
                {
                    for (int i = (this.X - 1); i >= destX; --i)
                    {
                        if (gb.NodeArray[this.Y, i].CurrentPiece is Piece)
                            return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
            }
            // If moving right and down //
            else if (destX > this.X && destY > this.Y)
            {
                for (int i = (this.Y + 1), j = (this.X + 1); (i <= destY || j <= destX); ++i, ++j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                    {
                        return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
            }
            // If moving left and up //
            else if (destX < this.X && destY < this.Y)
            {
                for (int i = (this.Y - 1), j = (this.X - 1); (i >= destY || j >= destX); --i, --j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece ||
                        gb.NodeArray[destY, destX].CurrentPiece is Piece)
                    {
                        return base.ScanForPiece(gb, player, destX, destY);
                    }
                }
            }
            // If moving right and up //
            else if (destX > this.X && destY < this.Y)
            {
                for (int i = (this.Y - 1), j = (this.X + 1); (i >= destY || j <= destX); --i, ++j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                        return base.ScanForPiece(gb, player, destX, destY);
                }
            }
            // If moving left and down //
            else if (destX < this.X && destY > this.Y)
            {
                for (int i = (this.Y + 1), j = (this.X - 1); (i <= destY || j >= destX); ++i, --j)
                {
                    if (gb.NodeArray[i, j].CurrentPiece is Piece)
                        return base.ScanForPiece(gb, player, destX, destY);
                }
            }

            return false;
        }

        #endregion
    }
}
