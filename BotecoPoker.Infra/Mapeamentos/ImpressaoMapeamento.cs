using BotecoPoker.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Infra.Mapeamentos
{
    public class ImpressaoMapeamento : EntityTypeConfiguration<Impressao>
    {
        public ImpressaoMapeamento()
        {
            ToTable("Impressao", "dbo");
            HasKey(d => d.Id);
            Property(d => d.IdObjetoImpressao).HasColumnType("bigint");
            Property(d => d.NomeImpressora).HasColumnType("varchar").HasMaxLength(20);
            Property(d => d.SituacaoImpressao).HasColumnType("smallint");
            Property(d => d.TipoImpressao).HasColumnType("smallint");
        }
    }
}
