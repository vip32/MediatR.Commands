namespace MediatR.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using NSwag;
    using NSwag.Generation.Processors;
    using NSwag.Generation.Processors.Contexts;

    public class CommandEndpointDocumentProcessor : IDocumentProcessor
    {
        private readonly ICommandEndpointConfiguration configuration;

        public CommandEndpointDocumentProcessor(ICommandEndpointConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Process(DocumentProcessorContext context)
        {
            if (this.configuration?.Registrations == null)
            {
                return;
            }

            foreach (var endpoint in this.configuration.Registrations
                .Where(e => !e.Pattern.IsNullOrEmpty()).GroupBy(e => e.Pattern))
            {
                //AddPathItem(context.Document.Paths, registrations.DistinctBy(r => r.RequestMethod), context);
            }
        }

        private static void AddPathItem(IDictionary<string, OpenApiPathItem> items, IEnumerable<CommandEndpointConfiguration> endpoints, DocumentProcessorContext context)
        {
            var item = new OpenApiPathItem();
        }
    }
}