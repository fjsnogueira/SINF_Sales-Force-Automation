﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.GcpBE900;
using Interop.CrmBE900;
using ADODB;
using PharmaCRM.Lib_Primavera.Model;

namespace PharmaCRM.Lib_Primavera
{
    public class PriIntegration
    {
        #region Vendedor

        public static int NumVendedores()
        {
            StdBELista objList = PriEngine.Engine.Consulta("SELECT COUNT(Vendedor) AS num FROM vendedores");
            if (!objList.NoFim())
            {
                return objList.Valor("num");
            }
            return -1;
        }

        public static List<Model.Vendedor> ListaVendedores()
        {

            StdBELista objList;
            List<Model.Vendedor> listVendedores = new List<Model.Vendedor>();
            objList = PriEngine.Engine.Consulta("SELECT Vendedor, Nome FROM vendedores");
            while (!objList.NoFim())
            {
                Model.Vendedor vendedor = new Model.Vendedor();
                vendedor.cod = objList.Valor("Vendedor");
                vendedor.nome = objList.Valor("Nome");
                listVendedores.Add(vendedor);

                objList.Seguinte();
            }
            return listVendedores;
        }

        public static Lib_Primavera.Model.Vendedor GetVendedor(string id)
        {
            StdBELista objVen = new StdBELista();


            Model.Vendedor myVend = new Model.Vendedor();
            if (PriEngine.Engine.Comercial.Vendedores.Existe(id) == true)
            {
                objVen = PriEngine.Engine.Consulta("SELECT Vendedor, Nome FROM vendedores WHERE Vendedor = " + "\'" + id + "\'");
                Model.Vendedor vendedor = new Model.Vendedor();
                vendedor.cod = objVen.Valor("Vendedor");
                vendedor.nome = objVen.Valor("Nome");

                return vendedor;
            }
            else
            {
                return null;
            }

        }

        public static List<Model.Atividade> GetVendedorAtividades(string vendedorID, string dataInicio = null, string dataFim = null)
        {
            StdBELista objList;

            List<Model.Atividade> listAtividades = new List<Model.Atividade>();

            string query = "SELECT Tarefas.* FROM Tarefas, CabecOportunidadesVenda WHERE Vendedor = " + "\'" + vendedorID + "\'"
                    + " AND IdCabecOVenda = CabecOportunidadesVenda.ID";
            if (dataInicio != null)
                query += " AND DataInicio >= \'" + dataInicio + "\'";
            if (dataFim != null)
                query += " AND DataFim <= \'" + dataFim + "\'";
            query += " ORDER BY DataInicio ASC";
            objList = PriEngine.Engine.Consulta(query);


            while (!objList.NoFim())
            {
                Model.Atividade atividade = new Model.Atividade();
                atividade.id = objList.Valor("Id");
                atividade.idTipoAtividade = objList.Valor("IdTipoActividade");
                atividade.estado = objList.Valor("Estado");
                atividade.resumo = objList.Valor("Resumo");
                atividade.descricao = objList.Valor("Descricao");
                atividade.dataInicio = objList.Valor("DataInicio");
                atividade.dataFim = objList.Valor("DataFim");
                atividade.local = objList.Valor("LocalRealizacao");
                // atividade.vendedor = vendedorID;
                atividade.idCabecalhoOportunidadeVenda = objList.Valor("IDCabecOVenda");

                listAtividades.Add(atividade);
                objList.Seguinte();
            }
            return listAtividades;
        }

        public static Lib_Primavera.Model.RespostaErro UpdVendedor(Lib_Primavera.Model.Vendedor vendedor)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();

            GcpBEVendedor objVendedor = new GcpBEVendedor();

            try
            {
                if (PriEngine.Engine.Comercial.Vendedores.Existe(vendedor.cod) == false)
                {
                    erro.Erro = 1;
                    erro.Descricao = "O vendedor não existe";
                    return erro;
                }
                else
                {
                    objVendedor = PriEngine.Engine.Comercial.Vendedores.Edita(vendedor.cod);
                    objVendedor.set_EmModoEdicao(true);

                    objVendedor.set_Nome(vendedor.nome);

                    PriEngine.Engine.Comercial.Vendedores.Actualiza(objVendedor);

                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static List<Model.Oportunidade> GetVendedorOportunidades(string idVendedor)
        {
            StdBELista objList;
            List<Model.Oportunidade> oportunidades = new List<Model.Oportunidade>();
            objList = PriEngine.Engine.Consulta("SELECT * FROM CabecOportunidadesVenda WHERE Vendedor = " + idVendedor);

            while (!objList.NoFim())
            {
                Model.Oportunidade oportunidade = new Model.Oportunidade();
                oportunidade.id = objList.Valor("ID");
                oportunidade.descricao = objList.Valor("Descricao");
                oportunidade.numEncomenda = objList.Valor("NumEncomenda");
                oportunidade.entidade = objList.Valor("Entidade");
                oportunidade.tipoEntidade = objList.Valor("TipoEntidade");
                oportunidade.dataCriacao = objList.Valor("DataCriacao");
                oportunidade.dataExpiracao = objList.Valor("DataExpiracao");
                oportunidade.vendedor = objList.Valor("Vendedor");
                oportunidade.valorTotalOV = objList.Valor("ValorTotalOV");
                oportunidade.estado = objList.Valor("EstadoVenda");

                oportunidades.Add(oportunidade);
                objList.Seguinte();
            }
            return oportunidades;
        }

        #endregion Vendedor;

        # region Cliente

        public static List<Model.Cliente> ListaClientes()
        {
            StdBELista objList;

            List<Model.Cliente> listClientes = new List<Model.Cliente>();

            objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Notas, Fac_Local, NumContrib, Fac_Mor, Fac_Tel FROM  CLIENTES");


            while (!objList.NoFim())
            {
                listClientes.Add(new Model.Cliente
                {
                    CodCliente = objList.Valor("Cliente"),
                    Nome = objList.Valor("Nome"),
                    NumContribuinte = objList.Valor("NumContrib"),
                    Morada = objList.Valor("Fac_Mor"),
                    Localidade = objList.Valor("Fac_Local"),
                    Notas = objList.Valor("Notas"),
                    Telefone = objList.Valor("Fac_Tel"),
                    DetalhesFaturacao = getClienteFaturacao(objList.Valor("Cliente"))
                });
                objList.Seguinte();

            }

            return listClientes;
        }

        public static Lib_Primavera.Model.Cliente GetCliente(string codCliente)
        {
            GcpBECliente objCli = new GcpBECliente();

            Model.Cliente myCli = new Model.Cliente();

            if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == true)
            {
                objCli = PriEngine.Engine.Comercial.Clientes.Edita(codCliente);
                myCli.CodCliente = objCli.get_Cliente();
                myCli.Nome = objCli.get_Nome();
                myCli.NumContribuinte = objCli.get_NumContribuinte();
                myCli.Morada = objCli.get_Morada();
                myCli.Localidade = objCli.get_Localidade();
                myCli.Notas = objCli.get_Observacoes();
                myCli.Telefone = objCli.get_Telefone();

                myCli.DetalhesFaturacao = getClienteFaturacao(codCliente);

                return myCli;
            }
            else
            {
                return null;
            }
        }

        public static Lib_Primavera.Model.RespostaErro UpdCliente(Lib_Primavera.Model.Cliente cliente)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();

            GcpBECliente objCli = new GcpBECliente();

            try
            {
                if (PriEngine.Engine.Comercial.Clientes.Existe(cliente.CodCliente) == false)
                {
                    erro.Erro = 1;
                    erro.Descricao = "O cliente não existe";
                    return erro;
                }
                else
                {
                    objCli = PriEngine.Engine.Comercial.Clientes.Edita(cliente.CodCliente);
                    objCli.set_EmModoEdicao(true);

                    objCli.set_Nome(cliente.Nome);
                    objCli.set_NumContribuinte(cliente.NumContribuinte);
                    objCli.set_Morada(cliente.Morada);
                    objCli.set_Localidade(cliente.Localidade);
                    objCli.set_Observacoes(cliente.Notas);
                    objCli.set_Telefone(cliente.Telefone);

                    PriEngine.Engine.Comercial.Clientes.Actualiza(objCli);

                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
            }
            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }

        public static Lib_Primavera.Model.RespostaErro DelCliente(string codCliente)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBECliente objCli = new GcpBECliente();

            try
            {
                if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == false)
                {
                    erro.Erro = 1;
                    erro.Descricao = "O cliente não existe";
                    return erro;
                }
                else
                {
                    PriEngine.Engine.Comercial.Clientes.Remove(codCliente);
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static Lib_Primavera.Model.RespostaErro InsereClienteObj(Model.Cliente cli)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();


            GcpBECliente myCli = new GcpBECliente();

            try
            {
                myCli.set_Cliente(cli.CodCliente);
                myCli.set_Nome(cli.Nome);
                myCli.set_NumContribuinte(cli.NumContribuinte);
                myCli.set_Morada(cli.Morada);
                myCli.set_Localidade(cli.Localidade);
                myCli.set_Observacoes(cli.Notas);
                myCli.set_Telefone(cli.Telefone);
                myCli.set_Moeda("EUR");

                PriEngine.Engine.Comercial.Clientes.Actualiza(myCli);

                erro.Erro = 0;
                erro.Descricao = "Sucesso";
                return erro;
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static Model.FaturacaoCliente getClienteFaturacao(string idCliente)
        {
            Model.FaturacaoCliente faturacaoCli = new FaturacaoCliente();

            string query = "SELECT id, Entidade, NumDoc, Responsavel, IdOportunidade "
               + "FROM CabecDoc WHERE TipoDoc='ECL' AND Entidade='" + idCliente + "'";

            StdBELista encomendas = PriEngine.Engine.Consulta(query);

            double valorFaturado = 0, valorPorFaturar = 0;
            while (!encomendas.NoFim())
            {
                string docID = encomendas.Valor("id"),
                    idOportunidade = encomendas.Valor("idOportunidade");

                if (!PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(docID))
                {
                    StdBELista linhasDoc = PriEngine.Engine.Consulta("SELECT PrecoLiquido FROM LinhasDoc WHERE IdCabecDoc='" + docID + "' ORDER BY NumLinha");

                    while (!linhasDoc.NoFim())
                    {
                        double preco = linhasDoc.Valor("PrecoLiquido");

                        if (EncomendaFaturada(idOportunidade))
                            valorFaturado += preco;
                        else
                            valorPorFaturar += preco;

                        linhasDoc.Seguinte();
                    }
                }
                encomendas.Seguinte();
            }

            faturacaoCli.ValorFaturado = valorFaturado;
            faturacaoCli.ValorPorFaturar = valorPorFaturar;

            return faturacaoCli;
        }

        #endregion Cliente;   // -----------------------------  END   CLIENTE    -----------------------

        #region Artigo

        public static Lib_Primavera.Model.Artigo GetArtigo(string codArtigo)
        {

            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Artigo myArt = new Model.Artigo();

            StdBELista objListCab;

            objListCab = PriEngine.Engine.Consulta("SELECT Artigo.Artigo, Artigo.Desconto, Artigo.Descricao, STKActual, PCUltimo, PCMedio, Iva, PrazoEntrega, PVP1, PVP2, PVP3, PVP4, PVP5, PVP6, Unidades.Unidade, Unidades.Descricao AS DescricaoUnidade "
                + "FROM Artigo, ArtigoMoeda, Unidades WHERE Artigo.Artigo = ArtigoMoeda.Artigo AND Artigo.Artigo = '" + codArtigo + "' AND Artigo.UnidadeVenda = Unidades.Unidade");

            if (objListCab.NoFim())
            {
                return null;
            }

            Model.Artigo art = new Model.Artigo();
            art.Codigo = objListCab.Valor("Artigo");
            art.Descricao = objListCab.Valor("Descricao");
            art.StockAtual = objListCab.Valor("STKActual");
            art.PrecoUltimo = objListCab.Valor("PCUltimo");
            art.PrecoMedio = objListCab.Valor("PCMedio");
            art.Iva = objListCab.Valor("Iva");
            art.PrazoEntrega = objListCab.Valor("PrazoEntrega");
            art.PVPs = new List<double>();
            art.PVPs.Add(objListCab.Valor("PVP1"));
            art.PVPs.Add(objListCab.Valor("PVP2"));
            art.PVPs.Add(objListCab.Valor("PVP3"));
            art.PVPs.Add(objListCab.Valor("PVP4"));
            art.PVPs.Add(objListCab.Valor("PVP5"));
            art.PVPs.Add(objListCab.Valor("PVP6"));
            art.unidade = objListCab.Valor("Unidade");
            art.descricaoUnidade = objListCab.Valor("DescricaoUnidade");
            art.desconto = objListCab.Valor("Desconto");

            return art;

        }

        public static List<Model.Artigo> ListaArtigos()
        {
            StdBELista objListCab;
            Model.Artigo art;
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            objListCab = PriEngine.Engine.Consulta("SELECT Artigo.Artigo, Artigo.Desconto, Artigo.Descricao, STKActual, PCUltimo, PCMedio, Iva, PrazoEntrega, PVP1, PVP2, PVP3, PVP4, PVP5, PVP6, Unidades.Unidade, Unidades.Descricao AS DescricaoUnidade "
                + "FROM Artigo, ArtigoMoeda, Unidades WHERE Artigo.Artigo = ArtigoMoeda.Artigo AND Artigo.UnidadeVenda = Unidades.Unidade");
            while (!objListCab.NoFim())
            {
                art = new Model.Artigo();
                art.Codigo = objListCab.Valor("Artigo");
                art.Descricao = objListCab.Valor("Descricao");
                art.StockAtual = objListCab.Valor("STKActual");
                art.PrecoUltimo = objListCab.Valor("PCUltimo");
                art.PrecoMedio = objListCab.Valor("PCMedio");
                art.Iva = objListCab.Valor("Iva");
                art.PrazoEntrega = objListCab.Valor("PrazoEntrega");
                art.PVPs = new List<double>();
                art.PVPs.Add(objListCab.Valor("PVP1"));
                art.PVPs.Add(objListCab.Valor("PVP2"));
                art.PVPs.Add(objListCab.Valor("PVP3"));
                art.PVPs.Add(objListCab.Valor("PVP4"));
                art.PVPs.Add(objListCab.Valor("PVP5"));
                art.PVPs.Add(objListCab.Valor("PVP6"));
                art.unidade = objListCab.Valor("Unidade");
                art.descricaoUnidade = objListCab.Valor("DescricaoUnidade");
                art.desconto = objListCab.Valor("Desconto");

                listArts.Add(art);

                objListCab.Seguinte();
            }

            return listArts;
        }
        public static Lib_Primavera.Model.ArtigoResumo GetArtigoResumo(string codArtigo)
        {

            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.ArtigoResumo myArt = new Model.ArtigoResumo();

            if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
            {
                return null;
            }
            else
            {
                objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                myArt.Codigo = objArtigo.get_Artigo();
                myArt.Descricao = objArtigo.get_Descricao();

                return myArt;
            }

        }

        public static List<Model.ArtigoResumo> ListaArtigosResumo()
        {

            StdBELista objList;

            Model.ArtigoResumo art = new Model.ArtigoResumo();
            List<Model.ArtigoResumo> listArts = new List<Model.ArtigoResumo>();

            objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

            while (!objList.NoFim())
            {
                art = new Model.ArtigoResumo();
                art.Codigo = objList.Valor("artigo");
                art.Descricao = objList.Valor("descricao");

                listArts.Add(art);
                objList.Seguinte();
            }

            return listArts;

        }

        #endregion Artigo

        #region Stock

        public static List<Lib_Primavera.Model.StockArtigo> GetStock()
        {
            StdBELista objListCab;

            objListCab = PriEngine.Engine.Consulta("SELECT Artigo FROM Artigo");

            List<Lib_Primavera.Model.StockArtigo> stocks = new List<Model.StockArtigo>();

            while (!objListCab.NoFim())
            {
                string codigo = objListCab.Valor("Artigo");
                Model.StockArtigo stk = ActualGetStockArtigo(codigo);
                stocks.Add(stk);
                objListCab.Seguinte();
            }

            return stocks;
        }

        public static Lib_Primavera.Model.StockArtigo GetStockArtigo(string codArtigo)
        {
            return ActualGetStockArtigo(codArtigo);
        }

        private static Lib_Primavera.Model.StockArtigo ActualGetStockArtigo(string codArtigo)
        {
            StdBELista objListCab;

            objListCab = PriEngine.Engine.Consulta("SELECT Artigo, Armazem, Lote, StkActual, Localizacao, QtReservada, UltimaContagem, DataUltimaContagem, PCMedio, PCUltimo, BloqueadoInventario "
                + "FROM ArtigoArmazem WHERE Artigo = '" + codArtigo + "'");

            Model.StockArtigo stockCompleto = new Model.StockArtigo();
            stockCompleto.CodigoArtigo = codArtigo;
            stockCompleto.Stocks = new List<Model.StockArtigoArmazem>();

            while (!objListCab.NoFim())
            {
                Model.StockArtigoArmazem stk = new Model.StockArtigoArmazem();
                stk.Armazem = objListCab.Valor("Armazem");
                stk.Lote = objListCab.Valor("Lote");
                stk.StockAtual = objListCab.Valor("StkActual");
                stk.Localizacao = objListCab.Valor("Localizacao");
                stk.QuantidadeReservada = objListCab.Valor("QtReservada");
                stk.UltimaContagem = objListCab.Valor("UltimaContagem");
                var data = objListCab.Valor("DataUltimaContagem");
                if (data is DateTime)
                {
                    stk.DataUltimaContagem = data;
                }
                stk.PrecoMedio = objListCab.Valor("PCMedio");
                stk.PrecoUltimo = objListCab.Valor("PCUltimo");
                stk.BloqueadoInventario = objListCab.Valor("BloqueadoInventario");

                stockCompleto.Stocks.Add(stk);

                objListCab.Seguinte();
            }


            return stockCompleto;
        }

        #endregion

        #region Encomenda

        public static Model.RespostaErro CreateEncomenda(Model.Encomenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEDocumentoVenda myEnc = new GcpBEDocumentoVenda();
            try
            {
                myEnc.set_Entidade(dv.Entidade);
                myEnc.set_Tipodoc("ECL");
                myEnc.set_TipoEntidade("C");

                PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc); // Utiliza os campos entidade, tipo de documento e tipo de entidade para preencher o cabeçalho com valores por defeito

                // Preencher o resto dos campos
                myEnc.set_DataDoc(dv.Data);
                myEnc.set_IdOportunidade(dv.idOportunidade);
                myEnc.set_Serie(dv.Serie);
                myEnc.set_Responsavel(dv.idResponsavel);
                myEnc.set_Filial(dv.Filial);
                myEnc.set_CondPag("3");

                if (dv.NumeroDocumento != -1)
                {
                    // EDIÇÃO
                    myEnc.set_NumDoc(dv.NumeroDocumento);
                    myEnc.set_ID(dv.idInterno);
                }

                // Linhas do documento para a lista de linhas
                List<Model.LinhaEncomenda> lstlindv = dv.LinhasDocumento;
                //PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc);
                foreach (Model.LinhaEncomenda lin in lstlindv)
                {
                    PriEngine.Engine.Comercial.Vendas.AdicionaLinha(myEnc, lin.CodigoArtigo, lin.Quantidade, "", "", lin.PrecoUnitario, lin.Desconto);
                }

                PriEngine.Engine.IniciaTransaccao();
                PriEngine.Engine.Comercial.Vendas.Actualiza(myEnc, "PharmaCRM");
                //PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc);
                PriEngine.Engine.TerminaTransaccao();

                dv.idInterno = myEnc.get_ID();
                dv.NumeroDocumento = myEnc.get_NumDoc();

                erro.Erro = 0;
                erro.Descricao = "Encomenda criada com sucesso.";
                return erro;

            }
            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static List<Model.Encomenda> GetEncomendas(bool incluirLinhas)
        {
            StdBELista objListCab;
            StdBELista objListLin;
            Model.Encomenda dv = new Model.Encomenda();
            List<Model.Encomenda> listdv = new List<Model.Encomenda>();
            Model.LinhaEncomenda lindv = new Model.LinhaEncomenda();
            List<Model.LinhaEncomenda> listlindv = new
            List<Model.LinhaEncomenda>();

            objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, IdOportunidade, Data, NumDoc, TotalMerc, Serie, Responsavel, Filial From CabecDoc where TipoDoc='ECL' ORDER BY Data DESC");
            while (!objListCab.NoFim())
            {
                dv = new Model.Encomenda();
                dv.idInterno = objListCab.Valor("id");
                dv.Entidade = objListCab.Valor("Entidade");
                dv.idOportunidade = objListCab.Valor("IdOportunidade");
                dv.NumeroDocumento = objListCab.Valor("NumDoc");
                dv.Data = objListCab.Valor("Data");
                dv.TotalMercadoria = objListCab.Valor("TotalMerc");
                dv.Serie = objListCab.Valor("Serie");
                dv.idResponsavel = objListCab.Valor("Responsavel");
                dv.Filial = objListCab.Valor("Filial");

                dv.Anulada = PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(dv.idInterno);

                listlindv = new List<Model.LinhaEncomenda>();

                if (incluirLinhas)
                {
                    double totalMerc = 0;

                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, PCM "
                        + "FROM LinhasDoc where IdCabecDoc='" + dv.idInterno + "' order By NumLinha");

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaEncomenda();
                        lindv.IdCabecaDocumento = objListLin.Valor("idCabecDoc");
                        lindv.CodigoArtigo = objListLin.Valor("Artigo");
                        lindv.DescricaoArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PCM") * lindv.Quantidade;
                        totalMerc += lindv.TotalLiquido;

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDocumento = listlindv;

                    if (dv.TotalMercadoria == 0)
                    {
                        // TODO apagar se "preencheDadosRelacionados" atualizar valor
                        dv.TotalMercadoria = totalMerc;
                    }
                }
                else
                {
                    dv.LinhasDocumento = new List<Model.LinhaEncomenda>();
                }

                dv.Faturada = EncomendaFaturada(dv.idOportunidade);

                listdv.Add(dv);
                objListCab.Seguinte();
            }
            return listdv;
        }

        public static Model.Encomenda GetEncomenda(string id, bool incluirLinhas)
        {
            StdBELista objListCab;
            StdBELista objListLin;
            Model.Encomenda dv = new Model.Encomenda();
            Model.LinhaEncomenda lindv = new Model.LinhaEncomenda();
            List<Model.LinhaEncomenda> listlindv = new List<Model.LinhaEncomenda>();

            string st = "SELECT Id, Entidade, IdOportunidade, Data, NumDoc, TotalMerc, Serie, Responsavel, Filial FROM CabecDoc where TipoDoc='ECL' and Id='" + id + "'";
            objListCab = PriEngine.Engine.Consulta(st);

            if (objListCab.NoFim())
            {
                return null;
            }

            dv = new Model.Encomenda();
            dv.idInterno = objListCab.Valor("Id");
            dv.Entidade = objListCab.Valor("Entidade");
            dv.idOportunidade = objListCab.Valor("IdOportunidade");
            dv.NumeroDocumento = objListCab.Valor("NumDoc");
            dv.Data = objListCab.Valor("Data");
            dv.TotalMercadoria = objListCab.Valor("TotalMerc");
            dv.Serie = objListCab.Valor("Serie");
            dv.idResponsavel = objListCab.Valor("Responsavel");
            dv.Filial = objListCab.Valor("Filial");

            dv.Anulada = PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(dv.idInterno);

            objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, PCM "
                + "FROM LinhasDoc where IdCabecDoc='" + dv.idInterno + "' order By NumLinha");

            if (incluirLinhas)
            {
                double totalMerc = 0;
                listlindv = new List<Model.LinhaEncomenda>();

                while (!objListLin.NoFim())
                {
                    lindv = new Model.LinhaEncomenda();
                    lindv.IdCabecaDocumento = objListLin.Valor("idCabecDoc");
                    lindv.CodigoArtigo = objListLin.Valor("Artigo");
                    lindv.DescricaoArtigo = objListLin.Valor("Descricao");
                    lindv.Quantidade = objListLin.Valor("Quantidade");
                    lindv.Unidade = objListLin.Valor("Unidade");
                    lindv.Desconto = objListLin.Valor("Desconto1");
                    lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                    
                    lindv.TotalLiquido = objListLin.Valor("PCM") * lindv.Quantidade;
                    listlindv.Add(lindv);
                    totalMerc += lindv.TotalLiquido;
                    objListLin.Seguinte();
                }

                dv.LinhasDocumento = listlindv;

                if (dv.TotalMercadoria == 0)
                {
                    // TODO apagar se "preencheDadosRelacionados" atualizar valor
                    dv.TotalMercadoria = totalMerc;
                }
            }
            else
            {
                dv.LinhasDocumento = new List<Model.LinhaEncomenda>();
            }

            if (!(dv.idOportunidade == null || dv.idOportunidade == ""))
                dv.Faturada = EncomendaFaturada(dv.idOportunidade);

            return dv;
        }

        public static bool EncomendaFaturada(string idOportunidade)
        {
            StdBELista objList;

            //checks if the saleOpportunity linked to the order is closed as won
            string query = "SELECT ID, EstadoVenda FROM CabecOportunidadesVenda WHERE ID = '" + idOportunidade + "'";
            objList = PriEngine.Engine.Consulta(query);

            return (objList.Valor("EstadoVenda") == 1) ? true: false;
        }

        public static List<Model.Encomenda> GetEncomendasVendedor(string idVendedor)
        {
            StdBELista objListCab;
            StdBELista objListLin;
            Model.Encomenda dv = new Model.Encomenda();
            List<Model.Encomenda> listdv = new List<Model.Encomenda>();
            Model.LinhaEncomenda lindv = new Model.LinhaEncomenda();
            List<Model.LinhaEncomenda> listlindv = new
            List<Model.LinhaEncomenda>();

            objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, IdOportunidade, Data, NumDoc, TotalMerc, Serie, Responsavel, Filial From CabecDoc where TipoDoc='ECL' AND Responsavel='"
                + idVendedor + "' ORDER BY Data DESC");
            while (!objListCab.NoFim())
            {
                dv = new Model.Encomenda();
                dv.idInterno = objListCab.Valor("id");
                dv.Entidade = objListCab.Valor("Entidade");
                dv.idOportunidade = objListCab.Valor("IdOportunidade");
                dv.NumeroDocumento = objListCab.Valor("NumDoc");
                dv.Data = objListCab.Valor("Data");
                dv.TotalMercadoria = objListCab.Valor("TotalMerc");
                dv.Serie = objListCab.Valor("Serie");
                dv.idResponsavel = objListCab.Valor("Responsavel");
                dv.Filial = objListCab.Valor("Filial");

                dv.Anulada = PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(dv.idInterno);
                
                if (!(dv.idOportunidade == null || dv.idOportunidade == ""))
                    dv.Faturada = EncomendaFaturada(dv.idOportunidade);

                /*listlindv = new List<Model.LinhaEncomenda>();

                objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, PCM "
                    + "FROM LinhasDoc where IdCabecDoc='" + dv.idInterno + "' order By NumLinha");

                double totalMerc = 0;
                while (!objListLin.NoFim())
                {
                    lindv = new Model.LinhaEncomenda();
                    lindv.IdCabecaDocumento = objListLin.Valor("idCabecDoc");
                    lindv.CodigoArtigo = objListLin.Valor("Artigo");
                    lindv.DescricaoArtigo = objListLin.Valor("Descricao");
                    lindv.Quantidade = objListLin.Valor("Quantidade");
                    lindv.Unidade = objListLin.Valor("Unidade");
                    lindv.Desconto = objListLin.Valor("Desconto1");
                    lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                    lindv.TotalLiquido = objListLin.Valor("PCM") * lindv.Quantidade;
                    totalMerc += lindv.TotalLiquido;

                    listlindv.Add(lindv);
                    objListLin.Seguinte();
                }

                dv.LinhasDocumento = listlindv;

                if (dv.TotalMercadoria == 0)
                {
                    // TODO apagar se "preencheDadosRelacionados" atualizar valor
                    dv.TotalMercadoria = totalMerc;
                }
                */
                listdv.Add(dv);
                objListCab.Seguinte();
            }
            return listdv;
        }

        public static List<Model.Encomenda> GetEncomendasCliente(string idCliente)
        {
            StdBELista objListCab;
            StdBELista objListLin;
            Model.Encomenda dv = new Model.Encomenda();
            List<Model.Encomenda> listdv = new List<Model.Encomenda>();
            Model.LinhaEncomenda lindv = new Model.LinhaEncomenda();
            List<Model.LinhaEncomenda> listlindv = new
            List<Model.LinhaEncomenda>();

            objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, IdOportunidade, Data, NumDoc, TotalMerc, Serie, Responsavel, Filial From CabecDoc where TipoDoc='ECL' AND Entidade='"
                + idCliente + "' ORDER BY Data DESC");
            while (!objListCab.NoFim())
            {
                dv = new Model.Encomenda();
                dv.idInterno = objListCab.Valor("id");
                dv.Entidade = objListCab.Valor("Entidade");
                dv.idOportunidade = objListCab.Valor("IdOportunidade");
                dv.NumeroDocumento = objListCab.Valor("NumDoc");
                dv.Data = objListCab.Valor("Data");
                dv.TotalMercadoria = objListCab.Valor("TotalMerc");
                dv.Serie = objListCab.Valor("Serie");
                dv.idResponsavel = objListCab.Valor("Responsavel");
                dv.Filial = objListCab.Valor("Filial");

                dv.Anulada = PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(dv.idInterno);

                listlindv = new List<Model.LinhaEncomenda>();

                objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, PCM "
                    + "FROM LinhasDoc where IdCabecDoc='" + dv.idInterno + "' order By NumLinha");

                double totalMerc = 0;
                while (!objListLin.NoFim())
                {
                    lindv = new Model.LinhaEncomenda();
                    lindv.IdCabecaDocumento = objListLin.Valor("idCabecDoc");
                    lindv.CodigoArtigo = objListLin.Valor("Artigo");
                    lindv.DescricaoArtigo = objListLin.Valor("Descricao");
                    lindv.Quantidade = objListLin.Valor("Quantidade");
                    lindv.Unidade = objListLin.Valor("Unidade");
                    lindv.Desconto = objListLin.Valor("Desconto1");
                    lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                    lindv.TotalLiquido = objListLin.Valor("PCM") * lindv.Quantidade;
                    totalMerc += lindv.TotalLiquido;

                    listlindv.Add(lindv);
                    objListLin.Seguinte();
                }

                dv.LinhasDocumento = listlindv;

                if (dv.TotalMercadoria == 0)
                {
                    // TODO apagar se "preencheDadosRelacionados" atualizar valor
                    dv.TotalMercadoria = totalMerc;
                }

                listdv.Add(dv);
                objListCab.Seguinte();
            }
            return listdv;
        }

        public static Lib_Primavera.Model.RespostaErro AnulaEncomenda(string id)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            try
            {
                Model.Encomenda enc = GetEncomenda(id, false);
                if (enc == null)
                {
                    erro.Erro = 1;
                    erro.Descricao = "A encomenda não existe";
                    return erro;
                }
                else
                {
                    PriEngine.Engine.Comercial.Vendas.AnulaDocumento(enc.Filial, "ECL", enc.Serie, enc.NumeroDocumento);
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
            }
            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        #endregion Encomenda

        # region Atividade

        public static List<Model.Atividade> GetListaAtividades()
        {
            StdBELista objList;
            List<Model.Atividade> listAtividades = new List<Model.Atividade>();

            objList = PriEngine.Engine.Consulta("SELECT * FROM Tarefas");
            while (!objList.NoFim())
            {
                Model.Atividade atividade = new Model.Atividade();
                atividade.id = objList.Valor("Id");
                atividade.idTipoAtividade = objList.Valor("IdTipoActividade");
                atividade.estado = objList.Valor("Estado");
                atividade.resumo = objList.Valor("Resumo");
                atividade.descricao = objList.Valor("Descricao");
                atividade.dataInicio = objList.Valor("DataInicio");
                atividade.dataFim = objList.Valor("DataFim");
                atividade.local = objList.Valor("LocalRealizacao");
                //atividade.vendedor = objList.Valor("Utilizador");
                atividade.tipoEntidadePrincipal = objList.Valor("TipoEntidadePrincipal");
                atividade.entidadePrincipal = objList.Valor("EntidadePrincipal");
                //atividade.idContactoPrincipal = objList.Valor("IdContactoPrincipal");
                atividade.idCabecalhoOportunidadeVenda = objList.Valor("IDCabecOVenda");
                listAtividades.Add(atividade);
                objList.Seguinte();
            }
            return listAtividades;
        }

        public static Model.Atividade GetAtividade(string id)
        {

            CrmBEActividade atividade = new CrmBEActividade();
            Model.Atividade model_actividade;

            if (PriEngine.Engine.CRM.Actividades.Existe(id) == false)
            {
                return null;
            }
            else
            {
                atividade = PriEngine.Engine.CRM.Actividades.Edita(id);
                model_actividade = new Model.Atividade();

                model_actividade.id = atividade.get_ID();
                model_actividade.idTipoAtividade = atividade.get_IDTipoActividade();
                model_actividade.estado = Int32.Parse(atividade.get_Estado());
                model_actividade.resumo = atividade.get_Resumo();
                model_actividade.descricao = atividade.get_Descricao();
                model_actividade.dataInicio = atividade.get_DataInicio();
                model_actividade.dataFim = atividade.get_DataFim();
                model_actividade.local = atividade.get_LocalRealizacao();
                //model_actividade.vendedor = atividade.get_Utilizador();
                model_actividade.tipoEntidadePrincipal = atividade.get_TipoEntidadePrincipal();
                model_actividade.entidadePrincipal = atividade.get_EntidadePrincipal();
                //model_actividade.idContactoPrincipal = atividade.get_IDContactoPrincipal();
                model_actividade.idCabecalhoOportunidadeVenda = atividade.get_IDCabecOVenda();

                return model_actividade;
            }
        }



        public static Lib_Primavera.Model.RespostaErro InsereObjAtividade(Model.Atividade actividade)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();


            CrmBEActividade objAtividade = new CrmBEActividade();

            try
            {
                objAtividade.set_IDTipoActividade(actividade.idTipoAtividade);
                objAtividade.set_Estado(actividade.estado.ToString());
                objAtividade.set_Descricao(actividade.descricao);
                objAtividade.set_Resumo(actividade.resumo);
                objAtividade.set_DataInicio(actividade.dataInicio);
                objAtividade.set_DataFim(actividade.dataFim);
                objAtividade.set_LocalRealizacao(actividade.local);
                //objAtividade.set_CriadoPor(actividade.vendedor);
                objAtividade.set_TipoEntidadePrincipal(actividade.tipoEntidadePrincipal);
                objAtividade.set_EntidadePrincipal(actividade.entidadePrincipal);
                //objAtividade.set_IDContactoPrincipal(actividade.idContactoPrincipal);
                objAtividade.set_IDCabecOVenda(actividade.idCabecalhoOportunidadeVenda);

                PriEngine.Engine.CRM.Actividades.Actualiza(objAtividade);

                erro.Erro = 0;
                erro.Descricao = "Sucesso";
                return erro;
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static Lib_Primavera.Model.RespostaErro UpdAtividade(Lib_Primavera.Model.Atividade atividade)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();

            CrmBEActividade objAtividade = new CrmBEActividade();

            try
            {
                if (PriEngine.Engine.CRM.Actividades.Existe(atividade.id) == false)
                {
                    erro.Erro = 1;
                    erro.Descricao = "A tipoAtividade não existe";
                    return erro;
                }
                else
                {
                    objAtividade = PriEngine.Engine.CRM.Actividades.Edita(atividade.id);
                    objAtividade.set_EmModoEdicao(true);

                    //actualizam-se todos os membros mesmo que so tenham sido editados alguns
                    objAtividade.set_Estado(atividade.estado.ToString());
                    objAtividade.set_Descricao(atividade.descricao);
                    objAtividade.set_Resumo(atividade.resumo);
                    objAtividade.set_DataInicio(atividade.dataInicio);
                    objAtividade.set_DataFim(atividade.dataFim);
                    objAtividade.set_LocalRealizacao(atividade.local);
                    //objAtividade.set_CriadoPor(atividade.vendedor);
                    objAtividade.set_TipoEntidadePrincipal(atividade.tipoEntidadePrincipal);
                    objAtividade.set_EntidadePrincipal(atividade.entidadePrincipal);
                    //objAtividade.set_IDContactoPrincipal(atividade.idContactoPrincipal);
                    objAtividade.set_IDCabecOVenda(atividade.idCabecalhoOportunidadeVenda);

                    PriEngine.Engine.CRM.Actividades.Actualiza(objAtividade);

                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

        public static Lib_Primavera.Model.RespostaErro DelAtividade(string actividadeID)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            CrmBEActividade objAtividade = new CrmBEActividade();

            try
            {
                if (PriEngine.Engine.CRM.Actividades.Existe(actividadeID) == false)
                {
                    erro.Erro = 1;
                    erro.Descricao = "A tipoAtividade não existe";
                    return erro;
                }
                else
                {

                    PriEngine.Engine.CRM.Actividades.Remove(actividadeID);
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }

        public static List<Model.TipoAtividade> GetListaTiposAtividade()
        {
            StdBELista objList;
            List<Model.TipoAtividade> listTiposAtividades = new List<Model.TipoAtividade>();
            objList = PriEngine.Engine.Consulta("SELECT * FROM TiposTarefa");
            while (!objList.NoFim())
            {
                Model.TipoAtividade tipoAtividade = new Model.TipoAtividade();
                tipoAtividade.id = objList.Valor("Id");
                tipoAtividade.descricao = objList.Valor("Descricao");
                listTiposAtividades.Add(tipoAtividade);
                objList.Seguinte();
            }
            return listTiposAtividades;
        }

        public static Lib_Primavera.Model.TipoAtividade getTipoAtividade(string id)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            CrmBETipoActividade objTipoAtividade = new CrmBETipoActividade();
            if (PriEngine.Engine.CRM.TiposActividade.EditaID(id) == null)
            {
                erro.Erro = 1;
                erro.Descricao = "O tipo de tipoAtividade não existe";
            }
            else
            {
                objTipoAtividade = PriEngine.Engine.CRM.TiposActividade.EditaID(id);

                Lib_Primavera.Model.TipoAtividade tipoAtividade = new Model.TipoAtividade();
                tipoAtividade.id = objTipoAtividade.get_ID();
                tipoAtividade.descricao = objTipoAtividade.get_Descricao();

                erro.Erro = 0;
                erro.Descricao = "Sucesso";
                return tipoAtividade;
            }
            return null;
        }

        public static List<Model.Atividade> GetAtividadesCliente(string idCliente)
        {
            Model.Atividade atividade = new Model.Atividade();
            List<Model.Atividade> atividades = new List<Model.Atividade>();

            StdBELista objListCab = PriEngine.Engine.Consulta("SELECT * FROM Tarefas WHERE TipoEntidadePrincipal = 'C' AND EntidadePrincipal = '" + idCliente + "'");
            while (!objListCab.NoFim())
            {
                atividade = new Model.Atividade()
                {
                    id = objListCab.Valor("Id"),
                    idTipoAtividade = objListCab.Valor("IdTipoActividade"),
                    estado = objListCab.Valor("Estado"),
                    descricao = objListCab.Valor("Descricao"),
                    resumo = objListCab.Valor("Resumo"),
                    dataInicio = objListCab.Valor("DataInicio"),
                    dataFim = objListCab.Valor("DataFim"),
                    local = objListCab.Valor("LocalRealizacao"),
                    tipoEntidadePrincipal = objListCab.Valor("TipoEntidadePrincipal"),
                    entidadePrincipal = objListCab.Valor("EntidadePrincipal"),
                    //vendedor = objListCab.Valor("ResponsavelPor"),
                    //idContactoPrincipal = objListCab.Valor("IdContactoPrincipal"),
                    idCabecalhoOportunidadeVenda = objListCab.Valor("IdCabecOVenda"),
                };

                atividades.Add(atividade);
                objListCab.Seguinte();
            }
            return atividades;
        }

        #endregion Actividade;   // -----------------------------  END   Actividade    -----------------------

        #region Oportunidade

        public static List<Model.Oportunidade> getOportunidades()
        {
            StdBELista objList;
            List<Model.Oportunidade> listLeads = new List<Model.Oportunidade>();
            objList = PriEngine.Engine.Consulta("SELECT * FROM CabecOportunidadesVenda");

            while (!objList.NoFim())
            {
                Model.Oportunidade oportunidade = new Model.Oportunidade();
                oportunidade.id = objList.Valor("ID");
                oportunidade.codigo = objList.Valor("Oportunidade");
                oportunidade.descricao = objList.Valor("Descricao");
                oportunidade.numEncomenda = objList.Valor("NumEncomenda");
                oportunidade.entidade = objList.Valor("Entidade");
                oportunidade.tipoEntidade = objList.Valor("TipoEntidade");
                oportunidade.dataCriacao = objList.Valor("DataCriacao");
                oportunidade.dataExpiracao = objList.Valor("DataExpiracao");
                oportunidade.vendedor = objList.Valor("Vendedor");
                oportunidade.valorTotalOV = objList.Valor("ValorTotalOV");
                oportunidade.estado = objList.Valor("EstadoVenda");

                listLeads.Add(oportunidade);
                objList.Seguinte();
            }
            return listLeads;
        }

        public static Lib_Primavera.Model.Oportunidade getOportunidade(string id)
        {
            StdBELista objList = PriEngine.Engine.Consulta("SELECT * FROM CabecOportunidadesVenda WHERE ID='" + id + "'");
            Model.Oportunidade oportunidade = new Model.Oportunidade();
            oportunidade.id = objList.Valor("ID");
            oportunidade.codigo = objList.Valor("Oportunidade");
            oportunidade.descricao = objList.Valor("Descricao");
            oportunidade.numEncomenda = objList.Valor("NumEncomenda");
            oportunidade.entidade = objList.Valor("Entidade");
            oportunidade.tipoEntidade = objList.Valor("TipoEntidade");
            oportunidade.dataCriacao = objList.Valor("DataCriacao");
            oportunidade.dataExpiracao = objList.Valor("DataExpiracao");
            oportunidade.vendedor = objList.Valor("Vendedor");
            oportunidade.valorTotalOV = objList.Valor("ValorTotalOV");
            oportunidade.estado = objList.Valor("EstadoVenda");
            return oportunidade;
        }

        public static List<Model.Atividade> getAtividadesDeOportunidade(string id)
        {
            StdBELista objList;
            List<Model.Atividade> listAtividades = new List<Model.Atividade>();

            objList = PriEngine.Engine.Consulta("SELECT * FROM Tarefas WHERE IDCabecOVenda = '" + id + "'");

            while (!objList.NoFim())
            {
                Model.Atividade atividade = new Model.Atividade();
                atividade.id = objList.Valor("Id");
                atividade.idTipoAtividade = objList.Valor("IdTipoActividade");
                atividade.estado = objList.Valor("Estado");
                atividade.resumo = objList.Valor("Resumo");
                atividade.descricao = objList.Valor("Descricao");
                atividade.dataInicio = objList.Valor("DataInicio");
                atividade.dataFim = objList.Valor("DataFim");
                atividade.local = objList.Valor("LocalRealizacao");
                //atividade.vendedor = objList.Valor("Utilizador");
                atividade.tipoEntidadePrincipal = objList.Valor("TipoEntidadePrincipal");
                atividade.entidadePrincipal = objList.Valor("EntidadePrincipal");
                //atividade.idContactoPrincipal = objList.Valor("IdContactoPrincipal");
                atividade.idCabecalhoOportunidadeVenda = objList.Valor("IDCabecOVenda");
                listAtividades.Add(atividade);
                objList.Seguinte();
            }
            return listAtividades;
        }

        public static Lib_Primavera.Model.RespostaErro createOportunidade(Model.Oportunidade oportunidade)
        {
            Lib_Primavera.Model.RespostaErro respostaErro = new Model.RespostaErro();
            try
            {
                CrmBEOportunidadeVenda oportunidadeVenda = new CrmBEOportunidadeVenda();
                oportunidadeVenda.set_ID(Guid.NewGuid().ToString());
                oportunidadeVenda.set_Oportunidade(oportunidade.codigo);
                oportunidadeVenda.set_Descricao(oportunidade.descricao);
                oportunidadeVenda.set_NumEncomenda(oportunidade.numEncomenda);
                oportunidadeVenda.set_Entidade(oportunidade.entidade);
                oportunidadeVenda.set_TipoEntidade(oportunidade.tipoEntidade);
                oportunidadeVenda.set_DataCriacao(DateTime.Now);
                oportunidadeVenda.set_DataExpiracao(oportunidade.dataExpiracao);
                oportunidadeVenda.set_CicloVenda("CV_HW");
                oportunidadeVenda.set_Moeda("EUR");
                oportunidadeVenda.set_Vendedor(oportunidade.vendedor);
                oportunidadeVenda.set_ValorTotalOV(oportunidade.valorTotalOV);
                oportunidadeVenda.set_EstadoVenda(oportunidade.estado);

                PriEngine.Engine.CRM.OportunidadesVenda.Actualiza(oportunidadeVenda);

                respostaErro.Erro = 0;
                respostaErro.Descricao = "Oportunidade criada com sucesso.";
            }
            catch (Exception ex)
            {
                respostaErro.Erro = 1;
                respostaErro.Descricao = ex.Message;
            }
            return respostaErro;
        }

        public static Lib_Primavera.Model.RespostaErro deleteOportunidade(string id)
        {
            Lib_Primavera.Model.RespostaErro respostaErro = new Model.RespostaErro();
            try
            {
                if (!PriEngine.Engine.CRM.OportunidadesVenda.ExisteID(id))
                {
                    respostaErro.Erro = 1;
                    respostaErro.Descricao = "Oportunidade não encontrada.";
                }
                else
                {
                    PriEngine.Engine.CRM.OportunidadesVenda.RemoveID(id);
                    respostaErro.Erro = 0;
                    respostaErro.Descricao = "Oportunidade apagada com sucesso.";
                }
            }
            catch (Exception ex)
            {
                respostaErro.Erro = 1;
                respostaErro.Descricao = ex.Message;
            }
            return respostaErro;
        }

        public static Lib_Primavera.Model.RespostaErro UpdOportunidade(Lib_Primavera.Model.Oportunidade oportunidade)
        {
            Lib_Primavera.Model.RespostaErro respostaErro = new Model.RespostaErro();
            CrmBEOportunidadeVenda objOportunidade = new CrmBEOportunidadeVenda();
            try
            {
                if (!PriEngine.Engine.CRM.OportunidadesVenda.ExisteID(oportunidade.id))
                {
                    respostaErro.Erro = 1;
                    respostaErro.Descricao = "Oportunidade não encontrada.";
                }
                else
                {
                    objOportunidade = PriEngine.Engine.CRM.OportunidadesVenda.EditaID(oportunidade.id);
                    objOportunidade.set_EmModoEdicao(true);

                    //actualizam-se todos os membros mesmo que so tenham sido editados alguns
                    objOportunidade.set_ID(oportunidade.id);
                    objOportunidade.set_Oportunidade(oportunidade.codigo);
                    objOportunidade.set_Descricao(oportunidade.descricao);
                    objOportunidade.set_NumEncomenda(oportunidade.numEncomenda);
                    objOportunidade.set_Entidade(oportunidade.entidade);
                    objOportunidade.set_TipoEntidade(oportunidade.tipoEntidade);
                    objOportunidade.set_DataCriacao(oportunidade.dataCriacao);
                    objOportunidade.set_DataExpiracao(oportunidade.dataExpiracao);
                    objOportunidade.set_CicloVenda("CV_HW");
                    objOportunidade.set_Moeda("EUR");
                    objOportunidade.set_Vendedor(oportunidade.vendedor);
                    objOportunidade.set_ValorTotalOV(oportunidade.valorTotalOV);

                    //apenas permite editar o estado se a oportunidade estiver em aberto
                    short estado = objOportunidade.get_EstadoVenda();
                    if (estado != oportunidade.estado && estado == 0) //0 -> em aberto
                    {   //1 = ganha
                        if (oportunidade.estado == 1 && !CanCloseOpportunityAsWon(oportunidade.id)) 
                        {
                            respostaErro.Erro = 1;
                            respostaErro.Descricao = "Não pode fechar uma oportunidade de venda como ganha se não existir nenhuma encomenda associada.";
                            return respostaErro;
                        }

                        objOportunidade.set_EstadoVenda(oportunidade.estado);
                    }                                     

                    PriEngine.Engine.CRM.OportunidadesVenda.Actualiza(objOportunidade);

                    respostaErro.Erro = 0;
                    respostaErro.Descricao = "Sucesso";
                }
                return respostaErro;
            }

            catch (Exception ex)
            {
                respostaErro.Erro = 1;
                respostaErro.Descricao = ex.Message;
                return respostaErro;
            }
        }

        public static Boolean CanCloseOpportunityAsWon(string idOportunidade)
        {
            StdBELista objList;

            //checks if there is an order linked to this opportunity
            string query = "SELECT id, IdOportunidade FROM CabecDoc where TipoDoc='ECL' and idOportunidade = " + idOportunidade;
            objList = PriEngine.Engine.Consulta(query);

            //TODO Testar com o metodo "PriEngine.Engine.CRM.OportunidadesVend.ExistemEncomendas"

            return !objList.NoFim();
        }

        public static List<Model.Oportunidade> GetOportunidadesCliente(string idCliente)
        {
            StdBELista objList;
            List<Model.Oportunidade> listLeads = new List<Model.Oportunidade>();
            objList = PriEngine.Engine.Consulta("SELECT * FROM CabecOportunidadesVenda WHERE TipoEntidade = 'C' AND Entidade = '" + idCliente + "'");

            while (!objList.NoFim())
            {
                Model.Oportunidade oportunidade = new Model.Oportunidade();
                oportunidade.id = objList.Valor("ID");
                oportunidade.codigo = objList.Valor("Oportunidade");
                oportunidade.descricao = objList.Valor("Descricao");
                oportunidade.numEncomenda = objList.Valor("NumEncomenda");
                oportunidade.entidade = objList.Valor("Entidade");
                oportunidade.tipoEntidade = objList.Valor("TipoEntidade");
                oportunidade.dataCriacao = objList.Valor("DataCriacao");
                oportunidade.dataExpiracao = objList.Valor("DataExpiracao");
                oportunidade.vendedor = objList.Valor("Vendedor");
                oportunidade.valorTotalOV = objList.Valor("ValorTotalOV");
                oportunidade.estado = objList.Valor("EstadoVenda");

                listLeads.Add(oportunidade);
                objList.Seguinte();
            }
            return listLeads;
        }

        #endregion

        #region KPI

        public static Model.KPI getKPIs()
        {
            return getVendedorKPIs(null);
        }

        public static Model.KPI getVendedorKPIs(string idVendedor)
        {
            Model.KPI kpis = new Model.KPI();
            kpis.IdVendedor = idVendedor;
            string restricaoVendedor;
            if (idVendedor == null)
            {
                restricaoVendedor = "";
            }
            else
            {
                restricaoVendedor = " AND Responsavel='" + idVendedor + "'";
            }

            // Encomendas último mês
            StdBELista encomendas1M = PriEngine.Engine.Consulta("SELECT CabecDoc.id, CabecDoc.Entidade, CabecDoc.NumDoc, CabecDoc.Responsavel, Vendedores.Nome "
                + "FROM CabecDoc INNER JOIN Vendedores ON Vendedores.Vendedor = CabecDoc.Responsavel WHERE TipoDoc='ECL' AND Data >= DATEADD(MONTH, -1, GETDATE())" + restricaoVendedor);

            // Encomendas dos 2 meses anteriores ao último
            StdBELista encomendas2_3M = PriEngine.Engine.Consulta("SELECT CabecDoc.id, CabecDoc.Entidade, CabecDoc.NumDoc, CabecDoc.Responsavel, Vendedores.Nome "
                + "FROM CabecDoc INNER JOIN Vendedores ON Vendedores.Vendedor = CabecDoc.Responsavel WHERE TipoDoc='ECL' AND Data >= DATEADD(MONTH, -3, GETDATE()) AND [Data] < DATEADD(MONTH, -1, GETDATE())" + restricaoVendedor);

            Dictionary<string, double> produtosQuantidadeVendida = new Dictionary<string, double>();
            Dictionary<string, double> clientesValorComprado = new Dictionary<string, double>();
            Dictionary<string, double> vendedoresQuantidadeVendida = new Dictionary<string, double>();

            while (!encomendas1M.NoFim())
            {
                string docID = encomendas1M.Valor("id");
                string cliente = encomendas1M.Valor("Entidade");
                string vendedor = encomendas1M.Valor("Responsavel");
                string vendedorNome = encomendas1M.Valor("Nome");

                kpis.NumTotalVendas++;

                if (!PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(docID))
                {
                    kpis.NumVendasCompletas++;

                    StdBELista linhasDoc = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Quantidade, PrecoLiquido "
                        + "FROM LinhasDoc WHERE IdCabecDoc='" + docID + "' ORDER BY NumLinha");

                    double precoEncomenda = 0;

                    while (!linhasDoc.NoFim())
                    {
                        string artigo = linhasDoc.Valor("Artigo");
                        double quantidade = linhasDoc.Valor("Quantidade");
                        double preco = linhasDoc.Valor("PrecoLiquido");

                        precoEncomenda += preco;

                        double qtd;
                        if (produtosQuantidadeVendida.TryGetValue(artigo, out qtd))
                        {
                            produtosQuantidadeVendida[artigo] = qtd + quantidade;
                        }
                        else
                        {
                            produtosQuantidadeVendida.Add(artigo, quantidade);
                        }

                        kpis.ValorTotalVendas += preco;

                        linhasDoc.Seguinte();
                    }

                    double valorCliente;
                    if (clientesValorComprado.TryGetValue(cliente, out valorCliente))
                    {
                        clientesValorComprado[cliente] = valorCliente + precoEncomenda;
                    }
                    else
                    {
                        clientesValorComprado.Add(cliente, precoEncomenda);
                    }

                    double valorVendedor;
                    if (vendedoresQuantidadeVendida.TryGetValue(vendedorNome, out valorVendedor))
                    {
                        vendedoresQuantidadeVendida[vendedorNome] = valorVendedor + precoEncomenda;
                    }
                    else
                    {
                        vendedoresQuantidadeVendida.Add(vendedorNome, precoEncomenda);
                    }

                    kpis.ValorTotalVendas += precoEncomenda;
                }

                encomendas1M.Seguinte();
            }

            while (!encomendas2_3M.NoFim())
            {
                string docID = encomendas2_3M.Valor("id");
                string cliente = encomendas2_3M.Valor("Entidade");
                string vendedor = encomendas2_3M.Valor("Responsavel");
                string vendedorNome = encomendas2_3M.Valor("Nome");

                if (!PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(docID))
                {
                    StdBELista linhasDoc = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Quantidade, PrecoLiquido "
                        + "FROM LinhasDoc WHERE IdCabecDoc='" + docID + "' ORDER BY NumLinha");

                    double precoEncomenda = 0;

                    while (!linhasDoc.NoFim())
                    {
                        string artigo = linhasDoc.Valor("Artigo");
                        double quantidade = linhasDoc.Valor("Quantidade");
                        double preco = linhasDoc.Valor("PrecoLiquido");

                        precoEncomenda += preco;

                        double qtd;
                        if (produtosQuantidadeVendida.TryGetValue(artigo, out qtd))
                        {
                            produtosQuantidadeVendida[artigo] = qtd + quantidade;
                        }
                        else
                        {
                            produtosQuantidadeVendida.Add(artigo, quantidade);
                        }

                        linhasDoc.Seguinte();
                    }

                    double valorCliente;
                    if (clientesValorComprado.TryGetValue(cliente, out valorCliente))
                    {
                        clientesValorComprado[cliente] = valorCliente + precoEncomenda;
                    }
                    else
                    {
                        clientesValorComprado.Add(cliente, precoEncomenda);
                    }

                    double valorVendedor;
                    if (vendedoresQuantidadeVendida.TryGetValue(vendedorNome, out valorVendedor))
                    {
                        vendedoresQuantidadeVendida[vendedorNome] = valorVendedor + precoEncomenda;
                    }
                    else
                    {
                        vendedoresQuantidadeVendida.Add(vendedorNome, precoEncomenda);
                    }
                }

                encomendas2_3M.Seguinte();
            }

            Dictionary<string, double> ordenado = produtosQuantidadeVendida.OrderByDescending(pair => pair.Value).Take(10).ToDictionary(pair => pair.Key, pair => pair.Value);
            Dictionary<string, double> artigosQuantidadesVendidas = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> entry in ordenado)
            {
                artigosQuantidadesVendidas.Add(GetArtigo(entry.Key).Descricao, entry.Value);
            }
            kpis.ProdutosMaisVendidos = artigosQuantidadesVendidas;

            kpis.NumClientesAtivos = clientesValorComprado.Count();
            kpis.MelhoresClientes = clientesValorComprado.OrderByDescending(pair => pair.Value).Take(10).ToDictionary(pair => pair.Key, pair => pair.Value);

            kpis.NovasOportunidades = KPI_getVendedorOportunidades(true, idVendedor);
            kpis.NumOportunidadesPendentes = KPI_getVendedorOportunidades(false, idVendedor);

            if (idVendedor == null)
            {
                Model.GlobalKPI kpi = new GlobalKPI();
                kpi.IdVendedor = kpis.IdVendedor;
                kpi.NumTotalVendas = kpis.NumTotalVendas;
                kpi.ValorTotalVendas = kpis.ValorTotalVendas;
                kpi.ProdutosMaisVendidos = kpis.ProdutosMaisVendidos;
                kpi.NumVendasCompletas = kpis.NumVendasCompletas;
                kpi.NumOportunidadesPendentes = kpis.NumOportunidadesPendentes;
                kpi.NumClientesAtivos = kpis.NumClientesAtivos;
                kpi.MelhoresClientes = kpis.MelhoresClientes;
                kpi.MelhoresVendedores = vendedoresQuantidadeVendida;
                int numVendedores = NumVendedores();
                if (numVendedores > 0)
                {
                    kpi.ValorMedioVendasPorVendedor = kpi.ValorTotalVendas / numVendedores;
                }
                return kpi;
            }

            return kpis;
        }

        private static int KPI_getVendedorOportunidades(bool recentes, string idVendedor)
        {
            string dataRestricao;
            if (recentes)
            {
                dataRestricao = " AND DataCriacao >= DATEADD(MONTH, -1, GETDATE())";
            }
            else
            {
                dataRestricao = " AND DataCriacao >= DATEADD(MONTH, -3, GETDATE())";
            }

            string restricaoVendedor;
            if (idVendedor == null)
            {
                restricaoVendedor = "";
            }
            else
            {
                restricaoVendedor = " Vendedor='" + idVendedor + "' AND";
            }

            StdBELista oportunidadesLista = PriEngine.Engine.Consulta("SELECT ID FROM CabecOportunidadesVenda WHERE" +
                restricaoVendedor + " DataExpiracao < CURRENT_TIMESTAMP" + dataRestricao);
            return oportunidadesLista.NumLinhas();
        }

        public static double getVendedorVendasMes(string idVendedor)
        {
            // Encomendas último mês
            StdBELista encomendas = PriEngine.Engine.Consulta("SELECT id, Entidade, NumDoc, Responsavel, IdOportunidade "
                + "FROM CabecDoc WHERE TipoDoc='ECL' AND Data >= DATEADD(MONTH, -1, GETDATE()) AND Responsavel='" + idVendedor + "'");

            double valorEncomendas = 0;
            while (!encomendas.NoFim())
            {
                string docID = encomendas.Valor("id"),
                    idOportunidade = encomendas.Valor("idOportunidade");

                if (!PriEngine.Engine.Comercial.Vendas.DocumentoAnuladoID(docID)
                    && EncomendaFaturada(idOportunidade))
                {
                    StdBELista linhasDoc = PriEngine.Engine.Consulta("SELECT PrecoLiquido FROM LinhasDoc WHERE IdCabecDoc='" + docID + "' ORDER BY NumLinha");

                    while (!linhasDoc.NoFim())
                    {
                        double preco = linhasDoc.Valor("PrecoLiquido");
                        valorEncomendas += preco;
                        linhasDoc.Seguinte();
                    }
                }
                encomendas.Seguinte();
            }

            return valorEncomendas;
        }
        
        #endregion
    }
}