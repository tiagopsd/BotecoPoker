using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.InterfacesRepositorio;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Aplicacao.Validadores
{
    public class ValidadorTorneioCliente
    {
        [Inject]
        public ITorneioClienteRepositorio TorneioClienteRepositorio { get; set; }

        public string ValidaTorneioCliente(TorneioCliente entidade)
        {
            StringBuilder erros = new StringBuilder();
            if (ExisteTorneioCliente(entidade))
            {
                erros.AppendLine("Cliente já está cadastrado no torneio selecionado");
            }
            else if (entidade.IdTorneio == 0)
            {
                erros.AppendLine("Favor selecionar uma torneio!");
            }
            else if (entidade.IdCliente == 0)
            {
                erros.AppendLine("Favor inserir um cliente!");
            }
            return erros.ToString();
        }

        private bool ExisteTorneioCliente(TorneioCliente entidade)
        {
            return TorneioClienteRepositorio.Filtrar(d => d.Situacao != Dominio.Enumeradores.SituacaoVenda.Pago
            && d.IdCliente == entidade.IdCliente
            && d.IdTorneio == entidade.IdTorneio)
            .Any();
        }
    }
}
