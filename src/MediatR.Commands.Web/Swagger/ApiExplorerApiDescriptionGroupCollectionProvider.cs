//// Copyright (c) .NET Foundation. All rights reserved.
//// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
//// https://raw.githubusercontent.com/aspnet/Mvc/master/src/Microsoft.AspNetCore.Mvc.ApiExplorer/ApiDescriptionGroupCollectionProvider.cs
//#pragma warning disable SA1642 // Constructor summary documentation should begin with standard text
//namespace Microsoft.AspNetCore.Mvc.ApiExplorer
//{
//    using System.Collections.Generic;
//    using System.Linq;
//    using Microsoft.AspNetCore.Mvc.Infrastructure;

//    /// <inheritdoc />
//    public class ApiExplorerApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
//    {
//        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
//        private readonly IApiDescriptionProvider[] apiDescriptionProviders;

//        private ApiDescriptionGroupCollection apiDescriptionGroups;

//        /// <summary>
//        /// Creates a new instance of <see cref="ApiExplorerApiDescriptionGroupCollectionProvider"/>.
//        /// </summary>
//        /// <param name="actionDescriptorCollectionProvider">
//        /// The <see cref="IActionDescriptorCollectionProvider"/>.
//        /// </param>
//        /// <param name="apiDescriptionProviders">
//        /// The <see cref="IEnumerable{IApiDescriptionProvider}"/>.
//        /// </param>
//        public ApiExplorerApiDescriptionGroupCollectionProvider(
//#pragma warning disable SA1114 // Parameter list should follow declaration
//            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
//#pragma warning restore SA1114 // Parameter list should follow declaration
//            IEnumerable<IApiDescriptionProvider> apiDescriptionProviders)
//        {
//            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
//            this.apiDescriptionProviders = apiDescriptionProviders.OrderBy(item => item.Order).ToArray();
//        }

//        /// <inheritdoc />
//        public virtual ApiDescriptionGroupCollection ApiDescriptionGroups
//        {
//            get
//            {
//                var actionDescriptors = this.actionDescriptorCollectionProvider.ActionDescriptors;
//                if (this.apiDescriptionGroups == null || this.apiDescriptionGroups.Version != actionDescriptors.Version)
//                {
//                    this.apiDescriptionGroups = this.GetCollection(actionDescriptors);
//                }

//                return this.apiDescriptionGroups;
//            }
//        }

//        private ApiDescriptionGroupCollection GetCollection(ActionDescriptorCollection actionDescriptors)
//        {
//            var context = new ApiDescriptionProviderContext(actionDescriptors.Items);

//            foreach (var provider in this.apiDescriptionProviders)
//            {
//                provider.OnProvidersExecuting(context);
//            }

//            for (var i = this.apiDescriptionProviders.Length - 1; i >= 0; i--)
//            {
//                this.apiDescriptionProviders[i].OnProvidersExecuted(context);
//            }

//            var groups = context.Results
//                .GroupBy(d => d.GroupName)
//                .Select(g => new ApiDescriptionGroup(g.Key, g.ToArray()))
//                .ToArray();

//            return new ApiDescriptionGroupCollection(groups, actionDescriptors.Version);
//        }
//    }
//}