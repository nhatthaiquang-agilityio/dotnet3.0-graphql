using dotnet_graphql.Services;
using dotnet_graphql.Data;
using dotnet_graphql.Models;
using dotnet_graphql.GraphQL;
using dotnet_graphql.Queries;
using ExpressMapper;
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
using System.Collections.Generic;

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

            ServiceProvider sp = services.BuildServiceProvider();
			//services.AddSingleton<ISchema>(new GraphQLSchema(new FuncDependencyResolver(sp.GetService)));
            services.AddScoped<ISchema>(
                _ => new GraphQLSchema(type => (GraphType)sp.GetService(type))
                {
                    Query = sp.GetService<APIQuery>()
                });

            services.AddMvc().AddNewtonsoftJson();

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

            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseRouting();
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

        private void MappingRegistration()
        {
            Mapper.Register<Size, SizeViewModel>();
            Mapper.Register<Product, ProductViewModel>()
                .Function(dest => dest.Sizes, src =>
                {
                    List<string> sizes = new List<string>(src.Sizes.Count);
                    foreach (var size in src.Sizes)
                    {
                        if (size != null)
                        {
                            sizes.Add(size.Name);
                        }
                    }
                    return sizes;
                });

            Mapper.Register<Book, BookViewModel>()
                .Member(dest => dest.Author, src => src.Author.FirstName + " " + src.Author.LastName)
                .Function(dest => dest.BookCategories, src =>
                {
                    List<string> categories = new List<string>(src.BookCategories.Count);
                    foreach (var bookCategory in src.BookCategories)
                    {
                        if (bookCategory != null && bookCategory.Category != null)
                        {
                            categories.Add(bookCategory.Category.CategoryName);
                        }
                    }
                    return categories;
                });
            Mapper.Register<Author, AuthorDTO>();
            Mapper.Register<BookCategory, BookCategoryViewModel>();
            Mapper.Register<Category, CategoryViewModel>();
            Mapper.Compile();
        }
    }
}
