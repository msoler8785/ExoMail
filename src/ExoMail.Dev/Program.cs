using System;

namespace ExoMail.Dev
{
    /// <summary>
    /// The dev project has methods for creating test data and seeding databases.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var service = new Service();
            service.Start().Wait();
        }
    }
}
