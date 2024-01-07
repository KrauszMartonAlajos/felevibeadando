using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static Kör_KM.Form1;
using NUnit.Framework;

namespace Kör_KM
{
    public partial class Form1 : Form
    {

        static List<KOR> körök = new List<KOR>();

        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// - Megrajzolja a koordináta rendszert
        /// - Le tiltja a körök megrajzolásának gombját
        /// - Meghívja az "adatfelvitel()" függvényt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Paint += KoordinataRendszerRajz;
            korok_rajz.Enabled = false;
            adatfelvitel();            
        }

        /// <summary>
        /// Le ellenőrzi hogy van e beolvasható file és ezt jelzi a felhsaználónak
        /// </summary>
        private void adatfelvitel()
        {
            try
            {
                StreamReader r = new StreamReader("input.txt");               
                label1.ForeColor = Color.Green;
            }
            catch
            {
                label1.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Fájlbeolvasás
        /// </summary>
        private void fajlbe()
        {
            StreamReader r = new StreamReader("input.txt");
            int n = 0;
            while (!r.EndOfStream)
            {
                string[] temp = r.ReadLine().Split(' ');
                körök.Add(new KOR(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2]), n + 1));
                n++;
            }
        }


        /// <summary>
        /// - Ki számolja az összes kör közös területét az átfedéséket is figyelembe véve
        /// - Először össze adja az összes kör területét
        /// - Majd egy ciklussal végig megy minden körön és kivonja azon körök metszetéből az átfedett területet melyeknek a sugarainak összege kisebb mint a két kör közti távolság
        /// </summary>
        /// <returns>Vissza adja az összesített területet (double típusban)</returns>
        public static double OsszesTerulet()
        {
            double OsszTerulet = 0.0;

            for (int i = 0; i < körök.Count; i++)
            {
                double aktkor_terulet = Math.PI * Math.Pow(körök[i].r, 2);
                OsszTerulet += aktkor_terulet;

                for (int j = i + 1; j < körök.Count; j++)
                {
                    double tavketkorkozott = KetKorKoztiTav(körök[j], körök[i]);

                    if (tavketkorkozott < körök[i].r + körök[j].r)
                    {
                        OsszTerulet -= AtfedettTerulet(körök[i].r, körök[j].r, tavketkorkozott);
                    }
                }
            }

            return OsszTerulet;
        }

        //teszt: simán nem átfedő körök, normális átfedés, nincs kör, sok kör, random körök generálása


        /// <summary>
        /// Ki számolja két kör metszetének területét
        /// </summary>
        /// <param name="r1">Az első kör sugara (double-ben megadva)</param>
        /// <param name="r2">A második kör sugara (double-ben megadva)</param>
        /// <param name="d">A két kör közti távolság (double-ben megadva)</param>
        /// <returns></returns>
        private static double AtfedettTerulet(double r1, double r2, double d)
        {
            double d1 = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            double d2 = d - d1;
            double atfedett_Terulet = r1*r1*Math.Acos(d1/r1)-d1*Math.Sqrt(r1*r1-d1*d1)+(r2 * r2 * Math.Acos(d2 / r2) - d2 * Math.Sqrt(r2 * r2 - d2 * d2));
            return atfedett_Terulet;
            //https://diego.assencio.com/?index=8d6ca3d82151bad815f78addf9b5c1c6#mjx-eqn-post_8d6ca3d82151bad815f78addf9b5c1c6_A_intersection
        }

        //teszt: két kör, egymást átfedő körök

        private static List<List<KOR>> korMatrix = new List<List<KOR>>();

        /// <summary>
        /// Egy mátrixba gyűjti a köröket amelyek érintik egymást majd ennek elemeit egy ListBoxba pakolja
        /// </summary>

        
        private void ErintosKorok()
        {
            korMatrix.Clear();
            for (int i = 0; i < körök.Count; i++)
            {
                AktKorMitErint(körök[i]);
            }
            listBox2.Items.Clear();

            for (int i = 0; i < korMatrix.Count(); i++)
            {
                listBox2.Items.Add(String.Format(korMatrix[i][0].sorszam + " <-Érinti-> " + korMatrix[i][1].sorszam));
            }
        }

        /// <summary>
        /// A beküldött kört megnézi hány másik kör érinti és az összes esetet beküldi a fentebb említett mátrixba (korMatrix)
        /// </summary>
        /// <param name="kor">Az akruálisan vizsgált kör</param>
        private static void AktKorMitErint(KOR kor)
        {
            for (int i = 0; i < körök.Count; i++)
            {
                double tav = KetKorKoztiTav(körök[i],kor);
                if (tav != 0 && (tav == (kor.r + körök[i].r)))
                {
                    korMatrix.Add(new List<KOR>() { kor, körök[i] });
                }
            }
        }


        /// <summary>
        /// Két kör közti területet adja vissza (double-ben)
        /// </summary>
        /// <param name="k1">KOR osztály első vizsgált példánya</param>
        /// <param name="k2">KOR osztály második vizsgált példánya</param>
        /// <returns>távolság a két kör között (double-ben)</returns>
        private static double KetKorKoztiTav(KOR k1, KOR k2)
        {
            return Math.Sqrt(Math.Pow(k1.X() - k2.X(), 2) + Math.Pow(k1.Y() - k2.Y(), 2));
        }
        [TestFixture]
        public class KetKorKoztiTavTestClass
        {
            [Test]
            public void TestKetKorKoztiTav()
            {
                // Arrange
                KOR k1 = new KOR(0, 0,5,1); // Replace with actual values
                KOR k2 = new KOR(3, 4,5,1); // Replace with actual values

                // Act
                double result = Form1.KetKorKoztiTav(k1, k2);

                // Assert
                //Assert.AreEqual(5, result, 0.001); // Adjust the tolerance (0.001 in this case) based on your needs
            }
        }

        /// <summary>
        /// Megadja hány db kör tartalmazza magán belül az origót
        /// </summary>
        /// <returns>A feltételnek megfelelő körök számát</returns>
        private int OrigotTartalmazoKorok(List<KOR> körök)
        {
            int db = 0;
            for (int i = 0; i < körök.Count; i++)
            {
                if (körök[i].tav <= körök[i].r)
                {
                    db++;
                }
            }
            return db;
        }

        //teszt: olyan lista amiben nincsen origós, olyan amiben van, random körök generálása


        /// <summary>
        /// Megadja melyik kör van a legtávolabb az origó ponttól
        /// </summary>
        private void OrigoMaxTav()
        {
            double kor = 0;
            int ssz = 0;
            for (int i = 0; i < körök.Count; i++)
            {
                if (körök[i].tav > kor)
                {
                    kor = körök[i].tav;
                    ssz = körök[i].sorszam;
                }
            }
            label5.Text = String.Format(ssz + "-es sorszámú kör van a legtávolabb az origótól");
        }

        /// <summary>
        /// Minden vizsgált kört listáz a combo boxokba hogy a felhasználó ki tudja őket választani távolság mérésnél
        /// </summary>
        private void ComboBoxbaPakol()
        {
            for (int i = 0; i < körök.Count; i++)
            {
                comboBox1.Items.Add(String.Format(Convert.ToString(körök[i].sorszam) + " Kör"));
                comboBox2.Items.Add(String.Format(Convert.ToString(körök[i].sorszam) + " Kör"));
            }
        }

        /// <summary>
        /// Megrajzolja a koordináta rendszert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KoordinataRendszerRajz(object sender, PaintEventArgs e)
        {
            Graphics rajz = e.Graphics;
            Pen tengely = new Pen(Color.Black, 2);
            Pen origo = new Pen(Color.Red, 4);
            rajz.DrawLine(tengely, panel1.Width / 2, 0, panel1.Width / 2, panel1.Height);
            rajz.DrawLine(tengely, 0, panel1.Height / 2, panel1.Width, panel1.Height / 2);
            rajz.DrawEllipse(origo, panel1.Width / 2 - 2, panel1.Height / 2 - 2, 4, 4);
        }

        /// <summary>
        /// Először eldönti hogy rajzolt körökkel lesz feltöltve a lista vagy generáltakkal
        /// Aztán függvénymeghívások kerülnek ide
        /// Majd megrajzolja a köröket egyesével a kor_rajzolas függvény segítségével
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void korok_rajz_Click(object sender, EventArgs e)
        {
            if (label2.BackColor == Color.DarkGreen)
            {
                KorGenRandom();
            }
            else if (label1.BackColor == Color.DarkGreen)
            {
                fajlbe();
            }
            ComboBoxbaPakol();
            OrigoMaxTav();
            ErintosKorok();
            label4.Text = String.Format("A körök összesített Területe: " + Convert.ToString(OsszesTerulet()));
            label3.Text = OrigotTartalmazoKorok(körök).ToString() + " kör tartalmazza az Origót";
            listBox1.Items.Clear();

            for (int i = 0; i < körök.Count; i++)
            {
                kor_rajzolas(körök[i].X(), körök[i].Y(), körök[i].r, körök[i].sorszam);
                listBox1.Items.Add(körök[i].ToString());
            }
            korok_rajz.Enabled = false;
        }

        /// <summary>
        /// Meg rajzolja az adott kört az adatai alapján
        /// </summary>
        /// <param name="x">a kör x koordinátája</param>
        /// <param name="y">a kör y koordinátája</param>
        /// <param name="r">a kör sugara</param>
        /// <param name="sorszam">a kör sorszáma</param>
        private void kor_rajzolas(int x, int y, int r, int sorszam)
        {
            Graphics rajz = panel1.CreateGraphics();
            Pen ceruza = new Pen(Color.Green, 2);
            rajz.DrawEllipse(ceruza, x - r, y - r, 2 * r, 2 * r);
            rajz.FillEllipse(Brushes.Red, x - 4 / 2, y - 4 / 2, 4, 4);
            Font betu = new Font("Arial", 12);
            rajz.DrawString(Convert.ToString(sorszam) + ".", betu, Brushes.Black, x + 10, y - 9);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Megadja lenyomásakor a két kör közti távolságot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                double tav = KetKorKoztiTav(körök[comboBox1.SelectedIndex], körök[comboBox2.SelectedIndex]);
                tavolsagaketkornek.Text = String.Format("A két kiválasztott kör távolsága: " + Convert.ToString(tav));
            }
            catch
            {
                MessageBox.Show("Válasszon két kört, ne egyet vagy eggyet se kettőt!");
            }

        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            körök.Clear();           
            MessageBox.Show("Adatok sikeresen felvéve");
        }



        public class KOR
        {
            public int x, y, r;
            public double tav;
            public int sorszam;


            public KOR(int x, int y, int r, int sorszam)
            {
                this.x = x;
                this.y = y;
                this.r = r;
                tav = Math.Sqrt(x * x + y * y);
                //tav számolása az origótol
                this.sorszam = sorszam;
            }

            public double T()
            {
                return (r * r * Math.PI);
            }
            //teszt jó területet ad e vissza

            public double K()
            {
                return (2 * r * Math.PI);
            }
            //teszt jó kerületet ad e vissza

            public int X()
            {
                return (1000 / 2 + x);
            }
            //x koordináta átszámolása koordináta rendszerbe

            public int Y()
            {
                return (1000 / 2 - y);
            }
            //y koordináta átszámolása koordináta rendszerbe

            public double TavolsagAzOrigotol()
            {
                return tav;
            }


            public override string ToString()
            {
                return String.Format(sorszam + ". (" + x + "," + y + ")" );
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }


        static int s = 0;
        // a gomb váltáshoz kell
        /// <summary>
        /// Ezzel vált vizuálisan a két lehetőség közül és változtatja azok színét
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            körök.Clear();
            korok_rajz.Enabled = true;
            if (s % 2 == 0)
            { 
                label2.BackColor = SystemColors.ActiveCaption;
                label1.BackColor = Color.DarkGreen;  
                s++;
            }
            else 
            {
                label2.BackColor = Color.DarkGreen;
                label1.BackColor = SystemColors.ActiveCaption;
                s++;
            }
        }

        /// <summary>
        /// Megadott darab kört generál figyelve arra hogy ne csússzon ki egy se a koordináta rendszerből
        /// </summary>
        private void KorGenRandom()
        {
            Random r = new Random();
            int dbkor = Convert.ToInt32(numericUpDown1.Value);
            for (int i = 0; i < dbkor; i++)
            {
                int a = r.Next(-500, 500);
                int b = r.Next(-500, 500);
                if ((Math.Min(500 - Math.Abs(a), 500 - Math.Abs(b))) < 10)
                {
                    a = r.Next(-500, 500);
                    b = r.Next(-500, 500);
                }
                else 
                {                    
                    int sugar = r.Next(10, Math.Min(500 - Math.Abs(a), 500 - Math.Abs(b)));
                    körök.Add(new KOR(a, b, sugar, i + 1));
                }
            }
            
        }

        /// <summary>
        /// Töröl minden háttér adatok és a kooridináta rendszert hogy új köröket rajzolhasson a felhasználó
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_2(object sender, EventArgs e)
        {
            körök.Clear();
            korok_rajz.Enabled = true;
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            panel1.Invalidate();
        }


        /// <summary>
        /// A lenyomásakor a kiválasztott kör adatai bekerülnek egy másik listBoxba
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                listBox3.Items.Clear();
                listBox3.Items.Add("Sorszám: " + körök[listBox1.SelectedIndex].sorszam + ".");
                listBox3.Items.Add("Közép pontja: (" + körök[listBox1.SelectedIndex].X() + "," + körök[listBox1.SelectedIndex].Y() + ")");
                listBox3.Items.Add("Sugár: " + körök[listBox1.SelectedIndex].r);
                listBox3.Items.Add("Kerület: " + körök[listBox1.SelectedIndex].K());
                listBox3.Items.Add("Terület: " + körök[listBox1.SelectedIndex].T());
                listBox3.Items.Add("Távolság az origótól: " + körök[listBox1.SelectedIndex].tav);
            }
            catch { };
        }
    }
}
