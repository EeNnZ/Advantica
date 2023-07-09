using Advantica.IntegrationSystem.Services;
using Advantica.IntegrationSystem.Protos;

namespace Advantica.IntegrationSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var service = new IntegrationService();
            service.Start();

            Console.ReadLine();
            service.Dispose();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        static async Task<WorkerMessage> GenerateRandomWorker()
        {
            throw new NotImplementedException();
        }

        static async Task<WorkerMessage> ChooseRandomExistingWorker()
        {
            throw new NotImplementedException();
        }

        static async Task UpdateWorker(WorkerMessage workerMessage)
        {

        }

        static async Task CreateWorker(WorkerMessage workerMessage)
        {

        }
    }
}