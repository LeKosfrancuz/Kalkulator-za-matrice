using NUnit.Framework;
using MatricaNS;
using KonzolnaKontrola;
using UserInputParser;
using FunkcijeZaMatrice;


namespace Testovi;

public class TestiranjeKonzole
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

        matricaT1 = new List<double> { -16.4, 12.34 };
        matricaT2 = new List<double> { 15555234234.5, 0 };
        List<double> matricaT3 = new List<double> { 75, -1 };
        matricaT = new List<List<double>> { matricaT1, matricaT2, matricaT3 };
        inputMatrica = new Matrica("TestC", matricaT);
        Matrice.Add(inputMatrica);

        matricaT1 = new List<double> { 0, 25, 92 };
        matricaT2 = new List<double> { -125, 78.13, 53 };
        matricaT = new List<List<double>> { matricaT1, matricaT2 };
        inputMatrica = new Matrica("TestD", matricaT);
        Matrice.Add(inputMatrica);
    }

    private Matrica CreateMatricaAplusB()
    {
        List<double> matricaT1 = new List<double> { 3, 6 };
        List<double> matricaT2 = new List<double> { 7, 5 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica AplusB = new Matrica("AplusB", matricaT);

        return AplusB;
    }

    private Matrica CreateMatricaAputaB()
    {
        List<double> matricaT1 = new List<double> { 10, 6 };
        List<double> matricaT2 = new List<double> { 22, 16 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica AputaB = new Matrica("AputaB", matricaT);

        return AputaB;
    }

    private Matrica CreateMatricaCputaD()
    {
        List<double> matricaT1 = new List<double> { -1542.5, 554.1242, -854.78};
        List<double> matricaT2 = new List<double> { 0, 388880855862.5, 1431081549574 };
        List<double> matricaT3 = new List<double> { 125, 1796.87, 6847 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2, matricaT3 };
        Matrica CputaD = new Matrica("CputaD", matricaT);

        return CputaD;
    }

    private Matrica CreateMatricaDputaC()
    {
        List<double> matricaT1 = new List<double> { 388880862762.5, -92 };
        List<double> matricaT2 = new List<double> { 1215330456766.4849, -1595.5 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica CputaD = new Matrica("CputaD", matricaT);

        return CputaD;
    }

    private Matrica CreateMatricaAxDxCxD()
    {
        List<double> matricaT1 = new List<double> { 410375, 70488544150885.95, 259397843245184.22};
        List<double> matricaT2 = new List<double> { 832250, 150699109863646.44, 554572725859642.44 };
        List<List<double>> matricaT = new List<List<double>> { matricaT1, matricaT2 };
        Matrica rjesenje = new Matrica("A*D*C*D", matricaT);

        return rjesenje;
    }

    [Test]
    public void ZbrajanjeDvijeMatrice()
    {
        Konzola konzola = new Konzola();
        konzola.Input(konzola.zbrajanje + " TestA TestB " + konzola.spremiUVarijablu + " A+B", Matrice, true);

        Matrica? dobiveno = Fn.FindPerName("A+B", Matrice);
        Assert.IsNotNull(dobiveno);

        Matrica AplusB = CreateMatricaAplusB();

        Assert.IsTrue(Fn.IsEqual(AplusB, dobiveno));
    }

    [Test]
    public void OgranicenjeZbrajanja_MatriceRazlicitihDimenzija()
    {
        Konzola konzola = new Konzola();
        try
        {
            konzola.Input(konzola.zbrajanje + " TestC TestD " + konzola.spremiUVarijablu + " C+D", Matrice, true);
        } catch(InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            Assert.Pass();
        }

        Assert.Fail();
    }

    [Test]
    public void MnozenjeDvijeMatrice_AxB()
    {
        Konzola konzola = new Konzola();
        konzola.Input(konzola.mnozenje + " TestA TestB " + konzola.spremiUVarijablu + " A*B", Matrice, true);

        Matrica? dobiveno = Fn.FindPerName("A*B", Matrice);
        Assert.IsNotNull(dobiveno);

        Matrica AputaB = CreateMatricaAputaB();

        Assert.IsTrue(Fn.IsEqual(AputaB, dobiveno));
    }

    [Test]
    public void MnozenjeDvijeMatrice_CxD()
    {
        Konzola konzola = new Konzola();
        konzola.Input(konzola.mnozenje + " TestC TestD " + konzola.spremiUVarijablu + " C*D", Matrice, true);

        Matrica? dobiveno = Fn.FindPerName("C*D", Matrice);
        Assert.IsNotNull(dobiveno);

        Matrica CputaD = CreateMatricaCputaD();

        Assert.IsTrue(Fn.IsEqual(CputaD, dobiveno));
    }

    [Test]
    public void MnozenjeDvijeMatrice_DxC()
    {
        Konzola konzola = new Konzola();
        konzola.Input(konzola.mnozenje + " TestD TestC " + konzola.spremiUVarijablu + " D*C", Matrice, true);

        Matrica? dobiveno = Fn.FindPerName("D*C", Matrice);
        Assert.IsNotNull(dobiveno);

        Matrica DputaC = CreateMatricaDputaC();

        Assert.IsTrue(Fn.IsEqual(DputaC, dobiveno));
    }

    [Test]
    public void MnozenjeMatrica_Ax_DxCxD_() // A * ( D * C * D )
    {
        Konzola konzola = new Konzola();
        konzola.Input(konzola.mnozenje + " TestD TestC " + konzola.spremiUVarijablu + " D*C", Matrice, true);
        konzola.Input(konzola.mnozenje + " D*C TestD " + konzola.spremiUVarijablu + " D*C*D", Matrice, true);
        konzola.Input(konzola.mnozenje + " TestA D*C*D " + konzola.spremiUVarijablu + " A*D*C*D", Matrice, true);

        Matrica? dobiveno = Fn.FindPerName("A*D*C*D", Matrice);
        Assert.IsNotNull(dobiveno);

        Matrica ocekivano = CreateMatricaAxDxCxD();

        Assert.IsTrue(Fn.IsEqual(ocekivano, dobiveno));
    }

    [Test]
    public void OgranicenjeMnozenja_MatriceNisuUlancane()
    {
        Konzola konzola = new Konzola();
        try
        {
            konzola.Input(konzola.mnozenje + " TestD TestA " + konzola.spremiUVarijablu + " D*A", Matrice, true);
        } catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            Assert.Pass();
        }

        Assert.Fail();
    }
}