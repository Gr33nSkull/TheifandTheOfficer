using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace OfficerAndTheTheif
{
    public class Test1
    {
        private Board board = new Board();
        public bool escaped;
        private string hash;
        private int rounds;
        public Dictionary<Bot, bool> free_thiefs = new Dictionary<Bot, bool>();
        public Bot[] officers = new Bot[3];



        public void GetBoard(Board board)
        {
            this.board = board;
        }

        public void PrintBoard()
        {
            this.board.PrintBoard();
        }

        public void Give(string hash, int rounds)
        {
            this.hash = hash;
            this.rounds = rounds;
        }

        public void Rebuild(Vector2 opos, Vector2 tpos)
        {
            //bool thief, int sight, Vector2 position, sys system
            
            this.board = new Board();
            this.board.UnHash(this.hash);

            int tsight = 0;
            int osight = this.officers[0].sight;
            sys tsys = sys.Random;
            sys osys = this.officers[0].system;
            this.officers[0] = null;
            foreach(var t in free_thiefs)
            {
                tsight = t.Key.sight;
                tsys = t.Key.system;
            }
            this.free_thiefs = new Dictionary<Bot, bool>();
            AddDude(false, osight, opos, osys);
            AddDude(true, tsight, tpos, tsys);
        }

        public void Learn()
        {
            int e = 0;
            int c = 0;
            while (!File.Exists(Directory.GetCurrentDirectory() + @"\Stop"))
            {
                if (Setup(this.hash, this.rounds, false))
                {
                    e++;
                }
                else
                {
                    c++;
                }
            }
            string[] s = new string[2] { e + "", c + "" };
            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\r", s);
        }

            public bool Setup(string hash, int rounds, bool spectate)
        {
            Vector2 tpos = new Vector2(1, 1);
            foreach (var t in free_thiefs)
            {
                tpos = t.Key.position;
            }
            Vector2 opos = this.officers[0].position;
            this.hash = this.board.Hash();
            // to je bil najhitrejsi nacin da resim neznano napako ker mi zmankuje casa
            string s = "";
            for (int i = 0; i < this.hash.Length; i++)
            {
                if (i == this.hash.Length - 1) s = s + "1";
                else s = s + this.hash[i];
            }//
            this.hash = s;
            MkFiles(this.hash);
            SetOfficers(GetOfcData(this.hash));
            SetThiefes(GetThfData(this.hash));

            this.escaped = TestPlay(rounds, spectate);
            SaveOfcData(officers[0].data, officers[0].moves_played, this.escaped);
            foreach (var t in this.free_thiefs)
            {
                if (t.Value)
                {
                    SaveThfData(t.Key.data, t.Key.moves_played, true);
                }
                else SaveThfData(t.Key.data, t.Key.moves_played, false);
            }
            Rebuild(opos, tpos);
            return escaped;
        }


        public bool TestSetup(bool spectator)
        {
            board.CreateBoard(new Vector2(6, 6));
            this.AddDude(false, 5, new Vector2(1, 1), sys.MachineLearning);
            this.AddDude(true, 5, new Vector2(4, 4), sys.MachineLearning);
            this.hash = board.Hash();

            MkFiles(this.hash);
            SetOfficers(GetOfcData(this.hash));
            SetThiefes(GetThfData(this.hash));
            this.escaped = TestPlay(7, spectator);
            SaveOfcData(officers[0].data, officers[0].moves_played, this.escaped);
            Console.WriteLine("saving");
            foreach(var t in this.free_thiefs)
            {
                if (t.Value)
                {
                    Console.WriteLine("ssaving");
                    SaveThfData(t.Key.data, t.Key.moves_played, true);
                }
                else SaveThfData(t.Key.data, t.Key.moves_played, false);
            }

            return escaped;
        }



        public void AddDude(bool thief, int sight, Vector2 position, sys system)
        {
            if (thief)
            {
                free_thiefs.Add(new Bot(true, sight, position, system), true);
                board.AddContestant(position, 'T');
            }
            else
            {
                for (int i = 0; i != 3; i++)
                {
                    if (officers[0] == null)
                    {
                        officers[0] = new Bot(false, sight, position, system);
                        board.AddContestant(position, 'O');
                        break;
                    }
                }
            }

        }

        private void PrintData(int[,] data)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 4; j < data.GetLength(1); j++)
                {
                    if (data[i, j] > 0 && data[i, 2] != -1)
                        Console.WriteLine(data[i, j]);
                }
            }
            return;
        }


        private void SetOfficers(int[,] data)
        {
            /*for (int i = 0; i < officers.Length; i++)
            {
                if (officers[i] != null)
                {
                    if (officers[i].system == sys.MachineLearning)
                    {
                        officers[i].SetData(data);
                    }
                }
            }*/
            if (officers[0].system == sys.MachineLearning)
            {
                officers[0].SetData(data);
            }

        }

        private void SetThiefes(int[,] data)
        {
            foreach (var thief in free_thiefs)
            {
                if (thief.Key.system == sys.MachineLearning)
                {
                    thief.Key.SetData(data);
                }
            }
        }


        private int[,] GetOfcData(string hash)
        {
            return Deserialize2DArray(File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Data\" + hash + ".ofc"));
        }

        private int[,] GetThfData(string hash) 
        { 
            return Deserialize2DArray(File.ReadAllLines(Directory.GetCurrentDirectory() + @"\Data\" + hash + ".thf"));
        }

        private void MkFiles(string hash)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Data\" + hash + ".ofc"))
            {
                CreateDataFile();   
            }
        }

        private string[] Serialize2DArray(int[,] arr)
        {
            string[] sarr = new string[arr.GetLength(0)];
            string str = "";

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    str = str + arr[i, j] + ",";
                }
                sarr[i] = str;
                str = "";
            }

            return sarr;
        }

        private int[,] Deserialize2DArray(string[] sarr)
        {
            int count = 0;
            foreach (char c in sarr[0])
                if (c == ',') count++;
            int[,] arr = new int[sarr.Length, count];
            int ind = 0;
            string num = "";
            for (int i = 0; i < sarr.Length; i++)
            {
                for (int j = 0; j < sarr[i].Length; j++)
                {
                    if (sarr[i][j] == ',')
                    {

                        arr[i, ind] = int.Parse(num);
                        num = "";
                        ind++;
                    }
                    else num = num + sarr[i][j];
                }
                ind = 0;
            }

            return arr;
        }

        private void CreateDataFile()
        {
            int first1 = 1;
            int first2 = 1;
            int cells = this.board.board.Length;
            //this.board.PrintBoard();
            for (int i = 0; i < this.board.num_cops; i++)
            {
                first1 = first1 * (cells - i);
            }
            for (int i = -1; i < this.board.num_cops; i++)
            {
                if(i == -1)
                first2 = first2 * (cells - 0);
                else first2 = first2 * (cells - i);
            }
            //Console.WriteLine("fs1 "+ first1);

            int[,] arr = new int[first1 + first2 + 1, 12 + ((this.board.num_cops - 1) * 2)];
            if (this.board.num_cops > 1)
            {
                Console.WriteLine("Work In Progress");
                int j = 11 + ((board.num_cops - 1) * 2);
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    arr[i, j] = 0;
                }
            }
            else
            {
                arr[0, 3] = 1;
                for (int i = 1;i <= first2; i++)
                {
                    if (arr[i-1,3] + 1 >= this.board.board_size.y)
                    {
                        if (arr[i-1,2] + 1 >= this.board.board_size.x)
                        {
                            if (arr[i - 1, 1] + 1 >= this.board.board_size.y)
                            {
                                if (arr[i - 1, 0] + 1 >= this.board.board_size.x)
                                {
                                    break;
                                }
                                else
                                {
                                    arr[i, 0] = arr[i - 1, 0] + 1;
                                    arr[i, 1] = 0;
                                    arr[i, 2] = 0;
                                    arr[i, 3] = 0;
                                }
                            }
                            else
                            {
                                arr[i, 0] = arr[i - 1, 0];
                                arr[i, 1] = arr[i - 1, 1] + 1;
                                arr[i, 2] = 0;
                                arr[i, 3] = 0;
                            }
                        }
                        else
                        {
                            arr[i, 0] = arr[i - 1, 0];
                            arr[i, 1] = arr[i - 1, 1];
                            arr[i, 2] = arr[i - 1, 2] + 1;
                            arr[i, 3] = 0;
                        }
                    }
                    else
                    {
                        arr[i,0] = arr[i - 1,0];
                        arr[i,1] = arr[i - 1,1];
                        arr[i,2] = arr[i - 1,2];
                        arr[i,3] = arr[i - 1,3] + 1;
                    }
                    /*if (arr[i, 0] == arr[i, 2] && arr[i, 1] == arr[i, 3])
                    {
                        if (arr[i, 3] + 1 >= this.board.board_size.y)
                        {
                            if (arr[i, 2] + 1 >= this.board.board_size.x)
                            {
                                break;
                            }
                            else
                            {
                                arr[i, 2] = arr[i - 1, 2] + 1;
                                arr[i, 3] = 0;
                            }

                        }
                        else
                        {
                            arr[i, 3] = arr[i, 3] + 1;
                        }
                    }*/
                }
                arr[first2 + 1, 2] = -1;
                arr[first2 + 1, 3] = -1;
                for (int i = first2 + 2; i < arr.GetLength(0); i++)
                {
                    if (arr[i - 1,1] + 1 >= this.board.board_size.y)
                    {
                        if (arr[i - 1, 0] + 1 >= this.board.board_size.x) break;
                        else
                        {
                            arr[i, 0] = arr[i - 1, 0] + 1;
                            arr[i, 1] = 0;
                            arr[i, 2] = -1;
                            arr[i, 3] = -1;
                        }

                    }
                    else
                    {
                        arr[i, 0] = arr[i - 1, 0];
                        arr[i, 1] = arr[i - 1, 1] + 1;
                        arr[i, 2] = -1;
                        arr[i, 3] = -1;
                    }
                }

            }

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 4; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = 20;
                }
            }

            string[] sarr = this.Serialize2DArray(arr);
            //Console.WriteLine(Directory.GetCurrentDirectory());
            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Data\" + this.board.Hash() + ".thf", sarr);
            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Data\" + this.board.Hash() + ".ofc", sarr);

        }

        private void SaveOfcData(int[,] data, List<KeyValuePair<int, char>> moves_played, bool escaped)
        {
            string moves = "ULDRQYCE";

            if (!escaped)
            {

                foreach (var move in moves_played)
                {
                    data[move.Key, moves.IndexOf(move.Value) + 4] += 1;
                }
                
            }
            else
            {
                foreach (var move in moves_played)
                {
                    data[move.Key, moves.IndexOf(move.Value) + 4] -= 1;
                }
            }
            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Data\" + hash + ".ofc", this.Serialize2DArray(data));
        }

        private void SaveThfData(int[,] data, List<KeyValuePair<int, char>> moves_played, bool escaped)
        {
            string moves = "ULDRQYCE";

            if (escaped)
            {
                foreach (var move in moves_played)
                {
                    data[move.Key, moves.IndexOf(move.Value) + 4] += 1;
                }
                
            }

            else
            {
                foreach (var move in moves_played)
                {
                    data[move.Key, moves.IndexOf(move.Value) + 4] -= 1;
                }
            }

            File.WriteAllLines(Directory.GetCurrentDirectory() + @"\Data\" + this.hash + ".thf", this.Serialize2DArray(data));
            /*foreach (var thief in this.free_thiefs)
            {
                if (escaped)
                {
                    Console.WriteLine("free");
                    foreach (var move in thief.Key.moves_played)
                    {
                        data[move.Key, moves.IndexOf(move.Value) + 4]++;
                    }
                }
            }*/
        }
        private bool TestPlay(int rounds, bool spectator)
        {
            Bot[] caught = new Bot[15];
            int b = 0;
            bool br = true;
            for (int i = 0; i < rounds; i++)
            {
                if (spectator)
                {
                    this.board.PrintBoard();
                    System.Threading.Thread.Sleep(1000);
                }
                foreach (var theif in free_thiefs)
                {
                    if (theif.Value) br = false;
                }
                if (br) return false;
                br = true;

                foreach (var thief in free_thiefs)
                {
                    if (thief.Value)
                    {
                        if (board.Move(thief.Key.position, thief.Key.MkMove(this.board.board, i)) == 1)
                        {
                            caught[b] = thief.Key;
                            //Console.WriteLine(i);
                            b++;
                        }
                        
                    }
                }
                
                while (b != 0)
                {
                    free_thiefs[caught[--b]] = false;
                }

                if (spectator)
                {
                    this.board.PrintBoard();
                    System.Threading.Thread.Sleep(1000);
                }

                foreach (var theif in free_thiefs)
                {
                    if (theif.Value) br = false;
                }
                if (br) return false;
                br = true;

                foreach (var officer in officers)
                {
                    if (officer != null)
                    {
                        if (board.Move(officer.position, officer.MkMove(this.board.board, i)) == 1)
                        {
                            foreach (var thief in free_thiefs)
                            {
                                if (thief.Key.position.x == officer.position.x && thief.Key.position.y == officer.position.y)
                                {
                                    caught[b] = thief.Key;
                                   // Console.WriteLine(i);
                                    b++;
                                }
                            }
                        }
                        
                    }
                }
                while (b != 0)
                {
                    free_thiefs[caught[--b]] = false;
                }
            }
            return true;
        }

    }
}
