using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using FunkcijeZaMatrice;

//Lokalni namespace
using KonzolnaKontrola;
using MatricaNS;


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

        bool useKonzola = false;
        string userInput = "TestA + TestB - TestA * TestB + TestA^T";
        
        UserInputParser.UserInputParser.UserToKonsoleTranslator(userInput, Matrice);
        useKonzola = true;


        if (useKonzola)
        {
            Konzola Konzola = new Konzola();
            Console.WriteLine("Kalkulator za Matrice");
            Console.WriteLine("Upisi HELP za listu komandi.\n");
            int exit = 0;

            while (exit < 1)
            {
                Console.Write("MC.KOS$ ");
                exit = Konzola.Input(Console.ReadLine() + "", Matrice);
            }
            useKonzola = false;
        }

        Console.WriteLine("Napravio Mateo Kos");
        Console.WriteLine("Kalkulator matrica v2.0, Lipanj 2022.");
        Thread.Sleep(500);
        
    }
}



////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace UserInputParser
{
    public class UserInputParser
    {
        public static int UserToKonsoleTranslator(string userInput, List<Matrica> Matrice, int tempInt = 0)
        {
            if (userInput == null) return (int)returnFlags.err;
            if (userInput.Length == 0) return (int)returnFlags.err;
            if (userInput.ToUpper() == "Q" || userInput.ToUpper() == "ESC") return (int)returnFlags.normal;

            bool internalCall = true;
            int prioritetPL = 3;
            int prioritetMIN = 3;
            int prioritetMUL = 2;
            int prioritetEQU = 9;
            int prioritetPOT = 1;


            List<string> userInputOperacije = new List<string>(userInput.Split(" "));
            int maxIndex = userInputOperacije.Count();
            int brojOperacija = maxIndex / 2;
            int brojOperanda = maxIndex / 2 + 1;

            if (brojOperacija + brojOperanda != userInputOperacije.Count()) throw new InvalidOperationException("Zbroj operanda i operacija nije jednak broju svih elemenata!");

            List<PrioritetOperacija> Operacije = new List<PrioritetOperacija>();

            for (int i = 0; i < maxIndex; i++)
            {
                var operacija = userInputOperacije[i];
                if (operacija == "=")
                {
                    Operacije.Add(IspuniListuPrioriteta(i, prioritetEQU, '='));
                } else if (operacija == "*")
                {
                    Operacije.Add(IspuniListuPrioriteta(i, prioritetMUL, '*'));
                } else if (operacija.Contains("^"))
                {
                    Operacije.Add(IspuniListuPrioriteta(i, prioritetPOT, '^'));
                }
                else if (operacija == "+")
                {
                    Operacije.Add(IspuniListuPrioriteta(i, prioritetPL, '+'));
                }
                else if (operacija == "-")
                {
                    Operacije.Add(IspuniListuPrioriteta(i, prioritetMIN, '-'));
                }
            }

            if (Operacije.Count > 0)
            {
                var sorter = new KComparerPrioriteta();
                Operacije.Sort(sorter);
            }
            else return (int)returnFlags.err;

            List<Matrica> MatriceCopy = new List<Matrica>(Matrice);
            Konzola Konzola = new Konzola();


            /*
            Console.WriteLine("I P O");
            foreach (var operacija in Operacije)
                Console.WriteLine(operacija.index + " " + operacija.prioritet + " " + operacija.operacija);

            Console.WriteLine(userInput);
            */
            if (Operacije[0].operacija == '+')
            {
                string KonzolniString;
                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
                bool numOnly = false;

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije, MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(userInputOperacije[Operacije[0].index - 1]) && IsNumber(posljeOperacije))
                    {
                        int rj = int.Parse(prijeOperacije) + int.Parse(posljeOperacije);
                        userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                        userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                        userInputOperacije.RemoveAt(Operacije[0].index - 1);
                        numOnly = true;
                    }
                    else if (IsNumber(prijeOperacije) || IsNumber(posljeOperacije))
                        throw new ArgumentException("Nije mogu�e zbrojiti matricu sa skalarom");
                    else
                        throw new ArgumentException($"Jedna od matrica nije definirana (\"{prijeOperacije} + {posljeOperacije}\")");


                if (!numOnly)
                {
                    KonzolniString = $"{Konzola.zbrajanje} {A.imeMatrice} {B.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";
                    Konzola.Input(KonzolniString, MatriceCopy, internalCall);

                    userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                }

                userInput = "";

                for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
                {
                    if (i == maxIndex - 3) userInput += userInputOperacije[i];
                    else userInput += userInputOperacije[i] + " ";
                }
                Operacije.RemoveAt(0);

            } else if (Operacije[0].operacija == '-')
            {
                string KonzolniString;

                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
                bool numOnly = false;

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije, MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(userInputOperacije[Operacije[0].index - 1]) && IsNumber(posljeOperacije))
                    {
                        int rj = int.Parse(prijeOperacije) - int.Parse(posljeOperacije);
                        userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                        userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                        userInputOperacije.RemoveAt(Operacije[0].index - 1);
                        numOnly = true;
                    }
                    else if (IsNumber(prijeOperacije) || IsNumber(posljeOperacije))
                        throw new ArgumentException("Nije mogu�e zbrojiti matricu sa skalarom");
                    else
                        throw new ArgumentException($"Jedna od matrica nije definirana (\"{prijeOperacije} - {posljeOperacije}\")");


                if (!numOnly)
                {
                    Konzola.Input($"{Konzola.mnozenje} -1 {B.imeMatrice} {Konzola.spremiUVarijablu} {B.imeMatrice}", MatriceCopy, internalCall);
                    KonzolniString = $"{Konzola.zbrajanje} {A.imeMatrice} {B.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";
                    Konzola.Input(KonzolniString, MatriceCopy, internalCall);

                    userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                }

                userInput = "";

                for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
                {
                    if (i == maxIndex - 3) userInput += userInputOperacije[i];
                    else userInput += userInputOperacije[i] + " ";
                }
                Operacije.RemoveAt(0);

            } else if (Operacije[0].operacija == '*')
            {
                string KonzolniString;
                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
                bool numOnly = false;

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije, MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                    {
                        int rj = int.Parse(prijeOperacije) * int.Parse(posljeOperacije);
                        userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                        userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                        userInputOperacije.RemoveAt(Operacije[0].index - 1);
                        numOnly = true;
                    } else if (A == null && !IsNumber(prijeOperacije) || B == null && !IsNumber(posljeOperacije))
                        throw new ArgumentException($"Jedna od matrica nije definirana (\"{prijeOperacije} + {posljeOperacije}\")");



                if (!numOnly)
                {
                    if (IsNumber(posljeOperacije))
                        KonzolniString = $"{Konzola.mnozenje} {int.Parse(posljeOperacije)} {A.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";
                    else if (IsNumber(prijeOperacije))
                        KonzolniString = $"{Konzola.mnozenje} {int.Parse(prijeOperacije)} {B.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";
                    else
                        KonzolniString = $"{Konzola.mnozenje} {A.imeMatrice} {B.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";

                    Konzola.Input(KonzolniString, MatriceCopy, internalCall);

                    userInputOperacije[Operacije[0].index + 1] = $"TMP{tempInt}"; //Sprema ime TMP varijable za kasniju upotrebu
                    userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                    userInputOperacije.RemoveAt(Operacije[0].index - 1);
                }

                userInput = "";

                for (int i = 0; i < maxIndex - 2; i++)      //Rekonstruira Input s imenima novih varijabli
                {
                    if (i == maxIndex - 3) userInput += userInputOperacije[i];
                    else userInput += userInputOperacije[i] + " ";
                }
                Operacije.RemoveAt(0);

            }else if (Operacije[0].operacija == '^') 
            {
                string KonzolniString;
                string[] razdvajanjeNaBazuIEksponent = userInputOperacije[Operacije[0].index].Split('^');
                string prijeOperacije = razdvajanjeNaBazuIEksponent[0];
                string posljeOperacije = razdvajanjeNaBazuIEksponent[1];
                bool numOnly = false;

                if (!IsNumber(posljeOperacije) && posljeOperacije != "T")
                {
                    Console.WriteLine("Nakon znaka \"^\" mora biti broj ili oznaka za transpoziciju \"T\"");
                    return (int)returnFlags.err;
                }

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                if (A == null)
                    if (IsNumber(prijeOperacije))
                    {
                        double rj;
                        numOnly = true;
                        if (posljeOperacije == "T")
                            rj = int.Parse(prijeOperacije);
                        else
                            rj = Math.Pow(double.Parse(prijeOperacije), double.Parse(posljeOperacije));

                        userInputOperacije[Operacije[0].index] = $"{rj}";

                    }
                    else throw new ArgumentException("A^B -> A mora biti broj ili ime matrice");

                if (!numOnly)
                {
                    if (posljeOperacije == "T")
                    {
                        KonzolniString = $"{Konzola.transpozicija} {A.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";
                        Konzola.Input(KonzolniString, MatriceCopy, internalCall);
                    } else if (int.Parse(posljeOperacije) > 0)
                    {
                        Konzola.Input($"{Konzola.mnozenje} 1 {A.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}", MatriceCopy, internalCall);
                        for (int i = 2; i <= int.Parse(posljeOperacije); i++)
                            Konzola.Input($"{Konzola.mnozenje} TMP{tempInt} {A.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}", MatriceCopy, internalCall);

                    }
                    else if (int.Parse(posljeOperacije) == 0)
                    {
                        Konzola.Input($"{Konzola.mnozenje} 0 {A.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}", MatriceCopy, internalCall);
                        Fn.DodajSvakomElementu(MatriceCopy[MatriceCopy.Count - 1], 1);
                    }
                    userInputOperacije[Operacije[0].index] = $"TMP{tempInt}";
                }
                userInput = "";

                for (int i = 0; i < maxIndex; i++)      //Rekonstruira Input s imenima novih varijabli
                {
                    if (i == maxIndex - 1) userInput += userInputOperacije[i];
                    else userInput += userInputOperacije[i] + " ";
                }
                Operacije.RemoveAt(0);

            }else if (Operacije[0].operacija == '=')
            {
                string KonzolniString;
                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije, MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                    {
                        if (int.Parse(prijeOperacije) == int.Parse(posljeOperacije) ) {
                            Console.WriteLine("Lijeva i desna strana su jednake!");
                            return (int)returnFlags.JednadzbaTrue;
                        }
                        return (int)returnFlags.JednadzbaFalse;
                    }
                    else if (A == null && IsNumber(posljeOperacije))
                        throw new ArgumentException($"Nije mogu�e spremiti broj�anu vrijednost u varijablu za matrice. (\"{prijeOperacije} = {posljeOperacije}\")");
                    else if (B == null)
                        throw new ArgumentException($"Nije prona�ena matrica {B.imeMatrice}!");
                    else if (A == null)
                    {
                        KonzolniString = $"{Konzola.mnozenje} 1 TMP{tempInt - 1} {Konzola.spremiUVarijablu} {prijeOperacije}";
                        Konzola.Input(KonzolniString, MatriceCopy);
                        Operacije.RemoveAt(0);
                        return (int)returnFlags.exitResultStored;
                    }
                    
               
                if (A != null)
                {
                    if (Fn.IsEqual(A, B))
                    {
                        Console.WriteLine("Lijeva i desna strana su jednake!");
                        Operacije.RemoveAt(0);
                        return (int)returnFlags.JednadzbaTrue;
                    }
                    else
                    {
                        Console.WriteLine("Lijeva i desna strana nisu jednake!");
                        Operacije.RemoveAt(0);
                        if (Fn.ReDefiniraj(A, MatriceCopy))
                        {
                            Console.Write("\nUpi�i ime novoj matrici: ");
                            string imeNoveMatrice = Console.ReadLine() + "";
                            if (imeNoveMatrice == null) imeNoveMatrice = B.imeMatrice;
                            Matrica Rj = new(imeNoveMatrice, B.elementiMatrice);
                            MatriceCopy.Add(Rj);
                            return (int)returnFlags.exitResultStored;
                        }
                        return (int)returnFlags.normal;
                    }
                }


            }

            //Console.WriteLine(userInput);
            int rfll = (int)returnFlags.normal;
            if (Operacije.Count() > 0)
                rfll = UserToKonsoleTranslator(userInput, MatriceCopy, tempInt + 1); // Return from lower layer
            
            if (rfll == (int)returnFlags.normal || rfll == (int)returnFlags.exitResultStored)
            Matrice.Add(MatriceCopy[MatriceCopy.Count - 1]);

            if (rfll == (int)returnFlags.err)
                return (int)returnFlags.err;

            if (tempInt == 0)
            Konzola.Input($"PRINT {MatriceCopy[MatriceCopy.Count - 1].imeMatrice}", Matrice);

            return (int)returnFlags.normal;
        }


        private static PrioritetOperacija IspuniListuPrioriteta(int index, int prioritet, char operacija)
        {
            PrioritetOperacija prioritetOperacija = new PrioritetOperacija();
            prioritetOperacija.prioritet = prioritet;
            prioritetOperacija.index = index;
            prioritetOperacija.operacija = operacija;

            return prioritetOperacija;
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

        public static bool IsNumber(string input)
        {
            if (input == null) return false;

            try
            {
                int.Parse(input);
            } catch (FormatException)
            {
                return false;
            }
            
            return true;
            
        }
    }

    enum returnFlags
    {
        err = -1, normal, exitResultStored, SaveToFile, JednadzbaTrue, JednadzbaFalse
    }

    struct PrioritetOperacija
    {
        public int prioritet;
        public int index;
        public char operacija;
    }

    class KComparerPrioriteta : IComparer<PrioritetOperacija>
    {
        public int Compare(PrioritetOperacija x, PrioritetOperacija y) 
        {
            if (x.prioritet < y.prioritet)                               //sortira prioritet rastuci
                return -1;
            else if (x.index < y.index)
                return -1;
            else
                return 1;
        }
    }
}


namespace MatricaNS
{
    public class Matrica
    {
        public List<List<double>> elementiMatrice { get; set; }
        public string imeMatrice { get; set; }


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
}
