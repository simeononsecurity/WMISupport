using Microsoft.Management.Infrastructure;
using System;

namespace Ghosts.Client.Infrastructure
{
    public class ProcessorOutput
    {
        //private readonly CimSession _session;
        private CimInstance _instance;

        public ProcessorOutput(CimSession session)
        {
            //_session = session;
            // use the CimSession to create a CimInstance object representing a WMI instance
            // in this case, we're using the Win32_Processor class to get information about the processor
            _instance = session.GetInstance(@"root\cimv2", new CimInstance(@"Win32_Processor"));
        }

        public void Print()
        {
            // print out the instance's properties
            OutputProperty("Processor Name: ", "Name");
            OutputProperty("Processor Manufacturer:", "Manufacturer");
            OutputProperty("Processor Speed: ", "MaxClockSpeed");
            OutputProperty("Processor Number of Cores: ", "NumberOfCores");
            OutputProperty("Processor Number of Logical Processors: ", "NumberOfLogicalProcessors");
            
        }

        private void OutputProperty(string prefix, string propName)
        {
            Console.WriteLine($"{prefix}{_instance.CimInstanceProperties[propName].Value}");
        }
    }
}
