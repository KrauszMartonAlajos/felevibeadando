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
            körök.Add(new KOR(50,50,100,1));
            körök.Add(new KOR(100, 400, 50, 2));
            körök.Add(new KOR(250, 50, 100, 3));
            OrigoMaxTav();
            
            ComboBoxbaPakol();
            //ide kerül a fájlbeolvasás
            //példányosítása a köröknek
            //itt rajzoljuk ki a koordináta rendszert
            //a körök sorszám alapján lesznek megkülönböztetve
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
        
        private static double AtfedettTerulet(double k1r, double k2r, double d)
        {
            double k1_kor = k1r - (d * d + k1r * k1r - k2r * k2r) / (2 * d);
            double k2_kor = k2r - (d * d + k2r * k2r - k1r * k1r) / (2 * d);
            double atfedett_Terulet = k1r * k1r * Math.Acos(k1_kor / k1r) - k1_kor * Math.Sqrt(k1r * k1r - k1_kor * k1_kor) + k2r * k2r * Math.Acos(k2_kor / k2r) - k2_kor * Math.Sqrt(k2r * k2r - k2_kor * k2_kor);
            return atfedett_Terulet;
        }

        private static List<List<KOR>> korMatrix = new List<List<KOR>>();

        private static void ErintosKorok()
        {
            for (int i = 0; i < körök.Count; i++)
            {
                AktKorMitErint(körök[i]);
            }            
        }

        private static void AktKorMitErint(KOR kor)
        {
            for (int i = 0; i < körök.Count; i++)
            {
                double tav = Math.Sqrt(Math.Pow(körök[i].X() - kor.X(), 2) + Math.Pow(körök[i].Y() - kor.Y(), 2));
                if (tav != 0)
                {
                    if (tav / 2 == kor.r)
                    {
                        korMatrix.Add(new List<KOR>() {kor, körök[i]});
                    }
                
                }
            }
        }

        private void OrigotTartalmazoKorok()
        {
            int db = 0;
            for (int i = 0;i<körök.Count;i++)
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
            for (int i = 0;i<körök.Count;i++) 
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
                comboBox1.Items.Add(String.Format(Convert.ToString(körök[i].sorszam)+" Kör"));
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
            ErintosKorok();
            label4.Text = String.Format("A körök összesített Területe: "+Convert.ToString(OsszesTerulet()));
            OrigotTartalmazoKorok();
            listBox1.Items.Clear();
            for (int i = 0;i<körök.Count;i++) 
            {
                kor_rajzolas(körök[i].X(), körök[i].Y(), körök[i].r, körök[i].sorszam);
                
                listBox1.Items.Add(körök[i].ToString());
            }
        }
        private void kor_rajzolas(int x, int y,int r, int sorszam)
        {
            Graphics rajz = panel1.CreateGraphics();
            Pen ceruza = new Pen(Color.Green, 2);
            rajz.DrawEllipse(ceruza, x - r, y - r, 2 * r, 2 * r);
            rajz.FillEllipse(Brushes.Red, x - 4 / 2, y - 4 / 2, 4, 4);
            Font betu = new Font("Arial", 12);
            rajz.DrawString(Convert.ToString(sorszam)+".", betu, Brushes.Black, x+10, y-9);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int k1x = körök[comboBox1.SelectedIndex].X();
            int k1y = körök[comboBox1.SelectedIndex].Y();
            int k2x = körök[comboBox2.SelectedIndex].X();
            int k2y = körök[comboBox2.SelectedIndex].Y();
            double tav = Math.Sqrt(Math.Pow(k2x - k1x, 2) + Math.Pow(k2y - k1y, 2));
            tavolsagaketkornek.Text = String.Format("A két kiválasztott kör távolsága: " + Convert.ToString(tav));

        }
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
            return (2*r* Math.PI);
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
            return String.Format(sorszam+". ("+ x + ","+y+")"+ "  " + "Távolság az origótól: "+ tav);
        }
    }
}
