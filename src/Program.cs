using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using System.Security;

namespace Ghosts.Client.Infrastructure
{
    public class WmiSupport
    {
        // declare the CimSession as a field of the WmiSupport class
        private CimSession session;
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
        }

        public void Connect()
        {
            // create Credentials
            CimCredential Credentials = new CimCredential(
                PasswordAuthenticationMechanism.Default,
                _domain,
                _username,
                _securepassword
            );

            // create SessionOptions using Credentials
            WSManSessionOptions SessionOptions = new WSManSessionOptions();
            SessionOptions.AddDestinationCredentials(Credentials);

            // create Session using computer, SessionOptions
            // use the field declared above to store the CimSession in the WmiSupport object
            session = CimSession.Create(_computerName, SessionOptions);

            // // create a connection options object with the target machine's name
            // var options = new CimSessionOptions { Host = _computerName };

            // // create a CimCredential object with the username and password
            // var credentials = new CimCredential(_username, _password);

            // // add the credentials to the CimSessionOptions object
            // options.AddCredential(credentials);

            // // create a CimSession with the target machine's name
            // var session = CimSession.Create(_computerName, options);

            // output

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

    public static class Program
    {
        public static void Main(string[] args)
        {
            // create a WmiSupport object with the computer name, username, and password
            // you can pass these values as arguments to the Main method or hard-code them here
            var wmiSupport = new WmiSupport("<computer-name>", "<username>", "<password>");

            // connect to the target computer and print out the information
            wmiSupport.Connect();
        }
    }

    public class OperatingSystemOutput
    {
        private readonly CimSession _session;

        public OperatingSystemOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            // use the CimSession to create a CimInstance object representing a WMI instance
            // in this case, we're using the Win32_OperatingSystem class to get information about the operating system
            var cimInstance = new CimInstance(@"Win32_OperatingSystem");
            var instance = _session.GetInstance(@"root\cimv2", cimInstance);

            // print out the instance's properties
            Console.WriteLine(
                "Operating System Name: {0}",
                instance.CimInstanceProperties["Name"].Value
            );
            Console.WriteLine(
                "Operating System Version: {0}",
                instance.CimInstanceProperties["Version"].Value
            );
            Console.WriteLine(
                "Operating System Install Date: {0}",
                instance.CimInstanceProperties["InstallDate"].Value
            );
        }
    }

    public class BiosOutput
    {
        private readonly CimSession _session;

        public BiosOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            // use the CimSession to create a CimInstance object representing a WMI instance
            // in this case, we're using the Win32_BIOS class to get information about the BIOS
            var cimInstance = new CimInstance(@"Win32_BIOS");
            var instance = _session.GetInstance(@"root\cimv2", cimInstance);

            // print out the instance's properties
            Console.WriteLine(
                "BIOS Manufacturer: {0}",
                instance.CimInstanceProperties["Manufacturer"].Value
            );
            Console.WriteLine("BIOS Version: {0}", instance.CimInstanceProperties["Version"].Value);
            Console.WriteLine(
                "BIOS Release Date: {0}",
                instance.CimInstanceProperties["ReleaseDate"].Value
            );
        }
    }

    public class ProcessorOutput
    {
        private readonly CimSession _session;

        public ProcessorOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            // use the CimSession to create a CimInstance object representing a WMI instance
            // in this case, we're using the Win32_Processor class to get information about the processor
            var cimInstance = new CimInstance(@"Win32_Processor");
            var instance = _session.GetInstance(@"root\cimv2", cimInstance);

            // print out the instance's properties
            Console.WriteLine("Processor Name: {0}", instance.CimInstanceProperties["Name"].Value);
            Console.WriteLine(
                "Processor Manufacturer: {0}",
                instance.CimInstanceProperties["Manufacturer"].Value
            );
            Console.WriteLine(
                "Processor Speed: {0}",
                instance.CimInstanceProperties["MaxClockSpeed"].Value
            );
            Console.WriteLine(
                "Processor Number of Cores: {0}",
                instance.CimInstanceProperties["NumberOfCores"].Value
            );
            Console.WriteLine(
                "Processor Number of Logical Processors: {0}",
                instance.CimInstanceProperties["NumberOfLogicalProcessors"].Value
            );
        }
    }

    public class FilesOutput
    {
        private readonly CimSession _session;

        public FilesOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            // use the CimSession to create a CimInstance object representing a WMI instance
            // in this case, we're using the Win32_Directory class to get information about a directory
            var cimInstance = new CimInstance(@"Win32_Directory");
            var instance = _session.GetInstance(@"root\cimv2", cimInstance);

            // print out the instance's properties
            Console.WriteLine("Directory Name: {0}", instance.CimInstanceProperties["Name"].Value);
            Console.WriteLine(
                "Directory AccessMask: {0}",
                instance.CimInstanceProperties["AccessMask"].Value
            );
            Console.WriteLine(
                "Directory Compressed: {0}",
                instance.CimInstanceProperties["Compressed"].Value
            );
        }
    }
}
