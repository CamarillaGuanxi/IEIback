using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IeIAPI.Entities
{
    public partial class Localidad
    {
        // [Key]
        public int codigo { get; set; }//Creo que no hace falta porque lo crearemos en la base de datos y entitiy framework lo mete solo como id
        public string nombre { get; set; }

        public Provincia? en_provincia { get; set; }

        public virtual ICollection<Centro_Educativo>? en_localidad { get; set;}
    }
}
