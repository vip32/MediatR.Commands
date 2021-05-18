namespace MediatR.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Humanizer;
    using Microsoft.AspNetCore.Mvc;
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

            AddDefaultResponseSchemas(context);

            foreach (var registrations in this.configuration.Registrations
                .Where(e => !e.Pattern.IsNullOrEmpty()).GroupBy(e => e.Pattern))
            {
                AddPathItem(context.Document.Paths, registrations.DistinctBy(r => r.Method), context);
            }
        }

        private static void AddDefaultResponseSchemas(DocumentProcessorContext context)
        {
            if (!ResponseSchemas.ContainsKey(nameof(ProblemDetails)))
            {
                ResponseSchemas.Add(nameof(ProblemDetails), context.SchemaGenerator.Generate(typeof(ProblemDetails), context.SchemaResolver));
            }

            //if (!ResponseSchemas.ContainsKey(nameof(ValidationProblemDetails)))
            //{
            //    ResponseSchemas.Add(nameof(ValidationProblemDetails), context.SchemaGenerator.Generate(typeof(ValidationProblemDetails), context.SchemaResolver));
            //}
        }

        private static void AddPathItem(IDictionary<string, OpenApiPathItem> items, IEnumerable<CommandEndpointRegistration> registrations, DocumentProcessorContext context)
        {
            var item = new OpenApiPathItem();

            foreach (var registration in registrations)
            {
                var method = registration.Method.ToString().ToLower();
                var operation = new NSwag.OpenApiOperation
                {
                    Description = registration.OpenApi.Description,
                    Summary = registration.OpenApi.Summary,
                    OperationId = registration.RequestType.Name, //GetStringSha256Hash($"{method} {registration.Pattern}"),
                    Tags = new[] { !registration.OpenApi.GroupName.IsNullOrEmpty() ? $"{registration.OpenApi.GroupPrefix} {registration.OpenApi.GroupName}".Trim() : registration.OpenApi.GroupPrefix }.ToList(),
                    Produces = !registration.OpenApi.Produces.IsNullOrEmpty() ? registration.OpenApi.Produces.Split(';').Distinct().ToList() : new[] { "application/json" }.ToList()
                    //RequestBody = new OpenApiRequestBody{}
                };

                item.Add(method, operation);

                var hasResponseModel = registration.Response?.IgnoreResponseBody == false && registration.ResponseType != typeof(Unit) && registration.ResponseType?.Name.SafeEquals("object") == false;
                var description = hasResponseModel ? registration.ResponseType.PrettyName() : string.Empty;
                var schema = context.SchemaGenerator.Generate(registration.ResponseType, context.SchemaResolver);
                var schemaKey = registration.ResponseType.PrettyName();

                // reuse some previously generated schemas, so schema $refs are avoided
                if (ResponseSchemas.ContainsKey(schemaKey))
                {
                    schema = ResponseSchemas[schemaKey];
                }
                else
                {
                    ResponseSchemas.Add(schemaKey, schema);
                }

                if (registration.Response == null)
                {
                    operation.Responses.Add(((int)HttpStatusCode.OK).ToString(), new OpenApiResponse
                    {
                        Description = description,
                        Schema = hasResponseModel ? schema : null,
                        //Examples = hasResponseModel ? Factory.Create(registration.ResponseType) : null // header?
                    });
                }
                else
                {
                    operation.Responses.Add(((int)registration.Response.OnSuccessStatusCode).ToString(), new OpenApiResponse
                    {
                        Description = description,
                        Schema = hasResponseModel ? schema : null,
                        //Examples = hasResponseModel ? Factory.Create(registration.ResponseType) : null // header?
                    });
                }

                operation.Responses.Add(((int)HttpStatusCode.BadRequest).ToString(), new OpenApiResponse
                {
                    Description = nameof(ProblemDetails),
                    Schema = ResponseSchemas[nameof(ProblemDetails)]
                });
                operation.Responses.Add(((int)HttpStatusCode.InternalServerError).ToString(), new OpenApiResponse
                {
                    Description = nameof(ProblemDetails),
                    Schema = ResponseSchemas[nameof(ProblemDetails)]
                });

                AddResponseHeaders(operation, registration);
                AddOperationParameters(operation, method, registration, context);
            }

            if (item.Any() && !items.ContainsKey(registrations.First().Pattern))
            {
                items?.Add(registrations.First().Pattern, item);
            }
        }

        private static void AddResponseHeaders(NSwag.OpenApiOperation operation, CommandEndpointRegistration registration)
        {
            foreach (var response in operation.Responses)
            {
                if (registration.RequestType.GetInterfaces().Contains(typeof(ICommand)))
                {
                    response.Value.Headers.Add("X-CommandId", new OpenApiHeader { Type = JsonObjectType.String });
                }
                else if (registration.RequestType.GetInterfaces().Contains(typeof(IQuery)))
                {
                    response.Value.Headers.Add("X-QueryId", new OpenApiHeader { Type = JsonObjectType.String });
                }
            }
        }

        private static void AddOperationParameters(NSwag.OpenApiOperation operation, string method, CommandEndpointRegistration registration, DocumentProcessorContext context)
        {
            if (registration.RequestType != null)
            {
                if (method.SafeEquals("get") || method.SafeEquals("delete"))
                {
                    AddQueryOperation(operation, registration);
                }
                else if (method.SafeEquals("post") || method.SafeEquals("put") || method.SafeEquals("patch") || method.SafeEquals(string.Empty))
                {
                    AddQueryOperation(operation, registration, true);
                    AddBodyOperation(operation, registration, context);
                }
                else
                {
                    // TODO: ignore for now, or throw? +log
                }
            }
        }

        private static void AddQueryOperation(NSwag.OpenApiOperation operation, CommandEndpointRegistration registration, bool patternParametersOnly = false)
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

                if (!patternParametersOnly)
                {
                    // always add parameter
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        //Description = "request model", // TODO: define a description per param (dictionary) and use it here
                        Kind = registration.Pattern.Contains($"{{{property.Name}", StringComparison.OrdinalIgnoreCase)
                            ? OpenApiParameterKind.Path
                            : OpenApiParameterKind.Query, // query routes are not really supported!
                        Name = property.Name.Camelize(),
                        Type = type,
                    });
                }
                else
                {
                    // only add parameter if it appears in the route pattern
                    if (registration.Pattern.Contains($"{{{property.Name}", StringComparison.OrdinalIgnoreCase))
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            //Description = "request model", // TODO: define a description per param (dictionary) and use it here
                            Kind = OpenApiParameterKind.Path,
                            Name = property.Name.Camelize(),
                            Type = type,
                        });
                    }
                }
            }
        }

        private static void AddBodyOperation(NSwag.OpenApiOperation operation, CommandEndpointRegistration registration, DocumentProcessorContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Description = registration.OpenApi?.RequestBodyDescription, //registration.RequestType.PrettyName(), //"request model",
                Kind = OpenApiParameterKind.Body,
                Name = registration.RequestType.PrettyName(), //"model",
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