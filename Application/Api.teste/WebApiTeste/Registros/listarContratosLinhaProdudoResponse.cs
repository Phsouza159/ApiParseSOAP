namespace WebApiTeste.Registros
{
    public class listarContratosLinhaProdudoResponse
    {
        public static string RecuperarLista()
        {
            return @"
                {
  ""listarContratosLinhaProdudoResponse"": {
    ""retorno"": {
      ""codRetorno"": 0,
      ""desMensagemSistema"": ""Operação realizada com sucesso."",
      ""desMensagemAmigavel"": """",
      ""lstContrato"": {
        ""objContrato"": [
          {
            ""numContrato"": 5035049,
            ""numApolice"": 101402541679,
            ""numEndosso"": 807899,
            ""numOriContrato"": 1182,
            ""numOperador"": 2,
            ""numMatriculaAgente"": 0,
            ""numApoliceConc"": """",
            ""objPessoa"": {
              ""numPessoa"": 6402416,
              ""nomRazaoSocial"": ""Antonio Lucca da Paz"",
              ""numCPFCNPJ"": ""42991099100"",
              ""objStatus"": {
                ""codStatus"": ""A"",
                ""desStatus"": ""ATIVO""
              },
              ""objStatusTitular"": {
                ""codStatus"": ""S"",
                ""desStatus"": ""TITULAR""
              },
              ""indTpPessoa"": ""F"",
              ""pctPactuacao"": 100.00,
              ""objEndereco"": {
                ""indTipo"": 1,
                ""desTipo"": ""IMOVEL"",
                ""indTipoImovel"": 1,
                ""desTipoImovel"": ""RESIDENCIAL URBANO"",
                ""numCEP"": ""79096368"",
                ""desLogradouro"": ""Rua Guajuvirai"",
                ""numEndereco"": 471,
                ""desComplemento"": ""Rua Guajuvirai"",
                ""nomBairro"": ""Rancho Alegre I"",
                ""nomCidade"": ""Campo Grande"",
                ""codUF"": ""MS""
              },
              ""objDadosContato"": {
                ""desTelefonePes"": ""PESSOAL"",
                ""desTelefoneRec"": ""RESIDENCIAL"",
                ""desTelefoneCom"": ""CONTATO""
              },
              ""dthNascimento"": ""1947-01-01"",
              ""indSexo"": ""M""
            },
            ""objProduto"": {
              ""codProduto"": 1409,
              ""numLinha"": 1
            },
            ""objContratoTerceiro"": {
              ""numContrato"": ""844441164494"",
              ""numDigito"": 8,
              ""dthIniVigencia"": ""2016-03-24"",
              ""dthFimVigencia"": ""2040-03-25"",
              ""staContrato"": ""A""
            },
            ""objCobertura"": {
              ""numCobertura"": 8,
              ""desCobertura"": ""COBERTURA RD PARA LAR MAIS""
            },
            ""objCertificado"": {
              ""seqCertificado"": 0,
              ""numCertificado"": 0,
              ""dthAssinatura"": ""2016-03-24"",
              ""codSorteio"": """",
              ""staCertificado"": """"
            },
            ""objRamo"": {
              ""codRamo"": 14,
              ""nomRamo"": ""COMPREENSIVO RESIDENCIAL""
            },
            ""objEstipulante"": {
              ""codPessoaEsti"": 6204,
              ""nomRazaoSocialEsti"": ""TESTE RAZAO SOCIAL"",
              ""codPesSubEsti"": 6204
            },
            ""objdadosFinanceiro"": {
              ""vlrImpSegurado"": 120849.72,
              ""codUnidOperacional"": 80,
              ""codBanco"": 104,
              ""nomAgencia"": """",
              ""codMoeda"": 1,
              ""codFonte"": 10,
              ""codAgenciaRelac"": 80,
              ""codAgenciaIden"": 80,
              ""codAgenciaCentral"": 80,
              ""indFaseFina"": 3,
              ""dthIniRef"": ""2024-12-25"",
              ""dthFimRef"": ""2025-01-24"",
              ""vlrIsMip"": 120849.72,
              ""objParcela"": {
                ""qtdParcelaEnca"": 1
              },
              ""objPremio"": {
                ""seqPremio"": 482,
                ""vlrPremio"": 5.61
              }
            },
            ""dthAverbacao"": ""2022-01-14""
          }
        ]
      }
    }
  }
}
            ";
        }
    }
}
