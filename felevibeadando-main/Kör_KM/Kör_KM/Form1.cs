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

namespace Kör_KM
{
    public partial class Form1 : Form
    {

        static List<KOR> körök = new List<KOR>();

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Paint += KoordinataRendszerRajz;
            korok_rajz.Enabled = false;
            adatfelvitel();
            
        }

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

        public static double OsszesTerulet()
        {
            double OsszTerulet = 0.0;

            for (int i = 0; i < körök.Count; i++)
            {
                double aktkot_terulet = Math.PI * Math.Pow(körök[i].r, 2);
                OsszTerulet += aktkot_terulet;

                for (int j = i + 1; j < körök.Count; j++)
                {
                    double tavketkorkozott = Math.Sqrt(Math.Pow(körök[j].X() - körök[i].X(), 2) + Math.Pow(körök[j].Y() - körök[i].Y(), 2));

                    if (tavketkorkozott < körök[i].r + körök[j].r)
                    {
                        OsszTerulet -= AtfedettTerulet(körök[i].r, körök[j].r, tavketkorkozott);
                    }
                }
            }

            return OsszTerulet;
        }

        private static double AtfedettTerulet(double r1, double r2, double d)
        {
            double d1 = (r1 * r1 - r2 * r2 + d * d) / (2 * d);
            double d2 = d - d1;
            double atfedett_Terulet = r1*r1*Math.Acos(d1/r1)-d1*Math.Sqrt(r1*r1-d1*d1)+(r2 * r2 * Math.Acos(d2 / r2) - d2 * Math.Sqrt(r2 * r2 - d2 * d2));
            return atfedett_Terulet;
            //https://diego.assencio.com/?index=8d6ca3d82151bad815f78addf9b5c1c6#mjx-eqn-post_8d6ca3d82151bad815f78addf9b5c1c6_A_intersection
        }

        private static List<List<KOR>> korMatrix = new List<List<KOR>>();

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

        private static void AktKorMitErint(KOR kor)
        {
            for (int i = 0; i < körök.Count; i++)
            {
                double tav = Math.Sqrt(Math.Pow(körök[i].X() - kor.X(), 2) + Math.Pow(körök[i].Y() - kor.Y(), 2));
                if (tav != 0 && (tav == (kor.r + körök[i].r)))
                {
                    korMatrix.Add(new List<KOR>() { kor, körök[i] });

                }
            }
        }

        private void OrigotTartalmazoKorok()
        {
            int db = 0;
            for (int i = 0; i < körök.Count; i++)
            {
                if (körök[i].tav <= körök[i].r)
                {
                    db++;
                }
            }
            label3.Text = db.ToString() + " kör tartalmazza az Origót";

        }



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

        private void ComboBoxbaPakol()
        {
            for (int i = 0; i < körök.Count; i++)
            {
                comboBox1.Items.Add(String.Format(Convert.ToString(körök[i].sorszam) + " Kör"));
                comboBox2.Items.Add(String.Format(Convert.ToString(körök[i].sorszam) + " Kör"));
            }
        }

        private void KoordinataRendszerRajz(object sender, PaintEventArgs e)
        {
            Graphics rajz = e.Graphics;
            Pen tengely = new Pen(Color.Black, 2);
            Pen origo = new Pen(Color.Red, 4);
            rajz.DrawLine(tengely, panel1.Width / 2, 0, panel1.Width / 2, panel1.Height);
            rajz.DrawLine(tengely, 0, panel1.Height / 2, panel1.Width, panel1.Height / 2);
            rajz.DrawEllipse(origo, panel1.Width / 2 - 2, panel1.Height / 2 - 2, 4, 4);
        }
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
            OrigotTartalmazoKorok();
            listBox1.Items.Clear();

            for (int i = 0; i < körök.Count; i++)
            {
                kor_rajzolas(körök[i].X(), körök[i].Y(), körök[i].r, körök[i].sorszam);

                listBox1.Items.Add(körök[i].ToString());
            }
            korok_rajz.Enabled = false;
            //listBox2.Items.Clear();
        }
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int k1x = körök[comboBox1.SelectedIndex].X();
                int k1y = körök[comboBox1.SelectedIndex].Y();
                int k2x = körök[comboBox2.SelectedIndex].X();
                int k2y = körök[comboBox2.SelectedIndex].Y();
                double tav = Math.Sqrt(Math.Pow(k2x - k1x, 2) + Math.Pow(k2y - k1y, 2));
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
                this.sorszam = sorszam;
            }

            public double T()
            {
                return (r * r * Math.PI);
            }

            public double K()
            {
                return (2 * r * Math.PI);
            }

            public int X()
            {
                return (1000 / 2 + x);
            }
            public int Y()
            {
                return (1000 / 2 - y);
            }

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

        private void button1_Click_2(object sender, EventArgs e)
        {
            körök.Clear();
            korok_rajz.Enabled = true;
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            panel1.Invalidate();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox3.Items.Clear();
            listBox3.Items.Add("Sorszám: "+körök[listBox1.SelectedIndex].sorszam+".");
            listBox3.Items.Add("Közép pontja: (" + körök[listBox1.SelectedIndex].X() + "," + körök[listBox1.SelectedIndex].Y() + ")");
            listBox3.Items.Add("Sugár: " + körök[listBox1.SelectedIndex].r);
            listBox3.Items.Add("Kerület: " + körök[listBox1.SelectedIndex].K());
            listBox3.Items.Add("Terület: " + körök[listBox1.SelectedIndex].T());
            listBox3.Items.Add("Távolság az origótól: " + körök[listBox1.SelectedIndex].tav);
        }
    }
}