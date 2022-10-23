using System.Diagnostics.Metrics;
using System;

namespace Game
{

    internal class Program
    {
        class Response
        {
            public bool responseAnswer;
            public Player playerToRaplace = null!;
            
            public Response(bool answer, Player player)
            {
                this.responseAnswer = answer;
                this.playerToRaplace = player;
            }
            public Response(bool answer)
            {
                this.responseAnswer = answer;
            }
        }
        class Position
        {
            public int x;
            public int y;
            public Position(int x = -1, int y=-1)
            {
                this.x = x;
                this.y = y;
            }

        }
        class Player
        {
            static Player[,] field;
            public int id;
            public Position MyPosition = new Position();
            public Position DreamPosition = new Position();
            public Player[] PlayersAroundMe = new Player[4];
            public int LastId = -1;
            public void Print()
            {
                this.WhereIam();
                Console.WriteLine($"My id:{this.id} position:{(MyPosition.x!=-1?$"[{MyPosition.x},{MyPosition.y}]":"out of field")} i`d like to be in[{DreamPosition.x},{DreamPosition.y}]");
            }
            public void Play()
            {
                while(true)
                {
                    WhereIam();
                    WhoRoundMe();
                    Move();
                    Console.Clear();
                    ShowField(Player.field);
                    if(this.DreamPosition.x == this.MyPosition.x && this.DreamPosition.y == this.MyPosition.y)
                    {
                        Console.WriteLine($"{this.id} on [{this.DreamPosition.x},{this.DreamPosition.y}]");
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            public void Move()
            {
                double[] distances = new double[4];
                for(int i =0; i<4; i++)
                {
                    distances[i] = -1;
                }
                int targetIndex = 0;
                for(int i =0; i < 4; i++)
                {
                    if (this.PlayersAroundMe[i] != null && this.PlayersAroundMe[i].id != this.LastId)
                    {
                        distances[i] = PointsDistance(PlayersAroundMe[i].MyPosition, this.DreamPosition);
                    }
                }
                
                for(int i =0; i<4; i++)
                {
                    if(distances.Where(x => x!=-1).Min() == distances[i])
                    {
                        targetIndex = i;
                        break;
                    }
                }
                Player temp = this.PlayersAroundMe[targetIndex];
                Player.field[temp.MyPosition.x, temp.MyPosition.y] = this;
                Player.field[this.MyPosition.x, this.MyPosition.y] = temp;
                this.LastId = temp.id;
                temp.WhereIam();
                this.WhereIam();
            }
            public Response Ask(Player asker)
            {
                this.WhereIam();
                if(this.MyPosition.x != this.DreamPosition.x || this.MyPosition.y != this.DreamPosition.y)
                {
                    return new Response(true, this);
                }
                if(asker.MyPosition.x == this.MyPosition.x)
                {
                    if (this.MyPosition.y > asker.MyPosition.y)
                    {
                        if(this.MyPosition.y +1 == Player.field.GetLength(1))
                        {
                            return new Response(false);
                        }
                        return Player.field[this.MyPosition.x, this.MyPosition.y + 1].Ask(this);
                    }
                    if (this.MyPosition.y < asker.MyPosition.y)
                    {
                        if(this.MyPosition.y -1 == -1)
                        {
                            return new Response(false);
                        }
                        return Player.field[this.MyPosition.x, this.MyPosition.y - 1].Ask(this);
                    }
                }
                if(asker.MyPosition.y == this.MyPosition.y)
                {
                    if (this.MyPosition.x > asker.MyPosition.x)
                    {
                        if(this.MyPosition.x + 1 == Player.field.GetLength(0))
                        {
                            return new Response(false);
                        }
                        return Player.field[this.MyPosition.x + 1, this.MyPosition.y].Ask(this);
                    }
                    if (this.MyPosition.x < asker.MyPosition.x)
                    {
                        if (this.MyPosition.x -1 == -1)
                        {
                            return new Response(false);
                        }
                        return Player.field[this.MyPosition.x -1, this.MyPosition.y].Ask(this);
                    }
                }
                return new Response(true, this);
            }

            public void WhoRoundMe()
            {
                
                if(field == null)
                {
                    return;
                }
                if(MyPosition.x == -1)
                {
                    WhereIam();
                }
                Position[] tempArray = new Position[4];
                for(int i =0; i < tempArray.Length; i++)
                {
                    tempArray[i] = new Position();
                }
                tempArray[0].x = this.MyPosition.x + 1;     //right player position
                tempArray[0].y = this.MyPosition.y;

                tempArray[1].x = this.MyPosition.x;
                tempArray[1].y = this.MyPosition.y+1;       //top player position

                tempArray[2].x = this.MyPosition.x-1;       //left player position
                tempArray[2].y = this.MyPosition.y;         

                tempArray[3].x = this.MyPosition.x;         //bottom player position
                tempArray[3].y = this.MyPosition.y-1;
                for(int i=0; i< 4; i++)
                {
                    if (tempArray[i].x < field.GetLength(0) && tempArray[i].y < field.GetLength(1) && tempArray[i].x>-1&& tempArray[i].y >-1) // are that position exist?
                    {
                        if(Player.field[tempArray[i].x, tempArray[i].y].Ask(this).responseAnswer)
                        {
                            this.PlayersAroundMe[i] = Player.field[tempArray[i].x, tempArray[i].y].Ask(this).playerToRaplace;
                        }
                    }
                }
                //string[] directions = new string[]{"right","top","left","bottom"};
                //Console.Write($"players round {this.id} are:");
                //for (int i=0; i<4; i++)
                //{
                //    if (this.PlayersAroundMe[i] != null)
                //    {       
                //        Console.Write($"{directions[i]} {this.PlayersAroundMe[i].id} ");
                //    }
                //}
                //Console.Write(".\n");
            }
            public void WhereIam()
            {


                if (field == null) 
                {
                    return;
                }
                    for (int i = 0; i < field.GetLength(0); i++)
                    {
                        for (int j = 0; j < field.GetLength(1); j++)
                        {
                            if (field[i, j].Equals(this))
                            {
                                MyPosition.x = i;
                                MyPosition.y = j;
                            }
                        }
                    }
                int width = Player.field.GetLength(0);
                DreamPosition.x = id % width;
                DreamPosition.y = (id - (id % width)) / width;
                return;
            }
            public static void SetField(Player[,] array)
            {
                Player.field = array;
                foreach(var player in Player.field)
                {
                    player.WhereIam();
                }
                
            }
            public Player(int id)
            {
                
                this.id = id;

            }
        }
        static double PointsDistance(Position start, Position end)
        {
            return Math.Sqrt(Math.Pow(end.x - start.x, 2) + Math.Pow(end.y - start.y, 2));
        }
        static void ShowField(Player[,] field)
        {
            for(int j = field.GetLength(1)-1; j > -1; j--)
            {
                Console.Write("|");
                for(int i = 0; i < field.GetLength(0); i++)
                {
                    if (field[i,j].id<10)
                    {
                        Console.Write($"({field[i, j].id} )");
                        continue;
                    }
                    Console.Write($"({field[i, j].id})");
                }
                Console.Write("|\n");
            }

            
        }
        static void ArrayFiller(ref Player[,] array)
        {
            int countOfCells = array.GetLength(0) * array.GetLength(1);
            int[] UniquiNumbers = UniqueNumbersArray(countOfCells, countOfCells);
            int counter = 0;
            for (int i =0; i < array.GetLength(0); i++)
            {
                for(int j =0; j < array.GetLength(1); j++)
                {
                    array[i, j] = new Player(UniquiNumbers[counter]);
                    counter++;
                }
            }
        }
        static int[] UniqueNumbersArray(int lenght, int MaxValue)
        {
            if (lenght  > MaxValue)
            {
                return new int[1];
            }
            int[] result = new int[lenght];
            for(int i=0; i < lenght; i++)
            {
                result[i] = -1;
            }
            int tempNumber = 0;
            for (int i = 0; i < lenght; i++)
            {
                tempNumber = new Random().Next(MaxValue);
                if (result.Contains(tempNumber))
                {
                    i--;
                    continue;
                }
                result[i] = tempNumber;
            }
            return result;
        }
        static void Main(string[] args)
        {  
            Player[,] Field = new Player[5, 5];
            ArrayFiller(ref Field);
            Player.SetField(Field);
            foreach (var player in Field)
            {
                player.Print();
            }
            ShowField(Field);
            Console.WriteLine($"{Field[1,3].id} on [1,3] his info:[{Field[1,3].MyPosition.x},{Field[1,3].MyPosition.y}]");
            Console.WriteLine($"{Field[2,1].id} on [2,1] his info:[{Field[2,1].MyPosition.x},{Field[2,1].MyPosition.y}]");
            Console.WriteLine($"{Field[4,4].id} on [4,4]");

            Field[1, 3].WhoRoundMe();
            Field[2, 1].WhoRoundMe();
            Field[4, 4].WhoRoundMe();
            Field[0, 0].WhoRoundMe();

            for(int i = 0; i < (Field.GetLength(0) * Field.GetLength(1)) -1; i++)
            {
                for(int j=0; j < Field.GetLength(0); j++)
                {
                    for(int k =0; k < Field.GetLength(1); k++)
                    {
                        if (Field[j, k].id == i)
                        {
                            Field[j, k].Play();
                        }
                    }
                }
            }
          
        }  

    }
}
/*
 
 
            int[] tempArray = new int[10];
            for(int i =0; i<10; i++)
            {
                tempArray = UniqueNumbersArray(tempArray.Length, 10);
                Console.Write("[");
                for(int j = 0; j<tempArray.Length; j++)
                {
                    Console.Write($" {tempArray[j]} ");
                }
                Console.Write("]\n");
            }




            Player[,] Field = new Player[5, 5];
            ArrayFiller(ref Field);
            for (int i = 0; i < Field.GetLength(0); i++)
            {
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    Console.WriteLine($"place in array [{i},{j}] id:{Field[i, j].id}");
                }
            }




                int width = Player.field.GetLength(0);
                DreamPosition.x = id % width;
                DreamPosition.y = (id - (id % width)) / width;
 */