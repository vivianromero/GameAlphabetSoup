using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoupGame
{
    static class Program
    {
       
        static int[,] dirs;       
        //arreglo de las orientaciones que pueden estar las palabras
        //    = {
        //    {1,0},{0,1},{1,1},{1,-1},{-1,0},{0,-1},{-1,-1},{-1,1}
        //};


        static int nRows;
        static int nCols;
        static int gridSize;
        readonly static Random rand = new Random();


        class Grid
        {
            public char[,] Cells = new char[nRows, nCols];       
            public List<string> words = new List<string>();    
            public List<string> Solutions = new List<string>();    
            public int NumAttempts;    
        }

       
         static void Main(string[] args)
        {
            TakeParameters();  
            Play(PrintResult(Createwordsearch(Readwords("unixdict.txt"))));
        }

        //Lee las palabras de un archivo especifico. Solo tomara las palabras que quepan en el Grid
        private static List<string> Readwords(string filename)
        {
            int maxLen = Math.Max(nRows, nCols);
            return System.IO.File.ReadAllLines(filename).Select(s => s.Trim().ToLower())
                .Where(s => Regex.IsMatch(s, "^[a-z]{3," + maxLen + "}$")).ToList();
        }


        private static Grid Createwordsearch(List<string> words)
        {
            
            int numAttempts = 0;
            
            //se hace un numero de intentos para obtener un Grid con todas las palabras.
            while (++numAttempts < 1000)
            {
                words.shuffle();
                var grid = new Grid();
                grid.words = words;
                // int messageLen = PlaceMessage(grid, GetTarget(words));
                // int target = gridSize - messageLen;
                int cellsFilled = 0;                   

                foreach (var word in words)
                {
                    cellsFilled += TryPlaceword(grid, word);

                    if (grid.Solutions.Count == words.Count())
                    {
                        grid.NumAttempts = numAttempts;
                        return grid;
                    }
                    //grid is full but didnt pack enough words
                    // else break;
                    //}

                }

            }
            return null;
        }

        private static int TryPlaceword(Grid grid, string word)
        {
            /*se escoge la direccion y la posicion inicial para la palabra de manera aleatoria
             nuevamente esto aumenta aun mas la probabilidad de tener Grids unicos */
            int randDir = rand.Next(dirs.GetLength(0));
            int randPos = rand.Next(gridSize);

            for (int dir = 0; dir < dirs.GetLength(0); dir++)
            {
                dir = (dir + randDir) % dirs.GetLength(0);

                for (int pos = 0; pos < gridSize; pos++)
                {
                    pos = (pos + randPos) % gridSize;
                    int lettersPlaced = TryLocation(grid, word, dir, pos);
                    if (lettersPlaced > 0)
                        return lettersPlaced;
                 }

            }
            return 0;
        }

        private static int TryLocation(Grid grid, string word, int dir, int pos)
        {
            int r = pos / nCols;
            int c = pos % nCols;
            int len = word.Length;

            //chequeo de las fronteras, que la palabra quepa desde dicha posicion y direccion.
            if ((dirs[dir, 0] == 1 && (len + c) > nCols)
                || (dirs[dir, 0] == -1 && (len - 1) > c)
                || (dirs[dir, 1] == 1 && (len + r) > nRows)
                || (dirs[dir, 1] == -1 && (len - 1) > r)
                )
                return 0;

            int rr, cc, i, overlaps = 0;

            //chequeo de las celdas, que exista una letra ya ubicada en ella y no coincida
            for (i = 0, rr = r, cc = c; i < len; i++)
            {
                if (grid.Cells[rr, cc] != 0 && grid.Cells[rr, cc] != word[i])
                {
                    return 0;
                }

                cc += dirs[dir, 0];
                rr += dirs[dir, 1];
            }

            //introduccion al grid de la palabra, pueden existir aclopamiento de varias letras para 
			//formar diferentes palabras
            for (i = 0, rr = r, cc = c; i < len; i++)
            {
                if (grid.Cells[rr, cc] == word[i]) overlaps++;
                else grid.Cells[rr, cc] = word[i];

                if (i < len - 1)
                {
                    cc += dirs[dir, 0];
                    rr += dirs[dir, 1];
                }
            }


            int lettersPlaced = len - overlaps;
            if (lettersPlaced > 0)
            {
                /*
                  esta son las coordenadas donde se encuentran las palabras, columna y fila inicial y 
				  final reespectivamente son mostradas para hacer mas facil la prueba para ustedes, 
				  encotrar palabras cortas en esas sopas no es facil.
                 */

                grid.Solutions.Add($"{word,-10} ({c},{r})({cc},{rr})");
            }
            return lettersPlaced;
        }
                    

        /*Reorganiza de forma aleatoria las palabras en la lista, para que no siempre se empiece de 
		igual manera, da mayor probabilidad a encontrar diferentes Grids para las mismas palabras */
        private static void shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;

            }
        }


        //Dibuja el Grid, los intentos que le tomo crearlo, las palabras y sus posiciones para la prueba facil.
        private static Grid PrintResult(Grid grid)
        {
            if (grid == null || grid.NumAttempts == 0)
            {
                Console.WriteLine("No Grid to Display");
                Console.ReadLine();

                return grid;
            }
            int size = grid.Solutions.Count;
            Console.WriteLine("Attempts: " + grid.NumAttempts);
            Console.WriteLine("Number of words: " + size);

            Console.Write("\n       ");
            for (int c = 0; c < nCols; c++)
                if (c < 10) Console.Write(" {0} |", c);
                else Console.Write(" {0}|", c);

            Console.Write("\n");


            for (int r = 0; r < nRows; r++)
            {
                if (r >= 10) Console.Write("   {0}| ", r);
                else Console.Write("    {0}| ", r);
                for (int c = 0; c < nCols; c++)
                    Console.Write(" {0} |", grid.Cells[r, c]);


                Console.WriteLine("");

            }
            Console.WriteLine("\n");

            for (int i = 0; i < size - 1; i += 2) {
                Console.WriteLine("{0}      {1}", grid.Solutions[i], grid.Solutions[i + 1]);
            }

            if (size % 2 == 1)
                Console.WriteLine(grid.Solutions[size - 1]);

            return grid;
        }

        private static void TakeParameters()
        {
            
            //Se introducen los primeros parametros, numero de filas y columnas.
            #region
            Console.WriteLine("Introduzca el numero de filas. Debe ser mayor que 14 y menor que 80.");
            while (!int.TryParse(Console.ReadLine(), out nRows) || nRows < 15 || nRows >80)
            {

                Console.WriteLine("Error al Introducir el numero de filas, intente de nuevo.");

            }

            Console.WriteLine("Introduzca el numero de columnas. debe ser mayor que 14 y menor que 80.");
            while (!int.TryParse(Console.ReadLine(), out nCols) || nCols < 15 || nCols > 80)
            {

                Console.WriteLine("Error al Introducir el numero de columnas, intente de nuevo.");

            }
            gridSize = nRows * nCols;
            #endregion

            //Despliegue del menu de las orientaciones de las palabras a activar o desactivar. 
          #region
         string[] dirArr = new string[8];
            for (int i = 0; i < 8; i++)
                dirArr[i] = "Activado";
            MenuDir(dirArr);

            int dirselected;

            string selected;
            while (true)
            {
                selected = Console.ReadLine();
                if (selected.ToLower() == "ok")
                    break;

                if (!int.TryParse(selected, out dirselected) && dirselected<8 && dirselected>-1)
                {
                    Console.WriteLine("Error al Introducir un numero de la seleccion o confirmar.\n");
                    
                }
                else
                {
                    if (dirArr[dirselected] == "Activado")
                        dirArr[dirselected] = "Desactivado";
                    else dirArr[dirselected] = "Activado";
                }
                MenuDir(dirArr);
            }

            int CantDir = 0; ;
            for (int i = 0; i < 8; i++)
            {
                if (dirArr[i] == "Activado")
                    CantDir++;
            }


            //activar y desactivar orientaciones
            dirs = new int[CantDir, 2];
            for (int i = 0, j = 0; i < 8; i++)
            {
                if (dirArr[i] == "Activado")
                    switch (i)
                    {
                        case 0:
                            {
                                dirs[j, 0] = 1;
                                dirs[j, 1] = 0;
                                j++;
                            }; break;
                        case 1:
                            {
                                dirs[j, 0] = 0;
                                dirs[j, 1] = 1;
                                j++;
                            }; break;
                        case 2:
                            {
                                dirs[j, 0] = 1;
                                dirs[j, 1] = 1;
                                j++;
                            }; break;
                        case 3:
                            {
                                dirs[j, 0] = 1;
                                dirs[j, 1] = -1;
                                j++;
                            }; break;
                        case 4:
                            {
                                dirs[j, 0] = -1;
                                dirs[j, 1] = 0;
                                j++;
                            }; break;
                        case 5:
                            {
                                dirs[j, 0] = 0;
                                dirs[j, 1] = -1;
                                j++;
                            }; break;
                        case 6:
                            {
                                dirs[j, 0] = -1;
                                dirs[j, 1] = -1;
                                j++;
                            }; break;
                        case 7:
                            {
                                dirs[j, 0] = -1;
                                dirs[j, 1] = 1;
                                j++;
                            }; break;
                    }


            }
        }
        #endregion

        private static void MenuDir(string [] dirlist) {
            Console.WriteLine("Introduzca el numero para activar y desactivar las orientaciones de la palabrea, la palabra OK para continuar.");
            Console.WriteLine("0 - Horizontal  ---                                  " + dirlist[0]);
            Console.WriteLine("1 - Vertical  ---                                    " + dirlist[1]);
            Console.WriteLine("2 - Diagonal hacia abajo de izquierda a derecha  --- " + dirlist[2]);
            Console.WriteLine("3 - Diagonal hacia arriba de Izquieda a Derecha ---  " + dirlist[3]);
            Console.WriteLine("4 - Horizontal invertida  ---                        " + dirlist[4]);
            Console.WriteLine("5 - Vertical inmvertida ---                          " + dirlist[5]);
            Console.WriteLine("6 - Diagonal hacia abajo de derecha a izquierda  --- " + dirlist[6]);
            Console.WriteLine("7 - Diagonal hacia arriba de derecha a izquierda --- " + dirlist[7]);
            Console.WriteLine("\n");

      }


       
        private static void Play(Grid grid) {

            int foundwords = 0;
            int Cantwords = grid.words.Count();

            int iniCol, iniFil, fnCol, fnFil, direction;
            int CiniCol, CiniFil, CfnCol, CfnFil;

            int WordPointer = 0;
            bool isCorrect = true;


            //Se pide la palabra encontrada, las coordenadas y la orientacion para 
			//verificar si la palabra es correcta, luego se resaltara en Mayusculas
            Console.WriteLine("Numero de palabras en grid: " + Cantwords + "\n");

            while (foundwords < Cantwords) {
                 WordPointer = 0;
            Console.WriteLine("Marque 0 para desplegar el grid nuevamente\n\nIntroduzca la palabra encontrada.");
                
                string encounterword = Console.ReadLine();
                if(encounterword != "0") { 
                if (grid.words.Contains(encounterword))
                {
                    Console.WriteLine("introduzca la columna inicial de la palabra.\n");
                    if (!int.TryParse(Console.ReadLine(), out iniCol))
                    {
                        Console.WriteLine("Error al Introducir coordenada.\n");
                        continue;
                    }
                    Console.WriteLine("introduzca la fila inicial de la palabra.\n");
                    if (!int.TryParse(Console.ReadLine(), out iniFil))
                    {
                        Console.WriteLine("Error al Introducir coordenada.\n");
                        continue;
                    }
                    Console.WriteLine("introduzca la columna final de la palabra.\n");
                    if (!int.TryParse(Console.ReadLine(), out fnCol))
                    {
                        Console.WriteLine("Error al Introducir coordenada.\n");
                        continue;
                    }
                    Console.WriteLine("introduzca la fila final de la palabra.\n");
                    if (!int.TryParse(Console.ReadLine(), out fnFil))
                    {
                        Console.WriteLine("Error al Introducir coordenada.\n");
                        continue;
                    }

                    Console.WriteLine("introduzca la direccion de la palabra.\n" +
                       " 0 - Horizontal.\n" +
                       " 1 - Vertical.\n" +
                       " 2 - Diagonal hacia abajo de izquierda a derecha.\n"+
                       " 3 - Diagonal hacia arriba de Izquieda a Derecha.\n" +
                       " 4 - Horizontal invertida.\n" +
                       " 5 - Vertical invertida.\n" +
                       " 6 - Diagonal hacia abajo de derecha a izquierda.\n" +
                       " 7 - Diagonal hacia arriba de derecha a izquierda.\n"
                        );

                    while(!int.TryParse(Console.ReadLine(), out direction) && direction>-1 && direction < 8)
                    {
                        Console.WriteLine("Error al Introducir la direccion.\n");
                    }

                    CiniCol = iniCol;
                    CiniFil = iniFil;
                    CfnCol = fnCol;
                    CfnFil = fnFil;

                        //Con la orientacion y las coordenadas dadas, se verifica que concuerde con la palabra "encontrada"
                        #region
                        switch (direction)
                    {
                        case 0: //Horizontal de izq a der
                            {
                                if((fnCol-iniCol)==(encounterword.Length-1))
                                while (iniCol <= fnCol) {
                                        if (Char.ToLower(encounterword.ElementAt(WordPointer)).CompareTo(grid.Cells[iniFil, iniCol])!=0)
                                    {
                                        isCorrect = false;
                                        break;
                                    }
                                    else {
                                        ++iniCol;
                                        ++WordPointer;
                                    }
                                }
                                else isCorrect = false;

                            }; break;
                        case 1: //Vertical de arriba a abajo
                            {
                                if ((fnFil - iniFil) == (encounterword.Length-1))
                                    while (iniFil <= fnFil)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            ++iniFil;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;

                            }; break;
                        case 2: //Diagonal hacia abajo de izquierda a derecha
                            {
                                if ((fnCol-iniCol) == (fnFil - iniFil) && (fnFil - iniFil) == (encounterword.Length-1))
                                    while (iniCol <= fnCol && fnFil >= iniFil)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            ++iniCol;
                                            ++iniFil;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;


                            }; break;
                        case 3: //Diagonal hacia arriba de Izquieda a Derecha
                            {
                                if ((fnCol - iniCol)  == (iniFil - fnFil) && (iniFil - fnFil) == (encounterword.Length - 1))
                                    while (iniCol <= fnCol && fnFil <= iniFil)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            ++iniCol;
                                            --iniFil;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;


                            }; break;
                        case 4: //Horizontal invertida
                            {
                                if ((iniCol - fnCol) == (encounterword.Length - 1))
                                    while (iniCol >= fnCol)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            --iniCol;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;


                            }; break;
                        case 5: //Vertical invertida
                            {
                                if ((iniFil - fnFil) == (encounterword.Length-1))
                                    while (iniFil >= fnFil)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            --iniFil;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;

                            }; break;
                        case 6: //Diagonal hacia abajo de derecha a izquierda
                            {
                                if ((iniCol - fnCol) == (iniFil - fnFil) && (iniFil - fnFil) == (encounterword.Length-1))
                                    while (iniCol >= fnCol && iniFil >= fnFil)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            --iniCol;
                                            --iniFil;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;
                            }; break;
                        case 7: //Diagonal hacia arriba de derecha a izquierda
                            {
                                if ((iniCol - fnCol) == (fnFil - iniFil)&& (fnFil - iniFil) == (encounterword.Length-1))
                                    while (iniCol >= fnCol && fnFil >= iniFil)
                                    {
                                        if (!Char.ToLower(encounterword.ElementAt(WordPointer)).Equals(grid.Cells[iniFil, iniCol]))
                                        {
                                            isCorrect = false;
                                            break;
                                        }
                                        else
                                        {
                                            --iniCol;
                                            ++iniFil;
                                            ++WordPointer;
                                        }
                                    }
                                else isCorrect = false;
                            }; break;
                    }
                        #endregion

                        if (isCorrect == true) {
                         //Se cambian las letras de la palabra encontrada por mayusculas
                            #region
                            Console.WriteLine("\nPalabra encontrada con exito.\n");
                        
                    switch (direction)
                               {
                        case 0:
                            {
                              while (CiniCol <= CfnCol) {
                                 grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                ++CiniCol;
                                ++WordPointer;
                                }
                          }; break;
                        case 1:
                            {
                              while (CiniFil <= CfnFil)
                               {
                                 grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                ++CiniFil;
                                ++WordPointer;
                              }
                            }; break;
                        case 2:
                            {
                          while (CiniCol <= CfnCol && CfnFil >= CiniFil)
                            {
                              grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                 ++CiniCol;
                                 ++CiniFil;
                                 ++WordPointer;
                              }
                            }; break;
                        case 3:
                            {
                             while (CiniCol <= CfnCol && CfnFil <= CiniFil)
                             {
                              grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                ++CiniCol;
                                --CiniFil;
                               ++WordPointer;
                                }
                            }; break;
                        case 4:
                            {
                            while (CiniCol >= CfnCol)
                             {
                             grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                             --CiniCol;
                             ++WordPointer;
                              }
                           }; break;
                        case 5:
                            {
                               while (CiniFil >= CfnFil)
                                  {
                                   grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                    --CiniFil;
                                    ++WordPointer;
                                        }
                                     }; break;
                        case 6:
                            {
                               while (CiniCol >= CfnCol && CiniFil >= CfnFil)
                                    {
                                grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                --CiniCol;
                                 --CiniFil;
                                 ++WordPointer;
                                   }
                              
                            }; break;
                        case 7:
                            {
                                while (CiniCol >= CfnCol && CfnFil >= CiniFil)
                                    {
                                 grid.Cells[CiniFil, CiniCol] = Char.ToUpper(grid.Cells[CiniFil, CiniCol]);
                                     --CiniCol;
                                     ++CiniFil;
                                     ++WordPointer;
                                        }
                               
                            }; break;
                    }


                        foundwords++;
                        grid.words.Remove(encounterword);
                        PrintResult(grid);
                       
                            #endregion
                        }
                        else Console.WriteLine("\nPosicion de la palabra no concuerda con las coordenadas insertadas.\n");

                  }
                else {
                    Console.WriteLine("Palabra no existente en el Grid.\n");
                    continue;

                }
               
              }
                else PrintResult(grid);
                Console.WriteLine("\nCantidad de palabras encontradas: " + foundwords + "\n");
            }
            Console.WriteLine("Todas las palabras han sido encontradas. Felicidades\n");
            Console.ReadLine();
        }


        //Insertan las letras aleatorias que no forman las palabras deseadas.
        private static Grid InsertRandomLetters(Grid grid) {
            string Alp = "abcdefghijklmnopqrstuvwxyz";

            for (int i = 0; i < nRows; i++)
                for (int j = 0; j < nCols; j++)
                   if(grid.Cells[i, j] == '\0')
                   grid.Cells[i, j] = Alp[rand.Next(0, Alp.Length - 1)];
                   return grid;
                 }
    }
}
