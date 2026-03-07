
namespace ECommerceWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region DataBaseConfigration
            builder.Services.AddDbContext<StoreDbContext>(opt =>
                {
                    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                });
            #endregion


            #region Service Registeration
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddAutoMapper(x=>x.AddProfile(typeof(ProductProfile)));
            //builder.Services.AddTransient<ProductPictureResolver>(); // Register the resolver
            builder.Services.AddAutoMapper(typeof(ServiceAssemplyRefrence).Assembly);
            builder.Services.AddScoped<IProductServices, ProductService>();
            builder.Services.AddKeyedScoped<IDataInitializer, DataIntializer>("Default");
            builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitalizer>("Identity");

            builder.Services.AddSingleton<IConnectionMultiplexer>(s =>
            {
                return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);
            });
            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped<IBasketServices, BasketServices>();
            builder.Services.AddScoped<ICachRepository, CachRepository>();
            builder.Services.AddScoped<ICashService, CashService>();
            builder.Services.AddScoped<IAuthenticationSerivce, AuthenticationSerivce>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.Configure<ApiBehaviorOptions>(option =>
            {
                option.InvalidModelStateResponseFactory = ApiResponceFactory.GenerateApiValidationResponce; // Validation ModelState

            });
            builder.Services.AddDbContext<StoreIdentityDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            //  builder.Services.AddIdentity<ApplicationUser,IdentityRole>(); 
            builder.Services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreIdentityDbContext>();

            // Configure JWT Bearer authentication and define token validation parameters.
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JwtOptions:issuer"],
                    ValidAudience = builder.Configuration["JwtOptions:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:secretKey"]!))
                };
            });
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            #endregion

            var app = builder.Build();

            #region DataSeeding
            await app.MigrateDatabaseAsync();
            await app.SeedDataAsync();
            await app.MigrateIdentityDatabaseAsync();
            await app.SeedIdentityDataAsync();
            #endregion

            #region Pipeline
            #region Handel Exceptions 
            //app.Use(async (context, next) =>
            //  {
            //      try
            //      {
            //          await next.Invoke(context);
            //      }
            //      catch (Exception ex)
            //      {
            //          Console.WriteLine(ex.Message);
            //          context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            //          // return response

            //          await context.Response.WriteAsJsonAsync(new
            //          {
            //              StatusCode = StatusCodes.Status500InternalServerError,
            //              Message = $"An unexpected error occurred=>{ex.Message}"
            //          });
            //      }
            //  }); 

            app.UseMiddleware<ExceptionHandlerMiddleWare>();
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // Enable serving static files
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();

            #endregion
        }
    }
}
