# BTG Challenge - Sistema de Gestão de Limites PIX

## 📋 Sobre o Projeto

Este projeto foi desenvolvido como resposta ao desafio técnico do **BTG Pactual** para a posição de **Desenvolvedor Full Stack - FraudSys**. O sistema gerencia limites de transações PIX, permitindo que analistas de fraude controlem e monitorem as operações financeiras dos clientes do Banco KRT.

### 🎯 Objetivo

- Cadastrar e gerenciar limites PIX por conta
- Validar transações PIX contra limites disponíveis
- Manter histórico de transações
- Garantir segurança e controle nas operações

## 🛠️ Tecnologias Utilizadas

### **Core Framework**

- **.NET 8**
- **ASP.NET Core Web API**

### **Banco de Dados**

- **Amazon DynamoDB**

### **Arquitetura e Padrões**

- **Clean Architecture**
- **Domain Driven Design (DDD)**
- **SOLID Principles**
- **Repository Pattern**

### **Qualidade e Validação**

- **FluentValidation**
- **AutoMapper**
- **XUnit**
- **FluentAssertions** (Asserções mais legíveis nos testes)

### **Documentação e DevOps**

- **Swagger/OpenAPI** -
- **Docker** - Containerização para ambiente local (DynamoDB Local e aplicação .Net)

## 📁 Estrutura do Projeto

```
BTGChallange/
│
├── 📂 BTGChallange.Api/                    # Camada de Apresentação
│   ├── Controllers/                        # Endpoints da API
│   ├── Configuration/                      # Configurações da aplicação
│   ├── Properties/                         # Configurações de build
│   └── Program.cs                          # Ponto de entrada da aplicação
│
├── 📂 BTGChallange.Service/               # Camada de Aplicação
│   ├── DTOs/                              # Objetos de transferência de dados
│   ├── Interfaces/                        # Contratos dos serviços
│   ├── Servicos/                          # Implementação dos casos de uso
│   ├── Validator/                         # Validações de entrada
│   └── Mapper/                            # Mapeamento entre objetos
│
├── 📂 BTGChallange.Domain/                # Camada de Domínio
│   ├── Entidades/                         # Entidades de negócio
│   ├── Interfaces/                        # Contratos dos repositórios
│   └── Excecoes/                          # Exceções específicas do domínio
│
├── 📂 BTGChallange.Repository/            # Camada de Infraestrutura
│   └── DynamoDb/                          # Implementação específica do DynamoDB
│
├── 📂 BTGChallange.Ioc/                   # Injeção de Dependências
│   └── DependecyInjector/                 # Configuração do container IoC
│
├── 📂 BTGChallange.Tests/                 # Testes
│   └── Domain/                            # Testes das entidades de domínio
│
└── docker-compose.yml                     # Ambiente local DynamoDB e aplicação .net
```

## 🏗️ Explicação da Arquitetura

Este modelo de arquitetura foi baseado em um template que eu e um amigo desenvolvemos para facilitar a aplicação das melhores práticas, ele traz uma arquitetura limpa, escalável e testavel facilitando a implementação do DDD.

caso queiram validar/baixar template podem acessar o link da loja da microsoft:
https://marketplace.visualstudio.com/items?itemName=WebApiDDD.WebApiDDD

## 🚀 Funcionalidades Implementadas

### **1. Gestão de Limites**

- ✅ **Cadastrar Limite**: Cria novo limite para uma conta com validação completa
- ✅ **Buscar Limite**: Consulta limite específico por agência/conta
- ✅ **Atualizar Limite**: Modifica limite existente mantendo proporção do disponível
- ✅ **Remover Limite**: Exclui registro de limite do sistema

### **2. Processamento PIX**

- ✅ **Validar Transação**: Verifica se há limite suficiente
- ✅ **Consumir Limite**: Desconta valor do limite em transações aprovadas
- ✅ **Rejeitar Transação**: Não consome limite quando rejeitada
- ✅ **Histórico**: Mantém log de todas as tentativas

### **3. Validações Robustas**

- ✅ **CPF**: Validação completa com dígito verificador
- ✅ **Agência**: Formato padrão de 4 dígitos
- ✅ **Conta**: Formato numérico flexível
- ✅ **Valores**: Limites monetários e casas decimais

## 🏃‍♂️ Como Executar

### **Pré-requisitos**

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [AWS CLI](https://aws.amazon.com/cli/) (opcional)

### **1. Clone o repositório**

```bash
git clone https://github.com/Luccas-dsm/BTGChallange_Api
cd BTGChallange
```

### **2. Execute o docker compose**

```bash
docker-compose up -d
```

### **3. Acesse a documentação**

- **Swagger UI**: https://localhost:8081/swagger

### **4. Execute os testes**

```bash
dotnet test
```

## 📊 Exemplos de Uso

### **Cadastrar Limite**

```json
POST /api/limiteconta/cadastrar
{
  "documento": "12345678901",
  "agencia": "1234",
  "conta": "567890",
  "limitePix": 5000.00
}
```

### **Processar PIX**

```json
POST /api/transacaopix/processar
{
  "agencia": "1234",
  "conta": "567890",
  "valor": 1500.00
}
```

## 🧪 Testes

O projeto inclui testes unitários focados nas regras de negócio:

```bash
# Executar todos os testes
dotnet test

```

#### 👨‍💻 Desenvolvido por Luccas da Silva Machado

---

_Desafio técnico BTG Pactual - Sistema de Gestão de Limites PIX_
