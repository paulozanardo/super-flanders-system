using System;

namespace CopyFlanders.ImpersonalHelper

{
    public interface IImpersonate
    {
        void Logon(string userName, string domainName, string password);
        bool LogonUser(string userName, string domainName, string password);
        void LogOut();
        event EventHandler ActionEvent;
    }
}