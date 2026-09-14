using Glowing.Api.Domain;
using Glowing.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<TopicCatalog>();
builder.Services.AddSingleton<LessonEvaluator>();
builder.Services.AddSingleton<GamificationService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("frontend");

app.MapGet("/api/topics", (TopicCatalog catalog) => Results.Ok(catalog.GetJourney()));

app.MapGet("/api/gamification/{userId}", (string userId, GamificationService service) =>
{
    return Results.Ok(service.GetOrCreateState(userId).ToSnapshot());
});

app.MapPost("/api/gamification/{userId}/streak-freeze/purchase", (string userId, GamificationService service) =>
{
    var result = service.PurchaseStreakFreeze(userId);
    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapPost("/api/lesson/evaluate", (
    EvaluateLessonRequest request,
    TopicCatalog catalog,
    LessonEvaluator evaluator,
    GamificationService gamificationService) =>
{
    if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.TopicSlug))
    {
        return Results.BadRequest(new { message = "userId và topicSlug là bắt buộc." });
    }

    var topic = catalog.FindTopic(request.TopicSlug);
    if (topic is null)
    {
        return Results.NotFound(new { message = "Không tìm thấy chủ đề." });
    }

    var evaluation = evaluator.Evaluate(topic, request.Transcript);
    var reward = gamificationService.ApplyPracticeResult(request, evaluation.Score, evaluation.XpEarned);

    return Results.Ok(new EvaluateLessonResponse(
        evaluation.Score,
        evaluation.PronunciationFeedback,
        evaluation.GrammarFeedback,
        evaluation.XpEarned,
        evaluation.NextAiResponse,
        evaluation.PromptTemplate,
        evaluation.Firework,
        reward));
});

app.Run();
