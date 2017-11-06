using System;
using Microsoft.Practices.Unity;

namespace CopyFlanders.ImpersonalHelper
{

    /// <summary>
    /// Efetua o login no windows com o usuario que for informado.
    /// É utilizado para gravar arquivos em pastas protegidas pelo sistema.
    /// ** Utilize sempre com using. Exemplo:  using (new ImpersonateHelper(usuario,dominio,senha)) {... implementacao ... } ***
    /// ** quando o dispose do using for executado, ele efetuara um logout e voltara ao usuário da maquina.
    /// </summary>
    public class ImpersonateHelper : IDisposable
    {
        private readonly IImpersonate impersonate;

        public ImpersonateHelper(EventHandler actionEvent)
        {
            impersonate = Container.Resolve<IImpersonate>();
            impersonate.ActionEvent += actionEvent;
        }

        public void Logon(string userName, string domain, string password)
        {
            impersonate.Logon(userName, domain, password);
        }

        public bool LogonUser(string userName, string domain, string password)
        {
            return impersonate.LogonUser(userName, domain, password);
        }

        public ImpersonateHelper(String userName,
            String domain,
            String password)
        {
            Container.RegisterType<IImpersonate, Impersonate>();
            impersonate = Container.Resolve<IImpersonate>();
            impersonate.Logon(userName, domain, password);
        }

        public void Dispose()
        {
            impersonate.LogOut();
        }

        internal static readonly UnityContainer container = new UnityContainer();

        public static UnityContainer Container
        {
            get
            {
                return container;
            }
        }
    }
}