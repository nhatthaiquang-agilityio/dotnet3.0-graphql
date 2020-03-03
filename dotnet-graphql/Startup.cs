using dotnet_graphql.Data;
using dotnet_graphql.Helpers;
using dotnet_graphql.GraphQL.InputTypes;
using dotnet_graphql.GraphQL.ObjectTypes;
using dotnet_graphql.Models;
using dotnet_graphql.Services;
using dotnet_graphql.Queries;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Validation;
using ExpressMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductType = dotnet_graphql.GraphQL.ObjectTypes.ProductType;

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
            services.AddScoped<IUserService, UserService>();

            services.AddMvc().AddNewtonsoftJson();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.RequireHttpsMetadata = false;
                    jwtOptions.SaveToken = true;
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

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

            app.UseAuthentication();
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
            // using for graphql query and mutation
            services.AddSingleton<ProductInputType>();
            services.AddSingleton<UserInputType>();

            services.AddSingleton<BookType>();
            services.AddSingleton<AuthorType>();
            services.AddSingleton<CategoryType>();
            services.AddSingleton<BookCategoriesType>();
            services.AddSingleton<ProductType>();
            services.AddSingleton<SizeType>();
            services.AddSingleton<UserType>();

            services.AddScoped<APIQuery>();
            services.AddScoped<APIMutation>();

            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(new GraphQLSchema(new FuncDependencyResolver(sp.GetService)));
            //services.AddScoped<ISchema>(
            //    _ => new GraphQLSchema(type => (GraphType)sp.GetService(type))
            //    {
            //        Query = sp.GetService<APIQuery>()
            //    });

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddScoped<IValidationRule, AuthValidationRule>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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
            Mapper.Register<Author, AuthorViewModel>();
            Mapper.Register<BookCategory, BookCategoryViewModel>();
            Mapper.Register<Category, CategoryViewModel>();
            Mapper.Register<User, UserViewModel>();
            Mapper.Compile();
        }
    }
}
