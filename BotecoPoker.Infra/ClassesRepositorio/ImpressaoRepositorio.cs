using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.InterfacesRepositorio;
using BotecoPoker.Infra.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Infra.ClassesRepositorio
{
    public class ImpressaoRepositorio : RepositorioBase<Impressao, long>, IImpressaoRepositorio
    {
        public ImpressaoRepositorio(DbContexto db) : base(db)
        {
        }
    }
}
