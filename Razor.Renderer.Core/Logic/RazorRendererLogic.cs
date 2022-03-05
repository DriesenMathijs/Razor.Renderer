using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Razor.Renderer.Core.Setup;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Renderer.Core.Logic
{
    internal class RazorRendererLogic
    {
        private IRazorViewEngine RazorViewEngine { get; }
        private ITempDataProvider TempDataProvider { get; }
        private IServiceProvider ServiceProvider { get; }

        public RazorRendererLogic(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            RazorViewEngine = razorViewEngine;
            TempDataProvider = tempDataProvider;
            ServiceProvider = serviceProvider;
        }

        public async Task<string> RenderViewAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model)
        {
            return await RenderViewAsync(viewName, model, new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()));
        }

        public async Task<string> RenderViewAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model, [DisallowNull] ViewDataDictionary viewDataDictionary)
        {
            // Setup the Action Context
            var actionContext = GetActionContext();
            // Fetch the Razor View 
            var view = FindRazorView(actionContext, viewName);


            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(viewDataDictionary, model),
                    new TempDataDictionary(actionContext.HttpContext, TempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private IView FindRazorView(ActionContext actionContext, string viewName)
        {
            // Search for view based on absolute path
            var viewResult = RazorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            
            // If view is found => return it
            if (viewResult.Success)
                return viewResult.View;

            // Search for view based on locations from actionContext
            var viewResultByActionContext = RazorViewEngine.FindView(actionContext, viewName, isMainPage: true);
            
            // If view is found => return it
            if (viewResultByActionContext.Success)
                return viewResultByActionContext.View;

            // Throw an exception with the searched locations for the view & suggestions for the end-user
            var searchedLocations = viewResult.SearchedLocations.Concat(viewResultByActionContext.SearchedLocations);
            var errors = string.Join(
                Environment.NewLine,
                new string[] { $"Could not find view '{viewName}'. We've searched on the following locations" }
                .Concat(searchedLocations)
                .Concat(new string[]
                {
                    $"Suggestions:",
                    $"* Check if you added the Razor Class Library (with the requested views) as a Project Reference",
                    $"* Check the path to the requested View, is it correct/existing?"
                }));

            throw new InvalidOperationException(errors);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = ServiceProvider
            };

            // Make a dummy router to setup an Action Context
            var routeData = new RouteData();
            routeData.Routers.Add(new CustomRouter());
            
            return new ActionContext(httpContext, routeData, new ActionDescriptor());
        }
    }
}
