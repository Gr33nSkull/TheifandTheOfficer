using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace OfficerAndTheTheif
{

	public class Input
	{
		private int GetNum(string msg)
        {
			string s = "";
			bool cn;
			do
			{
				cn = false;
				Console.WriteLine(msg);
				Console.Write(":");
				s = Console.ReadLine();
				foreach (char c in s)
				{
					if (!("1234567890".Contains(c)))
						cn = true;
				}
			} while (cn);
			return int.Parse(s);
        }

		private Vector2 ConvStrToV2(string s)
        {
			string[] ls = new string[2];
			int j = 0;
			for (int i = 0; i < s.Length; i++)
            {
				if (s.ToUpper()[i] == 'X')
                {
					j++;
					continue;
                }
                if (!("1234567890".Contains(s[i])))
                {
					return new Vector2(-1, -1);
                }
				ls[j] = ls[j] + s[i];
            }

			if(j != 1) return new Vector2(-1, -1);

			return new Vector2(int.Parse(ls[0]), int.Parse(ls[1]));
        }

		private Vector2 PickPos(string msg)
		{
			string i;
			Vector2 pos;
			do
			{
				Console.WriteLine(msg + " (format == numXnum)");
				Console.Write(":");
				i = Console.ReadLine();
				pos = ConvStrToV2(i);
			} while (pos.x == -1);
			return pos;
		}
		private Test1 SetupBot(Test1 idk, bool thief)
        {
			string i = "";
			int tmp;
			Vector2 pos;
			if (thief)
				pos = PickPos("Izberi pozicijo lopova");
			else
				pos = PickPos("Izberi pozicijo policaja");

			do
			{
				if(thief)Console.WriteLine("Izberi vrsto lopova: ");
				else Console.WriteLine("Izberi vrsto policaja: ");
				Console.WriteLine("/ 1 - UmetnaInteligenca / 2 - NaklkjučniPremiki / 3 - JazGaIgram /");
				Console.Write(":");
				i = Console.ReadLine();
			} while (!("123".Contains(i[0])));

			tmp = GetNum("kako dalec lahko vidi ?");

			switch (int.Parse(i[0] + ""))
			{
				case 2: idk.AddDude(thief, tmp, pos, sys.Random); break;
				case 3: idk.AddDude(thief, tmp, pos, sys.TerminalContolled); break;
				default: idk.AddDude(thief, tmp, pos, sys.MachineLearning); break;
			}
			return idk;
		}


		public Input()
		{
			Console.WriteLine("***************");
			Console.WriteLine("*Tag you're it*");
			Console.WriteLine("***************");

			string i = "";
			Vector2 pos;
			int tmp;
			Board board;
			string hash;
			string[] hashes = {"0AAA071", "0sY0Ys091", "000061"};

			while (i != "end")
			{
				board = new Board();
				Console.WriteLine(" 1 - Imam v naprej narejeno plosco");
				Console.WriteLine(" 2 - Sam jo bom ustvaril");
				Console.WriteLine(" 3 - Izbral bi iz vnaprej narejenih");
				Console.Write(":");
				i = Console.ReadLine();

				if (i[0] == '1')
				{
					Console.WriteLine("Prilepi kodo (pusti prazno da ga sam izberem)");
					Console.Write(":");
					hash = Console.ReadLine();

					if (hash != "")
					{
						board.UnHash(hash);
						
						board.PrintBoard();

						Start(board, hash);
					}
					else
                    {
						Random r = new Random();
						board.UnHash(hashes[r.Next(3)]);
						board.PrintBoard();

						Start(board, hash);
					}
				}
				else if (i[0] == '2')
				{
					pos = PickPos("Izberi velikost");

					board.CreateBoard(pos);

					board.PrintBoard();

					while (i != "exit")
					{
						Console.WriteLine("napisi exit ko koncas z zidi");
						Console.WriteLine("Postavi zid sem ( format == numXnum )");
						Console.Write(":");
						i = Console.ReadLine();
						pos = ConvStrToV2(i);
						if (pos.x != -1)
						{
							board.Wall(pos.x, pos.y);
							board.PrintBoard();
							Console.WriteLine("Postavi zid na isto pozicijo da ga izbrises");
						}
					}

					hash = board.Hash();
					board.PrintBoard();
					Console.WriteLine("hash tega board je: " + hash);

					Start(board, hash);
				}
				else if (i[0] == '3')
                {
					for (int h = 0; h< hashes.Length; h++)
                    {
						Console.WriteLine("Board " + h + ":");
						board.UnHash(hashes[h]);
						board.PrintBoard();
                    }
					do
					{
						tmp = GetNum("Izberi: ");
					} while (tmp >= hashes.Length);
					board.UnHash(hashes[tmp]);
					board.PrintBoard();
					Start(board, hashes[tmp]);
				}
			}
		}
		public void Start(Board board, string hash)
		{
			string[] r;
			Thread th;
			string i;


			Test1 idk = new Test1();
			idk.GetBoard(board);

			idk = SetupBot(idk, false);
			idk.PrintBoard();
			idk = SetupBot(idk, true);
			idk.PrintBoard();

			int tmp = GetNum("koliko je omejitev premikov za vsako stran: \n(ce napisete 5 bo vsaka lahko naredila 5 premikov)");
			do
			{
				Console.WriteLine("/ 1 - GledalBiIgro / 2 - NajSeUci/");
				Console.WriteLine("(ce bos igral izberi 3)");
				Console.Write(":");
				i = Console.ReadLine();
			} while (!("12".Contains(i[0])));

			if (i[0] == '2')
			{
				Console.WriteLine("bi radi videli prvo igro?(y/n)");
				Console.Write(":");
				if (Console.ReadLine().ToLower() == "y")
				{
					if (idk.Setup(hash, tmp, true))
						Console.WriteLine("Lopov je pobegnil");
					else Console.WriteLine("Ujel ga je");
					Console.WriteLine("Zacenjanje ucenja");
				}
				Console.WriteLine("Zacenjam");
				idk.Give(hash, tmp);
				th = new Thread(idk.Learn);
				th.Start();
				Console.WriteLine("ucim se...");
				while (i != "stop")
				{
					Thread.Sleep(500);
					Console.WriteLine("Napisi stop ko mislite da se je dovolj naucilo");
					Console.Write(":");
					i = Console.ReadLine();
				}
				var f = File.Create(Directory.GetCurrentDirectory() + @"\Stop");
				f.Close();
				while (th.IsAlive) Thread.Sleep(500);
				Thread.Sleep(1000);
				r = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\r");
				File.Delete(Directory.GetCurrentDirectory() + @"\Stop");

				Console.WriteLine("iz " + (int.Parse(r[0]) + int.Parse(r[1])) + " iger");
				Console.WriteLine("je lopov pobegnil " + r[0] + "-krat");
				Console.WriteLine("Kar je pusti policaju " + r[1] + " aretacij");

				Console.WriteLine("Bi radi videli kaj so se naucili?(y/n):");
				if (Console.ReadLine().ToLower() == "y")
				{
					if (idk.Setup(hash, tmp, true))
						Console.WriteLine("Lopov je pobegnil");
					else Console.WriteLine("Ujel ga je");
				}
			}
			else
			{
				do
				{
					if (idk.Setup(hash, tmp, true))
						Console.WriteLine("Lopov je pobegnil");
					else Console.WriteLine("Ujel ga je");
					Console.WriteLine("ponovi to igro?(y/n): ");
					i = Console.ReadLine();
				} while (i.ToLower() == "y");
			}
		}
	}
}

