namespace MinimalApi.Domain.ModelViews
{
    public struct Home
    {
        public string Mensagem {get => "Bem vindo a API de veículos -- minimal api";}
        public string Doc { get => "/swagger/index.html"; }
    }
}