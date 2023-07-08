using Grpc.Core;
using Advantica.Server.Protos;
using Microsoft.EntityFrameworkCore;

namespace Advantica.Server.Services
{
    public class WorkerService : WorkerIntegration.WorkerIntegrationBase
    {
        private readonly AdvanticaContext _dbContext;
        public WorkerService(AdvanticaContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Writes all workers into response stream.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns>An object that can be used to read stream data.</returns>
        /// <exception cref="RpcException"></exception>
        public override async Task GetWorkerStream(EmptyMessage request, IServerStreamWriter<WorkerAction> responseStream, ServerCallContext context)
        {
            var workers = _dbContext.Workers.Include(nav => nav.Sex);
            if (!workers.Any())
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Database returns empty dataset"));
            }

            foreach (var worker in workers)
            {
                var workerAction = new WorkerAction()
                {
                    ActionType = Advantica.Server.Protos.Action.Read,
                    Worker = new WorkerMessage().FromEntity(worker)
                };
                await responseStream.WriteAsync(workerAction);
            }
        }

        /// <summary>
        /// Returns worker with requested RowId.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>A <c>WorkerAction</c> object that contains requested worker.</returns>
        /// <exception cref="RpcException"></exception>
        public override Task<WorkerAction> GetWorkerById(WorkerRowIdMessage request, ServerCallContext context)
        {
            var targetWorker = _dbContext.Workers
                .Include(nav => nav.Sex)
                .FirstOrDefault(w => w.Id == request.WorkerRowId);
            WorkerAction workerAction;

            if (targetWorker == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"{nameof(targetWorker)} with requested id={request.WorkerRowId} was null"));
            }
            else
            {
                workerAction = new WorkerAction()
                {
                    ActionType = Protos.Action.Read,
                    Worker = new WorkerMessage().FromEntity(targetWorker)
                };
            }

            return Task.FromResult(workerAction);
        }

        /// <summary>
        /// Implements POST request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>A <c>WorkerAction</c> object that contains posted worker.</returns>
        public override Task<WorkerAction> PostWorker(WorkerAction request, ServerCallContext context)
        {

            var worker = request.Worker.ToEntity();

            /*Set worker.Id to zero to prevent passing it to db directly
             and allow db generate it by itself*/
            //TODO: Fix passing id directly
            worker.Id = 0;

            var result = _dbContext.Workers.Add(worker);
            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "Result of DbContext.Add() was null"));
            }

            int affectedCount = _dbContext.SaveChanges();
            if (affectedCount > 0)
            {
                context.Status = new Status(StatusCode.OK, $"{affectedCount} rows were added");
            }
            if (affectedCount == 0)
            {
                context.Status = new Status(StatusCode.AlreadyExists, "Same worker already exists");
            }

            var workerActionResult = new WorkerAction()
            {
                ActionType = Protos.Action.Create,
                Worker = new WorkerMessage().FromEntity(result.Entity)
            };

            return Task.FromResult(workerActionResult);
        }

        /// <summary>
        /// Updates record with Id given in <c>request.Worker.RowIdMessage.WorkerRowId.</c>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns><c>WorkerAction</c> object that contains updated worker.</returns>
        /// <exception cref="RpcException"></exception>
        public override Task<WorkerAction> PutWorker(WorkerAction request, ServerCallContext context)
        {
            bool exists = _dbContext.Workers.Any(w => w.Id == request.Worker.RowIdMessage.WorkerRowId);
            if (!exists)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Worker with id {request.Worker.RowIdMessage.WorkerRowId} wasn't found"));
            }

            var result = _dbContext.Update(request.Worker.ToEntity());
            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "Result of DbContext.Update() was null"));
            }

            int affectedCount = _dbContext.SaveChanges();
            if (affectedCount > 0)
            {
                context.Status = new Status(StatusCode.OK, $"{affectedCount} rows were updated");
            }

            var workerActionResult = new WorkerAction()
            {
                ActionType = Protos.Action.Update,
                Worker = new WorkerMessage().FromEntity(result.Entity)
            };

            return Task.FromResult(workerActionResult);
        }

        /// <summary>
        /// Deletes worker with requested id from database
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns><c>EmptyMessage</c></returns>
        /// <exception cref="RpcException"></exception>
        public override Task<EmptyMessage> DeleteWorker(WorkerRowIdMessage request, ServerCallContext context)
        {
            var targetWorker = _dbContext.Workers.FirstOrDefault(w => w.Id == request.WorkerRowId);
            if (targetWorker == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Worker with id {request.WorkerRowId} wasn't found"));
            }

            var result = _dbContext.Workers.Remove(targetWorker);
            if (result == null)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "Result of DbContext.Add() was null"));
            }

            int affectedCount = _dbContext.SaveChanges();
            if (affectedCount > 0)
            {
                context.Status = new Status(StatusCode.OK, $"{affectedCount} rows were deleted");
            }

            return Task.FromResult(new EmptyMessage());
        }

    }
}
