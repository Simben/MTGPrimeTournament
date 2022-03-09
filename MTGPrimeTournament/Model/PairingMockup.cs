using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGPrimeTournament.Model
{
    public class PairingLineMockup
    {
        public Pairing pairingLine { get; set; }
        public bool pairLine { get; set; }

    }

    public class ParingPageMockup
    {
        public List<PairingLineMockup> pairingLine { get; set; }

        public ParingPageMockup()
        {
            pairingLine = new List<PairingLineMockup>();
        }
    }
}
