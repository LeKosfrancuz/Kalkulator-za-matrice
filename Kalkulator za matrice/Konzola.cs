using FunkcijeZaMatrice;
using MatricaNS;

namespace KonzolnaKontrola
{
    enum returnFlagsKonsole
    {
        err = -1, normal, exit, saveNExit
    }
    public class Konzola
    {
        //Kljucne rijeci za komande (MORA BITI VELIKIM SLOVIMA / malim i metodom ToUpper())
        public string zbrajanje = "ADD".ToUpper();
        public string mnozenje = "MTY".ToUpper();
        public string ispisMatrice = "PRINT".ToUpper();
        public string definiranjeMatrice = "DEF".ToUpper();
        public string spremiUVarijablu = "ST".ToUpper();
        public string exitProgram = "ESC".ToUpper();
        public string helpMenu = "HELP".ToUpper();
        public string transpozicija = "T".ToUpper();


        public int Input(string userInput, List<Matrica> Matrice, bool internalCall = false)
        //vraca 0 - ako nema iznimki, -1 - ako se pojavila greska, ali je rjesena, 1 - za izlaz iz aplikacije, 2 - za izlaz sa spremanjem u file
        {
            bool saveToFile = false;
            string userCommand = userInput + "";
            string[] userCommandSplit = userCommand.Split(" ", 2); //0 - komanda, 1- operandi
            string userCommandFunc = userCommandSplit[0].ToUpper();

            if (userCommandFunc == "?" || userCommandFunc == helpMenu)
            {
                //Za Kalkulator
                Console.WriteLine("Operacije \"A + B\", \"A - B\", \"A * B\", \"A = B\"");
                Console.WriteLine("\t- Moraju imati 2 ili više operanda npr (A = B, A + B, ...)");
                Console.WriteLine("\t- Operandi su imena matrica i brojevi (numericke vrjednosti)");
                Console.WriteLine("\t- Ove operacije MORAJU se pisati sa razmakom oko njih npr (A_=_B) gdje je \"_\" razmak\n");

                Console.WriteLine("Operacije potenciranja: \"A^B\"");
                Console.WriteLine("\t- Moraju imati 2 operanda");
                Console.WriteLine("\t- Pišu se BEZ razmaka npr (2^3)");
                Console.WriteLine("\t- A - broj ili matrica");
                Console.WriteLine("\t- B - (decimalan broj ako je A broj), pozitivan broj ili slovo \"T\"\n");

                Console.WriteLine("HELP MENI ZA KALKULATOR MATRICA\n\n" +
                                  "od verzije 2.1 sljedeće komande su dostupne: \n\n" +
                                  "\"{0} [ImeMatrice]\" \n" + 
                                  "\t- Započinje kreaciju matrice [ImeMatrice]\n\n" + 
                                  "\"{1}\", \"?\"\n" + 
                                  "\t- Pokaže ovaj meni\n\n" +
                                  "\"{2}\" \n" +
                                  "\t- Izađe iz kalkulatora u konzolu\n\n" +
                                  "\"{3}\", \"ESC\"\n" +
                                  "\t- Izađe iz programa\n\n", UserInputParser.UserInputParser.defineVariable, UserInputParser.UserInputParser.helpMeni, UserInputParser.UserInputParser.exitToConsole, UserInputParser.UserInputParser.exitFromCalc);


                //Za Konzolu
                Console.WriteLine("\nOperacije s matricama:");
                Console.WriteLine("  \"ADD {Ime Matrice} {Ime Matrice}\"        - Zbrajanje matrica");
                Console.WriteLine("  \"MTY {Ime Matrice} {Ime Matrice}\"        - Množenje matrica");
                Console.WriteLine("  \"MTY {skalar} {Ime Matrice}\"             - Množenje matrice sa skalarom");
                Console.WriteLine("\nOperacije s podatcima:");
                Console.WriteLine("  \"PRINT {Ime Matrice}\"                    - Ispis Matrice");
                Console.WriteLine("  \"PRINT ALL\"                              - Ispis svih Matrica");
                Console.WriteLine("  \"DEF {Ime Nove Matrice}\"                 - Definiranje varijable za novu matricu");
                Console.WriteLine("  \"ST {Ime Nove Matrice}\"                  - Dolazi nakon matematičke operacije za spremanje rezultata u novu matricu npr(ADD A B ST C)");
                Console.WriteLine("  \"HELP\", \"?\"                              - Ispisuje ovaj meni");
                Console.WriteLine("  \"Q\", \"ESC\"                               - Zatvara program");
                return (int)returnFlagsKonsole.normal;
            }

            if (userCommandFunc == "Q" || userCommandFunc == exitProgram) { return (int)returnFlagsKonsole.exit; }


            if (userCommandSplit.Length == 1 && userCommandFunc != "Q" && userCommandFunc != exitProgram) { Console.WriteLine("Potrebni su operandi (nakon ključne riječi)"); return (int)returnFlagsKonsole.err; }

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
                    catch (InvalidOperationException e) { Console.WriteLine("{0}\n", e.Message); return -1; }
                }
                else if (userCommandArgs[3].Split(" ", 2).Length == 1 && userCommandArgs[2].ToUpper() == spremiUVarijablu && userCommandArgs[3] != "")
                {
                    Matrica Rjesenje;
                    if (!internalCall)
                        try
                        {
                            Rjesenje = new Matrica(userCommandArgs[3], Fn.ZbrajanjeMatrica(MatricaA, MatricaB));
                        }
                        catch (InvalidOperationException e) { Console.WriteLine("{0}\n", e.Message); return -1; }
                    else Rjesenje = new Matrica(userCommandArgs[3], Fn.ZbrajanjeMatrica(MatricaA, MatricaB));

                    //Provjera da ne postoji istoimena matrica u memoriji
                    var MatricaNameColTest = Fn.FindPerName(userCommandArgs[3], Matrice);
                    if (MatricaNameColTest == null)
                    {
                        Matrice.Add(Rjesenje);
                    }
                    else if (internalCall)
                    {
                        Matrice.Remove(MatricaNameColTest);
                        Matrice.Add(Rjesenje);
                    }
                    else
                    {
                        bool ReDefinirana = Fn.ReDefiniraj(MatricaNameColTest, Matrice);
                        if (ReDefinirana)
                            Matrice.Add(Rjesenje);
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
                        catch (InvalidOperationException e) { Console.WriteLine("{0}\n", e.Message); return -1; }
                    }
                }
                else if (userCommandArgs[3].Split(" ", 2).Length == 1 && userCommandArgs[2].ToUpper() == spremiUVarijablu && userCommandArgs[3] != "")
                {
                    Matrica Rjesenje;
                    if (!skalarnoMnozenje)
                        if (!internalCall)
                            try
                            {
                                Rjesenje = new Matrica(userCommandArgs[3], Fn.MnozenjeMatrica(MatricaA, MatricaB));
                            }
                            catch (InvalidOperationException e) { Console.WriteLine("{0}\n", e.Message); return -1; }
                        else Rjesenje = new Matrica(userCommandArgs[3], Fn.MnozenjeMatrica(MatricaA, MatricaB));
                    else Rjesenje = new Matrica(userCommandArgs[3], Fn.MnozenjeMatricaSkalarom(skalar, MatricaB));

                    //Provjera da ne postoji istoimena matrica u memoriji
                    var MatricaNameColTest = Fn.FindPerName(userCommandArgs[3], Matrice);
                    if (MatricaNameColTest == null)
                    {
                        Matrice.Add(Rjesenje);
                    }
                    else if (internalCall)
                    {
                        Matrice.Remove(MatricaNameColTest);
                        Matrice.Add(Rjesenje);
                    } 
                    else
                    {
                        bool ReDefinirana = Fn.ReDefiniraj(MatricaNameColTest, Matrice);
                        if (ReDefinirana)
                            Matrice.Add(Rjesenje);
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
                    if (imeVarijableNoveMatrice != "") throw new ArgumentException("Ime matrice ne smije biti razmak \" \"");
                    if (MatricaNameColTest == null)
                    {
                        Matrice.Add(Rjesenje);
                    }
                    else if (internalCall)
                    {
                        Matrice.Remove(MatricaNameColTest);
                        Matrice.Add(Rjesenje);
                    }
                    else
                    {
                        KonzolaRedTX("Nije moguće spremiti rezultat ");
                        Console.WriteLine("jer matrica s tim imenom već postoji!");
                    }
                }
                else
                    Fn.IspisMatrice(Rjesenje.elementiMatrice);
                return 0;
            }

            return 0;
        }

        public static void KonzolaRedBG(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(s);
            Console.ResetColor();
        }

        public static void KonzolaRedTX(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(s);
            Console.ResetColor();
        }

        public static void KonzolaGreenBG(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(s);
            Console.ResetColor();
        }
    }
}
