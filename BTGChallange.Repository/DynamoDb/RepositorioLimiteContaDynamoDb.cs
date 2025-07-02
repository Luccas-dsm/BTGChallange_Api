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

        public Task<bool> AtualizarAsync(LimiteContaCorrente limite)
        {
            throw new NotImplementedException();
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

        public Task<bool> ContaExisteAsync(string agencia, string conta)
        {
            throw new NotImplementedException();
        }

        public Task<LimiteContaCorrente?> ObterPorContaAsync(string agencia, string conta)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoverAsync(string agencia, string conta)
        {
            throw new NotImplementedException();
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
    }
}
