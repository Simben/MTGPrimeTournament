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
        public Form1()
        {
            InitializeComponent();
        }

        List<Pairing> Pairing = new List<Pairing>();

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            var Lines = this.textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (var line in Lines)
            {
                //detect if header
                if (line.Contains("Table") || line.Contains("Player 1") || line.Contains("Player 2"))
                    continue;

                var elements = line.Split('\t');
                Pairing.Add(new Model.Pairing() {
                Table = elements[0],
                Player1 = Regex.Replace(elements[1], @"\([0-9]+ ([A-Z])\w+\)", "") ,
                Player2 = Regex.Replace(elements[2], @"\([0-9]+ ([A-Z])\w+\)", ""),
                    Score = elements[3],
                Bye = elements[2] == "BYE"
                });

            }
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
            
            pdf.Generate(new {
                Round = 1,
                Image = "data:image/png;base64," + base64,
                //Image = "totot",
                List = Pairing
            });
            pdf.SaveAs("test.pdf");
        }
    }
}
