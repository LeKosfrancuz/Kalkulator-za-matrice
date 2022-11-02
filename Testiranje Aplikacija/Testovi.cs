using MatricaNS;
using FunkcijeZaMatrice;


namespace Testiranje_Aplikacija
{
    [TestClass]
    public class Testovi
    {
        [TestMethod]
        public void Test_OverWriteNakonMnoženja()
        {
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

            Matrice.Add( new ("AB", Fn.MnozenjeMatrica(Matrice[0], Matrice[1])) );

            Console.WriteLine("Poèetak Množenja");
            KonzolnaKontrola.Konzola Konzola = new KonzolnaKontrola.Konzola();
            var stringReader = new StringReader("Y");
            Console.SetIn(stringReader);

            Konzola.Input("MTY TestA TestB ST TestA", Matrice);

            
            for (int i = 0; i < Matrice[2].BrojRedaka(); i++)
                for (int j = 0; j < Matrice[2].BrojStupaca(); j++)
                    if (Matrice[0].elementiMatrice[i][j] != Matrice[1].elementiMatrice[i][j]) Assert.Fail();


            Assert.IsTrue(true);
            
        }
    }
}

/*
var stringBuilder = new StringBuilder();
stringBuilder.AppendLine("Hello World");
stringBuilder.AppendLine("Hi");
var stringReader = new StringReader(stringBuilder.ToString());
Console.SetIn(stringReader);
*/