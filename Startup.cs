using CoreTest1.Models;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
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
			//services.AddRouting(options => options.LowercaseUrls = true);
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
                        //.AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, sp => new StringAsEnumResolver()));
                        .AddService<ODataUriResolver>(Microsoft.OData.ServiceLifetime.Singleton, sp => new UnqualifiedCallAndEnumPrefixFreeResolver { EnableCaseInsensitive = true }));
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

    public class UnqualifiedCallAndEnumPrefixFreeResolver : ODataUriResolver
    {
        private readonly StringAsEnumResolver _stringAsEnum = new StringAsEnumResolver();
        private readonly UnqualifiedODataUriResolver _unqualified = new UnqualifiedODataUriResolver();

        private bool _enableCaseInsensitive;

        /// <inheritdoc/>
        public override bool EnableCaseInsensitive
        {
            get
            {
                return _enableCaseInsensitive;
            }
            set
            {
                _enableCaseInsensitive = value;
                _stringAsEnum.EnableCaseInsensitive = this._enableCaseInsensitive;
                _unqualified.EnableCaseInsensitive = this._enableCaseInsensitive;
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<IEdmOperation> ResolveBoundOperations(IEdmModel model, string identifier,
            IEdmType bindingType)
        {
            return _unqualified.ResolveBoundOperations(model, identifier, bindingType);
        }

        /// <inheritdoc/>
        public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind,
            ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
        {
            _stringAsEnum.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
        }

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type,
            IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            return _stringAsEnum.ResolveKeys(type, namedValues, convertFunc);
        }

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type,
            IList<string> positionalValues, Func<IEdmTypeReference, string, object> convertFunc)
        {
            return _stringAsEnum.ResolveKeys(type, positionalValues, convertFunc);
        }

        /// <inheritdoc/>
        public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
            IEdmOperation operation, IDictionary<string, SingleValueNode> input)
        {
            return _stringAsEnum.ResolveOperationParameters(operation, input);
        }
    }
}