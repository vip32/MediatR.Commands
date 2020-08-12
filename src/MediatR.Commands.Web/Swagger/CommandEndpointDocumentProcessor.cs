namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Humanizer;
    using NJsonSchema;
    using NSwag;
    using NSwag.Generation.Processors;
    using NSwag.Generation.Processors.Contexts;

    public class CommandEndpointDocumentProcessor : IDocumentProcessor
    {
        private static readonly IDictionary<string, JsonSchema> RequestSchemas = new Dictionary<string, JsonSchema>();
        private static readonly IDictionary<string, JsonSchema> ResponseSchemas = new Dictionary<string, JsonSchema>();
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

        private static void AddPathItem(IDictionary<string, OpenApiPathItem> items, IEnumerable<CommandEndpointRegistration> registrations, DocumentProcessorContext context)
        {
            var item = new OpenApiPathItem();

            foreach (var registration in registrations)
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

                var hasResponseModel = registration.ResponseType?.Name.SafeEquals("object") == false;
                var description = registration.OpenApi.Description ?? (hasResponseModel ? registration.ResponseType : null)?.PrettyName();
                var schema = context.SchemaGenerator.Generate(registration.ResponseType, context.SchemaResolver);
                var schemaKey = registration.ResponseType.PrettyName();
                // reuse some previously generated schemas, so schema $refs are avoided
                if (!description.IsNullOrEmpty())
                {
                    if (ResponseSchemas.ContainsKey(schemaKey))
                    {
                        schema = ResponseSchemas[schemaKey];
                    }
                    else
                    {
                        ResponseSchemas.Add(schemaKey, schema);
                    }
                }

                if(registration.Response != null)
                {
                    operation.Responses.Add(((int)registration.Response.OnSuccessStatusCode).ToString(), new OpenApiResponse
                    {
                        Description = description,
                        Schema = hasResponseModel ? schema : null,
                        //Examples = hasResponseModel ? Factory.Create(registration.ResponseType) : null // header?
                    });
                }
                else
                {
                    operation.Responses.Add(200.ToString(), new OpenApiResponse
                    {
                        Description = description,
                        Schema = hasResponseModel ? schema : null,
                        //Examples = hasResponseModel ? Factory.Create(registration.ResponseType) : null // header?
                    });
                }

                operation.Responses.Add(((int)HttpStatusCode.BadRequest).ToString(), new OpenApiResponse
                {
                    Description = string.Empty,
                });
                operation.Responses.Add(((int)HttpStatusCode.InternalServerError).ToString(), new OpenApiResponse
                {
                    Description = string.Empty
                });

                AddResponseHeaders(operation, registration);
                AddOperationParameters(operation, method, registration, context);
            }

            if (item.Any() && !items.ContainsKey(registrations.First().Pattern))
            {
                items?.Add(registrations.First().Pattern, item);
            }
        }

        private static void AddResponseHeaders(OpenApiOperation operation, CommandEndpointRegistration registration)
        {
            foreach (var response in operation.Responses)
            {
                response.Value.Headers.Add("X-CommandId", new JsonSchema { Type = JsonObjectType.String });
            }
        }

        private static void AddOperationParameters(OpenApiOperation operation, string method, CommandEndpointRegistration registration, DocumentProcessorContext context)
        {
            if (registration.RequestType != null)
            {
                if (method.SafeEquals("get") || method.SafeEquals("delete"))
                {
                    AddQueryOperation(operation, registration);
                }
                else if (method.SafeEquals("post") || method.SafeEquals("put") || method.SafeEquals(string.Empty))
                {
                    AddBodyOperation(operation, registration, context);
                }
                else
                {
                    // TODO: ignore for now, or throw? +log
                }
            }
        }

        private static void AddQueryOperation(OpenApiOperation operation, CommandEndpointRegistration registration)
        {
            foreach (var property in registration.RequestType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                // translate commandType properties to many OpenApiParameters
                if (!property.CanWrite || !property.CanRead)
                {
                    continue;
                }

                var type = JsonObjectType.String;
                if (property.PropertyType == typeof(int) || property.PropertyType == typeof(short))
                {
                    type = JsonObjectType.Integer;
                }
                else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(float) || property.PropertyType == typeof(long))
                {
                    type = JsonObjectType.Number;
                }
                else if (property.PropertyType == typeof(bool))
                {
                    type = JsonObjectType.Boolean;
                }
                else if (property.PropertyType == typeof(object))
                {
                    type = JsonObjectType.Object; // TODO: does not work for child objects
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    //Description = "request model",
                    Kind = registration.Pattern.Contains($"{{{property.Name}", StringComparison.OrdinalIgnoreCase)
                        ? OpenApiParameterKind.Path
                        : OpenApiParameterKind.Query,
                    Name = property.Name.Camelize(),
                    Type = type,
                });
            }
        }

        private static void AddBodyOperation(OpenApiOperation operation, CommandEndpointRegistration registration, DocumentProcessorContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                //Description = "request model",
                Kind = OpenApiParameterKind.Body,
                Name = (registration.RequestType ?? typeof(object)).PrettyName(), //"model",
                Type = JsonObjectType.Object,
                Schema = EnsureSchema(registration, context),
                //Example = registration.CommandType != null ? Factory.Create(registration.CommandType) : null //new Commands.Domain.EchoCommand() { Message = "test"},
            });
        }

        private static JsonSchema EnsureSchema(CommandEndpointRegistration registration, DocumentProcessorContext context)
        {
            var schemaKey = registration.RequestType.PrettyName();
            if (RequestSchemas.ContainsKey(schemaKey))
            {
                return RequestSchemas[schemaKey];
            }
            else
            {
                var schema = context.SchemaGenerator.Generate(registration.RequestType, context.SchemaResolver);
                RequestSchemas.Add(schemaKey, schema);
                return schema;
            }

            //var schema = result.AllOf.FirstOrDefault();
            //if (schema != null)
            //{
            //    // workaround: remove invalid first $ref in allof https://github.com/RicoSuter/NSwag/issues/2119
            //    result.AllOf.Remove(schema);
            //}

            // remove some more $refs
            //foreach(var definition in result.Definitions.Safe())
            //{
            //    var s = definition.Value.AllOf.FirstOrDefault();
            //    if(s != null)
            //    {
            //        definition.Value.AllOf.Remove(s);
            //    }
            //}
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