namespace CopyFlanders.Classes
{
    public class Logon
    {
        private string usuario;

        public string Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }

        private string senha;

        public string Senha
        {
            get { return senha; }
            set { senha = value; }
        }

        private string dominio;

        public string Dominio
        {
            get { return dominio; }
            set { dominio = value; }
        }
    }
}
