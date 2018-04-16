using CoreTest1.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Edm;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.UriParser;

namespace CoreTest1
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddOData();
			services.AddODataQueryFilter();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			var modelBuilder = new ODataConventionModelBuilder(app.ApplicationServices);
			modelBuilder.EntitySet<Product>("Products");

			app.UseMvc(builder =>
			{
				builder.EnableDependencyInjection(b =>
				{
					b.AddService(Microsoft.OData.ServiceLifetime.Singleton, typeof(ODataUriResolver), sp => new StringAsEnumResolver());
				});

				builder.MapODataServiceRoute("ODataRoute", "odata", modelBuilder.GetEdmModel());

				//builder.MapODataServiceRoute("ODataRoute", "odata", containerBuilder =>
			 //  containerBuilder.AddService(Microsoft.OData.ServiceLifetime.Singleton, sp => modelBuilder.GetEdmModel())
				//			   //.AddService(ServiceLifetime.Singleton, sp => DefaultRouteConventions(routeName, builder))
				//			   .AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, sp => new UnqualifiedODataUriResolver { EnableCaseInsensitive = true })
				//			   );
				//routeBuilder.SetDefaultODataOptions(new Microsoft.AspNet.OData.ODataOptions { NullDynamicPropertyIsEnabled = true });
				builder.Filter();
				builder.MaxTop(10);
				//routeBuilder.Select();
				//routeBuilder.OrderBy();
				//routeBuilder.Count();
				//routeBuilder.Expand();
				//routeBuilder.SetDefaultQuerySettings(new Microsoft.AspNet.OData.Query.DefaultQuerySettings { EnableCount = true, EnableFilter = true, MaxTop = null, EnableOrderBy = true, EnableExpand = true, EnableSelect = true });
			});

		}
	}

	public class CaseInsensitiveResolver : ODataUriResolver
	{
		private bool _enableCaseInsensitive;

		public override bool EnableCaseInsensitive
		{
			get => true;
			set => _enableCaseInsensitive = value;
		}
	}
}
