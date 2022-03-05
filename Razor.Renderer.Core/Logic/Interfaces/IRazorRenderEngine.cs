﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Razor.Renderer.Core.Logic.Interfaces
{
    public interface IRazorRenderEngine
    {
        /// <summary>
        /// Setup the RazorRenderEgine
        /// </summary>
        void Setup();

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <returns>Rendered string from the view</returns>
        Task<string> RenderAsync([DisallowNull] string viewName);

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object </param>
        /// <returns>Rendered string from the view</returns>
        Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model);

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="model">Strongly typed object</param>
        /// <param name="viewData">ViewData</param>
        /// <returns>Rendered string from the view</returns>
        Task<string> RenderAsync<TModel>([DisallowNull] string viewName, [DisallowNull] TModel model, [DisallowNull] Dictionary<string, object> viewData);

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewData">ViewData</param>
        /// <returns>Rendered string from the view</returns>
        Task<string> RenderAsync([DisallowNull] string viewName, [DisallowNull] Dictionary<string, object> viewData);
    }
}
