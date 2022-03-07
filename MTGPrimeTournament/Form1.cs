using MTGPrimeTournament.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTGPrimeTournament
{
    public partial class Form1 : Form
    {
        private const int PAPER_SLIP_MAX_PER_PAGE = 4;
        public Form1()
        {
            InitializeComponent();
        }

        List<Pairing> Pairing = new List<Pairing>();

        private void Deserialize()
        {
            if (Pairing != null)
                Pairing.Clear();
            int i = 0;
            var Lines = this.textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in Lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                //detect if header
                if (line.Contains("Table") || line.Contains("Player 1") || line.Contains("Player 2"))
                    continue;

                var elements = line.Split('\t');
                Pairing.Add(new Model.Pairing()
                {
                    Table = elements[0],
                    Player1 = Regex.Replace(elements[1], @"\([0-9]+ ([A-Z])\w+\)", ""),
                    Player1_Points = Regex.Match(elements[1], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(','\0').Replace(')', '\0'),
                    Player2 = Regex.Replace(elements[2], @"\([0-9]+ ([A-Z])\w+\)", ""),
                    Player2_Points = Regex.Match(elements[2], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                    Score = elements[3],
                    Bye = elements[2] == "BYE",
                    Breakpage = i % PAPER_SLIP_MAX_PER_PAGE == PAPER_SLIP_MAX_PER_PAGE - 1
                });
                i++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Deserialize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Helper.PDF pdf = new Helper.PDF("Model\\template.hbs");
            string base64 = "";
            using (Image image = Image.FromFile("Assets\\MTGPrime.png"))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    base64 = Convert.ToBase64String(imageBytes);

                }
            }

            pdf.Generate(new
            {
                Round = 1,
                Image = "data:image/png;base64," + base64,
                //Image = "totot",
                List = Pairing
            });
            pdf.SaveAs("test.pdf");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "Table	Player 1\tPlayer 2\tMatch Results	" + Environment.NewLine +
                       "1\tToto(0 Points)\tjohn(0 Points)\tNo results" + Environment.NewLine +
                       "2\ttiti(0 Points)\tpapa(0 Points)\tNo results" + Environment.NewLine +
                       "3\tpopo(0 Points)\tjane(0 Points)\tNo results" + Environment.NewLine +
                       "4\tMichel(0 Points)\tdoe(0 Points)\tNo results" + Environment.NewLine +
                       "5\tTata(0 Points)\tBYE\t2 - 0";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Deserialize();
            Helper.PDF pdf = new Helper.PDF("Model\\template.hbs");
            string base64 = "";
            using (Image image = Image.FromFile("Assets\\MTGPrime.png"))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    base64 = Convert.ToBase64String(imageBytes);

                }
            }
            pdf.GenerateHtml(new
            {
                Round = tb_RoundNumber.Text,
                Image = "data:image/png;base64," + base64,
                //Image = "totot",
                List = Pairing
            }, "html.html");
        }
    }
}
