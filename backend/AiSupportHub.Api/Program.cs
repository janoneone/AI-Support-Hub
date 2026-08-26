using AiSupportHub.Api.Services;
using AiSupportHub.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IAIService, GeminiService>();

builder.Services.AddScoped<
    IConversationRepository,
    ConversationRepository>();

builder.Services.AddScoped<
    IDocumentRepository,
    DocumentRepository>();

builder.Services.AddHttpClient<
    ISupportFlowService,
    SupportFlowService>();

builder.Services.AddScoped<
    ITicketRepository,
    TicketRepository>();

builder.Services.AddHttpClient<
    IN8nService,
    N8nService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "https://ai-support-hub-ashy.vercel.app"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.MapControllers();

app.Run();