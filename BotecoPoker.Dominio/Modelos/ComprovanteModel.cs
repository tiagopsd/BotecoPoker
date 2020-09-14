using BotecoPoker.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Dominio.modelos
{
    public class ComprovanteModel
    {
        public long IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public List<Venda> Vendas { get; set; }
        public List<CashGame> CashGames { get; set; }
        public List<TorneioCliente> TorneiosCliente { get; set; }
        public List<ParcelamentoPagamento> ParcelamentoPagamentos { get; set; }
        public Pagamento Pagamento { get; set; }
    }
}
