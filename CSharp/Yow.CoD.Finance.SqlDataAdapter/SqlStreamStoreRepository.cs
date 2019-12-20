using Newtonsoft.Json;
using SqlStreamStore;
using SqlStreamStore.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yow.CoD.Finance.Domain.Contracts;
using Yow.CoD.Finance.Domain.Model;
using Yow.CoD.Finance.Domain.Services;

namespace Yow.CoD.Finance.SqlDataAdapter
{
    public class SqlStreamStoreRepository : IRepository<Loan>
    {
        private const int ReadPageSize = 100;
        private readonly MsSqlStreamStoreSettings settings;

        public SqlStreamStoreRepository(string connectionString)
        {
            settings = new MsSqlStreamStoreSettings(connectionString);
        }

        public async Task<Loan> Get(Guid id)
        {
            var loan = new Loan(id);
            var streamId = GetStreamId(id);
            var eventStore = new MsSqlStreamStore(settings);

            var position = 0;
            ReadStreamPage page;
            do
            {
                page = await eventStore.ReadStreamForwards(streamId, position, ReadPageSize);
                foreach (var message in page.Messages)
                {
                    var evt = await Deserialize(message);
                    loan.ApplyEvent(evt);
                }
                position = page.NextStreamVersion;
            } while (!page.IsEnd);
            return loan;
        }

        public async Task Save(Loan loan)
        {
            var streamId = GetStreamId(loan.Id);
            var events = loan.GetUncommittedEvents();
            var expectedVersion = loan.Version - events.Length;
            var eventStore = new MsSqlStreamStore(settings);
            var messages = events.Select(Serialize).ToArray();
            await eventStore.AppendToStream(streamId, expectedVersion == 0 ? ExpectedVersion.NoStream : expectedVersion, messages);

            loan.ClearUncommittedEvents();
        }

        public async Task Ping(CancellationToken cancellationToken = default)
        {
            var eventStore = new MsSqlStreamStore(settings);
            await eventStore.CheckSchema(cancellationToken);
        }

        private static string GetStreamId(Guid loanId)
        {
            return $"Loan-{loanId}";
        }

        private static NewStreamMessage Serialize(Event evt)
        {
            var headers = new Dictionary<string, string>
            {
                {"ClrType", evt.GetType().AssemblyQualifiedName}
            };
            var headerJson = JsonConvert.SerializeObject(headers); ;
            var bodyJson = JsonConvert.SerializeObject(evt);
            var msg = new NewStreamMessage(Guid.NewGuid(), evt.GetType().Name, bodyJson, headerJson);
            return msg;
        }

        private static async Task<Event> Deserialize(StreamMessage streamMessage)
        {
            var headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(streamMessage.JsonMetadata);
            var assemblyQualifiedTypeName = headers["ClrType"];
            var clrType = Type.GetType(assemblyQualifiedTypeName, true);
            var jsonBody = await streamMessage.GetJsonData();
            var payload = (Event)JsonConvert.DeserializeObject(jsonBody, clrType);
            return payload;
        }
    }
}
