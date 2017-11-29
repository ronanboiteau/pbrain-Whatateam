using System;
using System.Threading;

internal class GomocupEngine : GomocupInterface
{
	private const int BoardSize = 19;
	int[,] board = new int[BoardSize, BoardSize];
	Random rand = new Random();

	public override string brain_about
	{
		get
		{
			return "name=\"Whatateam\", author=\"Fanny Tavart, Oscar Stefanini & Ronan Boiteau\", version=\"0.1\", country=\"France\", www=\"http://epitech.eu\"";
		}
	}

	public override void brain_init()
	{
		if (width != BoardSize || height != BoardSize)
		{
			Console.WriteLine("ERROR Board size has to be " + BoardSize + "x" + BoardSize);
			return;
		}
		Console.WriteLine("OK");
	}

	public override void brain_restart()
	{
		for (var x = 0; x < width; x++)
			for (var y = 0; y < height; y++)
				board[x, y] = 0;

		Console.WriteLine("OK");
	}

	private bool is_free(int x, int y)
	{
		return x >= 0 && y >= 0 && x < width && y < height && board[x, y] == 0;
	}

	public override void brain_my(int x, int y)
	{
		if (is_free(x, y))
		{
			board[x, y] = 1;
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
			board[x, y] = 2;
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
			board[x, y] = 3;
		}
		else
		{
			Console.WriteLine("ERROR winning move [{0},{1}]", x, y);
		}
	}

	public override int brain_takeback(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < width && y < height && board[x, y] != 0)
		{
			board[x, y] = 0;
			return 0;
		}
		return 2;
	}

	public override void brain_turn()
	{
		int x, y;
		var i = -1;
		do
		{
			x = rand.Next(width);
			y = rand.Next(height);
			i++;
			if (terminate != 0) return;
		} while (!is_free(x, y));

		if (i > 1) Console.WriteLine("DEBUG {0} coordinates didn't hit an empty field", i);
		do_mymove(x, y); // calls brain_my(x, y)
	}

	public override void brain_end()
	{
	}

	public override void brain_eval(int x, int y)
	{
	}
}
