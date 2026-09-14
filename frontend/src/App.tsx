import './App.css'
import { useEffect, useMemo, useState } from 'react'

type TopicItem = {
  slug: string
  title: string
  scene: string
  difficulty: string
  schoolContext: boolean
}

type TopicStage = {
  name: string
  topics: TopicItem[]
}

type GamificationSnapshot = {
  userId: string
  totalXp: number
  currentStreak: number
  bestStreak: number
  streakFreezeCount: number
  gold: number
  badges: string[]
  dailyQuests: {
    fiveMinuteSpeakingCompleted: boolean
    goldenHourCompleted: boolean
  }
  leaderboard: {
    tier: string
    groupSize: number
    rank: number
    promotionEligible: boolean
  }
}

type EvaluationResponse = {
  score: number
  pronunciationFeedback: string
  grammarFeedback: string
  xpEarned: number
  nextAiResponse: string
  promptTemplate: string
  firework: boolean
  gamification: GamificationSnapshot
}

function App() {
  const [stages, setStages] = useState<TopicStage[]>([])
  const [selectedTopic, setSelectedTopic] = useState<string>('coffee-ordering')
  const [transcript, setTranscript] = useState('I want a hot cappuccino please.')
  const [spokenSentenceCount, setSpokenSentenceCount] = useState(3)
  const [practiceMinutes, setPracticeMinutes] = useState(5)
  const [evaluation, setEvaluation] = useState<EvaluationResponse | null>(null)
  const [loading, setLoading] = useState(false)

  const userId = 'student-demo'

  useEffect(() => {
    fetch('/api/topics')
      .then((response) => response.json())
      .then((data: TopicStage[]) => setStages(data))
      .catch(() => setStages([]))
  }, [])

  const selectedTopicTitle = useMemo(
    () =>
      stages
        .flatMap((stage) => stage.topics)
        .find((topic) => topic.slug === selectedTopic)?.title ?? 'Gọi món tại quán Cafe',
    [selectedTopic, stages],
  )

  async function submitLesson() {
    setLoading(true)
    try {
      const response = await fetch('/api/lesson/evaluate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userId,
          topicSlug: selectedTopic,
          transcript,
          spokenSentenceCount,
          topicCompleted: true,
          practiceMinutes,
          practicedAt: new Date().toISOString(),
        }),
      })

      const payload = (await response.json()) as EvaluationResponse
      setEvaluation(payload)
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="mx-auto max-w-6xl space-y-6 px-4 py-6 text-slate-900">
      <section className="rounded-2xl bg-slate-900 p-6 text-white shadow-xl">
        <h1 className="text-2xl font-bold md:text-3xl">Web App Luyện Nói AI</h1>
        <p className="mt-2 text-slate-200">
          Tuyến chủ đề thực tế • Core AI Loop • Streak Engine • Gamification cho môi trường học đường
        </p>
      </section>

      <section className="grid gap-4 md:grid-cols-2">
        <div className="card">
          <h2 className="text-lg font-semibold">1) Tuyến chủ đề (Thematic Journey)</h2>
          <div className="mt-3 space-y-3">
            {stages.map((stage) => (
              <div key={stage.name} className="rounded-xl border border-slate-200 p-3">
                <p className="font-medium">{stage.name}</p>
                <div className="mt-2 grid gap-2">
                  {stage.topics.map((topic) => (
                    <button
                      key={topic.slug}
                      type="button"
                      className={`topic-btn ${selectedTopic === topic.slug ? 'topic-btn-active' : ''}`}
                      onClick={() => setSelectedTopic(topic.slug)}
                    >
                      <span>{topic.title}</span>
                      {topic.schoolContext ? <span className="badge">School</span> : null}
                    </button>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="card">
          <h2 className="text-lg font-semibold">2) Core AI Loop</h2>
          <p className="mt-1 text-sm text-slate-600">
            Chủ đề đang luyện: <strong>{selectedTopicTitle}</strong>
          </p>
          <label className="mt-3 block text-sm font-medium">Câu nói từ Mic (Whisper transcript)</label>
          <textarea
            className="input mt-1 h-28"
            value={transcript}
            onChange={(event) => setTranscript(event.target.value)}
          />
          <div className="mt-3 grid grid-cols-2 gap-2">
            <label className="text-sm">
              Số câu đã nói
              <input
                className="input mt-1"
                type="number"
                min={1}
                value={spokenSentenceCount}
                onChange={(event) => setSpokenSentenceCount(Number(event.target.value))}
              />
            </label>
            <label className="text-sm">
              Số phút luyện
              <input
                className="input mt-1"
                type="number"
                min={1}
                value={practiceMinutes}
                onChange={(event) => setPracticeMinutes(Number(event.target.value))}
              />
            </label>
          </div>
          <button type="button" className="action mt-4" onClick={() => void submitLesson()} disabled={loading}>
            {loading ? 'Đang chấm điểm...' : 'Chấm điểm & nhận thưởng'}
          </button>
          {evaluation ? (
            <div className={`mt-4 rounded-xl p-3 ${evaluation.firework ? 'bg-emerald-50' : 'bg-slate-50'}`}>
              <p className="text-xl font-bold">Score: {evaluation.score}</p>
              <p className="mt-1 text-sm">{evaluation.pronunciationFeedback}</p>
              <p className="mt-1 text-sm">{evaluation.grammarFeedback}</p>
              <p className="mt-1 text-sm font-medium">AI: “{evaluation.nextAiResponse}”</p>
              <p className="mt-1 text-sm text-emerald-700">+{evaluation.xpEarned} XP</p>
            </div>
          ) : null}
        </div>
      </section>

      {evaluation ? (
        <section className="grid gap-4 md:grid-cols-3">
          <div className="card">
            <h3 className="font-semibold">3) Streak Engine</h3>
            <p className="mt-2">🔥 Chuỗi hiện tại: {evaluation.gamification.currentStreak} ngày</p>
            <p>Best: {evaluation.gamification.bestStreak} ngày</p>
            <p>🛡️ Khiên: {evaluation.gamification.streakFreezeCount}</p>
            <p>🪙 Vàng: {evaluation.gamification.gold}</p>
          </div>
          <div className="card">
            <h3 className="font-semibold">4) Quests & Badges</h3>
            <p className="mt-2">
              Nói 5 phút: {evaluation.gamification.dailyQuests.fiveMinuteSpeakingCompleted ? '✅' : '❌'}
            </p>
            <p>Khung giờ vàng (20h-21h): {evaluation.gamification.dailyQuests.goldenHourCompleted ? '✅' : '❌'}</p>
            <div className="mt-2 flex flex-wrap gap-2">
              {evaluation.gamification.badges.map((badge) => (
                <span key={badge} className="badge">
                  {badge}
                </span>
              ))}
            </div>
          </div>
          <div className="card">
            <h3 className="font-semibold">Leaderboard tuần</h3>
            <p className="mt-2">Hạng: {evaluation.gamification.leaderboard.tier}</p>
            <p>Nhóm: {evaluation.gamification.leaderboard.groupSize} người</p>
            <p>Vị trí: #{evaluation.gamification.leaderboard.rank}</p>
            <p>{evaluation.gamification.leaderboard.promotionEligible ? '🏆 Có thể thăng hạng' : 'Tiếp tục luyện'}</p>
            <div className="mt-3 h-3 rounded bg-slate-200">
              <div
                className="h-3 rounded bg-indigo-500 transition-all"
                style={{ width: `${Math.min(100, (evaluation.gamification.totalXp % 200) / 2)}%` }}
              />
            </div>
            <p className="mt-1 text-xs text-slate-500">XP tổng: {evaluation.gamification.totalXp}</p>
          </div>
        </section>
      ) : null}

      <section className="card">
        <h3 className="font-semibold">Prompt Engineering sử dụng cho AI</h3>
        <pre className="mt-2 max-h-64 overflow-auto rounded-xl bg-slate-950 p-3 text-xs text-slate-100">
          {evaluation?.promptTemplate ??
            'Prompt sẽ hiện sau lần chấm điểm đầu tiên để phục vụ tích hợp OpenAI/Claude.'}
        </pre>
      </section>
    </main>
  )
}

export default App
