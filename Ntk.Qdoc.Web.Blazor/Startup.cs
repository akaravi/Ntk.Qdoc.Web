using Ntk.Qdoc.Web.Blazor.Models;
using Ntk.Qdoc.Web.Blazor.Providers;
using Ntk.Qdoc.Web.Blazor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ntk.Qdoc.Web.Blazor.Interfaces;

namespace Ntk.Qdoc.Web.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<IUserStateProvider, UserStateProvider>();

            services.AddScoped<IConnectedClientService, InMemoryConnectedClientService>();
            services.AddScoped<ClientCircuitHandler>();
            services.AddScoped<CircuitHandler>(ctx => ctx.GetService<ClientCircuitHandler>());

            var channel = System.Threading.Channels.Channel.CreateBounded<MessageModel>(100);

            services.AddSingleton<IMessagesPublisher>(ctx =>
            {
                return new MessagesPublisher(channel.Writer);
            });

            services.AddSingleton<IMessagesConsumer>(ctx =>
            {
                return new MessagesConsumer(channel.Reader);
            });

            // Add new services for multi-chat functionality
            services.AddSingleton<IChatThreadService, ChatThreadService>();
            services.AddSingleton<IMessageRepository, InMemoryMessageRepository>();

            services.AddHostedService<MessagesConsumerWorker>();

            services.AddSingleton<IChatService, ChatService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
