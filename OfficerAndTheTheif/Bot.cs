using System;
using System.Collections.Generic;
using System.Text;
public enum sys { 
    MachineLearning,
    Recursion, 
    PlayerControlled, 
    TerminalContolled,
    Random
}

namespace OfficerAndTheTheif
{
    public class Bot
    {
        public delegate Vector2 I_func(char[,] board, int round);
        I_func i_move;
        private bool thief;
        public bool located = false;
        public int sight;
        private int rounds = 40;
        public Vector2 position;
        private Vector2 closest_enemy = new Vector2(-1, -1);
        public sys system;
        public List<KeyValuePair<int, char>> moves_played = new List<KeyValuePair<int, char>>();
        private Random r = new Random();
        public int[,] data;

        public Bot(bool thief, int sight, Vector2 position, sys system)
        {
            this.thief = thief;
            this.sight = sight;
            this.position = position;
            this.system = system;
            
            switch (system)
            {
                case sys.MachineLearning: i_move = iMachineLearning; break;
                case sys.Recursion: break;
                case sys.PlayerControlled: break;
                case sys.TerminalContolled: i_move = iTerminal; break;
                case sys.Random: i_move = iRandom; break;
                default: break;
            }
        }

        public void SetRouds(int rounds)
        {
            this.rounds = rounds;
        }

        public void SetData(int[,] data)
        {
            this.data = data;
        }

        public Vector2 MkMove(char[,] board, int round)
        {
            this.rounds = this.rounds - 2;
            return i_move(board, round);
        }
        private Vector2 iTerminal(char[,] board, int round)
        {
            bool mlegal = true;
            Vector2 new_position;
            string i_direction;

            do
            {
                mlegal = true;
                Console.WriteLine("choose direction(WASD): ");
                i_direction = Console.ReadLine();
                new_position = new Vector2(position.x, position.y);

                if (i_direction == "W") new_position.y--;
                else if (i_direction == "A") new_position.x--;
                else if (i_direction == "S") new_position.y++;
                else if (i_direction == "D") new_position.x++;

                else if(i_direction == "WA")
                {
                    new_position.y--; new_position.x--;
                }
                else if (i_direction == "SA")
                {
                    new_position.y++; new_position.x--;
                }
                else if(i_direction == "SD")
                {
                    new_position.y++; new_position.x++;
                }
                else if(i_direction == "WD")
                {
                    new_position.y--; new_position.x++;
                }

                if (board[new_position.y, new_position.x] == 'W') mlegal = false;
                else if (board[new_position.y, new_position.x] == board[position.y, position.x]) mlegal = false;
            } while (!mlegal);
            position = new_position;
            return new_position;
        }
        private Vector2 iRandom(char[,] board, int round)
        {
            string[] moves = new string[] { "U", "L", "D", "R", "UL", "DL", "DR", "UR" };
            bool mlegal = true;
            Vector2 new_position;
            string i_direction;

            do
            {
                mlegal = true;
                i_direction = moves[r.Next(0, moves.Length)];
                new_position = new Vector2(position.x, position.y);

                if (i_direction == "U") new_position.y--;
                else if (i_direction == "L") new_position.x--;
                else if (i_direction == "D") new_position.y++;
                else if (i_direction == "R") new_position.x++;

                else if (i_direction == "UL")
                {
                    new_position.y--; new_position.x--;
                }
                else if (i_direction == "DL")
                {
                    new_position.y++; new_position.x--;
                }
                else if (i_direction == "DR")
                {
                    new_position.y++; new_position.x++;
                }
                else if (i_direction == "UR")
                {
                    new_position.y--; new_position.x++;
                }

                if (board[new_position.y, new_position.x] == 'W') mlegal = false;
                else if (board[new_position.y, new_position.x] == board[position.y, position.x]) mlegal = false;
            } while (!mlegal);
            position = new_position;
            return new_position;
        }        
        
        private string DuplicateStr(string ch, int times)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ch);
            for (int i = 0; i < times; i++)
            {
                sb.Append(ch);
            }
            return ch;
        }


        private Vector2 iMachineLearning(char[,] board, int round)
        {

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (thief)
                    {
                        if (board[i, j] == 'O')
                        {
                            if (this.closest_enemy.x != -1)
                            {
                                if (this.position.Distance(position, new Vector2(j, i)) < this.position.Distance(position, closest_enemy))
                                {
                                    this.closest_enemy.x = j;
                                    this.closest_enemy.y = i;
                                }
                            }
                            else if (this.position.InCircleArea(this.position, sight, new Vector2(j, i)))
                            {
                                located = true;
                                this.closest_enemy.x = j;
                                this.closest_enemy.y = i;
                            }
                        }

                    }
                    else
                    {
                        if (board[i, j] == 'T')
                        {
                            if (this.closest_enemy.x != -1)
                            {
                                if (this.position.Distance(position, new Vector2(j, i)) < this.position.Distance(position, closest_enemy))
                                {
                                    this.closest_enemy.x = j;
                                    this.closest_enemy.y = i;
                                }
                            }
                            else if (this.position.InCircleArea(this.position, sight, new Vector2(j, i)))
                            {
                                located = true;
                                this.closest_enemy.x = j;
                                this.closest_enemy.y = i;
                            }
                        }

                    }
                    
                }
            }
            int combination = -1;
            if(closest_enemy.x == -1)
            {
                for (int i = this.data.GetLength(0) - 1; this.data[i, 2] == -1; i--)
                {
                    if (this.data[i, 0] == this.position.x && this.data[i, 1] == this.position.y)
                    {
                        combination = i;
                        break;
                    }
                        
                }
            }
            else
            {
                for (int i = 0; i < this.data.GetLength(0); i++)
                {
                    if(this.data[i,0] == this.position.x && this.data[i, 1] == this.position.y)
                    {
                        while(!(this.data[i,2] == this.closest_enemy.x && this.data[i,3] == this.closest_enemy.y))
                        {
                            i++;
                        }
                        combination = i;
                        break;
                    }
                }
            }

            string moves =  
                this.DuplicateStr("U", this.data[combination,4]) + 
                this.DuplicateStr("L", this.data[combination, 5]) + 
                this.DuplicateStr("D", this.data[combination, 6]) + 
                this.DuplicateStr("R", this.data[combination, 7]) + 
                this.DuplicateStr("Q", this.data[combination, 8]) + 
                this.DuplicateStr("Y", this.data[combination, 9]) + 
                this.DuplicateStr("C", this.data[combination, 10])+ 
                this.DuplicateStr("E", this.data[combination, 11]);

            bool mlegal;
            Vector2 new_position;
            char i_direction;

            do
            {
                mlegal = true;
                i_direction = moves[r.Next(0, moves.Length)];
                new_position = new Vector2(position.x, position.y);

                if (i_direction == 'U') new_position.y--;
                else if (i_direction == 'L') new_position.x--;
                else if (i_direction == 'D') new_position.y++;
                else if (i_direction == 'R') new_position.x++;

                else if (i_direction == 'Q')
                {
                    new_position.y--; new_position.x--;
                }
                else if (i_direction == 'Y')
                {
                    new_position.y++; new_position.x--;
                }
                else if (i_direction == 'C')
                {
                    new_position.y++; new_position.x++;
                }
                else if (i_direction == 'E')
                {
                    new_position.y--; new_position.x++;
                }

                if (board[new_position.y, new_position.x] == 'W') mlegal = false;
                else if (board[new_position.y, new_position.x] == board[position.y, position.x]) mlegal = false;
            } while (!mlegal);
            position = new_position;

            this.moves_played.Add(new KeyValuePair<int, char>(combination, i_direction));
            this.closest_enemy.x = -1;
            this.closest_enemy.y = -1;
            return new_position;
        }

        public void PrintValues()
        {
            Console.WriteLine("thief = " + thief);
            Console.WriteLine("sight = " + sight);
            Console.WriteLine("sys = " + system);
        }
    }
}
