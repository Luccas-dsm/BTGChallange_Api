using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BTGChallange.Domain.Entidades;
using BTGChallange.Domain.Interfaces;

namespace BTGChallange.Repository.DynamoDb
{
    public class RepositorioTransacaoPixDynamoDb : IRepositorioTransacaoPix
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _nomeTabela = "transacoes-pix";

        public RepositorioTransacaoPixDynamoDb(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<TransacaoPix?> ObterPorIdAsync(string id)
        {
            var request = new GetItemRequest
            {
                TableName = _nomeTabela,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["Id"] = new AttributeValue { S = id }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);

            return response.IsItemSet ? ConverterParaEntidade(response.Item) : null;
        }

        public async Task<List<TransacaoPix>> ObterTransacoesPorContaAsync(string agencia, string conta)
        {
            var chaveConta = $"{agencia}#{conta}";

            var request = new QueryRequest
            {
                TableName = _nomeTabela,
                IndexName = "ChaveConta-DataTransacao-Index", // GSI
                KeyConditionExpression = "ChaveConta = :chaveConta",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":chaveConta"] = new AttributeValue { S = chaveConta }
                },
                ScanIndexForward = false // Ordenar por data decrescente (mais recentes primeiro)
            };

            try
            {
                var response = await _dynamoDb.QueryAsync(request);
                return response.Items.Select(ConverterParaEntidade).ToList();
            }
            catch (Exception)
            {
                return new List<TransacaoPix>();
            }
        }

        public async Task<bool> SalvarTransacaoAsync(TransacaoPix transacao)
        {
            var item = ConverterParaItem(transacao);

            var request = new PutItemRequest
            {
                TableName = _nomeTabela,
                Item = item
            };

            try
            {
                await _dynamoDb.PutItemAsync(request);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static Dictionary<string, AttributeValue> ConverterParaItem(TransacaoPix transacao)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                ["Id"] = new AttributeValue { S = transacao.Id },
                ["ChaveConta"] = new AttributeValue { S = $"{transacao.Agencia}#{transacao.Conta}" },
                ["Agencia"] = new AttributeValue { S = transacao.Agencia },
                ["Conta"] = new AttributeValue { S = transacao.Conta },
                ["Valor"] = new AttributeValue { N = transacao.Valor.ToString() },
                ["DataTransacao"] = new AttributeValue { S = transacao.DataTransacao.ToString("O") },
                ["Aprovada"] = new AttributeValue { BOOL = transacao.Aprovada }
            };

            // Adicionar motivo da rejeição se existir
            if (!string.IsNullOrEmpty(transacao.MotivoRejeicao))
            {
                item["MotivoRejeicao"] = new AttributeValue { S = transacao.MotivoRejeicao };
            }

            return item;
        }

        private static TransacaoPix ConverterParaEntidade(Dictionary<string, AttributeValue> item)
        {
            // Extrair dados básicos
            var agencia = item["Agencia"].S;
            var conta = item["Conta"].S;
            var valor = decimal.Parse(item["Valor"].N);

            // Criar entidade usando construtor público
            var transacao = new TransacaoPix(agencia, conta, valor);

            // Usar reflection para ajustar propriedades que vieram do banco
            var tipo = typeof(TransacaoPix);

            // Atualizar Id
            var idProp = tipo.GetProperty("Id");
            idProp?.SetValue(transacao, item["Id"].S);

            // Atualizar DataTransacao
            var dataTransacaoProp = tipo.GetProperty("DataTransacao");
            dataTransacaoProp?.SetValue(transacao, DateTime.Parse(item["DataTransacao"].S));

            // Atualizar status da transação
            var aprovada = item["Aprovada"].BOOL;
            if (aprovada.HasValue && aprovada.Value)
            {
                transacao.Aprovar();
            }
            else
            {
                var motivoRejeicao = item.ContainsKey("MotivoRejeicao") ? item["MotivoRejeicao"].S : "Não informado";
                transacao.Rejeitar(motivoRejeicao);
            }

            return transacao;
        }
    }
}

