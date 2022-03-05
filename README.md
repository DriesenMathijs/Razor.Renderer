# Razor Renderer 
We can put Razor Views in a seperate Razor Class Library to use these views as a template for emails, ... in other kinds of applications (Console, API's, WPF, Worker services,...)
It supports the main View Features & MVC Razor View Features.

In Razor.Renderer.Core we have some logic & the engine to render Razor Views to strings, all this using the Razor SDK

For usage see the Example project (this is in a console application)

## There is a ServiceCollection extension so you can use Dependency Injection in your templates using @inject()
Registration with:
```csharp
var services = new ServiceCollection();
services.AddSingleton<IRazorRenderEngine, RazorRenderEngine>();
services.AddRazorRenderer();  -- add to use @inject() in your templates
```
