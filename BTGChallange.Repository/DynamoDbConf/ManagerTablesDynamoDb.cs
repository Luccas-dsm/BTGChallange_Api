using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace BTGChallange.Repository.DynamoDbConf
{
    public class ManagerTablesDynamoDb
    {

        private readonly AmazonDynamoDBClient _client;

        public ManagerTablesDynamoDb(AmazonDynamoDBClient client)
        {
            _client = client;

            EnsureTableAsync("limites-conta", "ChaveConta").Wait();
            EnsureTableAsync("transacoes-pix", "Id").Wait();
        }

        async Task EnsureTableAsync(string tableName, string primaryKeyName)
        {
            var tables = await _client.ListTablesAsync();
            if (tables.TableNames.Contains(tableName))
            {
                Console.WriteLine($"Tabela {tableName} já existe.");
                return;
            }

            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = primaryKeyName, AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = primaryKeyName, KeyType = "HASH" }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            if (tableName == "transacoes-pix")
            {
                request.AttributeDefinitions.AddRange(new[]
                {
                    new AttributeDefinition { AttributeName = "ChaveConta", AttributeType = "S" },
                    new AttributeDefinition { AttributeName = "DataTransacao", AttributeType = "S" }
                });

                request.GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "ChaveConta-DataTransacao-Index",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement { AttributeName = "ChaveConta", KeyType = "HASH" },
                            new KeySchemaElement { AttributeName = "DataTransacao", KeyType = "RANGE" }
                        },
                        Projection = new Projection { ProjectionType = "ALL" },
                        ProvisionedThroughput = new ProvisionedThroughput
                        {
                            ReadCapacityUnits = 5,
                            WriteCapacityUnits = 5
                        }
                    }
                };
            }

            await _client.CreateTableAsync(request);
            Console.WriteLine($"Tabela {tableName} criada com chave '{primaryKeyName}'.");
        }
    }

}
