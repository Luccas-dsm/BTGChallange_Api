using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BTGChallange.Domain.Entidades;
using BTGChallange.Domain.Interfaces;

namespace BTGChallange.Repository.DynamoDb
{
    public class RepositorioLimiteContaDynamoDb : IRepositorioLimiteConta
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly string _nomeTabela = "limites-conta";

        public RepositorioLimiteContaDynamoDb(IAmazonDynamoDB dynamoDb)
        {
            _dynamoDb = dynamoDb;
        }

        public async Task<bool> AtualizarAsync(LimiteContaCorrente limite)
        {
            var chavePrimaria = limite.ObterChaveConta();

            var request = new UpdateItemRequest
            {
                TableName = _nomeTabela,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["ChaveConta"] = new AttributeValue { S = chavePrimaria }
                },
                UpdateExpression = "SET LimitePix = :limitePix, LimiteDisponivel = :limiteDisponivel, AtualizadoEm = :atualizadoEm",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":limitePix"] = new AttributeValue { N = limite.LimitePix.ToString() },
                    [":limiteDisponivel"] = new AttributeValue { N = limite.LimiteDisponivel.ToString() },
                    [":atualizadoEm"] = new AttributeValue { S = DateTime.UtcNow.ToString("O") }
                },
                ConditionExpression = "attribute_exists(ChaveConta)"
            };

            try
            {
                await _dynamoDb.UpdateItemAsync(request);
                return true;
            }
            catch (ConditionalCheckFailedException)
            {
                return false; // Conta não existe
            }
        }

        public async Task<bool> CadastrarAsync(LimiteContaCorrente limite)
        {
            var item = ConverterParaItem(limite);

            var request = new PutItemRequest
            {
                TableName = _nomeTabela,
                Item = item,
                ConditionExpression = "attribute_not_exists(ChaveConta)"
            };

            try
            {
                await _dynamoDb.PutItemAsync(request);
                return true;
            }
            catch (ConditionalCheckFailedException)
            {
                return false; // Conta já existe
            }
        }

        public async Task<LimiteContaCorrente?> ObterPorContaAsync(string agencia, string conta)
        {
            var chavePrimaria = $"{agencia}#{conta}";

            var request = new GetItemRequest
            {
                TableName = _nomeTabela,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["ChaveConta"] = new AttributeValue { S = chavePrimaria }
                }
            };

            var response = await _dynamoDb.GetItemAsync(request);

            if (!response.IsItemSet)
                return null;

            return ConverterParaEntidade(response.Item);
        }

        public async Task<bool> RemoverAsync(string agencia, string conta)
        {
            var chavePrimaria = $"{agencia}#{conta}";

            var request = new DeleteItemRequest
            {
                TableName = _nomeTabela,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["ChaveConta"] = new AttributeValue { S = chavePrimaria }
                },
                ConditionExpression = "attribute_exists(ChaveConta)"
            };

            try
            {
                await _dynamoDb.DeleteItemAsync(request);
                return true;
            }
            catch (ConditionalCheckFailedException)
            {
                return false; // Conta não existe
            }
        }

        private static Dictionary<string, AttributeValue> ConverterParaItem(LimiteContaCorrente limite)
        {
            return new Dictionary<string, AttributeValue>
            {
                ["ChaveConta"] = new AttributeValue { S = limite.ObterChaveConta() },
                ["Documento"] = new AttributeValue { S = limite.Documento },
                ["Agencia"] = new AttributeValue { S = limite.Agencia },
                ["Conta"] = new AttributeValue { S = limite.Conta },
                ["LimitePix"] = new AttributeValue { N = limite.LimitePix.ToString() },
                ["LimiteDisponivel"] = new AttributeValue { N = limite.LimiteDisponivel.ToString() },
                ["CriadoEm"] = new AttributeValue { S = limite.CriadoEm.ToString("O") },
                ["AtualizadoEm"] = limite.AtualizadoEm.HasValue ?
                    new AttributeValue { S = limite.AtualizadoEm.Value.ToString("O") } :
                    new AttributeValue { NULL = true }
            };
        }

        private static LimiteContaCorrente ConverterParaEntidade(Dictionary<string, AttributeValue> item)
        {
            // Extrair dados básicos
            var documento = item["Documento"].S;
            var agencia = item["Agencia"].S;
            var conta = item["Conta"].S;
            var limitePix = decimal.Parse(item["LimitePix"].N);

            // Criar entidade usando construtor público (mais limpo que reflection)
            var limite = new LimiteContaCorrente(documento, agencia, conta, limitePix);

            // Usar reflection apenas para ajustar propriedades que vieram do banco
            var tipo = typeof(LimiteContaCorrente);

            // Atualizar LimiteDisponivel
            var limiteDisponivelProp = tipo.GetProperty("LimiteDisponivel");
            limiteDisponivelProp?.SetValue(limite, decimal.Parse(item["LimiteDisponivel"].N));

            // Atualizar CriadoEm
            var criadoEmProp = tipo.GetProperty("CriadoEm");
            criadoEmProp?.SetValue(limite, DateTime.Parse(item["CriadoEm"].S));

            // Atualizar AtualizadoEm se existir - CORREÇÃO AQUI
            if (item.ContainsKey("AtualizadoEm"))
            {
                var atualizadoEmAttribute = item["AtualizadoEm"];

                // CORREÇÃO: Tratar bool? corretamente
                if (atualizadoEmAttribute.NULL != true && !string.IsNullOrEmpty(atualizadoEmAttribute.S))
                {
                    var atualizadoEmProp = tipo.GetProperty("AtualizadoEm");
                    atualizadoEmProp?.SetValue(limite, DateTime.Parse(atualizadoEmAttribute.S));
                }
            }

            return limite;
        }
    }
}
