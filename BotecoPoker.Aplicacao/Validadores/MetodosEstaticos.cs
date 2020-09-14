﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Aplicacao.Validadores
{
    public static class MetodosEstaticos
    {
        /// <summary>
        /// !string.IsNullOrWhiteSpace(valor)
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static bool TemValor(this string valor)
        {
            return (!string.IsNullOrWhiteSpace(valor));
        }

        public static int CalculaQtdPaginas(this int queryCount)
        {
            double resto = queryCount % 10;
            if (resto > 0)
                return queryCount / 10 + 1;
            return queryCount / 10;
        }

        public static List<int> TransformaEmLista(this int quantidadePagina)
        {
            var lista = new List<int>();
            for (int i = 0; i < quantidadePagina; i++)
            {
                lista.Add(i + 1);
            }
            return lista;
        }
    }
}
