using DesafioItau.InvestimentosApp.Common.Models;
using DesafioItau.InvestimentosApp.Domain;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DesafioItau.Tests.Domain
{
    public class CalcularPrecoMedioTest
    {
        [Fact]
        public async Task Calcular_DeveCalcularPrecoMedioCorretamente()
        {
            // Arrange  
            var operacoes = new List<OperacaoCompra>
               {
                   new() { Quantidade = 10, PrecoUnitario = 20.0m }, // Total: 200  
                   new() { Quantidade = 20, PrecoUnitario = 30.0m }  // Total: 600  
               };

            // Act  
            var precoMedio = await CalcularPrecoMedio.Calcular(operacoes);

            // Assert  
            Assert.Equal(26.6667m, Math.Round(precoMedio, 4));
        }

        [Fact]
        public async Task Calcular_DeveLancarExcecao_ListaNula()
        {
            // Act & Assert  
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => CalcularPrecoMedio.Calcular(null!));
            Assert.Equal("operacaoCompras", ex.ParamName);
        }

        [Fact]
        public async Task Calcular_DeveLancarExcecao_ListaVazia()
        {
            // Act & Assert  
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => CalcularPrecoMedio.Calcular([]));
            Assert.Equal("operacaoCompras", ex.ParamName);
        }

        /** 
         Conceito de TesteMutante:
            Teste Mutante é uma técnica de testes automatizados que cria versões alteradas do código original,
            mudando pequenas partes para verificar se os testes existentes conseguem detectar essas alterações. 

        Importância:
            Ele ajuda a garantir a qualidade dos seus testes automatizados, 
            demonstrando se o mesmo é capaz de averiguar erros reais no código.

         **/

        [Fact]
        public async Task Calcular_DeveLancarExcecao_QuantidadeInvalida()
        {
            // Arrange  
            var operacoes = new List<OperacaoCompra>
               {
                   new() { Quantidade = 0, PrecoUnitario = 10.0m }
               };

            // Act & Assert  
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => CalcularPrecoMedio.Calcular(operacoes));
            Assert.Contains("A quantidade deve ser maior que zero", ex.Message);
        }

        [Fact]
        public async Task Calcular_DeveLancarExcecao_PrecoUnitarioNegativo()
        {
            // Arrange  
            var operacoes = new List<OperacaoCompra>
            {
                new() { Quantidade = 10, PrecoUnitario = -5.0m }
            };

            // Act & Assert  
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => CalcularPrecoMedio.Calcular(operacoes));
            Assert.Contains("Preço unitário da operação não pode ser negativo", ex.Message);
        }

        [Fact]
        public async Task Calcular_DeveLancarDivideByZero_SeTodasAsQuantidadesForemZeradas()
        {
            // Arrange  
            var operacoes = new List<OperacaoCompra>
            {
                new() { Quantidade = 0, PrecoUnitario = 5.0m },
                new() { Quantidade = 0, PrecoUnitario = 10.0m }
            };

            // Act & Assert  
            await Assert.ThrowsAsync<ArgumentException>(() => CalcularPrecoMedio.Calcular(operacoes));
        }      
    }
}
