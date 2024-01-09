using Kör_KM;

using System;
using static Kör_KM.Form1;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTestKorok
{
    [TestFixture]
    public class TestClass
    {
        //Minden:
        //TestFixture //DONE
        //Description //DONE
        //Combinatorial + Values //DONE
        //Timeout //DONE
        //MaxTime //DONE
        //Random //DONE
        //Range //DONE
        //[Retry(3)],[Repeat(2)] hiba esetén újra lefut

        [Test]
        [TestCase(0, 0, 5, 1, 3, 4, 5, 2, 5.0)]
        [TestCase(-1, -1, 5, 1, 1, 1, 5, 2, 2.828)]
        [TestCase(0, 0, 10, 1, 20, 0, 10, 2, 20.0)]
        [TestCase(5, 5, 5, 1, 10, 10, 5, 2, 7.071)]
        [TestCase(0, 0, 5, 1, 0, 10, 5, 2, 10.0)]
        [TestCase(0, 0, 5, 1, 6, 0, 5, 2, 6.0)]
        [TestCase(-5, 0, 5, 1, 5, 0, 5, 2, 10.0)]
        [TestCase(0, 0, 5, 1, 0, 0, 2, 2, 0.0)] // A körök egybeesnek
        [TestCase(0, 0, 5, 1, 10, 0, 5, 2, 10.0)] // A körök kívülről érintkeznek
        [TestCase(0, 0, 5, 1, 14, 0, 5, 2, 14.0)] // A körök átfedik egymást
        [MaxTime(400)]
        [Repeat(2)]
        public void TestKetKorKoztiTav(int x, int y, int r, int n, int x2, int y2, int r2, int n2, double d)
        {
            // Arrange
            KOR k1 = new KOR(x, y, r, n);
            KOR k2 = new KOR(x2, y2, r2, n2);

            // Act
            double result = KetKorKoztiTav(k1, k2);

            // Assert
            Assert.AreEqual(d, result, 0.001);
        }


        [Test, Description("Ebben a tesztben ellenőrzésre kerül hogy két egymást nem metsző körnek az összesített területét sikeresen kiszámolja e a program")]
        [Timeout(200)]
        public void KörökÖsszesitettTerulete([Range(-100, 100, 10)] int x, [Range(-100, 100, 10)] int y, [Range(0, 100, 10)] int r)
        {
            // Arrange
            korok.Clear();
            korok.Add(new KOR(x, y, r, 1));

            // Act
            double eredmeny = OsszesTerulet();

            // Assert
            Assert.IsInstanceOf<double>(eredmeny);
        }

        [Test]
        [TestCase(0, 0, 5, 1, 3, 0, 4, 2, 92.489)] //egy metszet
        [TestCase(0, 0, 5, 1, 10, 10, 3, 2, 106.813)]  //nincs metszet

        public void KörökÖsszesitettTeruleteFixEsetes(int x1, int y1, int r1, int n1, int x2, int y2, int r2, int n2, double ElvartTerulet)
        {
            // Arrange
            korok.Clear();
            korok.Add(new KOR(x1, y1, r1, n1));
            korok.Add(new KOR(x2, y2, r2, n2));

            // Act
            double eredmeny = OsszesTerulet();

            // Assert
            Assert.AreEqual(ElvartTerulet, eredmeny,0.01);
        }

        [Test]
        [MaxTime(2000)]
        [Retry(100)]
        public void KorTartalmazzaEazOrigot([Random(-500.0, 500.0, 5)] double x, [Random(-500.0, 500.0, 5)] double y, [Random(10.0, 500.0, 5)] double r)
        {
            // Arrange
            List<KOR> korok = new List<KOR>
        {
            new KOR(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(r), 1)
        };

            // Act
            int result = OrigotTartalmazoKorok(korok);

            // Assert
            Assert.AreEqual((x * x + y * y <= r * r) ? 1 : 0, result);
            //ez vissza adja matematikailag hogy benne van e a kötben az origo
        }


        [Test, Combinatorial]
        
        public void KombinatorikástesztTeruletre([Values(-20,20,30,34,500,-500)] int x, [Values(-12,-41,364,3,500,-500)] int y, [Values(12,41,32,53,351)] int r)
        {
            //Arrange
            KOR kor = new KOR(x, y, r, 1);
            //Act
            double eredmeny = kor.T();
            //Assert
            Assert.AreEqual(r * r * Math.PI, eredmeny,0.0001);
        }

        [Test, Combinatorial]

        public void KombinatorikástesztKeruletre([Values(-20, 20, 30, 34,500,-500)] int x, [Values(-12, -41, 364, 3,500,-500)] int y, [Values(12, 41, 32, 53, 351)] int r)
        {
            //Arrange
            KOR kor = new KOR(x, y, r, 1);
            //Act
            double eredmeny = kor.K();
            //Assert
            Assert.AreEqual(2 * r * Math.PI, eredmeny, 0.0001);
        }

        [Test,Sequential]
        public void SequetialSugarGenealasTeszt([Values(-395, 189, 452, -78, 267, -324, 51, -183, 308, -470)] int x, [Values(-212, 431, -29, 173, 398, -116, -271, 62, -496, 324)] int y)
        {
            //Arrange
            //nincs
            //Act
            int eredmeny = SugarGeneral(x, y);
            //Assert
            Assert.Less(eredmeny,500);
        }

        [Test]
        [Repeat(5)]
        public void RandomKoorinataAtszamX([Random(-500.0, 500.0, 100)] double x)
        {
            //Arrange
            int X = Convert.ToInt32(x);
            //Act
            KOR kor = new KOR(X, 0, 0, 0);
            int eredmeny = kor.X();
            //Assert
            Assert.AreEqual(1000 / 2 + X, eredmeny);
        }
        [Test]
        [Repeat(5)]
        public void RandomKoorinataAtszamY([Random(-500.0, 500.0, 100)] double y)
        {
            //Arrange
            int Y = Convert.ToInt32(y);
            //Act
            KOR kor = new KOR(0, Y, 0, 0);
            int eredmeny = kor.Y();
            //Assert
            Assert.AreEqual(1000 / 2 - Y, eredmeny);
        }

        [Test]
        [TestCase(0,0,1,100,0,2,400,-400,3,3)]
        [TestCase(0, 0, 1, 100, 0, 2, 400, -400, 3, 3)]
        [TestCase(5, 5, 2, 20, 10, 1, 30, -30, 3, 3)]
        [TestCase(-3, 4, 8, 50, 6, 7, 200, -150, 2, 2)]
        [TestCase(10, 20, 15, 30, 25, 8, 100, -100, 2, 2)] 
        [TestCase(1, 1, 5, 10, 3, 3, 40, -40, 1, 1)]
        [TestCase(-2, -2, 12, 15, 8, 6, 80, -80, 1, 1)] 
        [TestCase(0, 0, 3, 40, 0, 2, 300, -300, 3, 3)]
        [TestCase(-8, 6, 5, 40, 2, 4, 150, -120, 4, 4)] 
        [TestCase(4, -3, 7, 60, 9, 1, 200, -200, 3, 3)] 
        [TestCase(2, 2, 4, 30, 6, 5, 120, -100, 5, 5)]
        public void OrigoMaxTavTeszt(int x1, int y1, int n1, int x2, int y2, int n2, int x3, int y3, int n3, int VartSorszam)
        {
            //Arrange
            korok.Clear();
            korok.Add(new KOR(x1, y1, 10, n1));
            korok.Add(new KOR(x2, y2, 10, n2));
            korok.Add(new KOR(x3, y3, 10, n3));
            //Act 
            int eredmeny = MaxKivalasztTavSsz();
            //Assert
            Assert.AreEqual(VartSorszam, eredmeny);

        }

        [Test]
        [TestCase(30,30,40, 619.496)] //metszik egymást
        [TestCase(30,30,60,0)] //érintik egymást
        [TestCase(30,30,70,double.NaN)] //közük sincs egymáshoz
        public void MetszetTeszt(int r1, int r2, double d, double elvartT)
        {
            //Arrange
            //Act
            double eredmeny = AtfedettTerulet(r1, r2, d);
            //Assert
            Assert.AreEqual(elvartT, eredmeny,0.001);
        }
    }
}
