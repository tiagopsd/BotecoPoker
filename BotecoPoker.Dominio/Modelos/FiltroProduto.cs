using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Dominio.modelos
{
    public class FiltroProduto
    {
        public string Nome { get; set; }
        public double? Valor { get; set; }
        public int? IdTipoProduto { get; set; }

        public FiltroProduto(string param1, int? param5)
        {
            Nome = param1;
            IdTipoProduto = param5;
        }

        public FiltroProduto()
        {
        }

        
    }
}
