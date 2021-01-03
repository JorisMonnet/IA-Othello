using System;
using System.Collections.Generic;
using System.Linq;

namespace IAOthelloMonnetPaysant
{
	//board square status
	public enum CellState
	{
		WHITE = 0, BLACK = 1, EMPTY = -1
	}

	//node of algorithm
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

	class OthelloGaletteSaucisseBoard: IPlayable.IPlayable
	{
		const int BOARDWIDTH = 9;
		const int BOARDHEIGHT = 7;
		const int ALPHAORBETA = 1;

		private int whiteScore = 0;
		private int blackScore = 0;
		private int roundCounting = 0;
		private int[,] board = new int[BOARDWIDTH,BOARDHEIGHT];

		bool GameEnded { get; set; }

		//useless comment to test

		public OthelloGaletteSaucisseBoard()
		{
			for(int i = 0;i < BOARDWIDTH;i++)
				for(int j = 0;j < BOARDHEIGHT;j++)
					board[i,j] = (int)CellState.EMPTY;

			board[3,3] = (int)CellState.WHITE;
			board[4,4] = (int)CellState.WHITE;
			board[3,4] = (int)CellState.BLACK;
			board[4,3] = (int)CellState.BLACK;
			ScoreComputing();
		}

		public string GetName()
		{
			return "Galette-Saucisse Squad ";
		}
		public int GetBlackScore()
		{
			return blackScore;
		}
		public int GetWhiteScore()
		{
			return whiteScore;
		}
		public int[,] GetBoard()
		{
			return board;
		}

		private void ScoreComputing()
		{
			whiteScore = 0;
			blackScore = 0;
			foreach(var cell in board)
			{
				if(cell == (int)CellState.WHITE) whiteScore++;
				else if(cell == (int)CellState.BLACK) blackScore++;
			}
			GameEnded = whiteScore == 0 || blackScore == 0 || whiteScore + blackScore == 63;
		}

		public Tuple<int,int> GetNextMove(int[,] game,int level,bool whiteTurn)
		{
			int round = 0;

			int[,] backupGame = (int[,])game.Clone();

			var node = AlphaBeta(game,level,whiteTurn,ALPHAORBETA,int.MaxValue,round);

			board = (int[,])backupGame.Clone();

			roundCounting += 1;

			return node.Move;
		}



		public bool IsPlayable(int column,int line,bool isWhite)
		{
			if(board[column,line] != (int)CellState.EMPTY) return false;
			
			bool result = false;
			for(int dLine = -1;dLine <= 1;dLine++)
				for(int dCol = -1;dCol <= 1;dCol++)
				{
					int c = column + dCol;
					int l = line + dLine;
					if(c < BOARDWIDTH && c >= 0 && l < BOARDHEIGHT && l >= 0
						&& board[c,l] == (int)(isWhite ? CellState.BLACK : CellState.WHITE))
					{
						bool breakBool = true;
						while(((c + dCol) < BOARDWIDTH) && c + dCol >= 0 &&
                                  (l + dLine) < BOARDHEIGHT && l + dLine >= 0 && breakBool)
						{
							c += dCol;
							l += dLine;
							if(board[c,l] == (int)(isWhite ? CellState.WHITE : CellState.BLACK))
							{
								result = true;
								breakBool = false;
							}
							else 
							{
								breakBool = board[c,l] == (int)(isWhite ? CellState.BLACK : CellState.WHITE);
							}
						}
					}
				}
			return result;
		}

		public bool PlayMove(int column,int line,bool isWhite)
		{
			if((column < 0) || column >= BOARDWIDTH || line < 0 || line >= BOARDHEIGHT)
			{
				return false;
			}
			if(!IsPlayable(column,line,isWhite))
			{
				return false;
			}

			List<Tuple<int,int,int>> cellsToReturn = new List<Tuple<int,int,int>>();

			bool result = false;
			int c;
			int l;
			for(int dLine = -1;dLine <= 1;dLine++)
			{
				for(int dCol = -1;dCol <= 1;dCol++)
				{
					c = column + dCol;
					l = line + dLine;
					if(c < BOARDWIDTH && c >= 0 && l < BOARDHEIGHT && l >= 0
						&& board[c,l] == (int)(isWhite ? CellState.BLACK : CellState.WHITE))
					{
						int k = 0;
						while(((c + dCol) < BOARDWIDTH) && c + dCol >= 0 &&
								  (l + dLine) < BOARDHEIGHT && l + dLine >= 0)
						{
							c += dCol;
							l += dLine;
							k++;

							if(board[c,l] == (int)((!isWhite) ? CellState.BLACK : CellState.WHITE))
							{
								result = true;
								board[column,line] = (int)((!isWhite) ? CellState.BLACK : CellState.WHITE);
								cellsToReturn.Add(new Tuple<int,int,int>(dCol,dLine,k));
							}
						}
					}
				}
			}

			foreach(var cell in cellsToReturn)
			{
				l = line;
				c = column;
				for(int i = 0;i < cell.Item3;i++)
				{
					c += cell.Item1;
					l += cell.Item2;
					board[c,l] = (int)((!isWhite) ? CellState.BLACK : CellState.WHITE);
				}
			}

			ScoreComputing();
			return result;
		}

		public Node AlphaBeta(int[,] game,int level,bool whiteTurn,int alphaORbeta,double parentFitness,int round,Tuple<int,int> lastMove = null,double pathFitness = 0.0)
		{
			List<Tuple<int,int>> possibleMoves = GetPossibleMoves(whiteTurn);
			double currentNodeFitness = 0.0;

			if(lastMove != null) // The root does not have a fitness
				currentNodeFitness = Evaluation(game,!whiteTurn,lastMove,round);

			if(level == 0 || possibleMoves.Count == 0 || GameEnded)
				return new Node(new Tuple<int,int>(-1,-1),currentNodeFitness + pathFitness);

			Node currentNode = new Node(new Tuple<int,int>(-1,-1),alphaORbeta * -int.MaxValue);

			foreach(Tuple<int,int> move in possibleMoves)
			{
				PlayMove(move.Item1,move.Item2,whiteTurn);
				Node children = AlphaBeta(
					game, // the game has changed
					level - 1, // one step deeper
					!whiteTurn, // next player
					-alphaORbeta, // min <-> max
					currentNode.Fitness, // parent's fitness
					round + 1, // turn count
					move, // move to explore
					currentNodeFitness + pathFitness); // path's price

				if(children.Fitness * alphaORbeta > currentNode.Fitness * alphaORbeta)
				{
					currentNode.Fitness = children.Fitness;
					currentNode.Move = move;

					// If the child has a better absolute fitness than his parent, something's wrong
					if(Math.Abs(currentNode.Fitness) > Math.Abs(parentFitness))
						break;
				}
			}
			return currentNode;
		}
		public double Evaluation(int[,] game,bool whiteTurn,Tuple<int,int> move,int round)
		{
			double fitness;
			if(whiteTurn)
			{
				fitness = GetWhiteScore();
			}
			else
			{
				fitness = GetBlackScore();
			}

			double coef = (double)((BOARDWIDTH * BOARDHEIGHT) - roundCounting + round) / BOARDWIDTH * BOARDHEIGHT;
			fitness *= coef;

			return fitness;
		}

		public List<Tuple<int,int>> GetPossibleMoves(bool whiteTurn)
		{
			List<Tuple<int,int>> possibleMoves = new List<Tuple<int,int>>();
			for(int i = 0;i < BOARDWIDTH;i++)
				for(int j = 0;j < BOARDHEIGHT;j++)
					if(IsPlayable(i,j,whiteTurn))
						possibleMoves.Add(new Tuple<int,int>(i,j));
			return possibleMoves;
		}
	}

}
