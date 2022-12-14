using System;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using System.Security;

namespace Ghosts.Client.Infrastructure
{
    public class WmiSupport
    {
        // declare the CimSession as a field of the WmiSupport class
        private CimSession? session = null;
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
            try
            {
                try
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

                    if (session.TestConnection())
                    {
                        Console.WriteLine($"Connection Test Was Successful. Continuing...");
                    }
                    else
                    {
                        Console.WriteLine($"Connection Test Failed!");
                    }
                }
                catch
                {
                    Console.WriteLine(
                        $"Failed to create session with remote host. Please check that the system is configured for WMI connections and double check that your credentials are accurate \n Additionally try adding the system to the trustedhost store for WMI with the following command: \n winrm s winrm/config/client '@{{TrustedHosts = system_name}}'"
                    );
                }

                // output

                try
                {
                    if (null != session)
                    {
                        var operatingSystemOutput = new OperatingSystemOutput(session);
                        operatingSystemOutput.Print();

                        var biosOutput = new BiosOutput(session);
                        biosOutput.Print();

                        var processorOutput = new ProcessorOutput(session);
                        processorOutput.Print();

                        var userlistOutput = new UserListOutput(session);
                        userlistOutput.Print();

                        var networkinfoOutput = new NetworkInfoOutput(session);
                        networkinfoOutput.Print();

                        // produces a lot of output use sparingly
                        // var filesOutput = new FilesOutput(session);
                        // filesOutput.Print();

                        // produces a lot of output use sparingly
                        // var processlistOutput = new ProcessListOutput(session);
                        // processlistOutput.Print();

                        // troubleshoot locally on system using powershell with:
                        // (Get-CimInstance -ClassName Win32_Directory -Property *).CimInstanceProperties
                        // https://powershell.one/wmi/root/cimv2/win32_directory
                    }
                }
                catch (CimException ex)
                {
                    // handle any errors that occur when connecting to the remote computer
                    Console.WriteLine(
                        "An error occurred while connecting to the remote computer: {0}",
                        ex.Message
                    );
                    throw;
                }
            }
            catch
            {
                // handle any errors that occur when connecting to the remote computer
                Console.WriteLine("Failed to inialize WmiSupport.Connect()");
                throw;
            }
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter the computer name: ");
                var icomputerName = Console.ReadLine();

                Console.WriteLine("Enter the domain: ");
                var idomain = Console.ReadLine();

                Console.WriteLine("Enter the username: ");
                var iusername = Console.ReadLine();

                Console.WriteLine("Enter the password: ");
                var ipassword = Console.ReadLine();

                // create a WmiSupport object with the computer name, username, and password
                var wmiSupport = new WmiSupport(icomputerName, idomain, iusername, ipassword);

                // connect to the target computer and print out the information
                wmiSupport.Connect();
            }
            catch (CimException ex)
            {
                // handle any errors that occur when connecting to the remote computer
                Console.WriteLine(
                    "An error occurred while connecting to the remote computer: {0}",
                    ex.Message
                );
            }
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
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_OperatingSystem class to get information about the operating system
                var cimInstance = new CimInstance(@"Win32_OperatingSystem");
                var instance = _session.GetInstance(@"root\cimv2", cimInstance);

                // print out the instance's properties
                if (null != instance.CimInstanceProperties["Name"].Value)
                {
                    Console.WriteLine(
                        "Operating System Name: {0}",
                        instance.CimInstanceProperties["Name"].Value
                    );
                }
                if (null != instance.CimInstanceProperties["Version"].Value)
                {
                    Console.WriteLine(
                        "Operating System Version: {0}",
                        instance.CimInstanceProperties["Version"].Value
                    );
                }
                if (null != instance.CimInstanceProperties["InstallDate"].Value)
                {
                    Console.WriteLine(
                        "Operating System Install Date: {0}",
                        instance.CimInstanceProperties["InstallDate"].Value
                    );
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on OperatingSystemOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
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
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_BIOS class to get information about the BIOS
                var instances = _session.EnumerateInstances(@"root\cimv2", "Win32_BIOS");

                // print out the instance's properties
                foreach (var instance in instances)
                {
                    if (null != instance.CimInstanceProperties["Version"].Value)
                    {
                        Console.WriteLine(
                            "BIOS Version: {0}",
                            instance.CimInstanceProperties["Version"].Value
                        );
                    }
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on BiosOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
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
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_Processor class to get information about the processor
                var instances = _session.EnumerateInstances(@"root\cimv2", "Win32_Processor");

                // print out the instance's properties
                foreach (var instance in instances)
                {
                    if (null != instance.CimInstanceProperties["Name"].Value)
                    {
                        Console.WriteLine(
                            "Processor Name: {0}",
                            instance.CimInstanceProperties["Name"].Value
                        );
                    }
                    if (null != instance.CimInstanceProperties["Manufacturer"].Value)
                    {
                        Console.WriteLine(
                            "Processor Manufacturer: {0}",
                            instance.CimInstanceProperties["Manufacturer"].Value
                        );
                    }
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on ProcessorOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
        }
    }

    public class UserListOutput
    {
        private readonly CimSession _session;

        public UserListOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_UserAccount class to get information about users on the system
                var instances = _session.EnumerateInstances(@"root\cimv2", "Win32_UserAccount");

                // print out the list of users on the system
                Console.WriteLine("List of users on the system:");
                foreach (var instance in instances)
                {
                    if (null != instance.CimInstanceProperties["Name"].Value)
                    {
                        Console.WriteLine(
                            " - User Name: {0}",
                            instance.CimInstanceProperties["Name"].Value
                        );
                    }
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on UserListOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
        }
    }

    public class NetworkInfoOutput
    {
        private readonly CimSession _session;

        public NetworkInfoOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_NetworkAdapter class to get information about network devices on the system
                var instances = _session.EnumerateInstances(@"root\cimv2", "Win32_NetworkAdapter");

                // print out the network information for each network device
                Console.WriteLine("Network information for each network device:");
                foreach (var instance in instances)
                {
                    if (null != instance.CimInstanceProperties["Name"].Value)
                    {
                        Console.WriteLine(
                            "Network Device: {0}",
                            instance.CimInstanceProperties["Name"].Value
                        );
                    }
                    if (null != instance.CimInstanceProperties["MACAddress"].Value)
                    {
                        Console.WriteLine(
                            " - MAC Address: {0}",
                            instance.CimInstanceProperties["MACAddress"].Value
                        );
                    }
                    if (null != instance.CimInstanceProperties["NetworkAddresses"].Value)
                    {
                        Console.WriteLine(
                            " - IP Address: {0}",
                            instance.CimInstanceProperties["NetworkAddresses"].Value
                        );
                    }
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on NetworkListOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
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
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_Directory class to get information about a directory
                var instances = _session.EnumerateInstances(@"root\cimv2", "Win32_Directory");

                // print out the list of users on the system
                Console.WriteLine("List of directories on the system:");
                foreach (var instance in instances)
                {
                    if (null != instance.CimInstanceProperties["Name"].Value)
                    {
                        Console.WriteLine(
                            " - Directory Name: {0}",
                            instance.CimInstanceProperties["Name"].Value
                        );
                    }
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on FilesOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
        }
    }

    public class ProcessListOutput
    {
        private readonly CimSession _session;

        public ProcessListOutput(CimSession session)
        {
            _session = session;
        }

        public void Print()
        {
            try
            {
                // use the CimSession to create a CimInstance object representing a WMI instance
                // in this case, we're using the Win32_Process class to get information about processes running on the system
                var instances = _session.EnumerateInstances(@"root\cimv2", "Win32_Process");

                // print out the list of processes
                Console.WriteLine("List of processes running on the system:");
                foreach (var instance in instances)
                {
                    if (null != instance.CimInstanceProperties["Name"].Value)
                    {
                        Console.WriteLine(
                            " - Process Name: {0}",
                            instance.CimInstanceProperties["Name"].Value
                        );
                    }
                    if (null != instance.CimInstanceProperties["Name"].Value)
                    {
                        Console.WriteLine(
                            " - Process ID: {0}",
                            instance.CimInstanceProperties["ProcessId"].Value
                        );
                    }
                }
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine(
                    "Failed on ProcessListOutput: An error occurred while querying the remote computer: {0}",
                    ex.Message
                );
            }
        }
    }
}
