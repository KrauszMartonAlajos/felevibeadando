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

        public static List<KOR> korok = new List<KOR>();

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
        public void Form1_Load(object sender, EventArgs e)
        {
            panel1.Paint += KoordinataRendszerRajz;
            korok_rajz.Enabled = false;
            adatfelvitel();            
        }

        /// <summary>
        /// Le ellenőrzi hogy van e beolvasható file és ezt jelzi a felhsaználónak
        /// </summary>
        public void adatfelvitel()
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
        public void fajlbe()
        {
            StreamReader r = new StreamReader("input.txt");
            int n = 0;
            while (!r.EndOfStream)
            {
                string[] temp = r.ReadLine().Split(' ');
                korok.Add(new KOR(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), Convert.ToInt32(temp[2]), n + 1));
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

            for (int i = 0; i < korok.Count; i++)
            {
                double aktkor_terulet =korok[i].T(); //fv
                OsszTerulet += aktkor_terulet;

                for (int j = i + 1; j < korok.Count; j++)
                {
                    double tavketkorkozott = KetKorKoztiTav(korok[j], korok[i]);

                    if (tavketkorkozott < korok[i].r + korok[j].r)
                    {
                        OsszTerulet -= AtfedettTerulet(korok[i].r, korok[j].r, tavketkorkozott);
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
        public static double AtfedettTerulet(double r1, double r2, double d)
        {
            double d1 = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            double d2 = d - d1;
            double atfedett_Terulet = r1*r1*Math.Acos(d1/r1)-d1*Math.Sqrt(r1*r1-d1*d1)+(r2 * r2 * Math.Acos(d2 / r2) - d2 * Math.Sqrt(r2 * r2 - d2 * d2));
            return atfedett_Terulet;
            //https://diego.assencio.com/?index=8d6ca3d82151bad815f78addf9b5c1c6#mjx-eqn-post_8d6ca3d82151bad815f78addf9b5c1c6_A_intersection
        }

        //teszt: két kör, egymást átfedő körök

        public static List<List<KOR>> korMatrix = new List<List<KOR>>();

        /// <summary>
        /// Egy mátrixba gyűjti a köröket amelyek érintik egymást majd ennek elemeit egy ListBoxba pakolja
        /// </summary>


        public void ErintosKorok()
        {
            korMatrix.Clear();
            for (int i = 0; i < korok.Count; i++) //fv
            {
                AktKorMitErint(korok[i]);
            }
            listBox2.Items.Clear();

            for (int i = 0; i < korMatrix.Count(); i++)
            {
                listBox2.Items.Add(String.Format(korMatrix[i][0].sorszam + " <-Érinti-> " + korMatrix[i][1].sorszam)); //fv
            }
        }

        /// <summary>
        /// A beküldött kört megnézi hány másik kör érinti és az összes esetet beküldi a fentebb említett mátrixba (korMatrix)
        /// </summary>
        /// <param name="kor">Az akruálisan vizsgált kör</param>
        public static void AktKorMitErint(KOR kor)
        {
            for (int i = 0; i < korok.Count; i++)
            {
                double tav = KetKorKoztiTav(korok[i],kor);
                if (tav != 0 && (tav == (kor.r + korok[i].r)))
                {
                    korMatrix.Add(new List<KOR>() { kor, korok[i] });
                }
            }
        }


        /// <summary>
        /// Két kör közti területet adja vissza (double-ben)
        /// </summary>
        /// <param name="k1">KOR osztály első vizsgált példánya</param>
        /// <param name="k2">KOR osztály második vizsgált példánya</param>
        /// <returns>távolság a két kör között (double-ben)</returns>
        public static double KetKorKoztiTav(KOR k1, KOR k2)
        {
            return Math.Sqrt(Math.Pow(k1.X() - k2.X(), 2) + Math.Pow(k1.Y() - k2.Y(), 2));
            //return Math.Sqrt(Math.Pow(k1.x - k2.x, 2) + Math.Pow(k1.y - k2.y, 2));
        }





        /// <summary>
        /// Megadja hány db kör tartalmazza magán belül az origót
        /// </summary>
        /// <returns>A feltételnek megfelelő körök számát</returns>
        public static int OrigotTartalmazoKorok(List<KOR> körök)
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
        public void OrigoMaxTav()
        {            
            label5.Text = String.Format(MaxKivalasztTavSsz() + "-es sorszámú kör van a legtávolabb az origótól");
        }


        /// <summary>
        /// Megadja melyik kör van a legtávolabb az origó ponttól
        /// </summary>
        /// <returns>az origótól legtávolabb lévő kör sorszáma</returns>
        public int MaxKivalasztTavSsz()
        {
            double kor = 0; 
            int ssz = 0;
            for (int i = 0; i < korok.Count; i++)
            {
                if (korok[i].tav > kor)
                {
                    kor = korok[i].tav;
                    ssz = korok[i].sorszam;
                }
            }
            return ssz;
        }

        /// <summary>
        /// Minden vizsgált kört listáz a combo boxokba hogy a felhasználó ki tudja őket választani távolság mérésnél
        /// </summary>
        public void ComboBoxbaPakol()
        {
            for (int i = 0; i < korok.Count; i++)
            {
                comboBox1.Items.Add(String.Format(Convert.ToString(korok[i].sorszam) + " Kör"));
                comboBox2.Items.Add(String.Format(Convert.ToString(korok[i].sorszam) + " Kör"));
            }
        }

        /// <summary>
        /// Megrajzolja a koordináta rendszert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KoordinataRendszerRajz(object sender, PaintEventArgs e)
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
        public void korok_rajz_Click(object sender, EventArgs e)
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
            label3.Text = OrigotTartalmazoKorok(korok).ToString() + " kör tartalmazza az Origót";

            listBox1.Items.Clear();

            for (int i = 0; i < korok.Count; i++)
            {
                kor_rajzolas(korok[i].X(), korok[i].Y(), korok[i].r, korok[i].sorszam);
                listBox1.Items.Add(korok[i].ToString());
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
        public void kor_rajzolas(int x, int y, int r, int sorszam)
        {
            Graphics rajz = panel1.CreateGraphics();
            Pen ceruza = new Pen(Color.Green, 2);
            rajz.DrawEllipse(ceruza, x - r, y - r, 2 * r, 2 * r);
            rajz.FillEllipse(Brushes.Red, x - 4 / 2, y - 4 / 2, 4, 4);
            Font betu = new Font("Arial", 12);
            rajz.DrawString(Convert.ToString(sorszam) + ".", betu, Brushes.Black, x + 10, y - 9);
        }

        public void button1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Megadja lenyomásakor a két kör közti távolságot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button2_Click(object sender, EventArgs e)
        {
            try
            {
                double tav = KetKorKoztiTav(korok[comboBox1.SelectedIndex], korok[comboBox2.SelectedIndex]);
                tavolsagaketkornek.Text = String.Format("A két kiválasztott kör távolsága: " + Convert.ToString(tav));
            }
            catch
            {
                MessageBox.Show("Válasszon két kört, ne egyet vagy eggyet se kettőt!");
            }

        }


        public void button1_Click_1(object sender, EventArgs e)
        {
            korok.Clear();           
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
            //range ide lesz


            public override string ToString()
            {
                return String.Format(sorszam + ". (" + x + "," + y + ")" );
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }


        public static int s = 0;
        // a gomb váltáshoz kell
        /// <summary>
        /// Ezzel vált vizuálisan a két lehetőség közül és változtatja azok színét
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button3_Click(object sender, EventArgs e)
        {
            korok.Clear();
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
        public void KorGenRandom() //fv
        {
            Random r = new Random();
            int dbkor = Convert.ToInt32(numericUpDown1.Value);
            for (int i = 0; i < dbkor; i++)
            {
                int a = r.Next(-500, 500);
                int b = r.Next(-500, 500);
                while (!((Math.Min(500 - Math.Abs(a), 500 - Math.Abs(b)) >= 10)))                    
                {
                    a = r.Next(-500, 500);
                    b = r.Next(-500, 500);
                }                             
                int sugar = r.Next(10, SugarGeneral(a, b));
                while (!EgybeCsusznakE(a, b, sugar, i))
                {
                    a = r.Next(-500, 500);
                    b = r.Next(-500, 500);
                    sugar = r.Next(10, SugarGeneral(a, b));
                }
                korok.Add(new KOR(a, b, sugar, i + 1));
            }

        }

        public int SugarGeneral(int a, int b)
        {
            int temp = Math.Min(500 - Math.Abs(a), 500 - Math.Abs(b)) / 2;
            if (temp < 10)
            {
                Random r = new Random();
                temp += r.Next(10, 50);
            }
            return temp;
        }

        public bool EgybeCsusznakE(int a, int b, int sugar,int i)
        {
            if (korok.Count() == 1)
            { 
                return true;
            }

            int k = 0;

            //meg kell nézni hogy benne van e a kör teljesen mert ha igen ez nem fog működni
            while (k< korok.Count() && (KetKorKoztiTav(new KOR(a, b, sugar, i), korok[k]) / (sugar + korok[k].r) >= 0.8))
            {
                k++;
            }
            return k >= korok.Count();

        }

        //private KOR Korgen(int i)
        //{
        //    return new KOR(a, b, sugar, i + 1);
        //}

        /// <summary>
        /// Töröl minden háttér adatok és a kooridináta rendszert hogy új köröket rajzolhasson a felhasználó
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button1_Click_2(object sender, EventArgs e)
        {
            korok.Clear();
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
        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                listBox3.Items.Clear();
                listBox3.Items.Add("Sorszám: " + korok[listBox1.SelectedIndex].sorszam + ".");
                listBox3.Items.Add("Közép pontja: (" + korok[listBox1.SelectedIndex].X() + "," + korok[listBox1.SelectedIndex].Y() + ")");
                listBox3.Items.Add("Sugár: " + korok[listBox1.SelectedIndex].r);
                listBox3.Items.Add("Kerület: " + korok[listBox1.SelectedIndex].K());
                listBox3.Items.Add("Terület: " + korok[listBox1.SelectedIndex].T());
                listBox3.Items.Add("Távolság az origótól: " + korok[listBox1.SelectedIndex].tav);
            }
            catch { };
        }

        public void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}