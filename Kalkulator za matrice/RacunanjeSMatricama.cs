using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using MatricaNS;

namespace FunkcijeZaMatrice
{
    public class Fn
    {

        public static List<List<double>> ZbrajanjeMatrica(Matrica A, Matrica B)
        {
            List<List<double>> rjesenje = new List<List<double>> { };
            List<double> redak = new List<double> { };

            if (!(A.BrojRedaka() == B.BrojRedaka() && A.BrojStupaca() == B.BrojStupaca()))
            {
                throw new InvalidOperationException("Matrice moraju biti jednakih dimanzija! A(m,n) i B(m, n)");
            }

            for (int i = 0; i < A.BrojStupaca(); i++)
                redak.Add(0);

            for (int i = 0; i < A.BrojRedaka(); i++)
                rjesenje.Add(DeepCopy(redak));

            for (int i = 0; i < A.BrojRedaka(); i++)
            {
                for (int j = 0; j < A.BrojStupaca(); j++)
                {
                    rjesenje[i][j] = A.elementiMatrice[i][j] + B.elementiMatrice[i][j];
                }
            }

            return rjesenje;
        }

        public static List<List<double>> MnozenjeMatrica(Matrica A, Matrica B)
        {
            List<List<double>> rjesenje = new List<List<double>> { };
            List<double> redak = new List<double> { };

            if (!(A.BrojStupaca() == B.BrojRedaka()))
            {
                throw new InvalidOperationException("Matrice moraju biti dimenzija A(m,n) i B(n, p)");
            }

            for (int i = 0; i < B.BrojStupaca(); i++)
                redak.Add(0);

            for (int i = 0; i < A.BrojRedaka(); i++)
                rjesenje.Add(DeepCopy(redak));

            for (int i = 0; i < A.BrojRedaka(); i++)
                for (int j = 0; j < B.BrojStupaca(); j++)
                    for (int k = 0; k < A.BrojStupaca(); k++)
                        rjesenje[i][j] += A.elementiMatrice[i][k] * B.elementiMatrice[k][j];

            return rjesenje;
        }

        public static List<List<double>> MnozenjeMatricaSkalarom(double skalar, Matrica A)
        {
            List<List<double>> rjesenje = new List<List<double>> { };
            List<double> redak = new List<double> { };

            for (int i = 0; i < A.BrojStupaca(); i++)
                redak.Add(0);

            for (int i = 0; i < A.BrojRedaka(); i++)
                rjesenje.Add(DeepCopy(redak));

            for (int i = 0; i < A.BrojRedaka(); i++)
                for (int j = 0; j < A.BrojStupaca(); j++)
                    rjesenje[i][j] = skalar * A.elementiMatrice[i][j];

            return rjesenje;
        }

        public static List<List<double>> Transpoziraj(Matrica A)
        {
            List<List<double>> rjesenje = new List<List<double>> { };
            List<double> redak = new List<double> { };


            for (int i = 0; i < A.BrojRedaka(); i++)
                redak.Add(0);

            for (int i = 0; i < A.BrojStupaca(); i++)
                rjesenje.Add(DeepCopy(redak));

            for (int i = 0; i < A.BrojRedaka(); i++)
                for (int j = 0; j < A.BrojStupaca(); j++)
                    rjesenje[j][i] = A.elementiMatrice[i][j];

            return rjesenje;
        }

        public static List<List<double>> DodajSvakomElementu(Matrica A, int skalar)
        {
            List<List<double>> rjesenje = new List<List<double>> { };
            List<double> redak = new List<double> { };


            for (int i = 0; i < A.BrojRedaka(); i++)
                redak.Add(0);

            for (int i = 0; i < A.BrojStupaca(); i++)
                rjesenje.Add(DeepCopy(redak));

            for (int i = 0; i < A.BrojRedaka(); i++)
                for (int j = 0; j < A.BrojStupaca(); j++)
                    rjesenje[i][j] = A.elementiMatrice[i][j] + 1;

            return rjesenje;

        }

        public static void IspisMatrice(List<List<double>> matrica)
        {
            int stupci = matrica[0].Count;
            int redci = matrica.Count;
            for (int i = 0; i < redci; i++)
            {
                if (redci == 1) Console.Write("[ ");
                else if (i == 0) Console.Write("I ");
                else if (i == redci - 1) Console.Write("I ");
                else Console.Write("| ");

                for (int j = 0; j < stupci; j++)
                {
                    Console.Write("{0} ", matrica[i][j]);
                }

                if (redci == 1) Console.Write("]");
                else if (i == 0) Console.Write("I");
                else if (i == redci - 1) Console.Write("I");
                else Console.Write("|");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void KreacijaNoveMatrice(string[] userCommandSplit, List<Matrica> Matrice)
        {
            string[] userCommandArgs = userCommandSplit[1].Split(" ", 2);
            string imeMatrice = userCommandArgs[0];
            if (imeMatrice == "") { Console.WriteLine("Potrebno je dati ime matrici!"); return; }
            var MatricaA = FindPerName(imeMatrice, Matrice);
            if (MatricaA != null)
            {
                ReDefiniraj(MatricaA, Matrice);
            }
            int brStupaca = 0;
            int brRedaka = 0;
            do
            {
                try
                {
                    Console.Write("Upiši broj redaka: ");
                    brRedaka = int.Parse(Console.ReadLine() + "");
                    Console.Write("Upiši broj stupaca: ");
                    brStupaca = int.Parse(Console.ReadLine() + "");
                }
                catch (FormatException) { continue; }
            } while (brStupaca <= 0 || brRedaka <= 0);
            List<List<double>> elementiMatrice = new List<List<double>> { };
            List<double> redak = new List<double> { };
            for (int i = 0; i < brStupaca; i++)
                redak.Add(0);
            for (int i = 0; i < brRedaka; i++)
                elementiMatrice.Add(DeepCopy(redak));

            //Ispis prazne matrice prije unosa podataka
            Console.WriteLine("\nPredložak matrice {0}:", imeMatrice);
            for (int i = 0; i < brRedaka; i++)
            {
                for (int j = 0; j < brStupaca; j++)
                {
                    Console.Write($"{imeMatrice.ToLower()}[{i + 1},{j + 1}] ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            //Unos podataka u matricu
            for (int i = 0; i < brRedaka; i++)
                for (int j = 0; j < brStupaca; j++)
                {
                    Console.Write($"Upisi {imeMatrice.ToLower()}[{i + 1},{j + 1}] = ");
                    try
                    {
                        elementiMatrice[i][j] = double.Parse(Console.ReadLine() + "");
                    }
                    catch (FormatException) { elementiMatrice[i][j] = 0; Console.WriteLine("element {0}[{1}][{2}] = 0", imeMatrice.ToLower(), i + 1, j + 1); continue; }
                }
            Matrica NovaMatrica = new Matrica(imeMatrice, elementiMatrice);
            Matrice.Add(NovaMatrica);
            return;
        }

        public static bool IsEqual(Matrica A, Matrica B)
        {
            if (A.BrojStupaca() != B.BrojStupaca()) return false;
            if (A.BrojRedaka() != B.BrojRedaka()) return false;

            for (int i = 0; i < A.BrojRedaka(); i++)
                for (int j = 0; j < A.BrojStupaca(); j++)
                    if (A.elementiMatrice[i][j] != B.elementiMatrice[i][j]) return false;

            return true;
        }

        public static Matrica? FindPerName(string imeMatriceZaUsporedit, List<Matrica> Matrice)
        {
            var RezultatPretrage = Matrice.FirstOrDefault(matrica => matrica.imeMatrice == imeMatriceZaUsporedit);
            if (RezultatPretrage == null)
            {
                return null;
            }
            else
            {
                return RezultatPretrage;
            }
        }

        public static bool ReDefiniraj(Matrica MatricaKojaSeBrise, List<Matrica> Matrice)
        {
            Console.Write("\nMatrica {0} već postoji! Želite li ju redefinirati? [Y/n]: ", MatricaKojaSeBrise.imeMatrice);
            string YNreDef = Console.ReadLine() + "";
            if (YNreDef.ToUpper() != "Y")
                return false;
            Matrice.Remove(MatricaKojaSeBrise);
            return true;
        }


        private static T DeepCopy<T>(T item)
        {
#pragma warning disable SYSLIB0011
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            if (item == null) throw new ArgumentNullException(nameof(item), "Argument funkcije DeepCopy ne smije biti NULL!");
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
#pragma warning restore SYSLIB0011
        }
    }
}