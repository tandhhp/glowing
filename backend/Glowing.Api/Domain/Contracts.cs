namespace Glowing.Api.Domain;

public record TopicStage(string Name, IReadOnlyList<TopicItem> Topics);

public record TopicItem(string Slug, string Title, string Scene, string Difficulty, bool SchoolContext = false);

public record EvaluateLessonRequest(
    string UserId,
    string TopicSlug,
    string Transcript,
    int SpokenSentenceCount = 1,
    bool TopicCompleted = false,
    int PracticeMinutes = 0,
    DateTimeOffset? PracticedAt = null);

public record EvaluateLessonResponse(
    int Score,
    string PronunciationFeedback,
    string GrammarFeedback,
    int XpEarned,
    string NextAiResponse,
    string PromptTemplate,
    bool Firework,
    GamificationSnapshot Gamification);

public record GamificationSnapshot(
    string UserId,
    int TotalXp,
    int CurrentStreak,
    int BestStreak,
    int StreakFreezeCount,
    int Gold,
    IReadOnlyCollection<string> Badges,
    DailyQuestProgress DailyQuests,
    LeaderboardSnapshot Leaderboard);

public record DailyQuestProgress(bool FiveMinuteSpeakingCompleted, bool GoldenHourCompleted);

public record LeaderboardSnapshot(string Tier, int GroupSize, int Rank, bool PromotionEligible);

public record StreakFreezePurchaseResult(bool Success, int Gold, int StreakFreezeCount, string Message);
