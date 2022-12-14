using Microsoft.Management.Infrastructure;
using System;

namespace Ghosts.Client.Infrastructure
{
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
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine("An error occurred while querying the remote computer: {0}", ex.Message);
            }
        }
    }
}
