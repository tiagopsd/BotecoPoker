using BotecoPoker.Aplicacao.Servicos;
using BotecoPoker.Aplicacao.Validadores;
using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.modelos;
using Ninject;
using System.Collections;
using System.Linq;
using System.Web.Mvc;

namespace BotecoPoker.Mvc.Controllers
{
    [Authorize]
    public class ProdutoController : Controller
    {
        [Inject]
        public ProdutoAplicacao ProdutoAplicacao { get; set; }
        [Inject]
        public TipoProdutoAplicacao TipoProdutoAplicacao { get; set; }

        public ActionResult FiltroProduto(PaginacaoModel<Produto, FiltroProduto> paginacao)
        {
            var selecione = new SelectListItem { Text = "Todos", Value = "0", Selected = true };
            var combo = TipoProdutoAplicacao.ObterTipoProdutoCombo();
            var newCombo = combo.ToList();
            newCombo.Add(selecione);
            ViewBag.ComboTiposProdutos = newCombo.OrderBy(d => d.Value);

            return View(ProdutoAplicacao.Filtrar(paginacao));
        }

        [HttpGet]
        public ActionResult CadastroProduto()
        {
            CarregarViewBag();
            return View(new Produto());
        }

        [HttpPost]
        public ActionResult CadastroProduto(Produto entidade)
        {
            CarregarViewBag();
            var result = ProdutoAplicacao.CadastroProduto(entidade);
            if (result.TemValor())
            {
                ViewBag.erro = result;
                return View(entidade);
            }
            return RedirectToAction("FiltroProduto");
        }

        public ActionResult BuscarPorId(int id)
        {
            CarregarViewBag();
            return View("AlterarProduto", ProdutoAplicacao.BuscarPorId(id));
        }

        [HttpPost]
        public ActionResult AlterarProduto(Produto entidade)
        {
            CarregarViewBag();
            var result = ProdutoAplicacao.AlterarProduto(entidade);
            if (result.TemValor())
            {
                ViewBag.erro = result;
                return View(entidade);
            }
            return RedirectToAction("FiltroProduto");
        }

        private void CarregarViewBag()
        {
            var selecione = new SelectListItem { Text = "Selecione", Value = "0", Selected = true };
            var combo = TipoProdutoAplicacao.ObterTipoProdutoCombo();
            var newCombo = combo.ToList();
            newCombo.Add(selecione);
            ViewBag.ComboTiposProdutos = newCombo.OrderBy(d => d.Value);
        }
    }
}