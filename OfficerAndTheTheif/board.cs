using System;

namespace OfficerAndTheTheif
{
    public class Board
    {
        private Base_Converter bc = new Base_Converter();
        public char[,] board;
        public Vector2 board_size;
        private int num_thiefs = 0;
        public int num_cops = 0;
        public int num_walls = 0;

        public void PrintBoard()
        {
            for (int r = 0; r < this.board.GetLength(0); r++)
            {
                Console.Write(r + " ");
                for (int c = 0; c < this.board.GetLength(1); c++) {
                    Console.Write(this.board[r, c]+" ");
                }
                Console.Write("\n");
            }
            Console.Write("X" + " ");
            for (int c = 0; c < this.board.GetLength(1); c++)
            {
                Console.Write(c + " ");
            }
            Console.WriteLine("\n");
        }
        public void CreateBoard(Vector2 grid_size)
        {
            this.board = new char[grid_size.y, grid_size.x];
            this.board_size = grid_size;
            for (int y = 0; y < grid_size.y; y++)
            {
                for (int x = 0; x < grid_size.x; x++)
                {
                    if (y == 0 || y == grid_size.y - 1 || x == 0 || x == grid_size.x - 1) board[y, x] = 'W';
                    else board[y, x] = '-';
                }
            }
            //PrintBoard(board);
            //Console.ReadLine();
        }

        public void Wall(int x, int y)
        {
            if (this.board[y, x] == 'W')
            {
                this.board[y, x] = '-'; this.num_walls--;
            }
            else if (this.board[y, x] == '-')
            {
                this.board[y, x] = 'W'; this.num_walls++;
            }
        } 

        public void AddContestant(Vector2 position, char type)
        {
            /* 
                       T ~ Thief, O ~ Officer
            */
            if (this.board[position.y, position.x] != type)
            {
                if (this.board[position.y, position.x] == 'T') this.num_thiefs--;
                else if (this.board[position.y, position.x] == 'O') this.num_cops--;
                if (type == 'T') this.num_thiefs++;
                else this.num_cops++;
                this.board[position.y, position.x] = type;
            }
            else
            {
                if (type == 'T') this.num_thiefs--;
                else this.num_cops--;
                this.board[position.y, position.x] = '-';
            }
        }

        public int Move(Vector2 position, Vector2 new_position)
        {
            int re = 0;
            if (this.board[new_position.y, new_position.x] == 'O')
            {
                this.board[position.y, position.x] = '-';
                num_thiefs--;
                return 1;
            }
            if (this.board[new_position.y, new_position.x] == 'T')
            {
                num_thiefs--;
                re = 1;
            }

            this.board[new_position.y, new_position.x] = this.board[position.y, position.x];
            this.board[position.y, position.x] = '-';
            return re;
        }

        public string Hash()
        {
            string hash = "";
            string[] nums = new string[this.board.GetLength(0) - 2];
            for (int i = 0; i < this.board.GetLength(0) - 2; i++)
            {
                for(int j = 0 + 1; j < this.board.GetLength(1) - 1; j++)
                {
                    if (this.board[i + 1,j] == 'W') nums[i] = nums[i] + "1";
                    else nums[i] = nums[i] + "0";
                }
            }
            int a = 0;
            for (int i = 0; i < nums.GetLength(0); i++)
            {
                a = this.bc.ToDec(nums[i], 2);

                hash = hash + this.bc.FromDec(a, 74);
            }
            hash = hash + this.bc.FromDec(this.board.GetLength(1), 74);            
            hash = hash + this.bc.FromDec(this.num_cops, 74);

            return hash;
        }

        public void UnHash(string hash)
        {
           // string[] nums = new string[hash.Length];
            CreateBoard(new Vector2(this.bc.ToDec("" + hash[hash.Length -2], 74), hash.Length));
            string n = "";
            int a = 0;
            for (int i = 0 + 1; i < this.board.GetLength(0) - 1; i++)
            {
                a = this.bc.ToDec(hash[i-1] + "", 74);

                n = this.bc.FromDec(a, 2);
                while (n.Length != this.board.GetLength(1))
                {
                    n = "0" + n;
                }
                for (int j = 0 + 1; j != this.board.GetLength(0) - 1; j++)
                {

                    if (n[j] == '1') this.board[i, j - 1] = 'W';
                    
                }
            }


        }
    }
}
