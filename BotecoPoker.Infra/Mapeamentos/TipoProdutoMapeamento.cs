using BotecoPoker.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Infra.Mapeamentos
{
    public class TipoProdutoMapeamento : EntityTypeConfiguration<TipoProduto>
    {
        public TipoProdutoMapeamento()
        {
            ToTable("TipoProduto");
            HasKey(d => d.Id);
            Property(d => d.Nome).HasColumnType("varchar").HasMaxLength(20);
        }
    }
}
