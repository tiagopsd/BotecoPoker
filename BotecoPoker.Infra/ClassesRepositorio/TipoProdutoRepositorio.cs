using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.InterfacesRepositorio;
using BotecoPoker.Infra.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BotecoPoker.Infra.ClassesRepositorio
{
    public class TipoProdutoRepositorio : RepositorioBase<TipoProduto, int>, ITipoProdutoRepositorio
    {
        public TipoProdutoRepositorio(DbContexto db) : base(db)
        {
        }

        public IEnumerable<SelectListItem> ObterComboTiposProdutos()
        {
            var TiposProdutos = Set.ToList();
            if (TiposProdutos.Count == 0 || TiposProdutos == null)
            {
                var nenhum = new List<SelectListItem>
                {
                    new SelectListItem()
                    {
                        Text = "Nenhum",
                        Value = "0"
                    }
                };
                return nenhum;
            }
            return TiposProdutos.Select(d => new SelectListItem
            {
                Text = d.Nome,
                Value = d.Id.ToString()
            })
          .AsEnumerable();
        }
    }
}
