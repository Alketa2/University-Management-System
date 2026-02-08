const stats = [
  { label: "Active Students", value: "12,480", trend: "+8.2%" },
  { label: "Faculty Members", value: "860", trend: "+3.1%" },
  { label: "Programs", value: "120", trend: "+4 new" },
  { label: "Campus Utilization", value: "93%", trend: "Peak" },
];

const actions = [
  {
    title: "Admissions Pipeline",
    description: "Track applicants, automate reviews, and forecast yield in real time.",
  },
  {
    title: "Academic Operations",
    description: "Coordinate schedules, faculty workload, and classroom allocation.",
  },
  {
    title: "Student Success",
    description: "Monitor engagement, alerts, and retention milestones from one view.",
  },
  {
    title: "Finance & Grants",
    description: "Align budgets, funding cycles, and approvals across departments.",
  },
];

const events = [
  {
    time: "08:30 AM",
    title: "Executive Council Briefing",
    meta: "Main Boardroom · 12 attendees",
  },
  {
    time: "10:15 AM",
    title: "Admissions Funnel Review",
    meta: "Virtual · Enrollment team",
  },
  {
    time: "01:00 PM",
    title: "Research Grant Handoff",
    meta: "Innovation Lab · Finance + Legal",
  },
  {
    time: "03:30 PM",
    title: "Student Success Sync",
    meta: "Wellness Center · Student affairs",
  },
];

const highlights = [
  {
    label: "Retention Risk",
    value: "2.1%",
    detail: "Down 0.4% since last term",
  },
  {
    label: "Average GPA",
    value: "3.42",
    detail: "Up 0.08 across STEM programs",
  },
  {
    label: "Scholarships Awarded",
    value: "$4.8M",
    detail: "86% allocated for fall",
  },
];

export default function App() {
  return (
    <div className="min-h-screen bg-slate-950 text-white">
      <div className="relative overflow-hidden">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_top,_rgba(99,102,241,0.18),_transparent_55%),radial-gradient(circle_at_30%_40%,_rgba(14,165,233,0.18),_transparent_50%)]" />
        <div className="relative mx-auto max-w-6xl px-6 pb-16 pt-8">
          <header className="flex flex-wrap items-center justify-between gap-6">
            <div className="flex items-center gap-3">
              <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-indigo-500/20 text-indigo-200 shadow-glow">
                <span className="text-xl font-semibold">UM</span>
              </div>
              <div>
                <p className="text-xs uppercase tracking-[0.3em] text-indigo-200/80">
                  University Suite
                </p>
                <h1 className="text-lg font-semibold text-white">
                  University Management System
                </h1>
              </div>
            </div>
            <nav className="flex flex-wrap items-center gap-4 text-sm text-slate-200">
              <button className="rounded-full border border-white/10 px-4 py-2 transition hover:border-white/30">
                Overview
              </button>
              <button className="rounded-full border border-white/10 px-4 py-2 transition hover:border-white/30">
                Academics
              </button>
              <button className="rounded-full border border-white/10 px-4 py-2 transition hover:border-white/30">
                Finance
              </button>
              <button className="rounded-full border border-white/10 px-4 py-2 transition hover:border-white/30">
                Analytics
              </button>
              <button className="rounded-full bg-white px-4 py-2 font-semibold text-slate-900 shadow-lg shadow-white/20">
                Launch Portal
              </button>
            </nav>
          </header>

          <section className="mt-14 grid gap-10 lg:grid-cols-[1.2fr_0.8fr]">
            <div>
              <div className="inline-flex items-center gap-2 rounded-full border border-white/10 bg-white/5 px-4 py-2 text-xs uppercase tracking-[0.2em] text-indigo-200">
                Real-time Operations
                <span className="h-1.5 w-1.5 rounded-full bg-emerald-400" />
              </div>
              <h2 className="mt-6 text-4xl font-semibold leading-tight text-white md:text-5xl">
                A modern command center for every campus, department, and student journey.
              </h2>
              <p className="mt-4 max-w-xl text-base text-slate-200/80">
                Unify admissions, academics, finance, and student success with a single
                dashboard that feels premium, responsive, and built for clarity.
              </p>
              <div className="mt-8 flex flex-wrap gap-4">
                <button className="rounded-full bg-indigo-500 px-6 py-3 text-sm font-semibold text-white shadow-glow transition hover:bg-indigo-400">
                  View Live Dashboard
                </button>
                <button className="rounded-full border border-white/20 px-6 py-3 text-sm font-semibold text-white transition hover:border-white/40">
                  Schedule a Demo
                </button>
              </div>
              <div className="mt-10 grid gap-4 sm:grid-cols-2">
                {stats.map((item) => (
                  <div
                    key={item.label}
                    className="rounded-2xl border border-white/10 bg-white/5 px-5 py-4"
                  >
                    <p className="text-xs uppercase tracking-[0.3em] text-slate-300">
                      {item.label}
                    </p>
                    <div className="mt-2 flex items-end justify-between">
                      <span className="text-2xl font-semibold">{item.value}</span>
                      <span className="rounded-full bg-emerald-500/15 px-3 py-1 text-xs font-semibold text-emerald-300">
                        {item.trend}
                      </span>
                    </div>
                  </div>
                ))}
              </div>
            </div>

            <div className="rounded-3xl border border-white/10 bg-white/5 p-6 shadow-xl">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-xs uppercase tracking-[0.3em] text-slate-300">
                    Today's Agenda
                  </p>
                  <h3 className="mt-2 text-xl font-semibold">Leadership Calendar</h3>
                </div>
                <button className="rounded-full border border-white/10 px-4 py-2 text-xs font-semibold text-white/80">
                  Manage
                </button>
              </div>
              <div className="mt-6 space-y-4">
                {events.map((event) => (
                  <div
                    key={event.title}
                    className="rounded-2xl border border-white/10 bg-slate-900/60 p-4"
                  >
                    <div className="flex items-center justify-between">
                      <p className="text-sm font-semibold text-white">{event.title}</p>
                      <span className="text-xs text-indigo-200">{event.time}</span>
                    </div>
                    <p className="mt-2 text-xs text-slate-300">{event.meta}</p>
                  </div>
                ))}
              </div>
            </div>
          </section>
        </div>
      </div>

      <section className="mx-auto -mt-10 max-w-6xl px-6 pb-20">
        <div className="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
          <div className="rounded-3xl border border-white/10 bg-slate-900/70 p-8">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-xs uppercase tracking-[0.3em] text-slate-300">
                  Operational Highlights
                </p>
                <h3 className="mt-2 text-2xl font-semibold text-white">
                  Performance indicators that keep you ahead.
                </h3>
              </div>
              <button className="rounded-full border border-white/10 px-4 py-2 text-xs font-semibold text-white/70">
                View reports
              </button>
            </div>
            <div className="mt-6 grid gap-4 sm:grid-cols-3">
              {highlights.map((item) => (
                <div
                  key={item.label}
                  className="rounded-2xl border border-white/10 bg-white/5 p-4"
                >
                  <p className="text-xs uppercase tracking-[0.25em] text-slate-400">
                    {item.label}
                  </p>
                  <p className="mt-3 text-2xl font-semibold text-white">{item.value}</p>
                  <p className="mt-2 text-xs text-slate-300">{item.detail}</p>
                </div>
              ))}
            </div>
          </div>

          <div className="rounded-3xl border border-white/10 bg-white/5 p-8">
            <p className="text-xs uppercase tracking-[0.3em] text-slate-300">
              Quick Actions
            </p>
            <h3 className="mt-2 text-2xl font-semibold text-white">
              Everything leadership needs, one tap away.
            </h3>
            <div className="mt-6 space-y-4">
              {actions.map((item) => (
                <div
                  key={item.title}
                  className="rounded-2xl border border-white/10 bg-slate-900/60 p-4"
                >
                  <p className="text-sm font-semibold text-white">{item.title}</p>
                  <p className="mt-2 text-xs text-slate-300">{item.description}</p>
                  <div className="mt-4 flex items-center justify-between">
                    <span className="text-xs font-semibold text-indigo-200">
                      Explore module
                    </span>
                    <span className="text-xs text-slate-500">Updated 2h ago</span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="mt-10 rounded-3xl border border-white/10 bg-gradient-to-r from-indigo-500/20 via-slate-900/80 to-slate-900/80 p-8">
          <div className="grid gap-6 md:grid-cols-[1.3fr_0.7fr] md:items-center">
            <div>
              <p className="text-xs uppercase tracking-[0.3em] text-indigo-200">
                Insights that inspire action
              </p>
              <h3 className="mt-3 text-3xl font-semibold text-white">
                Transform data into decisions with embedded analytics.
              </h3>
              <p className="mt-3 text-sm text-slate-200/80">
                Use predictive insights, automated workflows, and curated alerts to
                keep leadership aligned and students supported.
              </p>
            </div>
            <div className="flex flex-col gap-3 rounded-2xl border border-white/10 bg-white/5 p-4">
              <div className="flex items-center justify-between text-sm">
                <span className="text-slate-300">Operational readiness</span>
                <span className="font-semibold text-emerald-300">96%</span>
              </div>
              <div className="h-2 w-full rounded-full bg-white/10">
                <div className="h-full w-[96%] rounded-full bg-emerald-400" />
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-slate-300">Student sentiment</span>
                <span className="font-semibold text-indigo-200">88%</span>
              </div>
              <div className="h-2 w-full rounded-full bg-white/10">
                <div className="h-full w-[88%] rounded-full bg-indigo-400" />
              </div>
              <button className="mt-3 rounded-xl bg-white px-4 py-2 text-xs font-semibold text-slate-900">
                Export analytics
              </button>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
