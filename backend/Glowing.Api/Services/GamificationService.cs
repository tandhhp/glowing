using System.Collections.Concurrent;
using System.Globalization;
using Glowing.Api.Domain;

namespace Glowing.Api.Services;

public sealed class GamificationService
{
    private readonly ConcurrentDictionary<string, UserProgressState> _state = new();
    private const int StreakFreezeCost = 20;

    public UserProgressState GetOrCreateState(string userId) =>
        _state.GetOrAdd(userId, _ => new UserProgressState(userId));

    public StreakFreezePurchaseResult PurchaseStreakFreeze(string userId)
    {
        var state = GetOrCreateState(userId);
        lock (state.SyncRoot)
        {
            if (state.Gold < StreakFreezeCost)
            {
                return new StreakFreezePurchaseResult(false, state.Gold, state.StreakFreezeCount, "Không đủ vàng để mua khiên.");
            }

            state.Gold -= StreakFreezeCost;
            state.StreakFreezeCount += 1;
            return new StreakFreezePurchaseResult(true, state.Gold, state.StreakFreezeCount, "Mua khiên bảo vệ chuỗi thành công.");
        }
    }

    public GamificationSnapshot ApplyPracticeResult(EvaluateLessonRequest request, int score, int xpEarned)
    {
        var state = GetOrCreateState(request.UserId);
        var practicedAt = request.PracticedAt ?? DateTimeOffset.UtcNow;
        var practiceDate = DateOnly.FromDateTime(practicedAt.LocalDateTime);
        var qualifiesStreak = request.SpokenSentenceCount >= 3 || request.TopicCompleted;

        lock (state.SyncRoot)
        {
            state.TotalXp += xpEarned;
            state.Gold += Math.Max(1, xpEarned / 4);
            state.PerfectScoreStreak = score == 100 ? state.PerfectScoreStreak + 1 : 0;

            if (qualifiesStreak && state.LastStreakDate != practiceDate)
            {
                if (state.LastStreakDate is null)
                {
                    state.CurrentStreak = 1;
                }
                else
                {
                    var dayGap = practiceDate.DayNumber - state.LastStreakDate.Value.DayNumber;
                    if (dayGap == 1)
                    {
                        state.CurrentStreak += 1;
                    }
                    else if (dayGap > 1)
                    {
                        var missedDays = dayGap - 1;
                        if (state.StreakFreezeCount >= missedDays)
                        {
                            state.StreakFreezeCount -= missedDays;
                            state.CurrentStreak += 1;
                        }
                        else
                        {
                            state.CurrentStreak = 1;
                        }
                    }
                }

                state.LastStreakDate = practiceDate;
                state.BestStreak = Math.Max(state.BestStreak, state.CurrentStreak);
            }

            if (request.PracticeMinutes >= 5 && state.LastFiveMinuteQuestDate != practiceDate)
            {
                state.LastFiveMinuteQuestDate = practiceDate;
                state.Gold += 5;
            }

            var localHour = practicedAt.LocalDateTime.Hour;
            if (localHour is >= 20 and < 21 && state.LastGoldenHourQuestDate != practiceDate)
            {
                state.LastGoldenHourQuestDate = practiceDate;
                state.Gold += 5;
            }

            if (state.CurrentStreak >= 7)
            {
                state.Badges.Add("Cỗ máy nói");
            }

            if (state.PerfectScoreStreak >= 3)
            {
                state.Badges.Add("Bậc thầy phát âm");
            }

            return state.ToSnapshot(practiceDate);
        }
    }
}

public sealed class UserProgressState
{
    public object SyncRoot { get; } = new();
    public string UserId { get; }
    public int TotalXp { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public int StreakFreezeCount { get; set; } = 1;
    public int Gold { get; set; } = 10;
    public int PerfectScoreStreak { get; set; }
    public DateOnly? LastStreakDate { get; set; }
    public DateOnly? LastFiveMinuteQuestDate { get; set; }
    public DateOnly? LastGoldenHourQuestDate { get; set; }
    public HashSet<string> Badges { get; } = [];

    public UserProgressState(string userId)
    {
        UserId = userId;
    }

    public GamificationSnapshot ToSnapshot(DateOnly? today = null)
    {
        var date = today ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var tier = TotalXp switch
        {
            < 300 => "Đồng",
            < 800 => "Bạc",
            _ => "Vàng"
        };

        var groupSize = 10 + Math.Abs(HashCode.Combine(UserId, date.Year, ISOWeek.GetWeekOfYear(date.ToDateTime(TimeOnly.MinValue)))) % 11;
        var rank = Math.Max(1, groupSize - Math.Min(groupSize - 1, TotalXp / 50));

        return new GamificationSnapshot(
            UserId,
            TotalXp,
            CurrentStreak,
            BestStreak,
            StreakFreezeCount,
            Gold,
            Badges.Order().ToArray(),
            new DailyQuestProgress(LastFiveMinuteQuestDate == date, LastGoldenHourQuestDate == date),
            new LeaderboardSnapshot(tier, groupSize, rank, rank <= 3));
    }
}
