namespace MediatR.Commands
{
    using System;
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

            foreach (var registrations in this.configuration.Registrations
                .Where(e => !e.Pattern.IsNullOrEmpty()).GroupBy(e => e.Pattern))
            {
                AddPathItem(context.Document.Paths, registrations.DistinctBy(r => r.Method), context);
            }
        }

        private static void AddPathItem(IDictionary<string, OpenApiPathItem> items, IEnumerable<CommandEndpointRegistration> endpointRegistrations, DocumentProcessorContext context)
        {
            var item = new OpenApiPathItem();

            foreach (var registration in endpointRegistrations)
            {
                var method = registration.Method.ToString().ToLower();
                var operation = new OpenApiOperation
                {
                    Description = registration.OpenApi.Description,
                    Summary = registration.OpenApi.Summary,
                    OperationId = GetStringSha256Hash($"{method} {registration.Pattern}"),
                    Tags = new[] { !registration.OpenApi.GroupName.IsNullOrEmpty() ? $"{registration.OpenApi.GroupPrefix} ({registration.OpenApi.GroupName})" : registration.OpenApi.GroupPrefix }.ToList(),
                    Produces = !registration.OpenApi.Produces.IsNullOrEmpty() ? registration.OpenApi.Produces.Split(';').Distinct().ToList() : new[] { "application/json" }.ToList()
                    //RequestBody = new OpenApiRequestBody{}
                };

                item.Add(method, operation);
            }
        }

        private static string GetStringSha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                var textData = System.Text.Encoding.UTF8.GetBytes(text);
                var hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}