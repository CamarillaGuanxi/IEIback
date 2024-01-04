using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IeIAPI.Entities
{
     public partial class Localidad
    {


        public Localidad(int codigo, string nombre) {
            //Por ahi hace falta tambien el codigo no lo se
            this.codigo = codigo;
            this.nombre = nombre;
        }
    }
}
