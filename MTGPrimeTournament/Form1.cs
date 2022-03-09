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
        private const int PAIRING_MAX_PER_PAGE = 38;

        private const string PaperSlip_HBTemplate = "PaperSlipTemplate.hbs";
        private const string Pairing_HBTemplate = "PairingTemplate.hbs";

        public Form1()
        {
            InitializeComponent();
        }

        List<PaperSlipMockup> Pairing_PS = new List<PaperSlipMockup>();
        List<ParingPageMockup> Pairing_Full = new List<ParingPageMockup>();

        private void DeserializePaperSlip()
        {
            if (Pairing_PS != null)
                Pairing_PS.Clear();

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
                Pairing_PS.Add(new Model.PaperSlipMockup()
                {
                    pairingLine = new Pairing()
                    {
                        Table = elements[0],
                        Player1 = Regex.Replace(elements[1], @"\([0-9]+ ([A-Z])\w+\)", ""),
                        Player1_Points = Regex.Match(elements[1], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                        Player2 = Regex.Replace(elements[2], @"\([0-9]+ ([A-Z])\w+\)", ""),
                        Player2_Points = Regex.Match(elements[2], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                        Score = elements[3],
                        Bye = elements[2] == "BYE"
                    },
                    Breakpage = i % PAPER_SLIP_MAX_PER_PAGE == PAPER_SLIP_MAX_PER_PAGE - 1
                });
                i++;
            }
        }
        private void DeserializePairing()
        {
            if (Pairing_Full != null)
                Pairing_Full.Clear();


            var Lines = this.textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var Tmp = new List<Pairing>();

            foreach (var line in Lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                //detect if header
                if (line.Contains("Table") || line.Contains("Player 1") || line.Contains("Player 2"))
                    continue;

                

                var elements = line.Split('\t');
                //Add J1 first


                Tmp.Add(new Model.Pairing()
                {
                    Table = elements[0],
                    Player1 = Regex.Replace(elements[1], @"\([0-9]+ ([A-Z])\w+\)", ""),
                    Player1_Points = Regex.Match(elements[1], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                    Player2 = Regex.Replace(elements[2], @"\([0-9]+ ([A-Z])\w+\)", ""),
                    Player2_Points = Regex.Match(elements[2], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                    Score = elements[3],
                    Bye = elements[2] == "BYE"
                });

                if (elements[2] != "BYE")
                {
                    Tmp.Add(new Model.Pairing()
                    {
                        Table = elements[0],
                        Player2 = Regex.Replace(elements[1], @"\([0-9]+ ([A-Z])\w+\)", ""),
                        Player2_Points = Regex.Match(elements[1], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                        Player1 = Regex.Replace(elements[2], @"\([0-9]+ ([A-Z])\w+\)", ""),
                        Player1_Points = Regex.Match(elements[2], @"\([0-9]+ ([A-Z])\w+\)").Value.Replace('(', '\0').Replace(')', '\0'),
                        Score = elements[3],
                        Bye = elements[2] == "BYE"
                    });
                }
            }
            Tmp = Tmp.OrderBy(p=>p.Player1).ToList();

            if (this.Pairing_Full != null)
                this.Pairing_Full.Clear();

           
            int i = 0;
            int currpage = -1;
            foreach (var t in Tmp)
            {
                if (i % PAIRING_MAX_PER_PAGE == 0)
                {
                    this.Pairing_Full.Add(new ParingPageMockup());
                    currpage++;
                }

                this.Pairing_Full[currpage].pairingLine.Add(new PairingLineMockup()
                {
                    pairingLine = t,
                    pairLine = i%2 ==0
                    
                });


                i++;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeserializePaperSlip();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Helper.PDF pdf = new Helper.PDF("Model\\" + PaperSlip_HBTemplate);
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
                List = Pairing_PS
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
            DeserializePaperSlip();
            Helper.PDF pdf = new Helper.PDF("Model\\" + PaperSlip_HBTemplate);
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
                List = Pairing_PS
            }, "html.html");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DeserializePairing();
            

            Helper.PDF pdf = new Helper.PDF("Model\\" + Pairing_HBTemplate);
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
                List = Pairing_Full
            }, "html.html");

        }
    }
}
