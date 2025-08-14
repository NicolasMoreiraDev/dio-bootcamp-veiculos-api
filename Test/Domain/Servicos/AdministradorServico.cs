using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;

namespace Test.Servicos
{
    [TestClass]
    public class AdministradorServicoTeste 
    {
        private DbContexto _contexto;
        private IAdministradorServico _servico;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contexto = new DbContexto(options);
            _servico = new AdministradorServico(_contexto);
        }

        [TestMethod]
        public async Task IncluirAsync_DeveSalvarUmAdministradorNoBancoDeDados()
        {
            // Arrange
            var emailEsperado = "teste.salvar@email.com";
            var senhaEsperada = "senhaForte123";
            var perfilEsperado = "Adm";
            
            var administradorParaSalvar = new Administrador
            {
                Email = emailEsperado,
                Senha = senhaEsperada,
                Perfil = perfilEsperado
            };

            // Act
            await _servico.IncluirAsync(administradorParaSalvar);

            // Assert
            var administradorSalvo = await _contexto.Administradores.FirstOrDefaultAsync(a => a.Email == emailEsperado);

            Assert.IsNotNull(administradorSalvo);
            Assert.AreEqual(emailEsperado, administradorSalvo.Email);
            Assert.AreEqual(perfilEsperado, administradorSalvo.Perfil);
            Assert.AreNotEqual(senhaEsperada, administradorSalvo.Senha); 
        }
    }
}