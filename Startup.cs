using CoreTest1.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System.Collections.Generic;

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
				builder.MapODataServiceRoute("ODataRoute", "odata", containerBuilder =>
					containerBuilder.AddService(Microsoft.OData.ServiceLifetime.Singleton, sp => modelBuilder.GetEdmModel())
						.AddService<IEnumerable<IODataRoutingConvention>>(Microsoft.OData.ServiceLifetime.Singleton, sp => ODataRoutingConventions.CreateDefaultWithAttributeRouting("ODataRoute", builder))
						.AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, sp => new StringAsEnumResolver()));
						//.AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, sp => new UnqualifiedODataUriResolver { EnableCaseInsensitive = true }));

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
}
