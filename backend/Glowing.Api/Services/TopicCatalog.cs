using Glowing.Api.Domain;

namespace Glowing.Api.Services;

public sealed class TopicCatalog
{
    private readonly IReadOnlyList<TopicStage> _journey =
    [
        new("Chặng 1 - Survival English", [
            new TopicItem("self-introduction", "Giới thiệu bản thân", "Làm quen bạn mới", "Easy", true),
            new TopicItem("restaurant-ordering", "Gọi món tại nhà hàng", "Đi ăn trưa cùng bạn bè", "Easy"),
            new TopicItem("asking-directions", "Hỏi đường", "Đi tới thư viện", "Easy", true),
            new TopicItem("shopping", "Đi mua sắm", "Mua đồ dùng học tập", "Easy", true),
            new TopicItem("coffee-ordering", "Gọi món tại quán Cafe", "Khách hàng tại quán cafe", "Easy")
        ]),
        new("Chặng 2 - Socializing", [
            new TopicItem("small-talk", "Tán gẫu với đồng nghiệp", "Chuyện trò đầu giờ", "Medium"),
            new TopicItem("share-hobbies", "Chia sẻ sở thích", "Hoạt động câu lạc bộ", "Medium", true),
            new TopicItem("travel-story", "Kể về một chuyến đi", "Thuyết trình nhóm", "Medium", true)
        ]),
        new("Chặng 3 - Problem Solving", [
            new TopicItem("hotel-complaint", "Than phiền dịch vụ khách sạn", "Xử lý dịch vụ", "Hard"),
            new TopicItem("job-interview", "Phỏng vấn xin việc", "Mô phỏng phỏng vấn", "Hard"),
            new TopicItem("group-discussion", "Thảo luận nhóm", "Giờ học dự án", "Hard", true)
        ])
    ];

    public IReadOnlyList<TopicStage> GetJourney() => _journey;

    public TopicItem? FindTopic(string slug) =>
        _journey.SelectMany(stage => stage.Topics).FirstOrDefault(topic => topic.Slug == slug);
}
