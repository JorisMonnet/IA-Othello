using System;
using System.Collections.Generic;

namespace IAOthelloMonnetPaysant
{
    public class OthelloGaletteSaucisseBoard: IPlayable.IPlayable
    {
        const int BOARDWIDTH = 9;
        const int BOARDHEIGHT = 7;
        const int WHITEISSTARTING = 1;

        int[,] board = new int[BOARDWIDTH,BOARDHEIGHT];
        int whiteScore = 0;
        int blackScore = 0;
        int roundIterations = 0;
        public bool GameEnded { get; set; }

        public OthelloGaletteSaucisseBoard()
        {
            for(int i = 0;i < BOARDWIDTH;i++)
            {
                for(int j = 0;j < BOARDHEIGHT;j++)
                {
                    board[i,j] = (int)CellStatus.EMPTY;
                }
            }

            board[3,3] = (int)CellStatus.WHITE;
            board[4,4] = (int)CellStatus.WHITE;
            board[3,4] = (int)CellStatus.BLACK;
            board[4,3] = (int)CellStatus.BLACK;

            ScoreComputing();
        }


        public int[,] GetBoard()
        {
            return board;
        }
        public int GetWhiteScore()
        {
            return whiteScore;
        }
        public int GetBlackScore()
        {
            return blackScore;
        }
        public string GetName()
        {
            return "Galette-Saucisse Squad ";
        }

        public Tuple<int,int> GetNextMove(int[,] game,int level,bool whiteTurn)
        {
            int[,] saveMyBoard = (int[,])game.Clone();

            var node = AlphaBeta(game,level,whiteTurn,WHITEISSTARTING,0);
            board = (int[,])saveMyBoard.Clone();
            roundIterations++;
            return node.Move;
        }

        public Node AlphaBeta(int[,] game,int depth,bool whiteTurn,int whoIsPlaying,int round,Tuple<int,int> lastMove = null,double elderFitness = 0.0)
        {
            List<Tuple<int,int>> moves = GetPossibleMove(whiteTurn);
            double currentFitness = 0.0;

            if(lastMove != null)
            {
                currentFitness = Evaluation(game,!whiteTurn,lastMove,round);
            }
            if(depth == 0 || moves.Count == 0 || GameEnded)
            {
                return new Node(new Tuple<int,int>(-1,-1),currentFitness + elderFitness);
            }

            Node currentNode = new Node(new Tuple<int,int>(-1,-1),whoIsPlaying * -int.MaxValue);

            foreach(Tuple<int,int> move in moves)
            {
                if(IsPlayable(move.Item1,move.Item2,whiteTurn))
                {
                    PlayMove(move.Item1,move.Item2,whiteTurn);
                }
                Node children = AlphaBeta(game,depth - 1,!whiteTurn,-whoIsPlaying,round + 1,move,currentFitness + elderFitness);

                if(children.Fitness * whoIsPlaying > currentNode.Fitness * whoIsPlaying)
                {
                    currentNode.Fitness = children.Fitness;
                    currentNode.Move = move;
                }
            }
            return currentNode;
        }


        public double Evaluation(int[,] game,bool whiteTurn,Tuple<int,int> move,int round)
        {
            //based on https://www.ultraboardgames.com/othello/tips.php#:~:text=While%20the%20move%20that%20flips,key%20to%20winning%20the%20game.
            //also based on http://www.radagast.se/othello/Help/strategy.html
            //try to keep center, corner are good, avoid column and line before border column/line and try to go on the right or the left if white or black
            if(roundIterations<10){
                int[,] moveEvaluationBegin = (whiteTurn ? new int[,]{
                    { 40,   -20,    2,      2,      2,      2,      2,      -20,    40  },
                    { -20,   -20,   -1,     -1,     -1,     -1,     -1,     -20,    -20 },
                    { 10,    -1,     1,      5,      5,      5,      6,      0,      10   },
                    { 10,    -1,     1,      5,      5,      5,      6,      0,      10   },
                    { 10,    -1,     1,      5,      5,      5,      6,      0,      10   },
                    { -20,  -20,    -2,     -2,     -2,     -2,     -2,     -20,    10   },
                    { 40,   -20,    2,      2,      2,      2,      2,      -20,    40  },
                } : new int[,]{
                    { 40,   -20,    10,      10,      10,      10,      10,      -20,    40  },
                    { -20,   -20,   -1,     -1,     -1,     -1,     -1,     -20,    -20 },
                    { 10,    0,     6,      5,      5,      5,      1,      -1,      10   },
                    { 10,    0,     6,      5,      5,      5,      1,      -1,      10  },
                    { 10,    0,     6,      5,      5,      5,      1,      -1,      10   },
                    { -20,  -20,    -2,     -2,     -2,     -2,     -2,     -20,    10   },
                    { 40,   -20,    10,      10,      10,      10,      10,      -20,    40  },
                });
            } else {

            }
            

            double fitness = whiteTurn ? GetWhiteScore() : GetBlackScore();

            if(move != null)
            {
                fitness += moveEvaluation[move.Item2,move.Item1];
                fitness += HowMuchToSwipe(move.Item2,move.Item1,whiteTurn);
            }

            return fitness;
        }

        private int HowMuchToSwipe(int column,int line,bool isWhite)
        {
            if(!IsPlayable(column,line,isWhite))
            {
                return 0;
            }

            int howMuch = 0;
            int iterationColumnIndex, iterationLineIndex;
            for(int deltaLine = -1;deltaLine <= 1;deltaLine++)
            {
                for(int deltaColumn = -1;deltaColumn <= 1;deltaColumn++)
                {
                    iterationColumnIndex = column + deltaColumn;
                    iterationLineIndex = line + deltaLine;
                    if((iterationColumnIndex < BOARDWIDTH) && (iterationColumnIndex >= 0) && (iterationLineIndex < BOARDHEIGHT) && (iterationLineIndex >= 0)
                        && (board[iterationColumnIndex,iterationLineIndex] == (int)(isWhite ? CellStatus.BLACK : CellStatus.WHITE)))
                    {
                        while(((iterationColumnIndex + deltaColumn) < BOARDWIDTH) && (iterationColumnIndex + deltaColumn >= 0) &&
                                  ((iterationLineIndex + deltaLine) < BOARDHEIGHT) && ((iterationLineIndex + deltaLine >= 0))
                                   && (board[iterationColumnIndex,iterationLineIndex] == (int)(isWhite ? CellStatus.BLACK : CellStatus.WHITE)))
                        {
                            iterationColumnIndex += deltaColumn;
                            iterationLineIndex += deltaLine;
                            if(board[iterationColumnIndex,iterationLineIndex] == (int)((!isWhite) ? CellStatus.BLACK : CellStatus.WHITE))
                            {
                                howMuch++;
                            }
                        }
                    }
                }
            }
            return howMuch;
        }

        public bool PlayMove(int column,int line,bool isWhite)
        {
            int iterationColumnIndex, iterationLineIndex;
            bool result = false;

            List<Tuple<int,int,int>> cellsToSwitch = new List<Tuple<int,int,int>>();
            for(int deltaLine = -1;deltaLine <= 1;deltaLine++)
            {
                for(int deltaColumn = -1;deltaColumn <= 1;deltaColumn++)
                {
                    iterationColumnIndex = column + deltaColumn;
                    iterationLineIndex = line + deltaLine;
                    if((iterationColumnIndex < BOARDWIDTH) && (iterationColumnIndex >= 0) && (iterationLineIndex < BOARDHEIGHT) && (iterationLineIndex >= 0)
                        && (board[iterationColumnIndex,iterationLineIndex] == (int)(isWhite ? CellStatus.BLACK : CellStatus.WHITE)))
                    {
                        int counter = 0;
                        while(((iterationColumnIndex + deltaColumn) < BOARDWIDTH) && (iterationColumnIndex + deltaColumn >= 0) &&
                                  ((iterationLineIndex + deltaLine) < BOARDHEIGHT) && ((iterationLineIndex + deltaLine >= 0))
                                   && (board[iterationColumnIndex,iterationLineIndex] == (int)(isWhite ? CellStatus.BLACK : CellStatus.WHITE)))
                        {
                            iterationColumnIndex += deltaColumn;
                            iterationLineIndex += deltaLine;
                            counter++;
                            if(board[iterationColumnIndex,iterationLineIndex] == (int)((!isWhite) ? CellStatus.BLACK : CellStatus.WHITE))
                            {
                                result = true;
                                board[column,line] = (int)((!isWhite) ? CellStatus.BLACK : CellStatus.WHITE);
                                cellsToSwitch.Add(new Tuple<int,int,int>(deltaColumn,deltaLine,counter));
                            }
                        }
                    }
                }
            }

            foreach(var cell in cellsToSwitch)
            {
                iterationLineIndex = line;
                iterationColumnIndex = column;
                for(int i = 0;i < cell.Item3;i++)
                {
                    iterationColumnIndex += cell.Item1;
                    iterationLineIndex += cell.Item2;
                    board[iterationColumnIndex,iterationLineIndex] = (int)((!isWhite) ? CellStatus.BLACK : CellStatus.WHITE);
                }
            }
            ScoreComputing();
            return result;
        }


        public bool IsPlayable(int column,int line,bool isWhite)
        {
            if((column < 0) || (column >= BOARDWIDTH) || (line < 0) || (line >= BOARDHEIGHT))
            {
                return false;
            }
            if(board[column,line] != (int)CellStatus.EMPTY)
            {
                return false;
            }

            int iterationColumnIndex, iterationLineIndex;
            bool result = false;
            for(int deltaLine = -1;deltaLine <= 1;deltaLine++)
            {
                for(int deltaColumn = -1;deltaColumn <= 1;deltaColumn++)
                {
                    iterationColumnIndex = column + deltaColumn;
                    iterationLineIndex = line + deltaLine;
                    if((iterationColumnIndex < BOARDWIDTH) && (iterationColumnIndex >= 0) && (iterationLineIndex < BOARDHEIGHT) && (iterationLineIndex >= 0)
                        && (board[iterationColumnIndex,iterationLineIndex] == (int)(isWhite ? CellStatus.BLACK : CellStatus.WHITE)))
                    {
                        bool breakBool = false;
                        while(((iterationColumnIndex + deltaColumn) < BOARDWIDTH) && (iterationColumnIndex + deltaColumn >= 0) &&
                                  ((iterationLineIndex + deltaLine) < BOARDHEIGHT) && (iterationLineIndex + deltaLine >= 0) && !breakBool)
                        {
                            iterationColumnIndex += deltaColumn;
                            iterationLineIndex += deltaLine;
                            if(board[iterationColumnIndex,iterationLineIndex] == (int)((!isWhite) ? CellStatus.BLACK : CellStatus.WHITE))
                            {
                                result = true;
                                breakBool = true;
                            }
                            else if(board[iterationColumnIndex,iterationLineIndex] == (int)((isWhite) ? CellStatus.BLACK : CellStatus.WHITE))
                            {
                                breakBool = false;
                            }
                            //stop while if cell is empty
                            else if(board[iterationColumnIndex,iterationLineIndex] == (int)CellStatus.EMPTY)
                            {
                                breakBool = true;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<Tuple<int,int>> GetPossibleMove(bool whiteTurn)
        {
            List<Tuple<int,int>> moves = new List<Tuple<int,int>>();
            for(int columnIndex = 0;columnIndex < BOARDWIDTH;columnIndex++)
            {
                for(int lineIndex = 0;lineIndex < BOARDHEIGHT;lineIndex++)
                {
                    if(IsPlayable(columnIndex,lineIndex,whiteTurn))
                    {
                        moves.Add(new Tuple<int,int>(columnIndex,lineIndex));
                    }
                }
            }
            return moves;
        }

        private void ScoreComputing()
        {
            whiteScore = 0;
            blackScore = 0;
            foreach(var cell in board)
            {
                if(cell == (int)CellStatus.BLACK)
                {
                    blackScore++;
                }
                else if(cell == (int)CellStatus.WHITE)
                {
                    whiteScore++;
                }
            }
            GameEnded = ((whiteScore + blackScore == BOARDHEIGHT * BOARDWIDTH) || (blackScore == 0) || (whiteScore == 0));
        }
    }

    //tool objects

    public enum CellStatus
    {
        EMPTY = -1,
        WHITE = 0,
        BLACK = 1
    }

    public class Node
    {
        public Tuple<int,int> Move { get; set; }
        public double Fitness { get; set; }

        public Node(Tuple<int,int> move,double fitness)
        {
            Fitness = fitness;
            Move = move;
        }
    }
}