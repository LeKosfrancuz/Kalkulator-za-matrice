namespace Testovi;

public class TestiranjeKalkulatora
{
    List<Matrica> Matrice { get; set; }
    [SetUp]
    public void Setup()
    {
        List<double> matricaT1 = new List<double> { 1, 2 };
        List<double> matricaT2 = new List<double> { 3, 4 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica inputMatrica = new Matrica("TestA", matricaT);
        List<Matrica> matrice = new List<Matrica> { inputMatrica };
        Matrice = matrice;

        matricaT1 = new List<double> { 2, 4 };
        matricaT2 = new List<double> { 4, 1 };
        matricaT = new List<List<double>> { matricaT1, matricaT2 };
        inputMatrica = new Matrica("TestB", matricaT);
        Matrice.Add(inputMatrica);
    }

    private static Matrica CreateMatricaX()
    {
        List<double> matricaT1 = new List<double> { 565524063051.5769, 824210098857.2455 };
        List<double> matricaT2 = new List<double> { 1236315147783.2615, 1801839209507.147 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica inputMatrica = new Matrica("X", matricaT);

        return inputMatrica;
    }

    [Test]
    public void Test_XSeSpremaUPoljeMatrica()
    {
        List<string> userInput = new List<string> {"A = TestA * 2"};
        userInput.Add("B = TestB * 0.2");
        userInput.Add("X = A * B * B + B * A * B * B * A * B * A * B + A + B * B * A * B + B + A + B * B * 2 * 5 * B * TestB + TestA + TestB * 5 - TestA * 5 * 5 * 5 * 5 * TestA + A^12");

        foreach (string input in userInput)
        {
            int res = UserInputParser.UserInputParser.UserToKonsoleTranslator(input, Matrice);
            if (res == (int)returnFlagsParser.err) Assert.Fail();
        }

        Matrica? dobivena = Fn.FindPerName("X", Matrice);
        if (dobivena == null) { Assert.Fail(); return; }
        
        Matrica X = CreateMatricaX();
        //Assert.That(dobivena, Is.EqualTo(X));
        Assert.True(Fn.IsEqual(X, dobivena));
    }
}