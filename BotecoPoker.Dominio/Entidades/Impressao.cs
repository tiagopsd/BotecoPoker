using BotecoPoker.Dominio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Dominio.Entidades
{
    public class Impressao : Entidade<long>
    {
        public string NomeImpressora { get; set; }
        public TipoImpressao TipoImpressao { get; set; }
        public long IdObjetoImpressao { get; set; }
        public SituacaoImpressao SituacaoImpressao { get; set; }
    }

    public enum SituacaoImpressao : short
    {
        Pendente = 0,
        Impresso = 1
    }

    public enum TipoImpressao : short
    {
        Venda = 0,
        CashGame = 1,
        TorneioCliente = 2,
        Comprovante = 3
    }
}
