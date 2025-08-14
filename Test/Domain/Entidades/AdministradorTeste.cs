using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Dominio.Entidades; // <<< Usando o namespace correto

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorTeste
    {
        [TestMethod]
        public void Administrador_DeveSerCriadoComValoresCorretos()
        {
            //Arrange 
            var emailEsperado = "teste@email.com";
            var senhaEsperada = "senha123";
            var perfilEsperado = "Adm";

            //Act
            var administrador = new Administrador
            {
                Email = emailEsperado,
                Senha = senhaEsperada,
                Perfil = perfilEsperado
            };
            
            //Assert
            Assert.IsNotNull(administrador);
            Assert.AreEqual(emailEsperado, administrador.Email);
            Assert.AreEqual(senhaEsperada, administrador.Senha);
            Assert.AreEqual(perfilEsperado, administrador.Perfil);
        }
    }
}