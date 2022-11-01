using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using LocalFn;
using System.Linq;
using System.IO;
using KonzolnaKontrola;


class Program
{
    static void Main()
    {
        List<double> matricaT1 = new List<double> {1, 2};
        List<double> matricaT2 = new List<double> {3, 4};
        List<List<double>> matricaT = new List<List<double>> {matricaT1, matricaT2};
        Matrica inputMatrica = new Matrica("TestA", matricaT);
        List<Matrica> Matrice = new List<Matrica> {inputMatrica};

        matricaT1 = new List<double> {2, 4};
        matricaT2 = new List<double> {4, 1};
        matricaT = new List<List<double>> {matricaT1, matricaT2};
        inputMatrica = new Matrica("TestB", matricaT);
        Matrice.Add(inputMatrica);






        ///                         KOMANDNA LINIJA

        Konzola Konzola = new Konzola();

        Console.WriteLine("Kalkulator za Matrice");
        Console.WriteLine("Upisi HELP za listu komandi.\n");
        int exit = 0;
        while(exit < 1)
        {
            Console.Write("MC.KOS$ ");
            exit = Konzola.Input(Console.ReadLine()+"", Matrice);
        }

        Console.WriteLine("Napravio Mateo Kos");
        Console.WriteLine("Kalkulator matrica v1.3, Lipanj 2022.");
        Thread.Sleep(500);
        
    }
}



////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace LocalFn
{
    public class Fn
    {

        public static List<List<double>> ZbrajanjeMatrica(Matrica A, Matrica B)
        {   
            List<List<double>> rjesenje = new List<List<double>> {};
            List<double> redak = new List<double> {};

            if ( !( A.BrojRedaka() == B.BrojRedaka() && A.BrojStupaca() == B.BrojStupaca() ) )
            {
                throw new InvalidOperationException("\nMatrice moraju biti jednakih dimanzija! A(m,n) i B(m, n)\n");
            }

            for (int i = 0; i < A.BrojStupaca(); i++)
            redak.Add(0);

            for (int i = 0; i < A.BrojRedaka(); i++)
            rjesenje.Add(DeepCopy(redak));

            for(int i = 0; i < A.BrojRedaka(); i++) 
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
            List<List<double>> rjesenje = new List<List<double>> {};
            List<double> redak = new List<double> {};

            if ( !( A.BrojStupaca() == B.BrojRedaka() ) )
            {
                throw new InvalidOperationException("\nMatrice moraju biti dimenzija A(m,n) i B(n, p)\n");
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
            List<List<double>> rjesenje = new List<List<double>> {};
            List<double> redak = new List<double> {};    

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
            List<List<double>> rjesenje = new List<List<double>> {};
            List<double> redak = new List<double> {};

            
            for (int i = 0; i < A.BrojRedaka(); i++)
            redak.Add(0);

            for (int i = 0; i < A.BrojStupaca(); i++)
            rjesenje.Add(DeepCopy(redak));

            for (int i = 0; i < A.BrojRedaka(); i++)
                for (int j = 0; j < A.BrojStupaca(); j++)
                    rjesenje[j][i] = A.elementiMatrice[i][j];

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
                Console.Write("\nMatrica {0} već postoji! Želite li ju redefinirati? [Y/N]: ", imeMatrice);
                string YNreDef = Console.ReadLine() + "";
                if (YNreDef != "Y")
                return;
                Matrice.Remove(MatricaA);
            }   
            int brStupaca = 0;
            int brRedaka = 0;
            do {
                try 
                {
                    Console.Write("Upiši broj redaka: ");
                    brRedaka = int.Parse(Console.ReadLine() + "");
                    Console.Write("Upiši broj stupaca: ");
                    brStupaca = int.Parse(Console.ReadLine() + "");
                } catch (FormatException) {continue;}
            } while( brStupaca <= 0 || brRedaka <= 0);   
            List<List<double>> elementiMatrice = new List<List<double>> {};
            List<double> redak = new List<double> {};    
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
                    Console.Write($"{imeMatrice.ToLower()}[{i+1},{j+1}] ");
                }
                Console.WriteLine();
            }
            Console.WriteLine(); 
            //Unos podataka u matricu
            for (int i = 0; i < brRedaka; i++)
                for (int j = 0; j < brStupaca; j++)
                {
                    Console.Write($"Upisi {imeMatrice.ToLower()}[{i+1},{j+1}] = ");
                    try {
                        elementiMatrice[i][j] = double.Parse(Console.ReadLine() + "");
                    } catch (FormatException) { elementiMatrice[i][j] = 0; Console.WriteLine("element {0}[{1}][{2}] = 0", imeMatrice.ToLower(), i+1 ,j+1); continue; }
                }    
            Matrica NovaMatrica = new Matrica(imeMatrice, elementiMatrice);
            Matrice.Add(NovaMatrica);
            return;
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

public class Matrica
{
    public List<List<double>> elementiMatrice {get;set;}
    public string imeMatrice {get;set;}
    

    public Matrica(string ime, List<List<double>> elementi)
    {
        this.imeMatrice = ime;
        this.elementiMatrice = elementi;
        
    }

    public int BrojRedaka() 
    {
        return this.elementiMatrice.Count;   
    }

    public int BrojStupaca()
    {
        return this.elementiMatrice[0].Count;
    }
}

namespace KonzolnaKontrola
{
    enum returnFlags{
        err = -1, normal, exit, saveNExit
    }
    public class Konzola
    {
        //Kljucne rijeci za komande (MORA BITI VELIKIM SLOVIMA / malim i metodom ToUpper())
        private string zbrajanje = "ADD".ToUpper();  
        private string mnozenje = "MTY".ToUpper();
        private string ispisMatrice = "PRINT".ToUpper();
        private string definiranjeMatrice = "DEF".ToUpper();
        private string spremiUVarijablu = "ST".ToUpper();
        private string exitProgram = "ESC".ToUpper();
        private string helpMenu = "HELP".ToUpper();
        private string transpozicija = "T".ToUpper();


        public int Input(string userInput, List<Matrica> Matrice) 
            //vraca 0 - ako nema iznimki, -1 - ako se pojavila greska, ali je rjesena, 1 - za izlaz iz aplikacije, 2 - za izlaz sa spremanjem u file
        {
            bool saveToFile = false;
            string userCommand = userInput + "";
            string[] userCommandSplit = userCommand.Split(" ", 2); //0 - komanda, 1- operandi
            string userCommandFunc = userCommandSplit[0].ToUpper();

            if (userCommandFunc == "?" || userCommandFunc == helpMenu)
            {
                Console.WriteLine("\n\n\nOperacije s matricama:");
                Console.WriteLine("  \"ADD {Ime Matrice} {Ime Matrice}\"        - Zbrajanje matrica");
                Console.WriteLine("  \"MTY {Ime Matrice} {Ime Matrice}\"        - Množenje matrica");
                Console.WriteLine("  \"MTY {skalar} {Ime Matrice}\"             - Množenje matrice sa skalarom");
                Console.WriteLine("\nOperacije s podatcima:");
                Console.WriteLine("  \"PRINT {Ime Matrice}\"                    - Ispis Matrice");
                Console.WriteLine("  \"PRINT ALL\"                              - Ispis svih Matrica");
                Console.WriteLine("  \"DEF {Ime Nove Matrice}\"                 - Definiranje varijable za novu matricu");
                Console.WriteLine("  \"ST {Ime Nove Matrice}\"                  - Dolazi nakon matematičke operacije za spremanje rezultata u novu matricu npr(ADD A B ST C)");
                Console.WriteLine("  \"HELP\"                                   - Ispisuje ovaj meni");
                Console.WriteLine("  \"Q\", \"ESC\"                               - Zatvara program");
                return (int)returnFlags.normal;
            }

            if (userCommandFunc == "Q" || userCommandFunc == exitProgram) { return (int)returnFlags.exit; }


            if (userCommandSplit.Length == 1 && userCommandFunc != "Q" && userCommandFunc != exitProgram) { Console.WriteLine("Potrebni su operandi (nakon ključne riječi)"); return (int)returnFlags.err; }

            if (userCommandFunc == ispisMatrice)
            {
                string userCommandArgs = userCommandSplit[1];
                if (userCommandArgs.ToUpper() == "ALL")
                {
                    foreach (var matrica in Matrice)
                    {
                        Console.WriteLine("Matrica {0}:", matrica.imeMatrice);
                        Fn.IspisMatrice(matrica.elementiMatrice);
                    }
                    return 0;
                }
                var MatricaA = Fn.FindPerName(userCommandArgs, Matrice);
                if (MatricaA == null)
                {
                    Console.WriteLine("\nMatrica s imenom {0} nije pronađena!\n", userCommandArgs);
                    return -1;
                }
                Console.WriteLine("Matrica {0}: ", MatricaA.imeMatrice);
                Fn.IspisMatrice(MatricaA.elementiMatrice);
                return 0;
            }

            if (userCommandFunc == definiranjeMatrice)
            {
                Fn.KreacijaNoveMatrice(userCommandSplit, Matrice);
                return 0;
            }

            if (userCommandFunc == zbrajanje)
            {
                string[] userCommandArgs = userCommandSplit[1].Split(" ", 5);
                if (userCommandArgs.Length < 2)
                {
                    Console.WriteLine("Komanda nije dobro upisana!");
                    return -1;
                }

                var MatricaA = Fn.FindPerName(userCommandArgs[0], Matrice);
                if (MatricaA == null)
                {
                    Console.WriteLine("\nMatrica s imenom {0} nije pronađena!\n", userCommandArgs[0]);
                    return -1;
                }
                var MatricaB = Fn.FindPerName(userCommandArgs[1], Matrice);
                if (MatricaB == null)
                {
                    Console.WriteLine("\nMatrica s imenom {0} nije pronađena!\n", userCommandArgs[1]);
                    return -1;
                }

                if (userCommandArgs.Length < 4)
                {
                    try
                    {
                        Fn.IspisMatrice(Fn.ZbrajanjeMatrica(MatricaA, MatricaB));
                    }
                    catch (InvalidOperationException e) { Console.WriteLine("{0}", e.Message); return -1; }
                }
                else if (userCommandArgs[3].Split(" ", 2).Length == 1 && userCommandArgs[2].ToUpper() == spremiUVarijablu && userCommandArgs[3] != "")
                {
                    Matrica Rjesenje;
                    try
                    {
                        Rjesenje = new Matrica(userCommandArgs[3], Fn.ZbrajanjeMatrica(MatricaA, MatricaB));
                    }
                    catch (InvalidOperationException e) { Console.WriteLine("{0}", e.Message); return -1; }

                    //Provjera da ne postoji istoimena matrica u memoriji
                    var MatricaNameColTest = Fn.FindPerName(userCommandArgs[3], Matrice);
                    if (MatricaNameColTest == null)
                    {
                        Matrice.Add(Rjesenje);
                    }
                    else
                    {
                        Console.WriteLine("Nije moguće spremiti rezultat jer matrica s tim imenom već postoji!");
                    }
                }
                return 0;
            }

            if (userCommandFunc == mnozenje)
            {
                string[] userCommandArgs = userCommandSplit[1].Split(" ", 5);
                if (userCommandArgs.Length < 2)
                {
                    Console.WriteLine("Komanda nije dobro upisana!");
                    return -1;
                }

                double skalar; bool skalarnoMnozenje = false;
                try
                {
                    skalar = double.Parse(userCommandArgs[0]);
                    skalarnoMnozenje = true;
                }
                catch (FormatException) { skalar = double.NaN; skalarnoMnozenje = false; }

                var MatricaA = Fn.FindPerName(userCommandArgs[1], Matrice);
                if (!skalarnoMnozenje)
                { MatricaA = Fn.FindPerName(userCommandArgs[0], Matrice); }
                if (MatricaA == null)
                {
                    Console.WriteLine("\nMatrica s imenom {0} nije pronađena!\n", userCommandArgs[0]);
                    return -1;
                }

                var MatricaB = Fn.FindPerName(userCommandArgs[1], Matrice);
                if (MatricaB == null)
                {
                    Console.WriteLine("\nMatrica s imenom {0} nije pronađena!\n", userCommandArgs[1]);
                    return -1;
                }
                if (userCommandArgs.Length < 4)
                {
                    if (skalarnoMnozenje) Fn.IspisMatrice(Fn.MnozenjeMatricaSkalarom(skalar, MatricaB));
                    else
                    {
                        try
                        {
                            Fn.IspisMatrice(Fn.MnozenjeMatrica(MatricaA, MatricaB));
                        }
                        catch (InvalidOperationException e) { Console.WriteLine("{0}", e.Message); return -1; }
                    }
                }
                else if (userCommandArgs[3].Split(" ", 2).Length == 1 && userCommandArgs[2].ToUpper() == spremiUVarijablu && userCommandArgs[3] != "")
                {
                    Matrica Rjesenje;
                    if (!skalarnoMnozenje)
                        try
                        {
                            Rjesenje = new Matrica(userCommandArgs[3], Fn.MnozenjeMatrica(MatricaA, MatricaB));
                        }
                        catch (InvalidOperationException e) { Console.WriteLine("{0}", e.Message); return -1; }
                    else Rjesenje = new Matrica(userCommandArgs[3], Fn.MnozenjeMatricaSkalarom(skalar, MatricaB));

                    //Provjera da ne postoji istoimena matrica u memoriji
                    var MatricaNameColTest = Fn.FindPerName(userCommandArgs[3], Matrice);
                    if (MatricaNameColTest == null)
                    {
                        Matrice.Add(Rjesenje);
                    }
                    else
                    {
                        Console.WriteLine("Nije moguće spremiti rezultat jer matrica s tim imenom već postoji!");
                    }

                }

                return 0;

            }

            if (userCommandFunc == transpozicija)
            {
                string[] userCommandArgs = userCommandSplit[1].Split(" ", 4);
                string imeVarijableNoveMatrice = "NoName";
                bool saveToNewVariable = false;
                if (userCommandArgs.Length < 1)
                {
                    Console.WriteLine("Komanda nije dobro upisana!");
                    return -1;
                }
                else if (userCommandArgs.Length >= 3)
                {
                    imeVarijableNoveMatrice = userCommandArgs[2];
                    if (userCommandArgs[1] == spremiUVarijablu)
                        saveToNewVariable = true;
                }
                var MatricaA = Fn.FindPerName(userCommandArgs[0], Matrice);
                if (MatricaA == null)
                {
                    Console.WriteLine("\nMatrica s imenom {0} nije pronađena!\n", userCommandArgs[0]);
                    return -1;
                }
                Matrica Rjesenje = new Matrica(imeVarijableNoveMatrice, Fn.Transpoziraj(MatricaA));

                if (saveToNewVariable)
                {
                    var MatricaNameColTest = Fn.FindPerName(userCommandArgs[2], Matrice);
                    if (MatricaNameColTest == null && imeVarijableNoveMatrice != "")
                    {
                        Matrice.Add(Rjesenje);
                    }
                    else
                    {
                        Console.WriteLine("Nije moguće spremiti rezultat jer matrica s tim imenom već postoji!");
                    }
                }
                else
                    Fn.IspisMatrice(Rjesenje.elementiMatrice);
                return 0;
            }

            return 0;
        }
    }
}