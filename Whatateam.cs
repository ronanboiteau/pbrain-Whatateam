﻿using System;
using System.Collections.Generic;
using System.Linq;

internal class Position
{
	public int X, Y;

	public Position() {}

	public Position(int x, int y)
	{
		X = x;
		Y = y;
	}
}

internal enum Direction
{
	None = 0,
	Horizontal,
	Vertical,
	DiagonalS,
	DiagonalA
}

internal class GomocupEngine : GomocupInterface
{
	private const int MaxBoardSize = 100;
	private int[,] _board = new int[MaxBoardSize, MaxBoardSize];
	private Random _rand = new Random();
	private Position _opponentLastMove = new Position();
	private Position _attackZone;
	private Dictionary<Direction, bool> _canPlay = new Dictionary<Direction, bool>
	{
		{ Direction.Horizontal, false },
		{ Direction.Vertical, false },
		{ Direction.DiagonalS, false },
		{ Direction.DiagonalA, false }
	};
	public override string brain_about
	{
		get
		{
			return "name=\"Whatateam\", author=\"Fanny Tavart, Oscar Stefanini & Ronan Boiteau\", version=\"0.1\", country=\"France\", www=\"http://epitech.eu\"";
		}
	}
	
	public override void brain_init()
	{
		if (width < 5 || height < 5)
		{
			Console.WriteLine("ERROR The board is too small");
			return;
		}
		if (width > MaxBoardSize || height > MaxBoardSize)
		{
			Console.WriteLine("ERROR Maximal board size is " + MaxBoardSize);
			return;
		}
		Console.WriteLine("OK");
	}

	public override void brain_restart()
	{
		for (var x = 0; x < width; x++)
			for (var y = 0; y < height; y++)
				_board[x, y] = 0;

		Console.WriteLine("OK");
	}

	private bool is_out_of_board(int x, int y)
	{
		return !(x >= 0 && y >= 0 && x < width && y < height);
	}

	private bool is_free(int x, int y)
	{
		return !is_out_of_board(x, y) && _board[x, y] == 0;
	}

	private bool is_opponent_piece(int x, int y)
	{
		return !is_out_of_board(x, y) && _board[x, y] == 2;
	}

	private bool is_my_piece(int x, int y)
	{
		return !is_out_of_board(x, y) && _board[x, y] == 1;
	}

	public override void brain_my(int x, int y)
	{
		if (is_free(x, y))
		{
			_board[x, y] = 1;
		}
		else
		{
			Console.WriteLine("ERROR my move [{0},{1}]", x, y);
		}
	}

	public override void brain_opponents(int x, int y)
	{
		if (is_free(x, y))
		{
			_board[x, y] = 2;
			_opponentLastMove.X = x;
			_opponentLastMove.Y = y;
		}
		else
		{
			Console.WriteLine("ERROR opponents's move [{0},{1}]", x, y);
		}
	}

	public override void brain_block(int x, int y)
	{
		if (is_free(x, y))
		{
			_board[x, y] = 3;
		}
		else
		{
			Console.WriteLine("ERROR winning move [{0},{1}]", x, y);
		}
	}

	public override int brain_takeback(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < width && y < height && _board[x, y] != 0)
		{
			_board[x, y] = 0;
			return 0;
		}
		return 2;
	}
	
	private int find_line_horizontal_right(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.Horizontal] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_horizontal_right(x + 1, y, checkPiece, pieces + 1);
		return pieces;
	}
	
	private int find_line_horizontal_left(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.Horizontal] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_horizontal_left(x - 1, y, checkPiece, pieces + 1);
 		return pieces;
	}
	
	private int find_line_vertical_down(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.Vertical] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_vertical_down(x, y + 1, checkPiece, pieces + 1);
 		return pieces;
	}
	
	private int find_line_vertical_up(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.Vertical] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_vertical_up(x, y - 1, checkPiece, pieces + 1);
		return pieces;
	}
	
	private int find_line_diagonal_slash_up(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.DiagonalS] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_diagonal_slash_up(x + 1, y - 1, checkPiece, pieces + 1);
 		return pieces;
	}
	
	private int find_line_diagonal_slash_down(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.DiagonalS] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_diagonal_slash_down(x - 1, y + 1, checkPiece, pieces + 1);
 		return pieces;
	}
	
	private int find_line_diagonal_antislash_up(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.DiagonalA] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_diagonal_antislash_up(x - 1, y - 1, checkPiece, pieces + 1);
 		return pieces;
	}
	
	private int find_line_diagonal_antislash_down(int x, int y, Func<int, int, bool> checkPiece, int pieces)
	{
		if (is_out_of_board(x, y))
			return pieces;
		if (is_free(x, y))
		{
			_canPlay[Direction.DiagonalA] = true;
	 		return pieces;
		}
		if (checkPiece(x, y))
			return find_line_diagonal_antislash_down(x + 1, y + 1, checkPiece, pieces + 1);
 		return pieces;
	}
	
	private Direction find_danger_zone(Position pos)
	{
		foreach (var key in _canPlay.Keys.ToList())
			_canPlay[key] = false;
		var potentialLines = new Dictionary<Direction, int>
		{
			{ Direction.Horizontal, find_line_horizontal_left(pos.X - 1, pos.Y, is_opponent_piece, 0) + find_line_horizontal_right(pos.X + 1, pos.Y, is_opponent_piece, 0) + 1 },
			{ Direction.Vertical, find_line_vertical_down(pos.X, pos.Y + 1, is_opponent_piece, 0) + find_line_vertical_up(pos.X, pos.Y - 1, is_opponent_piece, 0) + 1 },
			{ Direction.DiagonalS, find_line_diagonal_antislash_up(pos.X - 1, pos.Y - 1, is_opponent_piece, 0) + find_line_diagonal_antislash_down(pos.X + 1, pos.Y + 1, is_opponent_piece, 0) + 1 },
			{ Direction.DiagonalA, find_line_diagonal_slash_up(pos.X + 1, pos.Y - 1, is_opponent_piece, 0) + find_line_diagonal_slash_down(pos.X - 1, pos.Y + 1, is_opponent_piece, 0) + 1 }
		};
		foreach (var key in potentialLines.Keys.ToList())
		{
			if (!_canPlay[key])
				potentialLines[key] = 0;
		}
		var max = potentialLines.Max(kvp => kvp.Value);
		if (max >= 3)
			return potentialLines.Where(kvp => kvp.Value == max).Select(kvp => kvp.Key).First();
		return Direction.None;
	}

	private Position play_horizontal_left(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_horizontal_left(x - 1, y, checkPiece);
 		return null;
	}

	private Position play_horizontal_right(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_horizontal_right(x + 1, y, checkPiece);
 		return null;
	}
	
	private Position play_vertical_down(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_vertical_down(x, y + 1, checkPiece);
 		return null;
	}

	private Position play_vertical_up(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_vertical_up(x, y - 1, checkPiece);
 		return null;
	}
	
	private Position play_diagonal_slash_down(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_diagonal_slash_down(x - 1, y + 1, checkPiece);
 		return null;
	}

	private Position play_diagonal_slash_up(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_diagonal_slash_up(x + 1, y - 1, checkPiece);
 		return null;
	}
	
		private Position play_diagonal_antislash_down(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_diagonal_antislash_down(x + 1, y + 1, checkPiece);
 		return null;
	}

	private Position play_diagonal_antislash_up(int x, int y, Func<int, int, bool> checkPiece)
	{
		if (is_out_of_board(x, y))
			return null;
		if (is_free(x, y))
			return new Position(x, y);
		if (checkPiece(x, y))
			return play_diagonal_antislash_up(x - 1, y - 1, checkPiece);
 		return null;
	}
	
	private Position play_at(Direction dir, Position pos, Func<int, int, bool> checkPiece)
	{
		Position ret;
		switch (dir)
		{
			case Direction.Horizontal:
				return (ret = play_horizontal_left(pos.X - 1, pos.Y, checkPiece)) != null ? ret
					: play_horizontal_right(pos.X + 1, pos.Y, checkPiece);
			case Direction.Vertical:
				return (ret = play_vertical_down(pos.X, pos.Y + 1, checkPiece)) != null ? ret
					: play_vertical_up(pos.X, pos.Y - 1, checkPiece);
			case Direction.DiagonalS:
				return (ret = play_diagonal_slash_down(pos.X - 1, pos.Y + 1, checkPiece)) != null ? ret
					: play_diagonal_slash_up(pos.X + 1, pos.Y - 1, checkPiece);
			case Direction.DiagonalA:
				return (ret = play_diagonal_antislash_down(pos.X + 1, pos.Y + 1, checkPiece)) != null ? ret
					: play_diagonal_antislash_up(pos.X - 1, pos.Y - 1, checkPiece);
		}
		return null;
	}
	
	private Position attack_at(Position pos)
	{
		foreach (var key in _canPlay.Keys.ToList())
			_canPlay[key] = false;
		var potentialLines = new Dictionary<Direction, int>
		{
			{ Direction.Horizontal, find_line_horizontal_left(pos.X - 1, pos.Y, is_my_piece, 0) + find_line_horizontal_right(pos.X + 1, pos.Y, is_my_piece, 0) + 1 },
			{ Direction.Vertical, find_line_vertical_down(pos.X, pos.Y + 1, is_my_piece, 0) + find_line_vertical_up(pos.X, pos.Y - 1, is_my_piece, 0) + 1 },
			{ Direction.DiagonalS, find_line_diagonal_antislash_up(pos.X - 1, pos.Y - 1, is_my_piece, 0) + find_line_diagonal_antislash_down(pos.X + 1, pos.Y + 1, is_my_piece, 0) + 1 },
			{ Direction.DiagonalA, find_line_diagonal_slash_up(pos.X + 1, pos.Y - 1, is_my_piece, 0) + find_line_diagonal_slash_down(pos.X - 1, pos.Y + 1, is_my_piece, 0) + 1 }
		};
		foreach (var key in potentialLines.Keys.ToList())
		{
			if (!_canPlay[key])
				potentialLines[key] = 0;
		}
		var max = potentialLines.Max(kvp => kvp.Value);
		return play_at(potentialLines.Where(kvp => kvp.Value == max).Select(kvp => kvp.Key).First(), pos, is_my_piece);
	}

	private Position random_play()
	{
		Position pos;
		var i = 0;
		do
		{
			pos = i < 1000 ? new Position(_rand.Next(4, width - 4), _rand.Next(4, height - 4))
				: new Position(_rand.Next(0, width), _rand.Next(0, height));
			++i;
		} while (!is_free(pos.X, pos.Y));
		return pos;
	}
	
	public override void brain_turn()
	{
		Position pos;
		try
		{
			var dangerZone = find_danger_zone(_opponentLastMove);
			if (dangerZone == Direction.None)
			{
				if (_attackZone == null)
					_attackZone = pos = random_play();
				else
				{
					pos = attack_at(_attackZone);
					if (_rand.Next(1, 4) == 1)
						_attackZone = pos;
				}
			}
			else
				pos = play_at(dangerZone, _opponentLastMove, is_opponent_piece);
		} catch (Exception) {
			pos = null;
			Console.WriteLine("MESSAGE [Whatateam AI] Something went wrong!");
		}
		if (terminate != 0)
			return;
		if (pos == null)
			_attackZone = pos = random_play();
		do_mymove(pos.X, pos.Y);
	}

	public override void brain_end()
	{
	}

	public override void brain_eval(int x, int y)
	{
	}
}
