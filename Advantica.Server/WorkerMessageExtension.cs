using Advantica.Server.Protos;
using System.Runtime.CompilerServices;

namespace Advantica.Server
{
    public static class WorkerMessageExtension
    {
        /// <summary>
        /// Converts <c>Worker</c> entity to <c>WorkerMessage</c> for data transfering.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="worker"></param>
        /// <returns>A <c>WorkerMessage</c> converted from <c>Worker</c> entity object.</returns>
        public static WorkerMessage FromEntity(this WorkerMessage message, Entities.Worker worker)
        {
            WorkerMessage workerMessage = new WorkerMessage()
            {
                RowIdMessage = new WorkerRowIdMessage() { WorkerRowId = worker.Id },
                FirstName = worker.FirstName,
                LastName = worker.LastName,
                MiddleName = worker.MiddleName ?? "",
                Birthday = worker.Birthday.ToBinary(),
                Sex = (Protos.Sex)worker.SexId,
                HasChildren = worker.HasChildren
            };
            return workerMessage;
        }

        /// <summary>
        /// Converts <c>WorkerMessage</c> object to <c>Worker</c> entity for performing database operations.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>A <c>Worker</c> entity object converted from <c>WorkerMessage</c> object.</returns>
        public static Entities.Worker ToEntity(this WorkerMessage message)
        {
            return new Entities.Worker()
            {
                Id = message.RowIdMessage.WorkerRowId,
                FirstName = message.FirstName,
                LastName = message.LastName,
                MiddleName = message.MiddleName ?? "",

                //Use ToUniversalTime() cause of postgres "timestamp with time zone" column type
                Birthday = DateTime.FromBinary(message.Birthday).ToUniversalTime(),
                SexId = (int)message.Sex,
                HasChildren = message.HasChildren
            };
        }
    }
}
