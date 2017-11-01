using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpolecneSoubory
{
    public class DruhyZprav
    {
        public enum DruhZpravy
        {
            Zprava = 1,
            Soubor = 2,
            Obrazek = 3,
            ServerUzavren = 4,
            Ostatni = 5,
            Kontrola = 6,
            Offline = 7
        }
    }
}
