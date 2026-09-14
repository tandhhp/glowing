using System.Text;
using Glowing.Api.Domain;

namespace Glowing.Api.Services;

public sealed class LessonEvaluator
{
    public LessonEvaluation Evaluate(TopicItem topic, string transcript)
    {
        var text = transcript?.Trim() ?? string.Empty;
        var lowered = text.ToLowerInvariant();
        var tokenCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;

        var score = 55;
        if (tokenCount >= 4) score += 10;
        if (lowered.Contains("please")) score += 10;
        if (lowered.StartsWith("i want")) score += 5;
        if (lowered.Contains("would like")) score += 15;
        if (lowered.Contains("thank you")) score += 5;
        if (lowered.Contains("cappuccino")) score += 5;
        if (lowered.Length > 45) score += 5;
        score = Math.Clamp(score, 0, 100);

        var pronunciation = lowered.Contains("cappuccino")
            ? "Từ 'cappuccino' phát âm gần đúng nhưng nên nhấn trọng âm vào âm tiết thứ 3 (ca-ppu-CI-no)."
            : "Phát âm khá rõ, hãy giữ tốc độ đều và nhấn trọng âm ở từ khóa chính.";

        var grammar = lowered.Contains("i want")
            ? "Câu đúng ngữ pháp, nhưng tự nhiên hơn nên dùng 'I would like to have...' thay vì 'I want'."
            : "Cấu trúc câu ổn. Bạn có thể thêm cụm lịch sự như 'Could I...' để tự nhiên hơn.";

        var xp = score >= 80 ? 10 : 6;
        if (tokenCount >= 8)
        {
            xp += 2;
        }

        var nextResponse = topic.Slug switch
        {
            "coffee-ordering" => "Great choice! Large or small size?",
            "self-introduction" => "Nice to meet you! Which class are you in?",
            "group-discussion" => "Good point. Can you explain your idea in one more sentence?",
            _ => "Great! Could you tell me one more detail?"
        };

        if (score < 70)
        {
            nextResponse = $"Thanks! Let's try once more in this scene: {topic.Scene}.";
        }

        return new LessonEvaluation(
            score,
            pronunciation,
            grammar,
            xp,
            nextResponse,
            BuildPromptTemplate(topic, text),
            score > 80);
    }

    private static string BuildPromptTemplate(TopicItem topic, string transcript)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Bạn là một giáo viên bản xứ chuyên dạy Tiếng Anh Giao Tiếp Hằng Ngày (General English).");
        builder.AppendLine("Nhiệm vụ của bạn là lắng nghe câu nói của học sinh (đã được chuyển thành văn bản), đánh giá nó và tiếp tục cuộc hội thoại theo chủ đề đã chọn.");
        builder.AppendLine();
        builder.AppendLine("Hãy trả về kết quả định dạng JSON với các trường sau:");
        builder.AppendLine("1. \"score\": Điểm số từ 0 đến 100 dựa trên độ tự nhiên và chính xác.");
        builder.AppendLine("2. \"pronunciation_feedback\": Nhận xét ngắn gọn về phát âm (viết bằng tiếng Việt).");
        builder.AppendLine("3. \"grammar_feedback\": Gợi ý cách diễn đạt tự nhiên hơn, lịch sự hơn của người bản xứ (viết bằng tiếng Việt).");
        builder.AppendLine("4. \"next_ai_response\": Câu phản hồi tiếp theo của bạn để duy trì cuộc hội thoại theo ngữ cảnh (bằng tiếng Anh).");
        builder.AppendLine();
        builder.AppendLine($"Ngữ cảnh hiện tại: {topic.Scene}. Chủ đề: {topic.Title}.");
        builder.AppendLine($"Câu nói của học sinh: {transcript}");
        return builder.ToString().Trim();
    }
}

public record LessonEvaluation(
    int Score,
    string PronunciationFeedback,
    string GrammarFeedback,
    int XpEarned,
    string NextAiResponse,
    string PromptTemplate,
    bool Firework);
