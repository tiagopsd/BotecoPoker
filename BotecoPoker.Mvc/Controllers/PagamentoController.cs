using BotecoPoker.Aplicacao.Servicos;
using BotecoPoker.Aplicacao.Validadores;
using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.modelos;
using BotecoPoker.Infra.Impressora;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BotecoPoker.Mvc.Controllers
{
    [Authorize]
    public class PagamentoController : Controller
    {
        [Inject]
        public PagamentosAplicacao PagamentosAplicacao { get; set; }
        [Inject]
        public ComprovanteAplicacao ComprovanteAplicacao { get; set; }
        [Inject]
        public UsuarioAplicacao UsuarioAplicacao { get; set; }
        [Inject]
        public ImpressaoAplicacao ImpressaoAplicacao { get; set; }

        public ActionResult FiltroPagamentos(PaginacaoModel<Cliente, FiltroPagamento> paginacaoModel)
        {
            return View(PagamentosAplicacao.ObterClientesComPendencia(paginacaoModel));
        }

        public ActionResult PendenciasCliente(long idCliente)
        {
            try
            {
                return View(PagamentosAplicacao.ObterPendenciaCliente(idCliente));
            }
            catch (Exception erro)
            {
                return Json(erro.GetBaseException());
            }
        }

        public ActionResult ComprovantePagamentos(PaginacaoModel<Pagamento, FiltroPagamento> paginacaoModel)
        {
            return View(ComprovanteAplicacao.FiltrarComprovante(paginacaoModel));
        }

        public ActionResult DetalhesComprovante(long idPagamento, FiltroPagamento filtroPagamento)
        {
            ViewBag.DetalhesPagamento = ComprovanteAplicacao.ObterDetalhesPagamento(idPagamento);
            return View("ComprovantePagamentos", ComprovanteAplicacao.FiltrarComprovante(new PaginacaoModel<Pagamento, FiltroPagamento> { Filtro = filtroPagamento }));
        }

        public ActionResult DetalhesComprovanteAberto(long idPagamento, FiltroPagamento filtroPagamento)
        {
            var detalhes = ComprovanteAplicacao.ObterDetalhesPagamento(idPagamento);
            ViewBag.DetalhesPagamento = detalhes;
            return View("PendenciasCliente", PagamentosAplicacao.ObterPendenciaCliente(detalhes.IdCliente));
        }

        [HttpPost]
        public ActionResult GerarPagamento(ParcelamentoPagamento parcelamentoPagamento)
        {
            var result = PagamentosAplicacao.ValidaPagamento(parcelamentoPagamento);
            if (result.TemValor())
            {
                ViewBag.valorPago = parcelamentoPagamento.ValorPago;
                ViewBag.tipoFinalizador = (int)parcelamentoPagamento.TipoFinalizador;
                ViewBag.erro = result;
                return View("PendenciasCliente", PagamentosAplicacao.ObterPendenciaCliente(parcelamentoPagamento.IdCliente ?? 0));
            }

            var pendencias = PagamentosAplicacao.GerarPagamento(parcelamentoPagamento);
            if (pendencias.Vendas.Count == 0 && pendencias.TorneiosCliente.Count == 0 && pendencias.CashGames.Count == 0 && pendencias.Pagamentos.Count == 0)
                return RedirectToAction("FiltroPagamentos");
            return View("PendenciasCliente", pendencias);
        }

        public ActionResult ImprimirComprovante(long idPagamento)
        {
            //var comprovanteModel = PagamentosAplicacao.ObterPagamentoCommpleto(idPagamento);
            var nomeImpressora = UsuarioAplicacao.ObterDadosUsuarioLogado().Impressora.ToString();
            ImpressaoAplicacao.GravarImpressao(idPagamento, nomeImpressora, TipoImpressao.Comprovante);
            //new ImprimeComprovante().Imprime(comprovanteModel, nomeImpressora);
            return RedirectToAction("ComprovantePagamentos");
        }
    }
}