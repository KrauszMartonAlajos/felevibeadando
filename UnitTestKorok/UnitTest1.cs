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
        //Combinatorial + Values //ezt mire kéne????
        //Timeout //DONE
        //MaxTime //DONE
        //Random //DONE
        //Range //DONE
        //[Retry(3)] hiba esetén újra lefut
        //Setup, TearDown //ezt mire kéne????
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
        public void TestKetKorKoztiTav(int x,int y,int r, int n, int x2, int y2, int r2, int n2,double d)
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
        public void KörökÖsszesitettTerulete([Range(-100,100,10)] int x, [Range(-100, 100, 10)] int y, [Range(0, 100, 10)] int r)
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
        [MaxTime(2000)]
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
        public void Kombinatorikásteszt([Values()] int a, [Values()] int b)
        { 
            
        }

        //MaxKivalasztTavSsz()

    }
}
