using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSWebApp.Data
{
    public class Bl
    {
        public string _name { get; set; }

        public string _fallzahl { get; set; }

        public Bl(string name, string fallzahl) {
            _name = name;
            _fallzahl = fallzahl;
        }
    }
}
