using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CopyFlanders
{
    internal class Key
    {
        public void Main()
        {
            try
            {
                // Create a key and save it in a container.
             //   GenKey_SaveInContainer("MyKeyContainer");

                // Retrieve the key from the container.
               // GetKeyFromContainer("MyKeyContainer");

               /* // Delete the key from the container.
                DeleteKeyFromContainer("MyKeyContainer");

                // Create a key and save it in a container.
                GenKey_SaveInContainer("MyKeyContainer");

                // Delete the key from the container.
                DeleteKeyFromContainer("MyKeyContainer");*/
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public RSACryptoServiceProvider GenKey_SaveInContainer(string containerName)
        {
            //CRIAÇÃO DO CONTAINER
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = containerName;
            
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            return rsa;
        }

        public RSACryptoServiceProvider GetKeyFromContainer(string containerName)
        {
            // Create the CspParameters object and set the key container 
            // name used to store the RSA key pair.
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = containerName;

            // Create a new instance of RSACryptoServiceProvider that accesses
            // the key container MyKeyContainerName.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);
            return rsa;

            // Display the key information to the console.
            //  Console.WriteLine("Key retrieved from container : \n {0}", rsa.ToXmlString(true));
        }

        public void DeleteKeyFromContainer(string containerName)
        {
            // Create the CspParameters object and set the key container 
            // name used to store the RSA key pair.
            CspParameters cp = new CspParameters();
            cp.KeyContainerName = containerName;

            // Create a new instance of RSACryptoServiceProvider that accesses
            // the key container.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

            // Delete the key entry in the container.
            rsa.PersistKeyInCsp = false;

            // Call Clear to release resources and delete the key from the container.
            rsa.Clear();
        }
    }
}
