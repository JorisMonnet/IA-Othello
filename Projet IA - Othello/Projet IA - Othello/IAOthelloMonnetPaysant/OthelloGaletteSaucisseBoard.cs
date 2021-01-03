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
        public bool GameEnded { get; set; }
        private int RoundIterations = 0;

        public OthelloGaletteSaucisseBoard()
        {
            for(int i = 0;i < BOARDWIDTH;i++)
                for(int j = 0;j < BOARDHEIGHT;j++)
                    board[i,j] = (int)CellStatus.EMPTY;

            board[3,3] = (int)CellStatus.WHITE;
            board[4,4] = (int)CellStatus.WHITE;
            board[3,4] = (int)CellStatus.BLACK;
            board[4,3] = (int)CellStatus.BLACK;

            ScoreComputing();
        }


        public int[,] GetBoard()
        {
            return (int[,])board;
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
            int round = 0;
            int[,] saveMyBoard = (int[,])game.Clone();

            var node = AlphaBeta(game,level,whiteTurn,WHITEISSTARTING,int.MaxValue,round);

            board = (int[,])saveMyBoard.Clone();

            RoundIterations++;
            return node.Move;
        }

        public Node AlphaBeta(int[,] game,int depth,bool whiteTurn,int whoIsPlaying,double parentFitness,int round,Tuple<int,int> lastMove = null,double elderFitness = 0.0)
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
                PlayMove(move.Item1,move.Item2,whiteTurn);
                Node children = AlphaBeta(game,depth - 1,!whiteTurn,-whoIsPlaying,currentNode.Fitness,round + 1,move,currentFitness + elderFitness);

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
            int[,] moveEvaluation ={
                { 40, -10, 2, 2, 2, 2, 2,-10, 40 },
                { -10,-5,-1,-1,-1,-1,-1,-5,-10 },
                { 2,-1, 1, 1, 1, 1, 1, -1, 2 },
                { 2,-1, 0, 1, 1, 1, 0, -1, 2 },
                { 2,-1, 1, 1, 1, 1, 1, -1, 2 },
                { -10,-5,-2,-2,-2,-2,-2,-5,-10 },
                { 40, -10, 2, 2, 2, 2, 2,-10, 40 },
            };

            double fitness;

            if(whiteTurn)
            {
                fitness = GetWhiteScore();
            }
            else
            {
                fitness = GetBlackScore();
            }


            fitness *= HowMuchToSwipe(move.Item2,move.Item1,whiteTurn);


            if(move != null)
            {
                fitness += moveEvaluation[move.Item2,move.Item1];
            }

            return fitness;
        }

        private int HowMuchToSwipe(int column,int line,bool isWhite)
        {
            if((column < 0) || (column >= BOARDWIDTH) || (line < 0) || (line >= BOARDHEIGHT))
            {
                return 0;
            }
            if(IsPlayable(column,line,isWhite) == false)
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
            if((column < 0) || (column >= BOARDWIDTH) || (line < 0) || (line >= BOARDHEIGHT))
            {
                return false;
            }
            if(IsPlayable(column,line,isWhite) == false)
            {
                return false;
            }

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
                int returnedCells = 0;
                while(returnedCells++ < cell.Item3)
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
                        bool breakBool = true;
                        while(((iterationColumnIndex + deltaColumn) < BOARDWIDTH) && (iterationColumnIndex + deltaColumn >= 0) &&
                                  ((iterationLineIndex + deltaLine) < BOARDHEIGHT) && ((iterationLineIndex + deltaLine >= 0)) && breakBool)
                        {
                            iterationColumnIndex += deltaColumn;
                            iterationLineIndex += deltaLine;
                            if(board[iterationColumnIndex,iterationLineIndex] == (int)((!isWhite) ? CellStatus.BLACK : CellStatus.WHITE))
                            {
                                result = true;
                                breakBool = false;
                            }
                            else if(board[iterationColumnIndex,iterationLineIndex] == (int)(isWhite ? CellStatus.BLACK : CellStatus.WHITE))
                            {
                                breakBool = true;
                            }
                            else if(board[iterationColumnIndex,iterationLineIndex] == (int)CellStatus.EMPTY)
                            {
                                breakBool = false;
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
                for(int lineIndex = 0;lineIndex < BOARDHEIGHT;lineIndex++)
                {
                    if(IsPlayable(columnIndex,lineIndex,whiteTurn))
                    {
                        moves.Add(new Tuple<int,int>(columnIndex,lineIndex));
                    }
                }
            return moves;
        }

        private void ScoreComputing()
        {
            whiteScore = 0;
            blackScore = 0;
            int sum = 0;
            foreach(var cell in board)
            {
                if(cell == (int)CellStatus.BLACK)
                {
                    blackScore++;
                    sum++;
                }
                else if(cell == (int)CellStatus.WHITE)
                {
                    whiteScore++;
                    sum++;
                }
            }
            //9*7=63
            GameEnded = ((sum == 63) || (blackScore == 0) || (whiteScore == 0));
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
            this.Fitness = fitness;
            this.Move = move;
        }
    }
}