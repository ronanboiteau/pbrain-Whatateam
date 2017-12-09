using System;
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
	Vertical,
	VerticalUp,
	VerticalDown,
	Horizontal,
	HorizontalLeft,
	HorizontalRight,
	DiagonalSlash,
	DiagonalSlashUp,
	DiagonalSlashDown,
	DiagonalAntislash,
	DiagonalAntislashUp,
	DiagonalAntislashDown
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
		{ Direction.VerticalUp, false },
		{ Direction.VerticalDown, false },
		{ Direction.HorizontalLeft, false },
		{ Direction.HorizontalRight, false },
		{ Direction.DiagonalSlashUp, false },
		{ Direction.DiagonalSlashDown, false },
		{ Direction.DiagonalAntislashUp, false },
		{ Direction.DiagonalAntislashDown, false }
	};
	private Dictionary<Direction, bool> _hasHole = new Dictionary<Direction, bool>
	{
		{ Direction.VerticalUp, false },
		{ Direction.VerticalDown, false },
		{ Direction.HorizontalLeft, false },
		{ Direction.HorizontalRight, false },
		{ Direction.DiagonalSlashUp, false },
		{ Direction.DiagonalSlashDown, false },
		{ Direction.DiagonalAntislashUp, false },
		{ Direction.DiagonalAntislashDown, false }
	};
	private Dictionary<Direction, int> _pieces = new Dictionary<Direction, int>
		{
			{ Direction.VerticalUp, 0 },
			{ Direction.VerticalDown, 0 },
			{ Direction.HorizontalLeft, 0 },
			{ Direction.HorizontalRight, 0 },
			{ Direction.DiagonalSlashUp, 0 },
			{ Direction.DiagonalSlashDown, 0 },
			{ Direction.DiagonalAntislashUp, 0 },
			{ Direction.DiagonalAntislashDown, 0 }
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
	
	private int find_line_horizontal_right(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.HorizontalRight] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.HorizontalRight] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			++x;
			++iterations;
		}
		return pieces;
	}
	
	private int find_line_horizontal_left(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.HorizontalLeft] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.HorizontalLeft] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			--x;
			++iterations;
		}
		return pieces;
	}

	private int find_line_vertical_up(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.VerticalUp] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.VerticalUp] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			--y;
			++iterations;
		}
		return pieces;
	}

	private int find_line_vertical_down(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.VerticalDown] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.VerticalDown] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			++y;
			++iterations;
		}
		return pieces;
	}
	
	private int find_line_diagonal_slash_up(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.DiagonalSlashUp] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.DiagonalSlashUp] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			++x;
			--y;
			++iterations;
		}
		return pieces;
	}
	
	private int find_line_diagonal_slash_down(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.DiagonalSlashDown] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.DiagonalSlashDown] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			--x;
			++y;
			++iterations;
		}
		return pieces;
	}

	private int find_line_diagonal_antislash_up(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.DiagonalAntislashUp] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.DiagonalAntislashUp] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			--x;
			--y;
			++iterations;
		}
		return pieces;
	}
	
	private int find_line_diagonal_antislash_down(int x, int y, Func<int, int, bool> checkPiece)
	{
		var pieces = 0;
		var iterations = 0;
		var isLastFree = false;
		while (iterations < 5)
		{
			if (is_out_of_board(x, y))
				return pieces;
			if (is_free(x, y))
			{
				_canPlay[Direction.DiagonalAntislashDown] = true;
				if (isLastFree)
					return pieces;
				isLastFree = true;
			}
			else if (checkPiece(x, y))
			{
				if (isLastFree)
					_hasHole[Direction.DiagonalAntislashDown] = true;
				isLastFree = false;
				++pieces;
			}
			else
				return pieces;
			++x;
			++y;
			++iterations;
		}
		return pieces;
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

	private Position play_horizontal(Position pos, Func<int, int, bool> checkPiece)
	{
		if (_canPlay[Direction.HorizontalLeft] && !_canPlay[Direction.HorizontalRight])
			return play_horizontal_left(pos.X, pos.Y, checkPiece);
		if (_canPlay[Direction.HorizontalRight] && !_canPlay[Direction.HorizontalLeft])
			return play_horizontal_right(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.HorizontalLeft] && _canPlay[Direction.HorizontalLeft])
			return play_horizontal_left(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.HorizontalRight] && _canPlay[Direction.HorizontalRight])
			return play_horizontal_right(pos.X, pos.Y, checkPiece);
		return _pieces[Direction.HorizontalLeft] >= _pieces[Direction.HorizontalRight]
			? play_horizontal_left(pos.X, pos.Y, checkPiece)
			: play_horizontal_right(pos.X, pos.Y, checkPiece);
	}

	private Position play_vertical(Position pos, Func<int, int, bool> checkPiece)
	{
		if (_canPlay[Direction.VerticalUp] && !_canPlay[Direction.VerticalDown])
			return play_vertical_up(pos.X, pos.Y, checkPiece);
		if (_canPlay[Direction.VerticalDown] && !_canPlay[Direction.VerticalUp])
			return play_vertical_down(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.VerticalUp] && _canPlay[Direction.VerticalUp])
			return play_vertical_up(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.VerticalDown] && _canPlay[Direction.VerticalDown])
			return play_vertical_down(pos.X, pos.Y, checkPiece);
		return _pieces[Direction.VerticalUp] >= _pieces[Direction.VerticalDown]
			? play_vertical_up(pos.X, pos.Y, checkPiece)
			: play_vertical_down(pos.X, pos.Y, checkPiece);
	}

	private Position play_diagonal_slash(Position pos, Func<int, int, bool> checkPiece)
	{
		if (_canPlay[Direction.DiagonalSlashUp] && !_canPlay[Direction.DiagonalSlashDown])
			return play_diagonal_slash_up(pos.X, pos.Y, checkPiece);
		if (_canPlay[Direction.DiagonalSlashDown] && !_canPlay[Direction.DiagonalSlashUp])
			return play_diagonal_slash_down(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.DiagonalSlashUp] && _canPlay[Direction.DiagonalSlashUp])
			return play_diagonal_slash_up(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.DiagonalSlashDown] && _canPlay[Direction.DiagonalSlashDown])
			return play_diagonal_slash_down(pos.X, pos.Y, checkPiece);
		return _pieces[Direction.DiagonalSlashUp] >= _pieces[Direction.DiagonalSlashDown]
			? play_diagonal_slash_up(pos.X, pos.Y, checkPiece)
			: play_diagonal_slash_down(pos.X, pos.Y, checkPiece);
	}

	private Position play_diagonal_antislash(Position pos, Func<int, int, bool> checkPiece)
	{
		if (_canPlay[Direction.DiagonalAntislashUp] && !_canPlay[Direction.DiagonalAntislashDown])
			return play_diagonal_antislash_up(pos.X, pos.Y, checkPiece);
		if (_canPlay[Direction.DiagonalAntislashDown] && !_canPlay[Direction.DiagonalAntislashUp])
			return play_diagonal_antislash_down(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.DiagonalAntislashUp] && _canPlay[Direction.DiagonalAntislashUp])
			return play_diagonal_antislash_up(pos.X, pos.Y, checkPiece);
		if (_hasHole[Direction.DiagonalAntislashDown] && _canPlay[Direction.DiagonalAntislashDown])
			return play_diagonal_antislash_down(pos.X, pos.Y, checkPiece);
		return _pieces[Direction.DiagonalAntislashUp] >= _pieces[Direction.DiagonalAntislashDown]
			? play_diagonal_antislash_up(pos.X, pos.Y, checkPiece)
			: play_diagonal_antislash_down(pos.X, pos.Y, checkPiece);
	}

	private Position play_at(Direction dir, Position pos, Func<int, int, bool> checkPiece)
	{
		switch (dir)
		{
			case Direction.Horizontal:
				return play_horizontal(pos, checkPiece);
			case Direction.Vertical:
				return play_vertical(pos, checkPiece);
			case Direction.DiagonalSlash:
				return play_diagonal_slash(pos, checkPiece);
			case Direction.DiagonalAntislash:
				return play_diagonal_antislash(pos, checkPiece);
		}
		return null;
	}
	
	private Position random_play()
	{
		Position pos;
		var i = 0;
		do
		{
			pos = i < 100 ? new Position(_rand.Next(4, width - 4), _rand.Next(4, height - 4))
				: new Position(_rand.Next(0, width), _rand.Next(0, height));
			++i;
		} while (!is_free(pos.X, pos.Y));
		return pos;
	}
	
	private Dictionary<Direction, int> get_dangerousness_analysis(Position pos, Func<int, int, bool> checkPiece)
	{
		foreach (var key in _canPlay.Keys.ToList())
			_canPlay[key] = false;
		foreach (var key in _hasHole.Keys.ToList())
			_hasHole[key] = false;
		_pieces[Direction.VerticalUp] = find_line_vertical_up(pos.X, pos.Y, checkPiece);
		_pieces[Direction.VerticalDown] = find_line_vertical_down(pos.X, pos.Y, checkPiece);
		_pieces[Direction.HorizontalLeft] = find_line_horizontal_left(pos.X, pos.Y, checkPiece);
		_pieces[Direction.HorizontalRight] = find_line_horizontal_right(pos.X, pos.Y, checkPiece);
		_pieces[Direction.DiagonalSlashUp] = find_line_diagonal_slash_up(pos.X, pos.Y, checkPiece);
		_pieces[Direction.DiagonalSlashDown] = find_line_diagonal_slash_down(pos.X, pos.Y, checkPiece);
		_pieces[Direction.DiagonalAntislashUp] = find_line_diagonal_antislash_up(pos.X, pos.Y, checkPiece);
		_pieces[Direction.DiagonalAntislashDown] = find_line_diagonal_antislash_down(pos.X, pos.Y, checkPiece);
		var potentialLines = new Dictionary<Direction, int>
		{
			{ Direction.Vertical, !_canPlay[Direction.VerticalUp] && !_canPlay[Direction.VerticalDown] ? 0 : _pieces[Direction.VerticalUp] + _pieces[Direction.VerticalDown] - 1 },
			{ Direction.Horizontal, !_canPlay[Direction.HorizontalLeft] && !_canPlay[Direction.HorizontalRight] ? 0 : _pieces[Direction.HorizontalLeft] + _pieces[Direction.HorizontalRight] - 1 },
			{ Direction.DiagonalSlash, !_canPlay[Direction.DiagonalSlashUp] && !_canPlay[Direction.DiagonalSlashDown] ? 0 : _pieces[Direction.DiagonalSlashUp] + _pieces[Direction.DiagonalSlashDown] - 1},
			{ Direction.DiagonalAntislash, !_canPlay[Direction.DiagonalAntislashUp] && !_canPlay[Direction.DiagonalAntislashDown] ? 0 : _pieces[Direction.DiagonalAntislashUp] + _pieces[Direction.DiagonalAntislashDown] - 1 }
		};
		return potentialLines;
	}

	private Direction check_defense_needs()
	{
		Console.WriteLine("MESSAGE [Whatateam] Analyzing from pos [" + _opponentLastMove.X + "," + _opponentLastMove.Y + "]");
		var potentialLines = get_dangerousness_analysis(_opponentLastMove, is_opponent_piece);
		var max = potentialLines.Max(kvp => kvp.Value);
		Console.WriteLine("MESSAGE [Whatateam] Most dangerous zone: " + potentialLines.Where(kvp => kvp.Value == max).Select(kvp => kvp.Key).First()
		                  + " (" + max + ")");
//		if (max >= 3 || (max == 2 && _rand.Next(0, 1) == 0))
		if (max >= 3)
			return potentialLines.Where(kvp => kvp.Value == max).Select(kvp => kvp.Key).First();
		return Direction.None;
	}

	private Position attack_in_zone()
	{
		var potentialLines = get_dangerousness_analysis(_attackZone, is_my_piece);
		var max = potentialLines.Max(kvp => kvp.Value);
		var dir = potentialLines.Where(kvp => kvp.Value == max).Select(kvp => kvp.Key).First();
		return play_at(dir, _attackZone, is_my_piece);
	}
	
	public override void brain_turn()
	{
		Position pos;
		try
		{
			var dangerZone = check_defense_needs();
			if (dangerZone != Direction.None)
			{
				Console.WriteLine("MESSAGE [Whatateam] Defending...");
				pos = play_at(dangerZone, _opponentLastMove, is_opponent_piece);
			}
			else
			{
				Console.WriteLine("MESSAGE [Whatateam] Attacking...");
				if (_attackZone == null)
					_attackZone = pos = random_play();
				else
				{
					pos = attack_in_zone();
					_attackZone = pos;
				}
			}
		} catch (Exception) {
			pos = null;
			Console.WriteLine("MESSAGE [Whatateam] Something went wrong!");
		}
		if (terminate != 0)
			return;
		if (pos == null)
		{
			Console.WriteLine("MESSAGE [Whatateam] Something went wrong! Random play...");
			_attackZone = pos = random_play();
		}
		do_mymove(pos.X, pos.Y);
	}

	public override void brain_end()
	{
	}

	public override void brain_eval(int x, int y)
	{
	}
}
