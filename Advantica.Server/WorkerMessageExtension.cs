using Advantica.Server.Protos;

namespace Advantica.Server
{
    public static class WorkerMessageExtension
    {
        /// <summary>
        /// Converts <see cref="Entities.Worker"/> entity to <see cref="Protos.WorkerMessage"/> for data transfering.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="worker"></param>
        /// <returns>A <see cref="Protos.WorkerMessage"/> converted from <see cref="Entities.Worker"/> entity object.</returns>
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
        /// Converts <see cref="Protos.WorkerMessage"/> object to <see cref="Entities.Worker"/> entity for performing database operations.
        /// </summary>
        /// <param name="message"></param>
        /// <returns><see cref="Entities.Worker"/> entity object converted from <see cref="Protos.WorkerMessage"/> object.</returns>
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
