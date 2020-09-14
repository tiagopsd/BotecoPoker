using System;
using System.Collections.Generic;
using System.Linq;
using BotecoPoker.Aplicacao.Validadores;
using BotecoPoker.Dominio.Entidades;
using BotecoPoker.Dominio.Enumeradores;
using BotecoPoker.Dominio.InterfacesRepositorio;
using BotecoPoker.Dominio.modelos;
using Ninject;

namespace BotecoPoker.Aplicacao.Servicos
{
    public class PagamentosAplicacao
    {
        [Inject]
        public IClienteRepositorio ClienteRepositorio { get; set; }
        [Inject]
        public IVendaRepositorio VendaRepositorio { get; set; }
        [Inject]
        public IPreVendaRepositorio PreVendaRepositorio { get; set; }
        [Inject]
        public ITorneioClienteRepositorio TorneioClienteRepositorio { get; set; }
        [Inject]
        public ICashGameRepositorio CashGameRepositorio { get; set; }
        [Inject]
        public IPagamentoRepositorio PagamentoRepositorio { get; set; }
        [Inject]
        public IParcelamentoPagamentoRepositorio ParcelamentoPagamentoRepositorio { get; set; }
        [Inject]
        public IDbContexto Contexto { get; set; }
        [Inject]
        public ITorneioRepositorio TorneioRepositorio { get; set; }

        public PaginacaoModel<Cliente, FiltroPagamento> ObterClientesComPendencia(PaginacaoModel<Cliente, FiltroPagamento> filtroPagamento)
        {
            return ClienteRepositorio.ObterClientesComPendencias(filtroPagamento);
        }

        public PendenciasCliente ObterPendenciaCliente(long idCliente)
        {
            var pendencias = new PendenciasCliente
            {
                CashGames = CashGameRepositorio.ObterCashGamesPendente(idCliente),
                TorneiosCliente = ObterItensTorneioModel(TorneioClienteRepositorio.ObterTorneioClientePendente(idCliente)),
                Vendas = VendaRepositorio.ObterVendaModelPendente(idCliente),
                Pagamentos = PagamentoRepositorio.ObterPagamentosPendentes(idCliente)
            };
            pendencias.IdCliente = idCliente;
            var cliente = ClienteRepositorio.Filtrar(d => d.Id == idCliente).Select(d => new { Nome = d.Nome, Saldo = d.Saldo }).FirstOrDefault();
            pendencias.NomeCliente = cliente.Nome;
            pendencias.Saldo = cliente.Saldo;
            pendencias.ValorTotal = pendencias.CashGames.Sum(d => d.Valor);
            pendencias.ValorTotal += pendencias.TorneiosCliente.Sum(d => d.ValorTotal - (d.ValorPago ?? 0));
            pendencias.ValorTotal += pendencias.Vendas.Sum(d => d.Valor);

            return pendencias;
        }

        public bool ExisteOperacaoPendente(DateTime dataCaixa)
        {
            var result = CashGameRepositorio.Filtrar(d => d.Situacao == Dominio.Enumeradores.SituacaoVenda.Pendente && d.DataCadastro >= dataCaixa).Any();
            if (result)
                return result;
            result = TorneioClienteRepositorio.Filtrar(d => d.Situacao == Dominio.Enumeradores.SituacaoVenda.Pendente && d.DataCadastro >= dataCaixa).Any();
            if (result)
                return result;
            return VendaRepositorio.Filtrar(d => d.Situacao == Dominio.Enumeradores.SituacaoVenda.Pendente && d.DataVenda >= dataCaixa).Any();
        }

        public List<TorneioCliente> ObterItensTorneioModel(List<TorneioCliente> torneiosCliente)
        {
            if (torneiosCliente != null && torneiosCliente.Any())
            {
                foreach (var torneioCliente in torneiosCliente)
                {
                    var torneio = TorneioRepositorio.Buscar(torneioCliente.IdTorneio);

                    double valorTotal = 0;
                    torneioCliente.ItensTorneio = new List<ItemTorneio>();
                    if (torneioCliente.JackPot != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "JackPot", ValorItem = ((double)torneio.JackPot).ToString("c2"), QtdItem = (short)torneioCliente.JackPot });
                        valorTotal += (short)torneioCliente.JackPot * (double)torneio.JackPot;
                    }
                    if (torneioCliente.BuyDouble != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Buy-Double", ValorItem = ((double)torneio.BuyDouble).ToString("c2"), QtdItem = (short)torneioCliente.BuyDouble });
                        valorTotal += (short)torneioCliente.BuyDouble * (double)torneio.BuyDouble;
                    }
                    if (torneioCliente.Addon != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Addon", QtdItem = (short)torneioCliente.Addon, ValorItem = ((double)torneio.Addon).ToString("c2") });
                        valorTotal += (short)torneioCliente.Addon * (double)torneio.Addon;
                    }
                    if (torneioCliente.BonusBeneficente != null && torneioCliente.BonusBeneficente != "0")
                    {
                        if (torneioCliente.BonusBeneficente.Contains("5"))
                        {
                            torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Bônus Beneficente", QtdItem = 1, ValorItem = "5" });
                            valorTotal += 5;
                        }
                        else
                            torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Bônus Beneficente", QtdItem = 1, ValorItem = "Alimento" });
                    }
                    if (torneioCliente.BuyIn != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Buy-In", QtdItem = (short)torneioCliente.BuyIn, ValorItem = ((double)torneio.BuyIn).ToString("c2") });
                        valorTotal += (short)torneioCliente.BuyIn * (double)torneio.BuyIn;
                    }
                    if (torneioCliente.Jantar != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Jantar", QtdItem = (short)torneioCliente.Jantar, ValorItem = ((double)torneio.Jantar).ToString("c2") });
                        valorTotal += (short)torneioCliente.Jantar * (double)torneio.Jantar;
                    }
                    if (torneioCliente.ReBuy != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Re-Buy", QtdItem = (short)torneioCliente.ReBuy, ValorItem = ((double)torneio.ReBuy).ToString("c2") });
                        valorTotal += (short)torneioCliente.ReBuy * (double)torneio.ReBuy;
                    }
                    if (torneioCliente.TaxaAdm != null)
                    {
                        torneioCliente.ItensTorneio.Add(new ItemTorneio { NomeItem = "Taxa Administrador", QtdItem = (short)torneioCliente.TaxaAdm, ValorItem = ((double)torneio.TaxaAdm).ToString("c2") });
                        valorTotal += (short)torneioCliente.TaxaAdm * (double)torneio.TaxaAdm;
                    }
                    torneioCliente.ValorTotal = valorTotal;
                    torneioCliente.NomeTorneio = torneio.Nome;
                }
            }
            return torneiosCliente;
        }

        public void GeraPagamentoTorneioCliente(long idCliente)
        {
            var torneiosCliente = ObterItensTorneioModel(TorneioClienteRepositorio.ObterTorneioClientePendente(idCliente));
            Pagamento pagamento = null;
            //quita se valor for igual
            if (torneiosCliente.Sum(d => d.ValorTotal) == torneiosCliente.Sum(d => d.ValorPago))
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = idCliente,
                    ValorAberto = 0,
                    ValorTotal = torneiosCliente.Sum(d => d.ValorTotal),
                    Situacao = SituacaoVenda.Pago
                };
            }
            //deixa pendente valor faltante
            else if (torneiosCliente.Sum(d => d.ValorTotal) > torneiosCliente.Sum(d => d.ValorPago))
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = idCliente,
                    ValorAberto = torneiosCliente.Sum(d => d.ValorTotal) - torneiosCliente.Sum(d => d.ValorPago ?? 0),
                    ValorTotal = torneiosCliente.Sum(d => d.ValorTotal),
                    Situacao = SituacaoVenda.Pendente
                };
            }
            //deixa como saldo valor superior
            else if (torneiosCliente.Sum(d => d.ValorTotal) < torneiosCliente.Sum(d => d.ValorPago))
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = idCliente,
                    ValorAberto = 0,
                    ValorTotal = torneiosCliente.Sum(d => d.ValorTotal),
                    Situacao = SituacaoVenda.Pago
                };
                AtualizaSaldoCliente(idCliente, torneiosCliente.Sum(d => d.ValorPago ?? 0) - torneiosCliente.Sum(d => d.ValorTotal));
            }
            if (pagamento != null)
            {
                var parcela = new ParcelamentoPagamento
                {
                    DataPagamento = pagamento.Data,
                    IdCliente = pagamento.IdCliente,
                    Pagamento = pagamento,
                    TipoFinalizador = TipoFinalizador.Dinheiro,
                    ValorPago = pagamento.ValorTotal - pagamento.ValorAberto
                };
                ParcelamentoPagamentoRepositorio.Cadastrar(parcela);
            }
            PagamentoRepositorio.Cadastrar(pagamento);
            AtualizaPendencias(new PendenciasCliente { TorneiosCliente = torneiosCliente, IdCliente = idCliente }, pagamento);
        }

        private void AtualizaSaldoCliente(long idCliente, double saldo)
        {
            var cliente = ClienteRepositorio.Buscar(idCliente);
            cliente.Saldo = saldo;
            ClienteRepositorio.Atualizar(cliente);
        }

        public PendenciasCliente GerarPagamento(ParcelamentoPagamento parcelamentoPagamento)
        {
            parcelamentoPagamento.ValorPago += (parcelamentoPagamento.Saldo ?? 0);
            Pagamento pagamento = null;
            if (parcelamentoPagamento.IdPagamento != null)
            {
                pagamento = PagamentoRepositorio.Buscar((long)parcelamentoPagamento.IdPagamento);
                if (pagamento.ValorAberto - parcelamentoPagamento.ValorPago <= 0)
                {
                    //pagamento.ValorAberto = 0;
                    pagamento.Situacao = Dominio.Enumeradores.SituacaoVenda.Pago;
                    pagamento.Data = DateTime.Now;
                }
                else
                {
                    pagamento.ValorAberto = pagamento.ValorAberto - parcelamentoPagamento.ValorPago;
                    pagamento.Situacao = Dominio.Enumeradores.SituacaoVenda.Pendente;
                }
                if (parcelamentoPagamento.TrocoSaldo ?? false)
                    parcelamentoPagamento.Saldo = parcelamentoPagamento.ValorPago - pagamento.ValorAberto;

                AtualizaSaldoCliente(pagamento.IdCliente, parcelamentoPagamento.TrocoSaldo, parcelamentoPagamento.Saldo, pagamento);
                PagamentoRepositorio.Atualizar(pagamento);
                GerarParcelamentoPagamento(parcelamentoPagamento, pagamento);
                var result = Contexto.Salvar();
            }
            else
            {
                pagamento = new Pagamento
                {
                    Data = DateTime.Now,
                    IdCliente = (long)parcelamentoPagamento.IdCliente
                };

                var pendencias = ObterPendenciaCliente((long)parcelamentoPagamento.IdCliente);
                if (pendencias != null)
                {
                    string troco = "";
                    var valorAntigo = pendencias.ValorTotal;
                    pagamento.ValorTotal = pendencias.ValorTotal + pendencias.TorneiosCliente.Sum(d => d.ValorPago ?? 0);
                    if ((valorAntigo - parcelamentoPagamento.ValorPago) <= 0 || parcelamentoPagamento.TipoFinalizador == TipoFinalizador.Cortesia)
                    {
                        //pagamento.ValorAberto = 0;
                        pagamento.Situacao = Dominio.Enumeradores.SituacaoVenda.Pago;
                        if (parcelamentoPagamento.ValorPago > 0)
                            troco = (valorAntigo - parcelamentoPagamento.ValorPago).ToString("C2");
                    }
                    else
                    {
                        pagamento.ValorAberto = pendencias.ValorTotal - parcelamentoPagamento.ValorPago;
                        pagamento.Situacao = Dominio.Enumeradores.SituacaoVenda.Pendente;
                    }
                    if (pendencias.TorneiosCliente.Sum(d => d.ValorPago) > 0)
                    {
                        foreach (var torneioCliente in pendencias.TorneiosCliente)
                        {
                            var parcela = new ParcelamentoPagamento();
                            parcela.DataPagamento = torneioCliente.DataAlteracao ?? torneioCliente.DataCadastro;
                            parcela.Pagamento = pagamento;
                            parcela.TipoFinalizador = TipoFinalizador.Dinheiro;
                            parcela.ValorPago = torneioCliente.ValorPago ?? 0;
                            ParcelamentoPagamentoRepositorio.Cadastrar(parcela);
                        }
                    }
                    if (parcelamentoPagamento.ValorPago - valorAntigo > 0)
                        parcelamentoPagamento.Saldo = parcelamentoPagamento.ValorPago - valorAntigo;
                    AtualizaSaldoCliente(pagamento.IdCliente, parcelamentoPagamento.TrocoSaldo, parcelamentoPagamento.Saldo, pagamento);
                    PagamentoRepositorio.Cadastrar(pagamento);
                    GerarParcelamentoPagamento(parcelamentoPagamento, pagamento);
                    AtualizaPendencias(pendencias, pagamento);
                    Contexto.Salvar();
                }
            }
            return ObterPendenciaCliente((long)parcelamentoPagamento.IdCliente);
        }

        private void AtualizaSaldoCliente(long idCliente, bool? TrocoSaldo, double? valorSaldo, Pagamento pagamento)
        {
            var cliente = ClienteRepositorio.Buscar(idCliente);
            if ((TrocoSaldo ?? false) && (valorSaldo ?? 0) > 0 && pagamento.Situacao == SituacaoVenda.Pago)
            {
                cliente.Saldo = valorSaldo;
            }
            else
            {
                cliente.Saldo = 0;
            }
            ClienteRepositorio.Atualizar(cliente);
        }

        public string ValidaPagamento(ParcelamentoPagamento parcelamentoPagamento)
        {
            var pagamento = PagamentoRepositorio.Buscar(parcelamentoPagamento.IdPagamento ?? 0);

            if (parcelamentoPagamento.TipoFinalizador == TipoFinalizador.Cortesia && parcelamentoPagamento.ValorPago > 0)
                return "Favor retirar valor preenchido se for uma cortesia!";
            if (pagamento != null && pagamento.Situacao == SituacaoVenda.Pendente && parcelamentoPagamento.TipoFinalizador == TipoFinalizador.Conta)
                return "Venda já está em aberto na conta do cliente";
            if (!new[] { TipoFinalizador.Saldo, TipoFinalizador.Cortesia, TipoFinalizador.Conta }.Contains(parcelamentoPagamento.TipoFinalizador) && parcelamentoPagamento.ValorPago /*+ parcelamentoPagamento.Saldo*/ == 0)
                return "Favor preencher o valor a ser pago";
            if (parcelamentoPagamento.TipoFinalizador == TipoFinalizador.Saldo && parcelamentoPagamento.Saldo <= 0)
                return "Cliente não possue saldo!";
            if (parcelamentoPagamento.TipoFinalizador == TipoFinalizador.Conta && parcelamentoPagamento.ValorPago > 0)
                return "Favor retirar valor preenchido se for colocar na conta do cliente!";


            return "";
        }

        public void GerarParcelamentoPagamento(ParcelamentoPagamento parcelamentoPagamento, Pagamento pagamento)
        {
            parcelamentoPagamento.DataPagamento = DateTime.Now;
            parcelamentoPagamento.Pagamento = pagamento;
            //if (parcelamentoPagamento.ValorPago > pagamento.ValorAberto)
            //{
            //    parcelamentoPagamento.ValorPago = pagamento.ValorAberto;
            //    pagamento.ValorAberto = 0;
            //}
            if (parcelamentoPagamento.TipoFinalizador == TipoFinalizador.Cortesia)
                parcelamentoPagamento.ValorPago = pagamento.ValorTotal;

            ParcelamentoPagamentoRepositorio.Cadastrar(parcelamentoPagamento);
        }

        public void AtualizaPendencias(PendenciasCliente pendenciasCliente, Pagamento pagamento)
        {
            if (pendenciasCliente.CashGames != null)
                foreach (var cash in pendenciasCliente.CashGames)
                {
                    cash.Situacao = Dominio.Enumeradores.SituacaoVenda.Pago;
                    cash.Pagamento = pagamento;
                    CashGameRepositorio.Atualizar(cash);
                }
            if (pendenciasCliente.TorneiosCliente != null)
                foreach (var torneio in pendenciasCliente.TorneiosCliente)
                {
                    torneio.Situacao = Dominio.Enumeradores.SituacaoVenda.Pago;
                    torneio.Pagamento = pagamento;
                    TorneioClienteRepositorio.Atualizar(torneio);
                }
            if (pendenciasCliente.Vendas != null)
                foreach (var vendaModel in pendenciasCliente.Vendas)
                {
                    var venda = VendaRepositorio.Buscar(vendaModel.IdVenda);
                    venda.Situacao = Dominio.Enumeradores.SituacaoVenda.Pago;
                    venda.Pagamento = pagamento;
                    VendaRepositorio.Atualizar(venda);
                }
        }

        public Pagamento ObterPagamentoCliente(long idPagamento)
        {
            return PagamentoRepositorio.Buscar(idPagamento);
        }

        public ComprovanteModel ObterPagamentoCommpleto(long idPagamento)
        {
            var pagamento = ObterPagamentoCliente(idPagamento);
            var cash = CashGameRepositorio.Filtrar(d => d.IdComprovantePagamento == idPagamento).ToList();
            var parcelas = ParcelamentoPagamentoRepositorio.Filtrar(d => d.IdPagamento == idPagamento).ToList();
            var vendas = VendaRepositorio.Filtrar(d => d.IdComprovantePagamento == idPagamento).ToList();
            var torneioClientes = TorneioClienteRepositorio.Filtrar(d => d.IdComprovantePagamento == idPagamento).ToList();
            torneioClientes.ForEach(t => CalculaValorTotal(t));

            return new ComprovanteModel
            {
                Pagamento = pagamento,
                CashGames = cash,
                IdCliente = pagamento.IdCliente,
                NomeCliente = pagamento.Cliente.Nome,
                ParcelamentoPagamentos = parcelas,
                Vendas = vendas,
                TorneiosCliente = torneioClientes
            };
        }

        public void CalculaValorTotal(TorneioCliente torneioCliente)
        {
            torneioCliente.ValorTotal =
            (torneioCliente.Torneio.Addon * torneioCliente?.Addon ?? 0) +
            (torneioCliente.Torneio.BuyDouble * torneioCliente?.BuyDouble ?? 0) +
            (torneioCliente.Torneio.BuyIn * torneioCliente?.BuyIn ?? 0) +
            (torneioCliente.Torneio.JackPot * torneioCliente?.JackPot ?? 0) +
            (torneioCliente.Torneio.Jantar * torneioCliente?.Jantar ?? 0) +
            (torneioCliente.Torneio.ReBuy * torneioCliente?.ReBuy ?? 0) +
            (torneioCliente.Torneio.TaxaAdm * torneioCliente?.TaxaAdm ?? 0) +
            (torneioCliente.BonusBeneficente.TemValor() && torneioCliente.BonusBeneficente.Contains("5") ? 5 : 0)
            ;
        }

    }
}
