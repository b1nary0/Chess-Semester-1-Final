using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JPODevChess
{    /* THE CHESSBOARD CLASS MANAGES THE GAME BOARD AND ITS INFORMATION. EACH SQUARE IS ITSELF AN INDIVIDUAL
     NODE (64 NODES TOTAL). EACH SQUARE NODE CONTAINS A CURRENTPIECE NODE WHICH IS USED TO STORE EITHER
     A PIECE (ROOK, PAWN, ETC) OR AN EMPTY NODE. */

    public class ChessBoard
    {
        #region Fields

        const int BOARD_LENGTH = 8;
        const int BOARD_WIDTH = 8;
        private string lastMove;

        private Piece[] whitePieces;
        private Piece[] blackPieces;
        private Node[,] nodeArray;

        private List<Piece> p1Takes;
        private List<Piece> p2Takes;

        ConsoleColor darkSquares;
        ConsoleColor lightSquares;
        ConsoleColor whitePiecesColour;
        ConsoleColor blackPiecesColour;

        #endregion

        #region Properties

        public int Board_Length
        {
            get { return BOARD_LENGTH; }
        }

        public int Board_Width
        {
            get { return BOARD_WIDTH; }
        }

        public string LastMove
        {
            get { return lastMove; }
        }

        public Node[,] NodeArray
        {
            get { return nodeArray; }
        }

        public Piece[] WhitePieces
        {
            get { return whitePieces; }
        }

        public Piece[] BlackPieces
        {
            get { return blackPieces; }
        }

        public List<Piece> P1Takes
        {
            get { return p1Takes; }
        }

        public List<Piece> P2Takes
        {
            get { return p2Takes; }
        }

        #endregion

        #region Constructors

        public ChessBoard()
        {
            //board = new char[BOARD_LENGTH, BOARD_WIDTH];

            whitePieces = new Piece[16];
            blackPieces = new Piece[16];
            p1Takes = new List<Piece>();
            p2Takes = new List<Piece>();
            nodeArray = new Node[BOARD_LENGTH, BOARD_WIDTH];

        }

        #endregion

        #region Methods

        public void Initialize(string colourOption)
        {
            /* INITIALIZE SETS UP THE PROPER COLOUR SCHEME ACCORDING TO THE PARAMETER PASSED 
             * AND POPULATES THE PIECE ARRAYS, NODES, AND BOARD IN THE PROPER ORDER */

            if (colourOption == "christmas")
            {
                Console.Title = "Christmas Chess!";
                darkSquares = ConsoleColor.DarkGreen;
                lightSquares = ConsoleColor.Red;
                whitePiecesColour = ConsoleColor.White;
                blackPiecesColour = ConsoleColor.Black;
            }
            else if (colourOption == "default")
            {
                darkSquares = ConsoleColor.Black;
                lightSquares = ConsoleColor.White;
                whitePiecesColour = ConsoleColor.Red;
                blackPiecesColour = ConsoleColor.DarkGreen;
            }

            PopulatePieces();
            PopulateNodes();
            PopulateBoard();
        }

        public void Draw()
        {
            /* DRAW DISPLAYS A PROPERLY FORMATTED CHESS BOARD TO THE SCREEN. IT ALSO DRAWS THE 
             * LEGEND USED FOR MOVEMENT OF PIECES. EVERY EVEN NUMBER AND ODD NUMBER DISPLAYS
             * A CERTAIN SQUARE COLOUR. THE IDENTIFIER FOR EACH PIECE AT THE NODE ACCESSED
             * IS DISPLAYED AT ITS POSITION. */

            Console.WriteLine("\n     A   B   C   D   E   F   G   H  ");
            Console.Write("   ---------------------------------\n");

            for (int i = 0; i < BOARD_WIDTH; ++i)
            {
                for (int j = 0; j < BOARD_LENGTH; ++j)
                {
                    if (j == 0)
                        Console.Write(" " + (i + 1) + " |");
                    else
                    {
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = darkSquares;
                    }

                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0)
                            Console.BackgroundColor = darkSquares;
                        else
                            Console.BackgroundColor = lightSquares;
                    }
                    else if (j % 2 == 0)
                        Console.BackgroundColor = lightSquares;

                    if (nodeArray[i, j].CurrentPiece is Piece)
                    {
                        Piece p = nodeArray[i, j].CurrentPiece as Piece;

                        if (whitePieces.Contains<Piece>(p))
                        {
                            Console.Write(" ");
                            Console.ForegroundColor = whitePiecesColour;
                            Console.Write(nodeArray[i, j].CurrentPiece.Identifier);
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.Write(" ");
                            Console.ForegroundColor = blackPiecesColour;
                            Console.Write(nodeArray[i, j].CurrentPiece.Identifier);
                            Console.Write(" ");
                        }
                    }

                    else if (nodeArray[i, j].CurrentPiece is EmptyNode)
                        Console.Write("   ");

                    Console.BackgroundColor = darkSquares;

                    if (j == 7)
                    {
                        Console.ResetColor();
                        Console.Write("| " + (i + 1) + "\n  ");

                        for (int k = 0; k < BOARD_LENGTH; ++k)
                        {
                            Console.ResetColor();

                            if (k == 0)
                                Console.Write(" |");
                            else
                                Console.Write("|");

                            if ((k % 2 != 0) && (i % 2 == 0))
                            {
                                Console.BackgroundColor = lightSquares;
                                Console.Write("   ");
                            }
                            else if (k % 2 == 0 && i % 2 != 0)
                            {
                                Console.BackgroundColor = lightSquares;
                                Console.Write("   ");
                            }
                            else
                            {
                                Console.BackgroundColor = darkSquares;
                                Console.Write("   ");
                            }
                        }

                        Console.ResetColor();
                        Console.Write("|\n   ");
                        Console.Write("---------------------------------\n");
                    }
                }
            }
            Console.WriteLine("     A   B   C   D   E   F   G   H  \n");
        }

        public void PopulatePieces()
        {
            /* POPULATEPIECES FILLS THE WHITE AND BLACK PIECE ARRAYS WITH THE PROPER OBJECTS.
             * ALL OBJECTS ARE CREATED WITH OTHER OBJECTS OF THE SAME TYPE. */

            /***********/
            /***WHITE***/
            /***********/

            for (int i = 0; i < whitePieces.Length; ++i)
            {
                if (i <= 7)
                {
                    // Pawns are first 8 positions in array
                    whitePieces[i] = new Pawn("white");
                }
                else if (i > 7 && i <= 9)
                {
                    //Then Rooks
                    whitePieces[i] = new Rook("white");
                }
                else if (i > 9 && i <= 11)
                {
                    // Then Knights
                    whitePieces[i] = new Knight("white");
                }
                else if (i > 11 && i <= 13)
                {
                    // Then Bishops
                    whitePieces[i] = new Bishop("white");
                }
                else if (i == 14)
                {
                    // Then Queen
                    whitePieces[i] = new Queen("white");
                }
                else if (i == 15)
                {
                    // Then King
                    whitePieces[i] = new King("white");
                }
            }

            /***********/
            /***BLACK***/
            /***********/

            for (int i = 0; i < whitePieces.Length; ++i)
            {
                if (i <= 7)
                {
                    // Pawns are first 8 positions in array
                    blackPieces[i] = new Pawn("black");
                }
                else if (i > 7 && i <= 9)
                {
                    //Then Rooks
                    blackPieces[i] = new Rook("black");
                }
                else if (i > 9 && i <= 11)
                {
                    // Then Knights
                    blackPieces[i] = new Knight("black");
                }
                else if (i > 11 && i <= 13)
                {
                    // Then Bishops
                    blackPieces[i] = new Bishop("black");
                }
                else if (i == 14)
                {
                    // Then Queen
                    blackPieces[i] = new Queen("black");
                }
                else if (i == 15)
                {
                    // Then King
                    blackPieces[i] = new King("black");
                }
            }
        }

        public void PopulateBoard()
        {
            /* POPULATE BOARD INSERTS ALL PIECES INTO THEIR STARTING POSITIONS.
             * ALL LEFTOVER NODES ARE FILLED WITH EMPTY NODES */

            // Populate White Pawns
            for (int i = 0; i < BOARD_LENGTH; ++i)
            {
                nodeArray[1, i].CurrentPiece = whitePieces[i];
                nodeArray[1, i].CurrentPiece.X = i;
                nodeArray[1, i].CurrentPiece.Y = 1;
            }

            // Populate White Rooks
            nodeArray[0, 0].CurrentPiece = whitePieces[8];
            nodeArray[0, 0].CurrentPiece.X = 0;
            nodeArray[0, 0].CurrentPiece.Y = 0;

            nodeArray[0, 7].CurrentPiece = whitePieces[9];
            nodeArray[0, 7].CurrentPiece.X = 7;
            nodeArray[0, 7].CurrentPiece.Y = 0;

            // Populate White Bishops
            nodeArray[0, 1].CurrentPiece = whitePieces[10];
            nodeArray[0, 1].CurrentPiece.X = 1;
            nodeArray[0, 1].CurrentPiece.Y = 0;

            nodeArray[0, 6].CurrentPiece = whitePieces[11];
            nodeArray[0, 6].CurrentPiece.X = 6;
            nodeArray[0, 6].CurrentPiece.Y = 0;

            // Populate White Knights
            nodeArray[0, 2].CurrentPiece = whitePieces[12];
            nodeArray[0, 2].CurrentPiece.X = 2;
            nodeArray[0, 2].CurrentPiece.Y = 0;

            nodeArray[0, 5].CurrentPiece = whitePieces[13];
            nodeArray[0, 5].CurrentPiece.X = 5;
            nodeArray[0, 5].CurrentPiece.Y = 0;

            // Populate White King
            nodeArray[0, 3].CurrentPiece = whitePieces[14];
            nodeArray[0, 3].CurrentPiece.X = 3;
            nodeArray[0, 3].CurrentPiece.Y = 0;

            nodeArray[0, 4].CurrentPiece = whitePieces[15];
            nodeArray[0, 4].CurrentPiece.X = 4;
            nodeArray[0, 4].CurrentPiece.Y = 0;

            // Populate Black Pawns
            for (int i = 0; i < BOARD_LENGTH; ++i)
            {
                nodeArray[6, i].CurrentPiece = blackPieces[i];
                nodeArray[6, i].CurrentPiece.X = i;
                nodeArray[6, i].CurrentPiece.Y = 6;
            }

            // Populate Black Rooks
            nodeArray[7, 0].CurrentPiece = blackPieces[8];
            nodeArray[7, 0].CurrentPiece.X = 0;
            nodeArray[7, 0].CurrentPiece.Y = 7;

            nodeArray[7, 7].CurrentPiece = blackPieces[9];
            nodeArray[7, 7].CurrentPiece.X = 7;
            nodeArray[7, 7].CurrentPiece.Y = 7;

            // Populate Black Bishops
            nodeArray[7, 1].CurrentPiece = blackPieces[10];
            nodeArray[7, 1].CurrentPiece.X = 1;
            nodeArray[7, 1].CurrentPiece.Y = 7;

            nodeArray[7, 6].CurrentPiece = blackPieces[11];
            nodeArray[7, 6].CurrentPiece.X = 6;
            nodeArray[7, 6].CurrentPiece.Y = 7;

            // Populate Black Knights
            nodeArray[7, 2].CurrentPiece = blackPieces[12];
            nodeArray[7, 2].CurrentPiece.X = 2;
            nodeArray[7, 2].CurrentPiece.Y = 7;

            nodeArray[7, 5].CurrentPiece = blackPieces[13];
            nodeArray[7, 5].CurrentPiece.X = 5;
            nodeArray[7, 5].CurrentPiece.Y = 7;

            // Populate Black King
            nodeArray[7, 3].CurrentPiece = blackPieces[14];
            nodeArray[7, 3].CurrentPiece.X = 3;
            nodeArray[7, 3].CurrentPiece.Y = 7;

            nodeArray[7, 4].CurrentPiece = blackPieces[15];
            nodeArray[7, 4].CurrentPiece.X = 4;
            nodeArray[7, 4].CurrentPiece.Y = 7;

            // Populate Empty Nodes
            for (int i = 2; i <= 5; ++i)
            {
                for (int j = 0; j < Math.Sqrt(nodeArray.Length); ++j)
                {
                    nodeArray[i, j].CurrentPiece = new EmptyNode();
                }
            }
        }

        public void PopulateWhiteNodes()
        {
            /* POPULATEWHITENODES FILLS EACH INDEX OF THE NODEARRAY WITH A PROPER
             * NODE OF A CORRESPONDING COLOUR */

            for (int i = 0; i < Math.Sqrt(nodeArray.Length); ++i)
            {
                for (int j = 0; j < Math.Sqrt(nodeArray.Length); ++j)
                {
                    if (i % 2 == 0 && j % 2 != 0)
                    {
                        nodeArray[i, j] = new Node("white");
                    }
                    else if (i % 2 != 0 && j % 2 == 0)
                    {
                        nodeArray[i, j] = new Node("white");
                    }
                }
            }
        }

        public void PopulateBlackNodes()
        {
            /* POPULATEBLACKNODES IS THE SAME AS POPULATEWHITENODES, BUT FOR BLACK SQUARES */
            for (int i = 0; i < Math.Sqrt(nodeArray.Length); ++i)
            {
                for (int j = 0; j < Math.Sqrt(nodeArray.Length); ++j)
                {
                    if (i % 2 == 0 && j % 2 == 0)
                    {
                        nodeArray[i, j] = new Node("black");
                    }
                    else if (i % 2 != 0 && j % 2 != 0)
                    {
                        nodeArray[i, j] = new Node("black");
                    }
                }
            }
        }

        public void PopulateNodes()
        {
            /* SIMPLE CONSOLIDATION OF NODE POPULATION FOR CALLING IN INITIALIZE */

            PopulateWhiteNodes();
            PopulateBlackNodes();
        }

        public void SetLastMove(char fromX, char toX, int fromY, int toY)
        {
            /* SETS THE LAST MOVE FOR USE */

            lastMove = fromX + fromY.ToString() + "->" + toX + toY.ToString();
        }

        #endregion
    }
}
