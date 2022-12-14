using System;

namespace Ghosts.Client.Infrastructure
{

    public static class Program
    {
        public static void Main(string[] args)
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
    }
}
