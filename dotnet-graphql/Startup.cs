
using dotnet_graphql.Services;
using dotnet_graphql.Data;
using dotnet_graphql.GraphQL;
using dotnet_graphql.Queries;
using GraphiQl;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using GraphQL;

namespace dotnet_graphql
{
    public class Startup
    {
        public const string GraphQlPath = "/graphql";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Add framework services.
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(Configuration["ConnectionString"], sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                //Configuring Connection Resiliency:
                // https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            }));

            InitData(services);

            services.AddScoped<BookService>();
            services.AddScoped<ProductService>();
            services.AddScoped<AuthorService>();

            // using for graphql schema and mutation
            services.AddSingleton<ProductInputType>();
            services.AddSingleton<BookType>();
            services.AddSingleton<AuthorType>();
            services.AddSingleton<CategoryType>();
            services.AddSingleton<BookCategoriesType>();
            services.AddSingleton<GraphQL.ProductType>();
            services.AddSingleton<SizeType>();


            services.AddScoped<APIQuery>();
            services.AddScoped<ProductMutation>();

            var sp = services.BuildServiceProvider();
			services.AddSingleton<ISchema>(new GraphQLSchema(new FuncDependencyResolver(sp.GetService)));
			//services.AddSingleton<ISchema>(
			//	_ => new GraphQLSchema(type => (GraphType)sp.GetService(type))
			//	{
			//		Query = sp.GetService<APIQuery>()
			//	});

            services.AddMvc().AddNewtonsoftJson();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseGraphiQl(GraphQlPath);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // Seed Data
        private void InitData(IServiceCollection services)
        {
            ServiceProvider sp = services.BuildServiceProvider();
            var context = sp.GetRequiredService<AppDbContext>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Migration Database");
                Console.WriteLine(e);
            }

            new AppContextSeed().SeedAsync(context).Wait();
        }
    }
}
