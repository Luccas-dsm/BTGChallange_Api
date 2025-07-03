using BTGChallange.Domain.Entidades;
using BTGChallange.Domain.Excecoes;
using FluentAssertions;

namespace BTGChallange.Test
{

    public class LimiteContaCorrenteTests
    {
        [Fact]
        public void CriarLimiteConta_ComDadosValidos_DeveRetornarSucesso()
        {
            // Arrange
            var documento = "12345678901";
            var agencia = "1234";
            var conta = "567890";
            var limite = 1000m;

            // Act
            var limiteConta = new LimiteContaCorrente(documento, agencia, conta, limite);

            // Assert
            limiteConta.Documento.Should().Be(documento);
            limiteConta.Agencia.Should().Be(agencia);
            limiteConta.Conta.Should().Be(conta);
            limiteConta.LimitePix.Should().Be(limite);
            limiteConta.LimiteDisponivel.Should().Be(limite);
        }

        [Theory]
        [InlineData("", "1234", "567890", 1000)]
        [InlineData("12345678901", "", "567890", 1000)]
        [InlineData("12345678901", "1234", "", 1000)]
        [InlineData("12345678901", "1234", "567890", -100)]
        public void CriarLimiteConta_ComDadosInvalidos_DeveLancarExcecao(
            string documento, string agencia, string conta, decimal limite)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new LimiteContaCorrente(documento, agencia, conta, limite));
        }

        [Fact]
        public void PodeProcessarTransacao_ComLimiteSuficiente_DeveRetornarTrue()
        {
            // Arrange
            var limiteConta = new LimiteContaCorrente("12345678901", "1234", "567890", 1000m);

            // Act
            var resultado = limiteConta.PodeProcessarTransacao(500m);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void PodeProcessarTransacao_ComLimiteInsuficiente_DeveRetornarFalse()
        {
            // Arrange
            var limiteConta = new LimiteContaCorrente("12345678901", "1234", "567890", 1000m);

            // Act
            var resultado = limiteConta.PodeProcessarTransacao(1500m);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void ConsumirLimite_ComLimiteSuficiente_DeveDecrementarLimite()
        {
            // Arrange
            var limiteConta = new LimiteContaCorrente("12345678901", "1234", "567890", 1000m);
            var valorTransacao = 300m;

            // Act
            limiteConta.ConsumirLimite(valorTransacao);

            // Assert
            limiteConta.LimiteDisponivel.Should().Be(700m);
            limiteConta.AtualizadoEm.Should().NotBeNull();
        }

        [Fact]
        public void ConsumirLimite_ComLimiteInsuficiente_DeveLancarExcecao()
        {
            // Arrange
            var limiteConta = new LimiteContaCorrente("12345678901", "1234", "567890", 1000m);

            // Act & Assert
            Assert.Throws<LimiteInsuficienteException>(() =>
                limiteConta.ConsumirLimite(1500m));
        }

        [Fact]
        public void AtualizarLimite_ComNovoLimiteMaior_DeveAumentarLimiteDisponivel()
        {
            // Arrange
            var limiteConta = new LimiteContaCorrente("12345678901", "1234", "567890", 1000m);
            limiteConta.ConsumirLimite(300m); // Limite disponível = 700

            // Act
            limiteConta.AtualizarLimite(1500m);

            // Assert
            limiteConta.LimitePix.Should().Be(1500m);
            limiteConta.LimiteDisponivel.Should().Be(1200m); // 700 + (1500 - 1000)
        }

        [Fact]
        public void ObterChaveConta_DeveRetornarFormatoCorreto()
        {
            // Arrange
            var limiteConta = new LimiteContaCorrente("12345678901", "1234", "567890", 1000m);

            // Act
            var chave = limiteConta.ObterChaveConta();

            // Assert
            chave.Should().Be("1234#567890");
        }
    }
}
