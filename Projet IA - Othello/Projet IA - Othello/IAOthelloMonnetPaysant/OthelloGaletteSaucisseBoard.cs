using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAOthelloMonnetPaysant
{
    //board square status
    public enum SquareState
    {
        WHITE=0,BLACK=1,EMPTY=2
    }

    //node of algorith
    public class Node
    {
        public Tuple<int, int> move { get; set; }
        public double fitness { get; set; }

        public Node(Tuple<int, int> move, double fitness)
        {
            this.fitness = fitness;
            this.move = move;
        }
    }

    class OthelloGaletteSaucisseBoard : IPlayable.IPlayable
    {
        const int BOARDWIDTH = 9;
        const int BOARDHEIGHT = 7;

        private int whiteScore = 0;
        private int blackScore = 0;
        private int roundCounting = 0;
        private int[,] board = new int[BOARDWIDTH,BOARDHEIGHT];

        bool gameEnd { get; set; }

                
        public OthelloGaletteSaucisseBoard()
        {
            for (int i = 0; i < BOARDWIDTH; i++)
                for (int j = 0; j < BOARDHEIGHT; j++)
                    board[i, j] = (int)SquareState.EMPTY;

            board[3, 3] = (int)SquareState.WHITE;
            board[4, 4] = (int)SquareState.WHITE;
            board[3, 4] = (int)SquareState.BLACK;
            board[4, 3] = (int)SquareState.BLACK;
        }

        string IPlayable.IPlayable.GetName()
        {
            return "OthelloGaletteSaucisse";
        }
        int IPlayable.IPlayable.GetBlackScore()
        {
            return blackScore;
        }
        int IPlayable.IPlayable.GetWhiteScore()
        {
            return whiteScore;
        }
        int[,] IPlayable.IPlayable.GetBoard()
        {
            return board;
        }

       

        Tuple<int, int> IPlayable.IPlayable.GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            throw new NotImplementedException();
        }

      
        bool IPlayable.IPlayable.IsPlayable(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }

        bool IPlayable.IPlayable.PlayMove(int column, int line, bool isWhite)
        {
            throw new NotImplementedException();
        }
    }

}
