using Glowing.Api.Domain;
using Glowing.Api.Services;

namespace Glowing.Api.Tests;

public class GamificationServiceTests
{
    [Fact]
    public void ApplyPracticeResult_IncreasesStreak_WhenQualifiesOnConsecutiveDays()
    {
        var service = new GamificationService();
        var firstDay = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        var secondDay = firstDay.AddDays(1);

        service.ApplyPracticeResult(new EvaluateLessonRequest("u1", "coffee-ordering", "hello", 3, true, 5, firstDay), 85, 12);
        var snapshot = service.ApplyPracticeResult(new EvaluateLessonRequest("u1", "coffee-ordering", "hello", 3, true, 5, secondDay), 90, 12);

        Assert.Equal(2, snapshot.CurrentStreak);
        Assert.Equal(2, snapshot.BestStreak);
    }

    [Fact]
    public void ApplyPracticeResult_AwardsPronunciationMasterBadge_AfterThreePerfectScores()
    {
        var service = new GamificationService();
        var day = new DateTimeOffset(2026, 9, 10, 20, 15, 0, TimeSpan.Zero);

        service.ApplyPracticeResult(new EvaluateLessonRequest("u2", "coffee-ordering", "a", 3, true, 5, day), 100, 12);
        service.ApplyPracticeResult(new EvaluateLessonRequest("u2", "coffee-ordering", "a", 3, true, 5, day.AddDays(1)), 100, 12);
        var snapshot = service.ApplyPracticeResult(new EvaluateLessonRequest("u2", "coffee-ordering", "a", 3, true, 5, day.AddDays(2)), 100, 12);

        Assert.Contains("Bậc thầy phát âm", snapshot.Badges);
        Assert.True(snapshot.DailyQuests.GoldenHourCompleted);
    }
}
