using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpolecneSoubory
{
    public class TypZpravy
    {
        public TypZpravy()
        {

        }

        public TypZpravy(string ModZprava)
        {
            try
            {
                string[] Zprava = ModZprava.Split('|');

                if (Zprava.Length < 6)
                {
                    Typ = Convert.ToInt32(Zprava[0]);
                    Odesilatel = Convert.ToString(Zprava[1]);
                    PrezdivkaOdesilatele = Convert.ToString(Zprava[2]);
                    Obsah = Convert.ToString(Zprava[3]);
                    BytyObsah = Encoding.UTF8.GetBytes(Zprava[5].Trim("\0".ToCharArray()));
                }
                else
                {
                    Typ = 0;
                    Odesilatel = "";
                    PrezdivkaOdesilatele = "";
                    Obsah = "";
                    BytyObsah = new byte[1];
                }
            }
            catch
            {
                Typ = 0;
                Odesilatel = "";
                PrezdivkaOdesilatele = "";
                Obsah = "";
                BytyObsah = new byte[1];
            }
        }

        private int _Typ;

        public int Typ
        {
            get
            {
                return _Typ;
            }
            set
            {
                _Typ = value;
            }
        }

        public string Odesilatel
        {
            get;
            set;
        }

        public string PrezdivkaOdesilatele
        {
            get;
            set;
        }

        public string Obsah
        {
            get;
            set;
        }

        private byte[] _BytyObsah = new byte[1];

        public byte[] BytyObsah
        {
            get
            {
                return _BytyObsah;
            }
            set
            {
                _BytyObsah = value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", Typ, Odesilatel, PrezdivkaOdesilatele, Obsah, Encoding.UTF8.GetString(BytyObsah));
        }

        public byte[] ToBytes()
        {
            byte[] byty = Encoding.UTF8.GetBytes(this.ToString());
            return byty;
        }
    }
}
