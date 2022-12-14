using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using System;
using System.Security;

namespace Ghosts.Client.Infrastructure
{
    public class WmiSupport
    {
        private readonly string _computerName;
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private readonly SecureString _securepassword;

        public WmiSupport(string computerName, string domain, string username, string password)
        {
            _computerName = computerName;
            _domain = domain;
            _username = username;
            _password = password;
            _securepassword = new SecureString();
            foreach (char c in _password)
            {
                _securepassword.AppendChar(c);
            }
            // session = new CimSession();
        }

        public void Connect()
        {
            // create Credentials
            CimCredential Credentials = new CimCredential(
                PasswordAuthenticationMechanism.Default, _domain, _username, _securepassword);

            // create SessionOptions using Credentials
            WSManSessionOptions SessionOptions = new WSManSessionOptions();
            SessionOptions.AddDestinationCredentials(Credentials);

            // create Session using computer, SessionOptions
            // use the field declared above to store the CimSession in the WmiSupport object

            /**
             * Using ensures proper disposal of asset once finished. see:
             * https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-statement
             * 
             */
            using var session = CimSession.Create(_computerName, SessionOptions); 

            if (session.TestConnection() == false)
            {
                Console.WriteLine($"Connection Test Failed!");
                return;
            }


            Console.WriteLine($"Connection Test Was Successful. Continuing...");

            var operatingSystemOutput = new OperatingSystemOutput(session);
            operatingSystemOutput.Print();

            var biosOutput = new BiosOutput(session);
            biosOutput.Print();

            var processorOutput = new ProcessorOutput(session);
            processorOutput.Print();

            var filesOutput = new FilesOutput(session);
            filesOutput.Print();
        }
    }
}
