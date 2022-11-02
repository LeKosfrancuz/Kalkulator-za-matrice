using System.Runtime.Serialization.Formatters.Binary;

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
