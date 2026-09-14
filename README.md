# Glowing - AI English Speaking Practice

Ứng dụng web luyện nói tiếng Anh giao tiếp theo chủ đề quen thuộc, có gamification cho bối cảnh trường học.

## Stack

- Frontend: React + Tailwind CSS (`/home/runner/work/glowing/glowing/frontend`)
- Backend: .NET Web API (`/home/runner/work/glowing/glowing/backend/Glowing.Api`)
- Database target: SQL Server (`/home/runner/work/glowing/glowing/backend/sql/schema.sql`)

## Tính năng chính đã triển khai

1. **Thematic Journey**: tuyến chủ đề theo 3 chặng (Survival, Socializing, Problem Solving), có các tình huống học đường.
2. **Core AI Loop**: người học nhập transcript từ mic, backend chấm điểm và trả JSON gồm:
   - `score`
   - `pronunciationFeedback`
   - `grammarFeedback`
   - `xpEarned`
   - `nextAiResponse`
3. **Streak Engine & Gamification**:
   - Giữ chuỗi theo ngày nếu nói >= 3 câu hoặc hoàn thành chủ đề
   - Hỗ trợ `Streak Freeze` bằng vàng
   - XP, tier leaderboard tuần (Đồng/Bạc/Vàng), quests hằng ngày, badges
4. **Prompt Engineering**:
   - Backend sinh prompt tiếng Việt đúng định dạng JSON để gửi OpenAI/Claude cho từng lượt luyện.

## Chạy ứng dụng

### Backend

```bash
cd /home/runner/work/glowing/glowing
dotnet run --project /home/runner/work/glowing/glowing/backend/Glowing.Api/Glowing.Api.csproj
```

### Frontend

```bash
cd /home/runner/work/glowing/glowing/frontend
npm install
npm run dev
```

Frontend dùng proxy `/api` tới `http://localhost:5004`.

## Chạy test

```bash
cd /home/runner/work/glowing/glowing
dotnet test /home/runner/work/glowing/glowing/backend/Glowing.Api.Tests/Glowing.Api.Tests.csproj
```