using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace How.To.Use.TheHttpClient
{
    public class MusicalInstrument
    {
        public string Name { get; set; }
    }

    public class Cajon : MusicalInstrument
    {
        public bool HasSnare { get; set; }
    }

    public class Guitar : MusicalInstrument
    {
        public int StringCount { get; set; }
    }
}
