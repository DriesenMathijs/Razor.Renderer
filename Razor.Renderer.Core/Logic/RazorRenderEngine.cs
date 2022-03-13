using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Razor.Renderer.Core.Logic.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Razor.Renderer.Core.Logic
{
    public class RazorRenderEngine : IRazorRenderEngine
    {
        private static IServiceScopeFactory RenderererServiceScopeFactory;

        /// <inheritdoc />
        public void Setup()
        {
            RenderererServiceScopeFactory = null;
            GetRendererServiceScopeFactory();
        }

        /// <summary>
        /// Get the serviceScopeFactory object from static property cache if it exists else a new one is set-up
        /// </summary>
        /// <returns></returns>
        private IServiceScopeFactory GetRendererServiceScopeFactory()
        {
            if (RenderererServiceScopeFactory is null)
                RenderererServiceScopeFactory = new RazorRendererLogicFactory().CreateRendererServicesScopeFactory();
            
            return RenderererServiceScopeFactory;
        }

        /// <inheritdoc />
        public async Task<string> RenderAsync([DisallowNull] string viewName)
        {
            using (var serviceScope = GetRendererServiceScopeFactory().CreateScope())
            {
                var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorRendererLogic>();
                return await renderer.RenderViewAsync<object>(viewName, default).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model)
        {
            using (var serviceScope = GetRendererServiceScopeFactory().CreateScope())
            {
                var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorRendererLogic>();
                return await renderer.RenderViewAsync(viewName, model).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model, [DisallowNull] Dictionary<string, object> viewData)
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            if (!(viewData is null))
            {
                foreach (var keyValuePair in viewData.ToList())
                {
                    viewDataDictionary.Add(keyValuePair);
                }
            }

            using (var serviceScope = GetRendererServiceScopeFactory().CreateScope())
            {
                var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorRendererLogic>();
                return await renderer.RenderViewAsync(viewName, model, viewDataDictionary).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task<string> RenderAsync([DisallowNull] string viewName, [DisallowNull] Dictionary<string, object> viewData)
        {
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            if (!(viewData is null))
            {
                foreach (var keyValuePair in viewData.ToList())
                {
                    viewDataDictionary.Add(keyValuePair);
                }
            }

            using (var serviceScope = GetRendererServiceScopeFactory().CreateScope())
            {
                var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorRendererLogic>();
                return await renderer.RenderViewAsync<object>(viewName, null, viewDataDictionary).ConfigureAwait(false);
            }
        }        
    }
}
