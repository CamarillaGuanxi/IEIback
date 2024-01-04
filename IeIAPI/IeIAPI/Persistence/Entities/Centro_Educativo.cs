using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IeIAPI.Entities
{
   
    public partial class Centro_Educativo
    {
       
        public string nombre { get; set; }

        public string? direcccion { get; set; }

        public string codigo_postal { get; set; }

        public int? longitud { get; set; }

        public int? latitud { get; set; }

        public int? telefono { get; set; }//puede ser string

        public string? description { get; set; }
        public enum TipoCentro
        {
            Publico,
            Concertado,
            Privado,
            Otro
        }
        public TipoCentro? tipo { get; set; }

        public Localidad? en_localidad { get; set; }

    }
}
