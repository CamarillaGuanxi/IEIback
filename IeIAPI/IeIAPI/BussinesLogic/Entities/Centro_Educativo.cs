using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IeIAPI.Entities
{
    public partial class Centro_Educativo
    {
  

        public Centro_Educativo(string nombre, string codigo_postal)
        {
            this.nombre = nombre;
     
            this.codigo_postal = codigo_postal;
           
        }
    }
}
