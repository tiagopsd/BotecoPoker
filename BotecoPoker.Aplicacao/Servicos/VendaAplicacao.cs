﻿using BotecoPoker.Aplicacao.Validadores;
using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.Enumeradores;
using BotecoPoker.Dominio.InterfacesRepositorio;
using BotecoPoker.Dominio.modelos;
using BotecoPoker.Dominio.Modelos;
using BotecoPoker.Infra.Impressora;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotecoPoker.Aplicacao.Servicos
{
    public class VendaAplicacao
    {
        [Inject]
        public IVendaRepositorio VendaRepositorio { get; set; }
        [Inject]
        public IPreVendaRepositorio PreVendaRepositorio { get; set; }
        [Inject]
        public IItemVendaRepositorio ItemVendaRepositorio { get; set; }
        [Inject]
        public IDbContexto Contexto { get; set; }
        [Inject]
        public IPagamentoRepositorio PagamentoRepositorio { get; set; }
        [Inject]
        public IParcelamentoPagamentoRepositorio ParcelamentoPagamentoRepositorio { get; set; }
        [Inject]
        public ClienteAplicacao ClienteAplicacao { get; set; }
        [Inject]
        public ProdutoAplicacao ProdutoAplicacao { get; set; }
        [Inject]
        public AutenticacaoAplicacao AutenticacaoAplicacao { get; set; }
        [Inject]
        public CaixaAplicacao CaixaAplicacao { get; set; }
        //[Inject]
        //public UsuarioAplicacao UsuarioAplicacao { get; set; }

        public PaginacaoModel2<Venda, PreVenda, FiltroVenda> ObterDetalhesVenda
            (PaginacaoModel2<Venda, PreVenda, FiltroVenda> paginacaoModel, long idVenda)
        {
            paginacaoModel.ListaModel2 = PreVendaRepositorio.Filtrar(d => d.IdVenda == idVenda).ToList();
            paginacaoModel.NomeCliente = VendaRepositorio.Filtrar(d => d.Id == idVenda).Select(d => d.Cliente.Nome).FirstOrDefault();
            return paginacaoModel;
        }

        public VendaModelo ObterPreVendaAtual()
        {
            var idUsuarioAtual = AutenticacaoAplicacao.ObterUsuarioLogado().Id;
            var preVendas = PreVendaRepositorio.ObterPreVendaAtual(idUsuarioAtual);
            foreach (var item in preVendas)
            {
                if (item.Produto == null)
                    item.Produto = ProdutoAplicacao.BuscarPorId(item.IdProduto);
            }

            return new VendaModelo
            {
                PreVendas = preVendas,
                PaginacaoCliente = new Dominio.modelos.PaginacaoModel<Cliente, Dominio.modelos.FiltroCliente>(),
                ValorTotal = preVendas.Sum(d => (d.Produto.Valor * d.Quantidade)),
            };
        }

        public string DeletarVenda(long idVenda)
        {
            try
            {
                var venda = VendaRepositorio.Buscar(idVenda);
                if (venda.Situacao == SituacaoVenda.Pago)
                    return "Impossivel excluir uma venda paga!";
                var preVenda = PreVendaRepositorio.Filtrar(d => d.IdVenda == idVenda).ToList();
                if (preVenda != null)
                    preVenda.ForEach(d => PreVendaRepositorio.Excluir(d));
                var itemVenda = ItemVendaRepositorio.Filtrar(d => d.IdVenda == idVenda).ToList();
                if (itemVenda != null)
                    itemVenda.ForEach(d => ItemVendaRepositorio.Excluir(d));
                if (venda != null)
                    VendaRepositorio.Excluir(venda);


                var linhas = Contexto.Salvar();
                return "";
            }
            catch (Exception ex)
            {
                return "Erro ao tentar excluir Venda. Tente novamente!";
            }
        }

        public void ImprimirVenda(long idVenda)
        {
            //var venda = VendaRepositorio.Buscar(idVenda);
            //var nomeImpressora = UsuarioAplicacao.ObterDadosUsuarioLogado().Impressora.ToString();
            //new ImprimeVenda().Imprime(venda, nomeImpressora);
        }

        public PaginacaoModel2<Venda, PreVenda, FiltroVenda> FiltrarVenda(PaginacaoModel2<Venda, PreVenda, FiltroVenda> paginacao)
        {
            if (paginacao.Filtro == null)
                paginacao.Filtro = new FiltroVenda(paginacao.Parametro1, paginacao.Parametro12, paginacao.Parametro13);
            else
            {
                paginacao.Parametro1 = paginacao?.Filtro?.NomeCliente;
                paginacao.Parametro12 = paginacao?.Filtro?.CodigoCliente;
                paginacao.Parametro13 = paginacao?.Filtro?.ApelidoCliente;
            }
            var dataCaixa = CaixaAplicacao.ObterDataCaixaAtivo();
            if (dataCaixa == DateTime.MinValue)
                dataCaixa = DateTime.Now;
            var query = VendaRepositorio.Query().Where(d => d.DataVenda >= dataCaixa);
            if (paginacao.Filtro.NomeCliente.TemValor())
            {
                query = query.Where(d => d.Cliente.Nome.Contains(paginacao.Filtro.NomeCliente));
            }
            if (paginacao.Filtro.CodigoCliente.TemValor())
            {
                query = query.Where(d => d.Cliente.Codigo.Contains(paginacao.Filtro.CodigoCliente));
            }
            if (paginacao.Filtro.ApelidoCliente.TemValor())
            {
                query = query.Where(d => d.Cliente.Apelido.Contains(paginacao.Filtro.ApelidoCliente));
            }
            paginacao.ListaModel = query.OrderBy(d => d.Id).Skip(((paginacao.Pagina - 1) * 10)).Take(10).ToList();
            paginacao.QtdPaginas = query.Count().CalculaQtdPaginas().TransformaEmLista();
            foreach (var item in paginacao.ListaModel)
            {
                if (item?.Cliente == null)
                    item.Cliente = ClienteAplicacao.BuscarClientePorId(item.IdCliente);
            }
            return paginacao;
        }

        public string DeletarPreVenda(long idPreVenda)
        {
            try
            {
                var preVenda = PreVendaRepositorio.Buscar(idPreVenda);
                PreVendaRepositorio.Excluir(preVenda);
                Contexto.Salvar();
                return "";
            }
            catch
            {
                return "Erro ao tentar excluir item. Tente novamente!";
            }
        }

        public string GravarPreVenda(PreVenda preVenda)
        {
            if (preVenda.Quantidade == 0)
                return "Favor inserir quantidade nesse produto";
            preVenda.Ativo = Ativo.Ativo;
            preVenda.DataHora = DateTime.Now;
            preVenda.IdUsuario = AutenticacaoAplicacao.ObterUsuarioLogado().Id;
            PreVendaRepositorio.Cadastrar(preVenda);
            Contexto.Salvar();
            return "";
        }

        public string FinalizarVenda(FinalizacaoVendaModel finalizacaoVendaModel)
        {
            try
            {
                if (finalizacaoVendaModel.IdCliente == 0)
                    return "Favor selecionar o cliente";

                if (finalizacaoVendaModel.ValorPago > 0 && finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.EmAberto)
                    return "Favor selecionar o tipo do pagamento!";

                if (finalizacaoVendaModel.ValorPago > 0 && finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.Cortesia)
                    return "Favor retirar o valor se for uma cortesia";

                if ((finalizacaoVendaModel?.ValorPago ?? 0) == 0
                    && finalizacaoVendaModel.TipoFinalizador != TipoFinalizador.Saldo
                    && finalizacaoVendaModel.TipoFinalizador != TipoFinalizador.EmAberto
                    && finalizacaoVendaModel.TipoFinalizador != TipoFinalizador.Cortesia)
                    return "Favor digite o valor a ser pago ou selecione o tipo de pagamento correto!";
                var idUsuarioLogado = AutenticacaoAplicacao.ObterUsuarioLogado().Id;
                var preVendas = PreVendaRepositorio.Filtrar(d => d.Ativo == Ativo.Ativo && d.IdUsuario == idUsuarioLogado).ToList();
                var venda = GravarVenda(finalizacaoVendaModel.IdCliente, preVendas);

                if (finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.Saldo && finalizacaoVendaModel.ValorPago > venda.Cliente.Saldo)
                    return "Favor selecionar o tipo do pagamento!";

                if (finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.Saldo && ((venda?.Cliente?.Saldo ?? 0) + (finalizacaoVendaModel?.ValorPago ?? 0)) < venda.Valor)
                    return "Cliente não possui saldo!";

                GeraPagamentoVenda(finalizacaoVendaModel, venda);

                var resultado = Contexto.Salvar();
                return "";
            }
            catch (Exception ex)
            {
                return "Erro ao tentar finalizar a venda, tente novamente!";
            }
        }

        private string GeraPagamentoVenda(FinalizacaoVendaModel finalizacaoVendaModel, Venda venda)
        {
            if (finalizacaoVendaModel.ValorPago > 0 && finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.Cortesia)
                return "Favor não colocar valor pago em uma cortesia!";

            Pagamento pagamento = null;
            if (finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.Saldo)
            {
                var valorTotalPago = (finalizacaoVendaModel.ValorPago ?? 0) + venda.Cliente.Saldo;
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = venda.IdCliente,
                    ValorAberto = 0,
                    ValorTotal = venda.Valor,
                    Situacao = SituacaoVenda.Pago
                };
                //if (finalizacaoVendaModel.TrocoSaldo ?? false)
                //    venda.Cliente.Saldo = valorTotalPago - venda.Valor;
                //else
                //    venda.Cliente.Saldo = 0;
            }
            else if (venda.Valor > ((finalizacaoVendaModel?.ValorPago ?? 0) + (venda?.Cliente?.Saldo ?? 0)) && finalizacaoVendaModel.ValorPago > 0)
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = venda.IdCliente,
                    ValorAberto = (double)(venda.Valor - finalizacaoVendaModel.ValorPago),
                    ValorTotal = venda.Valor,
                    Situacao = SituacaoVenda.Pendente
                };
            }
            else if (((finalizacaoVendaModel?.ValorPago ?? 0) + (venda?.Cliente?.Saldo ?? 0)) > venda.Valor && (finalizacaoVendaModel.TrocoSaldo ?? false))
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = venda.IdCliente,
                    ValorAberto = 0,
                    ValorTotal = venda.Valor,
                    Situacao = SituacaoVenda.Pago
                };
                //if (finalizacaoVendaModel.TipoFinalizador != TipoFinalizador.Cortesia)
                //    venda.Cliente.Saldo += (finalizacaoVendaModel.ValorPago - venda.Valor);
            }
            else if (finalizacaoVendaModel.TipoFinalizador == TipoFinalizador.Cortesia)
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    Cliente = venda.Cliente,
                    ValorAberto = 0,
                    ValorTotal = venda.Valor,
                    Situacao = SituacaoVenda.Pago,
                };
            }
            else if (finalizacaoVendaModel.ValorPago > 0)
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    Cliente = venda.Cliente,
                    ValorAberto = 0,
                    ValorTotal = venda.Valor,
                    Situacao = SituacaoVenda.Pago,
                };
            }
            if (pagamento != null)
            {
                ParcelamentoPagamento parcelamentoPagamento = new ParcelamentoPagamento
                {
                    Pagamento = pagamento,
                    TipoFinalizador = finalizacaoVendaModel.TipoFinalizador ?? TipoFinalizador.Dinheiro,
                    DataPagamento = pagamento.Data,
                    IdCliente = venda.IdCliente,
                    ValorPago = pagamento.ValorTotal,
                };
                if (pagamento.ValorAberto > 0)
                    parcelamentoPagamento.ValorPago = finalizacaoVendaModel.ValorPago ?? 0;

                if (((finalizacaoVendaModel?.ValorPago ?? 0) + (venda?.Cliente?.Saldo ?? 0)) == venda.Valor || !(finalizacaoVendaModel?.TrocoSaldo ?? false))
                {
                    if (finalizacaoVendaModel.TipoFinalizador != TipoFinalizador.Cortesia)
                        venda.Cliente.Saldo = 0;
                }
                else
                {
                    venda.Cliente.Saldo = ((finalizacaoVendaModel?.ValorPago ?? 0) + (venda?.Cliente?.Saldo ?? 0)) - venda.Valor;
                }

                PagamentoRepositorio.Cadastrar(pagamento);
                ParcelamentoPagamentoRepositorio.Cadastrar(parcelamentoPagamento);
                venda.Pagamento = pagamento;
                venda.Situacao = SituacaoVenda.Pago;
            }
            return "";
        }

        private Venda GravarVenda(long idCliente, List<PreVenda> preVendas)
        {
            var venda = new Venda
            {
                Cliente = ClienteAplicacao.BuscarClientePorId(idCliente),
                DataVenda = DateTime.Now,
                IdUsuario = AutenticacaoAplicacao.ObterUsuarioLogado().Id,
                QtdItem = RetornaQtdTotalVenda(preVendas.Select(d => d.Quantidade).ToList()),
                Valor = RetornaValorTotalVenda(preVendas)
            };
            VendaRepositorio.Cadastrar(venda);
            GravarItemVenda(preVendas, venda);
            preVendas.ForEach(d => d.Venda = venda);
            preVendas.ForEach(d => PreVendaRepositorio.Atualizar(d));
            return venda;
        }

        private double RetornaValorTotalVenda(List<PreVenda> preVendas)
        {
            double valortotal = 0;
            foreach (var preVenda in preVendas)
            {
                valortotal += (double)(ProdutoAplicacao.ObterValorProduto(preVenda.IdProduto) * preVenda.Quantidade);
            }
            return valortotal;
        }

        private short RetornaQtdTotalVenda(List<short> listaQtdItens)
        {
            short qtdTotal = 0;
            foreach (var qtd in listaQtdItens)
            {
                qtdTotal += qtd;
            }
            return qtdTotal;
        }

        private void GravarItemVenda(List<PreVenda> preVendas, Venda venda)
        {
            foreach (var preVenda in preVendas)
            {
                var itemVenda = new ItemVenda
                {
                    IdProduto = preVenda.IdProduto,
                    Venda = venda,
                    QtdProduto = preVenda.Quantidade,
                    ValorProduto = ProdutoAplicacao.ObterValorProduto(preVenda.IdProduto),
                };
                itemVenda.ValorTotal = itemVenda.ValorProduto * itemVenda.QtdProduto;
                ItemVendaRepositorio.Cadastrar(itemVenda);
                InativaPreVenda(preVenda);
            }
        }

        private void InativaPreVenda(PreVenda preVenda)
        {
            preVenda.Ativo = Ativo.Inativo;
            PreVendaRepositorio.Atualizar(preVenda);
        }
    }
}
