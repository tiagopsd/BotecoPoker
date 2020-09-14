using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.InterfacesRepositorio;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Aplicacao.Servicos
{
    public class ImpressaoAplicacao
    {
        [Inject]
        public IImpressaoRepositorio ImpressaoRepositorio { get; set; }
        [Inject]
        public IDbContexto Contexto { get; set; }

        public void GravarImpressao(long idObjeto,string nomeImpressora,TipoImpressao tipoImpressao)
        {
            var impressao = new Impressao
            {
                IdObjetoImpressao = idObjeto,
                NomeImpressora = nomeImpressora,
                SituacaoImpressao = SituacaoImpressao.Pendente,
                TipoImpressao = tipoImpressao
            };
            ImpressaoRepositorio.Cadastrar(impressao);
            Contexto.Salvar();
        }
    }
}
