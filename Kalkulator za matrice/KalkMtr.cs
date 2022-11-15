using FunkcijeZaMatrice;

//Lokalni namespace
using KonzolnaKontrola;
using MatricaNS;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using UserInputParser;

class Program
{
    static void Main()
    {
        //Console.SetWindowSize(120, 30);  //Windows Only!

        List<double> matricaT1 = new List<double> { 1, 2 };
        List<double> matricaT2 = new List<double> { 3, 4 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica inputMatrica = new Matrica("TestA", matricaT);
        List<Matrica> Matrice = new List<Matrica> { inputMatrica };

        matricaT1 = new List<double> { 2, 4 };
        matricaT2 = new List<double> { 4, 1 };
        matricaT = new List<List<double>> { matricaT1, matricaT2 };
        inputMatrica = new Matrica("TestB", matricaT);
        Matrice.Add(inputMatrica);


        // ESC[?25l	make cursor invisible
        // ESC[?25h make cursor visible
        // ESC[2K	erase the entire line
        // ESC[3J	erase saved lines
        // ESC[s	save cursor position (DEC)
        // ESC[u    restores the cursor to the last saved position(DEC)
        // \x1b[    ESC caracter



        ///                         KOMANDNA LINIJA

        bool useKonzola = false;
        int exitProgram = 1;

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("Dobro došli u ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Kalulator za matrice");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Upišite HELP za listu komandi");
        Console.ForegroundColor = ConsoleColor.White;

        while (exitProgram != (int)returnFlags.SaveToFile && exitProgram != (int)returnFlags.exitProgram)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(">");
//          Console.Write("\x1b[s");
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string userInput = Console.ReadLine() + "";

                userInput = CommandColor.LineColoring(userInput, Matrice);

                Console.ForegroundColor = ConsoleColor.White;
                exitProgram = UserInputParser.UserInputParser.UserToKonsoleTranslator(userInput, Matrice);
            }
            catch (ArgumentException e) { Konzola.KonzolaRedTX("Greška: "); Console.WriteLine(e.Message); }
            catch (InvalidOperationException e) { Konzola.KonzolaRedTX("Greška: "); Console.WriteLine(e.Message); }
            catch (FormatException e) { Konzola.KonzolaRedTX("Greška: "); Console.WriteLine(e.Message); }
            useKonzola = false;

            if (exitProgram == (int)returnFlags.exitToConsole)
                useKonzola = true;


            if (useKonzola)
            {
                Konzola Konzola = new Konzola();
                Console.WriteLine("\nKalkulator za Matrice");
                Console.WriteLine("Upisi HELP za listu komandi.\n");
                int exit = 0;

                while (exit < 1)
                {
                    Console.Write("MC.KOS$ ");
                    exit = Konzola.Input(Console.ReadLine() + "", Matrice);
                }
                useKonzola = false;
            }
        }
        Console.WriteLine("\nNapravio Mateo Kos");
        Console.Write("Kalkulator matrica ");
        Konzola.KonzolaYellowTX("v2.5"); Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(", Lipanj 2022.");
        Thread.Sleep(500);

    }
}



////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace UserInputParser
{
    public class UserInputParser
    {
        public static string exitFromCalc = "Q";
        public static string exitToConsole = "ASM";
        public static string helpMeni = "HELP";
        public static string defineVariable = "DEF";
        public static int UserToKonsoleTranslator(string userInput, List<Matrica> Matrice, int tempInt = 0)
        {
            bool print = true;
            if (userInput == null) return (int)returnFlags.err;
            if (userInput.Length == 0) return (int)returnFlags.err;
            if (userInput.ToUpper() == exitFromCalc || userInput.ToUpper() == "ESC") return (int)returnFlags.exitProgram;
            if (userInput.ToUpper() == exitToConsole) return (int)returnFlags.exitToConsole;
            if (userInput.ToUpper() == helpMeni || userInput == "?")
            {
                PrintHelpMeni();
                return (int)returnFlags.softExit;
            }

            bool internalCall = true;
            int prioritetPL = 3;
            int prioritetMIN = 3;
            int prioritetMUL = 2;
            int prioritetEQU = 9;
            int prioritetPOT = 1;


            List<string> userInputOperacije = new List<string>(userInput.Split(" "));
            if (userInputOperacije.Count == 1 && !userInputOperacije[0].Contains("^"))
            {
                Konzola TKonzola = new Konzola();
                TKonzola.Input(TKonzola.ispisMatrice + " " + userInput, Matrice);
                return (int)returnFlags.softExit;
            }
            if (userInputOperacije[0].ToUpper() == defineVariable)
            {
                Konzola TKonzola = new Konzola();
                TKonzola.Input(TKonzola.definiranjeMatrice + " " + userInputOperacije[1], Matrice);
                return (int)returnFlags.softExit;
            }
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
                }
                else if (operacija == "*")
                {
                    Operacije.Add(IspuniListuPrioriteta(i, prioritetMUL, '*'));
                }
                else if (operacija.Contains("^"))
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
                var B = Fn.FindPerName(posljeOperacije + "", MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(userInputOperacije[Operacije[0].index - 1]) && IsNumber(posljeOperacije))
                    {
                        double rj = double.Parse(prijeOperacije) + double.Parse(posljeOperacije);
                        userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                        userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                        userInputOperacije.RemoveAt(Operacije[0].index - 1);
                        numOnly = true;
                        print = false;
                    }
                    else if (IsNumber(prijeOperacije) || IsNumber(posljeOperacije))
                        throw new ArgumentException("Nije moguæe zbrojiti matricu sa skalarom");
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

            }
            else if (Operacije[0].operacija == '-')
            {
                string KonzolniString;

                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
                bool numOnly = false;

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije + "", MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(userInputOperacije[Operacije[0].index - 1]) && IsNumber(posljeOperacije))
                    {
                        double rj = double.Parse(prijeOperacije) - double.Parse(posljeOperacije);
                        userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                        userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                        userInputOperacije.RemoveAt(Operacije[0].index - 1);
                        numOnly = true;
                        print = false;
                    }
                    else if (IsNumber(prijeOperacije) || IsNumber(posljeOperacije))
                        throw new ArgumentException("Nije moguæe zbrojiti matricu sa skalarom");
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

            }
            else if (Operacije[0].operacija == '*')
            {
                string KonzolniString;
                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];
                bool numOnly = false;

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije + "", MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                    {
                        double rj = double.Parse(prijeOperacije) * double.Parse(posljeOperacije);
                        userInputOperacije[Operacije[0].index + 1] = $"{rj}";
                        userInputOperacije.RemoveAt(Operacije[0].index);    //Uklanja operande i operaciju
                        userInputOperacije.RemoveAt(Operacije[0].index - 1);
                        numOnly = true;
                        print = false;

                    }
                    else if (A == null && !IsNumber(prijeOperacije) || B == null && !IsNumber(posljeOperacije))
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

            }
            else if (Operacije[0].operacija == '^')
            {
                string KonzolniString;
                string[] razdvajanjeNaBazuIEksponent = userInputOperacije[Operacije[0].index].Split('^');
                string prijeOperacije = razdvajanjeNaBazuIEksponent[0];
                string posljeOperacije = razdvajanjeNaBazuIEksponent[1];
                bool numOnly = false;

                if (!IsNumber(posljeOperacije) && posljeOperacije != "T")
                {
                    throw new ArgumentException("Nakon znaka \"^\" mora biti broj ili oznaka za transpoziciju \"T\"");
                }

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                if (A == null)
                    if (IsNumber(prijeOperacije))
                    {
                        double rj;
                        numOnly = true;
                        if (posljeOperacije == "T")
                            rj = double.Parse(prijeOperacije);
                        else
                            rj = Math.Pow(double.Parse(prijeOperacije), double.Parse(posljeOperacije));

                        userInputOperacije[Operacije[0].index] = $"{rj}";
                        print = false;

                    }
                    else throw new ArgumentException("A^B -> A mora biti broj ili ime matrice");

                if (!numOnly)
                {
                    if (posljeOperacije == "T")
                    {
                        KonzolniString = $"{Konzola.transpozicija} {A.imeMatrice} {Konzola.spremiUVarijablu} TMP{tempInt}";
                        Konzola.Input(KonzolniString, MatriceCopy, internalCall);
                    }
                    else if (int.Parse(posljeOperacije) > 0)
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

            }
            else if (Operacije[0].operacija == '=')
            {
                string KonzolniString;
                string prijeOperacije = userInputOperacije[Operacije[0].index - 1];
                string posljeOperacije = userInputOperacije[Operacije[0].index + 1];

                var A = Fn.FindPerName(prijeOperacije + "", MatriceCopy);
                var B = Fn.FindPerName(posljeOperacije + "", MatriceCopy);
                if (A == null || B == null)
                    if (IsNumber(prijeOperacije) && IsNumber(posljeOperacije))
                    {
                        if (double.Parse(prijeOperacije) == double.Parse(posljeOperacije))
                        {
                            Console.Write("Lijeva i desna strana su jednake! ");
                            Konzola.KonzolaGreenBGLine("TRUE");
                            return (int)returnFlags.JednadzbaTrue;
                        }
                        Console.Write("Lijeva i desna strana nije jednaka! ");
                        Konzola.KonzolaRedBGLine("FALSE");
                        return (int)returnFlags.JednadzbaFalse;
                    }
                    else if (A == null && IsNumber(posljeOperacije))
                        throw new ArgumentException($"Nije moguæe spremiti brojèanu vrijednost u varijablu za matrice. (\"{prijeOperacije} = {posljeOperacije}\")");
                    else if (B == null)
                        throw new ArgumentException($"Nije pronaðena matrica {posljeOperacije}!");
                    else if (A == null) 
                    {
                        KonzolniString = $"{Konzola.mnozenje} 1 {B.imeMatrice} {Konzola.spremiUVarijablu} {prijeOperacije}";
                        Konzola.Input(KonzolniString, Matrice, internalCall);
                        Operacije.RemoveAt(0);
                        return (int)returnFlags.exitResultStored;
                    }


                if ( A != null )
                {
                    if (Fn.IsEqual(A, B))
                    {
                        Console.Write("Lijeva i desna strana su jednake! ");
                        Konzola.KonzolaGreenBGLine("TRUE");
                        Operacije.RemoveAt(0);
                        return (int)returnFlags.JednadzbaTrue;
                    }
                    else
                    {
                        Console.Write("Lijeva i desna strana nisu jednake! ");
                        Konzola.KonzolaRedBGLine("FALSE");
                        Operacije.RemoveAt(0);
                        string imeNoveMatrice = A.imeMatrice;
                        if (!A.imeMatrice.Contains("TMP"))
                        if (Fn.ReDefiniraj(A, Matrice))
                        {
                            Matrica Rj = new(imeNoveMatrice, B.elementiMatrice);
                            Matrice.Remove(A);
                            Matrice.Add(Rj);
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

            if ((int)returnFlags.JednadzbaFalse == rfll || (int)returnFlags.JednadzbaTrue == rfll) return (int)returnFlags.softExit;
            if ((int)returnFlags.softExit == rfll) return (int)returnFlags.softExit;

            if (rfll == (int)returnFlags.normal)
                Matrice.Add(MatriceCopy[MatriceCopy.Count - 1]);

            if (rfll == (int)returnFlags.exitResultStored)
            {
                var NameColTest = Fn.FindPerName(MatriceCopy[MatriceCopy.Count - 1].imeMatrice, Matrice);
                if (NameColTest != null) Matrice.Remove(NameColTest);
                Matrice.Add(MatriceCopy[MatriceCopy.Count - 1]);
            }

            if (IsNumber(userInput)) Console.WriteLine(userInput);

            if (rfll == (int)returnFlags.err)
                return (int)returnFlags.err;

            if (tempInt == 0 && !IsNumber(userInput) && print)
                Konzola.Input($"PRINT {MatriceCopy[MatriceCopy.Count - 1].imeMatrice}", Matrice);

            if (tempInt == 0 && rfll == (int)returnFlags.normal)
                Matrice.Remove(MatriceCopy[MatriceCopy.Count - 1]);


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

        private static void PrintHelpMeni()
        {
            //Za Kalkulator
            Console.WriteLine("\nHELP MENI ZA KALKULATOR MATRICA\n\n" +
                              "od verzije 2.1 sljedeæe komande su dostupne: \n\n\n" +
                              "\"{0} [ImeMatrice]\" \n" +
                              "\t- Zapoèinje kreaciju matrice [ImeMatrice]\n\n" +
                              "\"{1}\", \"?\"\n" +
                              "\t- Pokaže ovaj meni\n\n" +
                              "\"{2}\" \n" +
                              "\t- Izaðe iz kalkulatora u konzolu\n\n" +
                              "\"{3}\", \"ESC\"\n" +
                              "\t- Izaðe iz programa\n\n", UserInputParser.defineVariable, UserInputParser.helpMeni, UserInputParser.exitToConsole, UserInputParser.exitFromCalc);
            Console.WriteLine("Operacije \"A + B\", \"A - B\", \"A * B\", \"A = B\"");
            Console.WriteLine("\t- Moraju imati 2 ili više operanda npr (A = B, A + B, ...)");
            Console.WriteLine("\t- Operandi su imena matrica i brojevi (numericke vrjednosti)");
            Console.WriteLine("\t- Ove operacije MORAJU se pisati sa razmakom oko njih npr (A_=_B) gdje je \"_\" razmak\n");

            Console.WriteLine("Operacije potenciranja: \"A^B\"");
            Console.WriteLine("\t- Moraju imati 2 operanda");
            Console.WriteLine("\t- Pišu se BEZ razmaka npr (2^3)");
            Console.WriteLine("\t- A - broj ili matrica");
            Console.WriteLine("\t- B - (decimalan broj ako je A broj), pozitivan broj ili slovo \"T\"\n");
            return;
        }

        public static bool IsNumber(string input)
        {
            if (input == null) return false;

            try
            {
                double.Parse(input);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;

        }
    }

    public static class CommandColor
    {
        public static string LineColoring(string userInput, List<Matrica> Matrice)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            //          Console.Write("\x1b[u\x1b[2K\r>");
            Console.Write("\x1b[1A\r\x1b[2K\r>");

            List<string> userInputOperandi = new List<string> { "EMPTY" };
            userInputOperandi.RemoveAt(0);
            if (userInputOperandi.Count > 0) throw new InvalidProgramException("Prazno polje operanada nije prazno!");

            string tempInput = "";
            int brojOperanada = 0;


            for (int i = 0; i < userInput.Length; i++)
            {
                tempInput += userInput[i];
                
                if ( tempInput == "+" || tempInput == "-" || tempInput == "*" || tempInput == "/" || tempInput == "^" || tempInput == "=" )
                {
                    userInputOperandi.Add("\x1b[97m" + tempInput);
                    brojOperanada++;
                    tempInput = "";
                    continue;
                }

                if (tempInput == " ")
                {
                    tempInput = "";
                    continue;
                }

                if (userInput[(i+1)%userInput.Length] == ' ' || i == userInput.Length - 1 || char.IsAscii(userInput[(i + 1) % userInput.Length]) && !char.IsLetterOrDigit(userInput[(i + 1) % userInput.Length]))
                {
                    //Ako je kljuèna rijeè -> obojaj u ljubièasto
                    if (tempInput.ToUpper() == UserInputParser.defineVariable || tempInput.ToUpper() == UserInputParser.helpMeni ||
                        tempInput.ToUpper() == UserInputParser.exitFromCalc || tempInput.ToUpper() == UserInputParser.exitToConsole)
                    {
                        userInputOperandi.Add("\x1b[95m" + tempInput);
                    }
                    else
                    {
                        //Ako matrica ne postoji, ovisi o mjestu u komandi obojaj u zeleno(spremanje u novu) ili crveno(raèunanje s novom)
                        var Matrica = Fn.FindPerName(tempInput, Matrice);
                        if (Matrica == null)
                            if (UserInputParser.IsNumber(tempInput))
                                //Ako je broj -> obojaj u cyane
                                userInputOperandi.Add("\x1b[96m" + tempInput);
                            else if (tempInput == userInput.Split("=")[0].Split(" ")[0])
                                userInputOperandi.Add("\x1b[92m" + tempInput);
                            else
                                userInputOperandi.Add("\x1b[91m" + tempInput);
                        else
                            //Ime postojeæe matrice obojaj u žutu
                            userInputOperandi.Add("\x1b[93m" + tempInput);
                    }
                    
                    brojOperanada++;
                    tempInput = "";
                }
                
            }

            string outputString = "";

            for (int i = 0; i < userInputOperandi.Count; i++)
            {
                if ( userInputOperandi[(i+1)%userInputOperandi.Count].Contains('^') || userInputOperandi[i].Contains('^') || i == userInputOperandi.Count - 1 )
                outputString += userInputOperandi[i];
                else
                    outputString += userInputOperandi[i] + " ";
            }

            Console.WriteLine(outputString + "                        ");

            string outputStringUnEscaped = "";
            for (int i = 0; i < userInputOperandi.Count; i++)
            {
                tempInput = "";
                for (int j = 0; j < userInputOperandi[i].Length; j++)
                {
                    if ( char.IsControl( userInputOperandi[i][j] ) ) continue;
                    if (userInputOperandi[i][j] == '[')
                        for (; j < userInputOperandi[i].Length; j++)
                            if (userInputOperandi[i][j-1] == 'm') 
                                break;

                    tempInput += userInputOperandi[i][j];
                }

                if (userInputOperandi[(i + 1) % userInputOperandi.Count].Contains('^') || userInputOperandi[i].Contains('^') || i == userInputOperandi.Count - 1 )
                    outputStringUnEscaped += tempInput;
                else
                    outputStringUnEscaped += tempInput + " ";
            }

            return outputStringUnEscaped;

        }
    }

    enum returnFlags
    {
        err = -1, normal, exitResultStored, SaveToFile, JednadzbaTrue, JednadzbaFalse, exitToConsole, exitProgram, softExit
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
            else if (x.prioritet > y.prioritet)
                return 1;
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
