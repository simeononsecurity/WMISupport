using Microsoft.Management.Infrastructure;
using System;

namespace Ghosts.Client.Infrastructure
{
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
