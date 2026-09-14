using Glowing.Api.Services;

namespace Glowing.Api.Tests;

public class LessonEvaluatorTests
{
    private readonly LessonEvaluator _evaluator = new();
    private readonly TopicCatalog _catalog = new();

    [Fact]
    public void Evaluate_WithPoliteCoffeeOrder_ReturnsHighScoreAndExpectedFeedback()
    {
        var topic = _catalog.FindTopic("coffee-ordering");
        Assert.NotNull(topic);

        var result = _evaluator.Evaluate(topic!, "I want a hot cappuccino please.");

        Assert.True(result.Score >= 80);
        Assert.Contains("cappuccino", result.PronunciationFeedback, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("I would like", result.GrammarFeedback, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("Great choice! Large or small size?", result.NextAiResponse);
        Assert.True(result.Firework);
    }

    [Fact]
    public void Evaluate_AlwaysIncludesPromptTemplate()
    {
        var topic = _catalog.FindTopic("self-introduction");
        Assert.NotNull(topic);

        var result = _evaluator.Evaluate(topic!, "My name is Linh.");

        Assert.Contains("\"score\"", result.PromptTemplate);
        Assert.Contains("Câu nói của học sinh", result.PromptTemplate);
        Assert.Contains("My name is Linh.", result.PromptTemplate);
    }
}
