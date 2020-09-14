using BotecoPoker.Aplicacao.Servicos;
using BotecoPoker.Aplicacao.Validadores;
using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.modelos;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BotecoPoker.Mvc.Controllers
{
    [Authorize]
    public class TipoProdutoController : Controller
    {
        [Inject]
        public TipoProdutoAplicacao TipoProdutoAplicacao { get; set; }

        public ActionResult FiltroTipoProduto(PaginacaoModel<TipoProduto, FiltroProduto> paginacao)
        {
            return View(TipoProdutoAplicacao.Filtrar(paginacao));
        }

        [HttpGet]
        public ActionResult CadastroTipoProduto()
        {
            return View(new TipoProduto());
        }

        [HttpPost]
        public ActionResult CadastroTipoProduto(TipoProduto entidade)
        {
            var result = TipoProdutoAplicacao.CadastroTipoProduto(entidade);
            if (result.TemValor())
            {
                ViewBag.erro = result;
                return View(entidade);
            }
            return RedirectToAction("FiltroTipoProduto");
        }

        public ActionResult BuscarPorId(int id)
        {
            return View("AlterarTipoProduto", TipoProdutoAplicacao.BuscarPorId(id));
        }

        [HttpPost]
        public ActionResult AlterarTipoProduto(TipoProduto entidade)
        {
            var result = TipoProdutoAplicacao.AlterarTipoProduto(entidade);
            if (result.TemValor())
            {
                ViewBag.erro = result;
                return View(entidade);
            }
            return RedirectToAction("FiltroTipoProduto");
        }
    }
}