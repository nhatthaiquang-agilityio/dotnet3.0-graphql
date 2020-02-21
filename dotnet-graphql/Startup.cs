using dotnet_graphql.Services;
using dotnet_graphql.Data;
using dotnet_graphql.Models;
using dotnet_graphql.GraphQL;
using dotnet_graphql.Queries;
using ExpressMapper;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_graphql
{
    public class Startup
    {
        public const string GraphQlPath = "/graphql";
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<BookService>();
            services.AddScoped<ProductService>();
            services.AddScoped<AuthorService>();

            services.AddMvc().AddNewtonsoftJson();

            ConfigureDatabase(services);

            InitGraphQLSchema(services);

            MappingRegistration();

            InitData(services);
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
            app.UseEndpoints(endpoints =>
            {
                //route map configuration
                endpoints.MapControllers();
            });

            app.UseGraphQL<GraphQLSchema>();
            app.UseGraphQLPlayground(options: new GraphQLPlaygroundOptions());
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <param name="services"> IServiceCollection.</param>
        private static void InitData(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
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

        /// <summary>
        /// Config Database.
        /// </summary>
        /// <param name="services"> IServiceCollection.</param>
        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>(
                options => options.UseNpgsql(Configuration["ConnectionString_Postgres"])
            );
        }

        /// <summary>
        /// Init GraphQL Schema.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        private void InitGraphQLSchema(IServiceCollection services)
        {
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
            //services.AddScoped<ISchema>(
            //    _ => new GraphQLSchema(type => (GraphType)sp.GetService(type))
            //    {
            //        Query = sp.GetService<APIQuery>()
            //    });
        }

        /// <summary>
        /// Mapping Model.
        /// </summary>
        private static void MappingRegistration()
        {
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Product, ProductViewModel>()
                .Function(dest => dest.Sizes, src =>
                {
                    var sizes = new List<string>(src.Sizes.Count);
                    sizes.AddRange(from size in src.Sizes where size != null select size.Name);
                    return sizes;
                });

            Mapper.Register<Book, BookViewModel>()
                .Member(dest => dest.Author, src => src.Author.FirstName + " " + src.Author.LastName)
                .Function(dest => dest.BookCategories, src =>
                {
                    var categories = new List<string>(src.BookCategories.Count);
                    categories.AddRange(from bookCategory in src.BookCategories
                        where bookCategory?.Category != null select bookCategory.Category.CategoryName);
                    return categories;
                });
            Mapper.Register<Author, AuthorDTO>();
            Mapper.Register<BookCategory, BookCategoryViewModel>();
            Mapper.Register<Category, CategoryViewModel>();
            Mapper.Compile();
        }
    }
}
