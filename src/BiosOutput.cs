using Microsoft.Management.Infrastructure;
using System;

namespace Ghosts.Client.Infrastructure
{
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
                var cimInstance = new CimInstance(@"Win32_BIOS");
                var instance = _session.GetInstance(@"root\cimv2", cimInstance);

                // print out the instance's properties
                Console.WriteLine("BIOS Manufacturer: {0}",instance.CimInstanceProperties["Manufacturer"].Value);
                Console.WriteLine("BIOS Version: {0}", instance.CimInstanceProperties["Version"].Value);
                Console.WriteLine("BIOS Release Date: {0}", instance.CimInstanceProperties["ReleaseDate"].Value);
            }
            catch (CimException ex)
            {
                // handle any errors that occur when querying the remote computer
                Console.WriteLine("An error occurred while querying the remote computer: {0}", ex.Message);
            }
        }
    }
}
