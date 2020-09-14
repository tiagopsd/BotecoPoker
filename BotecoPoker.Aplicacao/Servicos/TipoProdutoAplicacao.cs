using BotecoPoker.Aplicacao.Validadores;
using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.InterfacesRepositorio;
using BotecoPoker.Dominio.modelos;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BotecoPoker.Aplicacao.Servicos
{
    public class TipoProdutoAplicacao
    {
        [Inject]
        public ITipoProdutoRepositorio TipoProdutoRepositorio { get; set; }
        [Inject]
        public IDbContexto Contexto { get; set; }

        public IEnumerable<SelectListItem> ObterTipoProdutoCombo()
        {
            return TipoProdutoRepositorio.ObterComboTiposProdutos();
        }

        public PaginacaoModel<TipoProduto, FiltroProduto> Filtrar(PaginacaoModel<TipoProduto, FiltroProduto> paginacao)
        {
            if (paginacao.Filtro == null)
                paginacao.Filtro = new FiltroProduto(paginacao.Parametro1, paginacao.Parametro5);
            else
                paginacao.Parametro1 = paginacao.Filtro.Nome;
            var query = TipoProdutoRepositorio.Query();
            if (paginacao.Filtro.Nome.TemValor())
                query = query.Where(d => d.Nome.Contains(paginacao.Filtro.Nome));

            paginacao.ListaModel = query.OrderBy(d => d.Id).Skip(((paginacao.Pagina - 1) * 10)).Take(10).ToList();
            paginacao.QtdPaginas = query.Count().CalculaQtdPaginas().TransformaEmLista();
            Contexto.FecharConexao();
            return paginacao;
        }

        public string CadastroTipoProduto(TipoProduto entidade)
        {
            if (entidade.Nome.TemValor())
            {
                TipoProdutoRepositorio.Cadastrar(entidade);
                Contexto.Salvar();
                return "";
            }
            return "Erro no casdastro, favor digite o nome do tipo do produto a ser cadastrado!";

        }

        public string AlterarTipoProduto(TipoProduto entidade)
        {
            var tipo = TipoProdutoRepositorio.Buscar(entidade.Id);
            if (entidade.Nome.TemValor())
            {
                tipo.Nome = entidade.Nome;
                TipoProdutoRepositorio.Atualizar(tipo);
                Contexto.Salvar();
                return "";
            }
            return "Erro ao atualizar casdastro, favor digite o nome do tipo produto a ser atualizado!";
        }

        public TipoProduto BuscarPorId(int id)
        {
            return TipoProdutoRepositorio.Buscar(id);
        }
    }
}
