//namespace MediatR.Commands.OpenApi
//{
//    using System.Collections.Generic;
//    using System.Reflection;
//    using Microsoft.AspNetCore.Mvc.ApiExplorer;
//    using Microsoft.AspNetCore.Mvc.Controllers;
//    using Microsoft.AspNetCore.Mvc.Infrastructure;

//    public class CommandEndpointApiDescriptionGroupCollectionProvider : ApiExplorerApiDescriptionGroupCollectionProvider //IApiDescriptionGroupCollectionProvider
//    {
//        private readonly ICommandEndpointConfiguration configuration;

//        public CommandEndpointApiDescriptionGroupCollectionProvider(
//            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
//            IEnumerable<IApiDescriptionProvider> apiDescriptionProviders,
//            ICommandEndpointConfiguration configuration)
//            : base(actionDescriptorCollectionProvider, apiDescriptionProviders)
//        {
//            this.configuration = configuration;
//        }

//        public override ApiDescriptionGroupCollection ApiDescriptionGroups
//        {
//            get
//            {
//                var groups = base.ApiDescriptionGroups;

//                var apiDescription1 = new ApiDescription
//                {
//                    GroupName = "Commands111",
//                    HttpMethod = "GET",
//                    RelativePath = "/test1",
//                    ActionDescriptor = new ControllerActionDescriptor()
//                    {
//                        DisplayName = "Disp1",
//                        ActionName = "Act",
//                        ControllerName = "Ctr",
//                        ControllerTypeInfo = typeof(EchoCommandHandler).GetTypeInfo(),
//                        MethodInfo = typeof(EchoCommandHandler).GetMethod("Process"),
//                    }
//                };
//                //apiDescription1.SupportedResponseTypes.Add(typeof(Unit));

//                var apiDescription2 = new ApiDescription
//                {
//                    GroupName = "Commands222",
//                    HttpMethod = "GET",
//                    RelativePath = "/test2",
//                    ActionDescriptor = new ControllerActionDescriptor()
//                    {
//                        DisplayName = "Disp2",
//                        ActionName = "Act",
//                        ControllerName = "Ctr",
//                        ControllerTypeInfo = typeof(EchoCommandHandler).GetTypeInfo(),
//                        MethodInfo = typeof(EchoCommandHandler).GetMethod("Process")
//                    }
//                };

//                var group1 = new ApiDescriptionGroup("Group1", new List<ApiDescription>(new[] { apiDescription1 }));
//                var group2 = new ApiDescriptionGroup("Group2", new List<ApiDescription>(new[] { apiDescription2 }));

//                //return new ApiDescriptionGroupCollection(
//                //    groups.Items
//                //        .Concat(new[] { group1})
//                //        .Concat(new[] { group2 }).ToList().AsReadOnly(), 1);

//                return new ApiDescriptionGroupCollection(new[] { group1 }, 1);
//                //return new ApiDescriptionGroupCollection(groups.Items, 1);
//            }
//        }
//    }
//}
