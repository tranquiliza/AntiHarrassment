using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace AntiHarassment.Messaging.NServiceBus
{
    public interface IEndpointBuilder
    {
        /// <summary>
        /// Configure routing of messages/commands
        /// </summary>
        /// <param name="routing">Delegate to access NServiceBus' routing settings.</param>
        IEndpointBuilder ConfigureRouting(Action<RoutingSettings<SqlServerTransport>> routing);

        /// <summary>
        /// Configure persistence for endpoint.
        /// Persistence is needed for endpoints with Sagas.
        /// </summary>
        /// <param name="persistenceConfigure">Optional delegate to override persistence defaults (schema, table-prefix, cache, etc).</param>
        IEndpointBuilder EnablePersistence(Action<PersistenceExtensions<SqlPersistence>> persistenceConfigure = null);
    }
}
