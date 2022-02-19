using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGPrimeTournament.Model
{
    public class Pairing
    {
        public string Table { set; get; }
        public string Player1 { set; get; }
        public string Player2 { set; get; }
        public string Score { set; get; }
        public bool Bye { get; set; }

    }
}
