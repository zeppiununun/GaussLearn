using System;

namespace matrixinput
{
    public class Gaussnew
    {
        private double[,] M;
        public int r;

        public Gaussnew(double[,] A)
        {
            r = A.GetLength(0);
            M = new double[r, r + 1];

            M=(double[,])A.Clone();
        }

        public double this[int a, int b]
        {


            get
            {
                if ((a < r) && (b <= r))
                    return M[a, b];
                else
                    throw new Exception("out of bound");
            }
            set
            {
                if ((a < r) && (b <= r))
                    M[a, b] = value;
                else
                    throw new Exception("out of bound");
            }

        }

        // Normieren der k-ten Zeile (d.h. Teilen aller Einträge in der Zeile durch den Diagonaleinrag M[k,k])

        public bool NormRow(int k, out String log)
        {
            bool welldefined = true;
            log = "";
            // wenn der Diagonaleintrag=0 vertauschen die Zeilen, so dass
            // er nicht Null ist 
            //Falls das unmöglich ist, besitzt die Gleichungssystem keine
            //einfeutige Lösung

            if (M[k, k] == 0)
            {
                welldefined = false;
                for (int h = k + 1; h < r; h++)
                {
                    if (M[h, k] != 0)
                    {
                        log = String.Format("Vertauschen von {0}. und {1}. Zeilen \r\n", k+1, h+1);
                          welldefined = true;

                        for (int j = 0; j <= r; j++)
                        { 
                            double a = M[k, j];
                            M[k, j] = M[h, j];
                            M[h, j] = a;

                        }

                    }
                }
            }

            if (!welldefined) return(false);
            //alter Code für die erste Zeile 
            /*for (int j = r; j >= 0; j--)
                M[0, j] /= (-M[0, 0]);*/

            // Normieren der k-ten Zeile
            if (M[k,k]<0)
                log = log+ String.Format("Teilen der {0}. Zeile durch {1:0.#####}, R{0}/{1:0.#####}-> R{0}:", k+1, -M[k,k]);
            else
                log = log + String.Format("Teilen der {0}. Zeile durch {1:0.#####}, R{0}/({1:0.#####})-> R{0}:", k + 1, -M[k, k]);

            for (int j = r; j >= k; j--)
                M[k, j] /= (-M[k, k]);

            return (welldefined);

        }

        // Eliminierung der k-ten Variable, d.h.  Erzeugen von 0
        //in der k-ten Spalte unterm Diagonaleintrag 

        public void Elimination(int k, out String log)
        {
            log ="";
            for (int i = k + 1; i < r; i++)
                { if (M[i, k] == 0) continue;
              
                  if (M[i,k]>=0)
                    log = log + String.Format("R{0} + {1:0.#####} * R{2} -> R{0}\r\n", i+1,M[i,k],k+1);
                else
                    log = log + String.Format("R{0} + ({1:0.#####}) * R{2} -> R{0}\r\n", i + 1, M[i, k], k + 1);

                for (int j = r; j >= k; j--)
                {
                    M[i, j] += M[i, k] * M[k, j];


                }
            }
            //alter Code korrigiert for X1
            /*  for (int i = 1; i < r; i++) 
                   for (int j = r; j>= 0; j--)
                       M[i, j] = M[i, j] + M[i, 0] * M[0, j];*/

        }

        // Rückeinsetzen der k-ter Variable( X_(k+1)) d.h.
        // Erzeugen von Nullen in der k-ten Spalte
        public void BackSubstitution(int k, out String log)
        {
            log = "";
            for (int i=k-1; i>=0; i--)
            {
                if (M[i, k] == 0) continue;
            if (M[i,k]>=0)
                    log = log + String.Format("R{0}+{1:0.#####}*R{2} -> R{0}\r\n", i+1, M[i,k], k+1);
               else
                    log = log + String.Format("R{0}+({1:0.#####})*R{2} -> R{0}\r\n", i + 1, M[i, k], k + 1);
                M[i, r] = M[i, r] + M[i,k]*M[k,r];
                M[i, k] = 0;
            }

            if (log != "")
                log = String.Format("Addieren Vielfachen der {0}. Zeile \r\n", k + 1) + log; 
        }
        // Die Lösung ablesen
        public double[] GetSolution()
        {
            double[] x = new double[r];

            for (int i = 0; i < r; i++)
                x[i] = -M[i, r];
            return x;
        }

        // Ausgabe der Matrix mit max. 5 Nachkommazahlen
        public void Matrix_Output()
        {
            for (int i = 0; i < r; i++)
            {

                for (int j = 0; j < r + 1; j++)
                {
                    Console.Write(String.Format("{0:0.#####} ", M[i,j]).PadRight(9));
                }
                Console.WriteLine("");

            }
            Console.WriteLine("");
        }

        // Ausgabe der Lösung
        public void Solution_Output()
        {
            double[] x = GetSolution();

                for (int j = 0; j < r; j++)
                {
                    Console.WriteLine("x" + (j+1).ToString() +" =" + String.Format("{0:0.#####} ", x[j]).PadRight(9));
                }
             
            }
            
       }
}