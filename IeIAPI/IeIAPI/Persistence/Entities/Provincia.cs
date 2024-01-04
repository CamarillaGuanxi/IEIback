using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IeIAPI.Entities
{
    public partial class Provincia
    {
       // [Key]
        public int codigo { get; set; }//Creo que no hace falta porque lo crearemos en la base de datos y entitiy framework lo mete solo como id
        public string nombre { get; set; }

        public virtual ICollection<Localidad>? en_provincia { get; set; }
    }
}
