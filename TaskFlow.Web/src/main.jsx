import React, { useEffect, useMemo, useState } from 'react'
import { createRoot } from 'react-dom/client'
import './styles.css'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5017/api'

const icons = {
  grid: <svg viewBox="0 0 24 24"><rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/></svg>,
  folder: <svg viewBox="0 0 24 24"><path d="M3 7.5A2.5 2.5 0 0 1 5.5 5H10l2 2h6.5A2.5 2.5 0 0 1 21 9.5v7A2.5 2.5 0 0 1 18.5 19h-13A2.5 2.5 0 0 1 3 16.5z"/></svg>,
  check: <svg viewBox="0 0 24 24"><path d="m5 12 4 4L19 6"/></svg>,
  chart: <svg viewBox="0 0 24 24"><path d="M4 19V5M4 19h16"/><path d="m7 15 3-4 3 2 5-7"/></svg>,
  settings: <svg viewBox="0 0 24 24"><path d="M12 15.2a3.2 3.2 0 1 0 0-6.4 3.2 3.2 0 0 0 0 6.4Z"/><path d="m19.4 15 .1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.9 1.9 0 0 0-3.2 1.3v.2a2 2 0 1 1-4 0v-.2a1.9 1.9 0 0 0-3.2-1.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1A1.9 1.9 0 0 0 2.2 12a1.9 1.9 0 0 0 1.3-3.2l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.9 1.9 0 0 0 3.2-1.3v-.2a2 2 0 1 1 4 0v.2a1.9 1.9 0 0 0 3.2 1.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1A1.9 1.9 0 0 0 20.8 12a1.9 1.9 0 0 0-1.4 3Z"/></svg>,
  plus: <svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg>,
  bell: <svg viewBox="0 0 24 24"><path d="M18 8a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9ZM10 21h4"/></svg>,
  search: <svg viewBox="0 0 24 24"><circle cx="11" cy="11" r="7"/><path d="m20 20-4-4"/></svg>,
  arrow: <svg viewBox="0 0 24 24"><path d="m9 18 6-6-6-6"/></svg>,
  close: <svg viewBox="0 0 24 24"><path d="m6 6 12 12M18 6 6 18"/></svg>,
}

async function api(path, options = {}) {
  const token = localStorage.getItem('taskflow_token')
  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...(options.headers || {}) },
  })
  const body = await response.json().catch(() => ({}))
  if (!response.ok) throw new Error(body.message || body.title || 'درخواست با خطا مواجه شد')
  return body
}

const sampleProjects = [
  { projectId: 'sample-1', title: 'راه‌اندازی محصول جدید', description: 'برنامه‌ریزی و اجرای نسخه‌ی اولیه محصول', createdAt: new Date().toISOString(), createByUser: 'شما' },
  { projectId: 'sample-2', title: 'بازاریابی تابستانه', description: 'کمپین‌های شبکه‌های اجتماعی و ایمیل', createdAt: new Date(Date.now() - 86400000 * 3).toISOString(), createByUser: 'شما' },
  { projectId: 'sample-3', title: 'بهبود تجربه کاربری', description: 'تحقیق، تست و بهینه‌سازی جریان‌های اصلی', createdAt: new Date(Date.now() - 86400000 * 7).toISOString(), createByUser: 'شما' },
]

function App() {
  const [user, setUser] = useState(() => JSON.parse(localStorage.getItem('taskflow_user') || 'null'))
  const [view, setView] = useState('dashboard')
  const [projects, setProjects] = useState([])
  const [tasks, setTasks] = useState([])
  const [loading, setLoading] = useState(false)
  const [notice, setNotice] = useState('')
  const [showProject, setShowProject] = useState(false)
  const [showTask, setShowTask] = useState(false)
  const [activeProject, setActiveProject] = useState(null)

  const loadProjects = async () => {
    setLoading(true)
    try { const result = await api('/Project/GetAllCurrentUserProjects?PageNumber=1&PageSize=20'); setProjects(result.items || []) }
    catch { setProjects(sampleProjects) }
    finally { setLoading(false) }
  }

  const loadTasks = async () => {
    try { const result = await api('/Task/GetAllCurrentUserTasks?PageNumber=1&PageSize=50'); setTasks(result.items || []) }
    catch { setTasks([]) }
  }

  useEffect(() => { if (user) { loadProjects(); loadTasks() } }, [user])

  const logout = () => { localStorage.removeItem('taskflow_token'); localStorage.removeItem('taskflow_user'); setUser(null) }
  if (!user) return <Auth onSuccess={setUser} />

  const initials = (user.fullname || user.email || 'TF').split(' ').map(x => x[0]).join('').slice(0, 2).toUpperCase()
  const openTask = (project = null) => { setActiveProject(project); setShowTask(true) }

  return <div className="app-shell">
    <aside className="sidebar">
      <div className="brand"><span className="brand-mark">✓</span><span>Task<span>Flow</span></span></div>
      <div className="workspace-label">فضای کاری شخصی <span>⌄</span></div>
      <nav>
        <NavItem icon={icons.grid} label="نمای کلی" active={view === 'dashboard'} onClick={() => setView('dashboard')} />
        <NavItem icon={icons.folder} label="پروژه‌ها" active={view === 'projects'} onClick={() => setView('projects')} />
        <NavItem icon={icons.check} label="تسک‌های من" active={view === 'tasks'} onClick={() => setView('tasks')} />
        <NavItem icon={icons.chart} label="گزارش‌ها" active={view === 'reports'} onClick={() => setView('reports')} />
      </nav>
      <div className="sidebar-bottom"><NavItem icon={icons.settings} label="تنظیمات" active={view === 'settings'} onClick={() => setView('settings')} /></div>
      <div className="profile-mini"><div className="avatar">{initials}</div><div className="profile-copy"><strong>{user.fullname || 'کاربر TaskFlow'}</strong><span>{user.role || 'کاربر'}</span></div><button className="logout-btn" onClick={logout} title="خروج">↪</button></div>
    </aside>
    <main className="main-content">
      <header className="topbar"><div className="breadcrumbs"><span>فضای کاری</span><b>/</b><strong>{view === 'dashboard' ? 'نمای کلی' : view === 'projects' ? 'پروژه‌ها' : view === 'tasks' ? 'تسک‌های من' : view === 'reports' ? 'گزارش‌ها' : 'تنظیمات'}</strong></div><div className="top-actions"><button className="icon-btn"><span className="notif-dot" />{icons.bell}</button><div className="top-avatar">{initials}</div></div></header>
      {notice && <div className="notice">{notice}<button onClick={() => setNotice('')}>{icons.close}</button></div>}
      {view === 'dashboard' && <Dashboard projects={projects} tasks={tasks} loading={loading} user={user} onNewProject={() => setShowProject(true)} onNewTask={() => openTask()} onProjects={() => setView('projects')} />}
      {view === 'projects' && <Projects projects={projects} loading={loading} onNewProject={() => setShowProject(true)} onNewTask={openTask} onRefresh={loadProjects} />}
      {view === 'tasks' && <Tasks tasks={tasks} onNewTask={() => openTask()} />}
      {view === 'reports' && <EmptyView icon={icons.chart} title="گزارش‌ها" text="گزارش پیشرفت پروژه‌ها را با یک نگاه دنبال کنید. این بخش در حال آماده‌سازی است." />}
      {view === 'settings' && <EmptyView icon={icons.settings} title="تنظیمات" text="تنظیمات فضای کاری شما به‌زودی در دسترس خواهد بود." />}
    </main>
    {showProject && <ProjectModal onClose={() => setShowProject(false)} onCreated={(project) => { setProjects(p => [project, ...p]); setShowProject(false); setNotice('پروژه با موفقیت ساخته شد') }} />}
    {showTask && <TaskModal project={activeProject} projects={projects} onClose={() => setShowTask(false)} onCreated={() => { setShowTask(false); setNotice('تسک با موفقیت ساخته شد') }} />}
  </div>
}

function NavItem({ icon, label, active, onClick }) { return <button className={`nav-item ${active ? 'active' : ''}`} onClick={onClick}><span className="nav-icon">{icon}</span><span>{label}</span>{active && <i />}</button> }

function Dashboard({ projects, tasks, loading, user, onNewProject, onNewTask, onProjects }) {
  const activeTasks = tasks.filter(task => task.status !== 'Done').length
  const completedTasks = tasks.filter(task => task.status === 'Done').length
  const dueTasks = tasks.filter(task => task.deadLine && new Date(task.deadLine) < new Date(Date.now() + 86400000 * 3) && task.status !== 'Done').length
  const stats = [{ label: 'کل پروژه‌ها', value: projects.length || 0, color: 'purple', icon: icons.folder }, { label: 'تسک‌های فعال', value: activeTasks, color: 'blue', icon: icons.check }, { label: 'تکمیل‌شده', value: completedTasks, color: 'green', icon: icons.chart }, { label: 'نزدیک به موعد', value: dueTasks, color: 'orange', icon: icons.bell }]
  return <section className="page"><div className="welcome-row"><div><p className="eyebrow">شنبه، ۲۷ مرداد ۱۴۰۳</p><h1>روز بخیر، {user.fullname?.split(' ')[0] || 'دوست'} <span>👋</span></h1><p className="subtle">بیایید امروز کارهای مهم را جلو ببریم.</p></div><button className="primary-btn" onClick={onNewProject}>{icons.plus}<span>پروژه جدید</span></button></div>
    <div className="stats-grid">{stats.map(stat => <div className="stat-card" key={stat.label}><div className={`stat-icon ${stat.color}`}>{stat.icon}</div><div><span>{stat.label}</span><strong>{stat.value}</strong></div><div className="stat-trend">{stat.color === 'green' ? '↑ ۱۲٪' : 'این ماه'}</div></div>)}</div>
    <div className="section-grid"><div className="panel project-panel"><div className="panel-head"><div><h2>پروژه‌های اخیر</h2><p>پروژه‌هایی که اخیراً روی آن‌ها کار کرده‌اید</p></div><button className="text-btn" onClick={onProjects}>مشاهده همه {icons.arrow}</button></div>{loading ? <div className="loading">در حال دریافت پروژه‌ها...</div> : <div className="project-list">{projects.slice(0, 3).map((project, index) => <ProjectRow key={project.projectId || index} project={project} />)}{!projects.length && <div className="empty-inline">هنوز پروژه‌ای نساخته‌اید.</div>}</div>}</div>
      <div className="panel quick-panel"><div className="panel-head"><div><h2>شروع سریع</h2><p>از اینجا کار بعدی‌تان را شروع کنید</p></div></div><button className="quick-action" onClick={onNewTask}><span className="quick-icon blue-bg">{icons.check}</span><span><strong>ساخت تسک جدید</strong><small>یک کار را به پروژه اضافه کنید</small></span>{icons.arrow}</button><button className="quick-action" onClick={onNewProject}><span className="quick-icon purple-bg">{icons.folder}</span><span><strong>ساخت پروژه جدید</strong><small>یک فضای کاری تازه بسازید</small></span>{icons.arrow}</button></div></div>
    <div className="panel focus-panel"><div className="panel-head"><div><h2>تمرکز امروز</h2><p>پیشرفت کلی فعالیت‌های شما</p></div><span className="progress-label">۶۸٪ تکمیل شده</span></div><div className="progress-track"><div style={{ width: '68%' }} /></div><div className="progress-foot"><span>۰ کار انجام نشده</span><span>۳۲ کار باقی‌مانده</span></div></div>
  </section>
}

function Projects({ projects, loading, onNewProject, onNewTask, onRefresh }) { return <section className="page"><div className="page-heading"><div><p className="eyebrow">فضای کاری</p><h1>پروژه‌ها</h1><p className="subtle">همه‌ی پروژه‌هایتان را یک‌جا مدیریت کنید.</p></div><div className="heading-actions"><button className="secondary-btn" onClick={onRefresh}>↻ بروزرسانی</button><button className="primary-btn" onClick={onNewProject}>{icons.plus} پروژه جدید</button></div></div><div className="projects-grid">{loading ? <div className="loading">در حال دریافت پروژه‌ها...</div> : projects.map(project => <article className="project-card" key={project.projectId}><div className="project-card-top"><span className="project-color" /><button className="more-btn">•••</button></div><h3>{project.title}</h3><p>{project.description || 'بدون توضیحات'}</p><div className="project-progress"><div className="progress-track"><div style={{ width: '64%' }} /></div><span>۶۴٪</span></div><div className="project-card-foot"><span>{formatDate(project.createdAt)}</span><button className="small-action" onClick={() => onNewTask(project)}>{icons.plus} تسک</button></div></article>)}{!projects.length && <div className="empty-state">هنوز پروژه‌ای ثبت نشده. اولین پروژه‌تان را بسازید.</div>}</div></section> }

function Tasks({ tasks, onNewTask }) { return <section className="page"><div className="page-heading"><div><p className="eyebrow">پیگیری کارها</p><h1>تسک‌های من</h1><p className="subtle">کارهای خود را اولویت‌بندی و پیگیری کنید.</p></div><button className="primary-btn" onClick={onNewTask}>{icons.plus} تسک جدید</button></div><div className="task-table panel">{tasks.length ? <>{tasks.map(task => <div className="task-row" key={task.id}><span className={`task-status ${task.status === 'Done' ? 'done' : ''}`}>{task.status === 'InProgress' ? 'در حال انجام' : task.status === 'Done' ? 'انجام شده' : 'برای انجام'}</span><div className="task-copy"><strong>{task.title}</strong><span>{task.project || 'بدون پروژه'}</span></div><span className={`priority ${String(task.priority).toLowerCase()}`}>{task.priority === 'High' ? 'زیاد' : task.priority === 'Low' ? 'کم' : 'متوسط'}</span><span className="task-deadline">{formatDate(task.deadLine)}</span></div>)}</> : <div className="empty-inline">هنوز تسکی برای نمایش وجود ندارد.</div>}</div></section> }

function ProjectRow({ project }) { return <div className="project-row"><span className="project-color" /><div className="project-row-copy"><strong>{project.title}</strong><span>{project.description || 'بدون توضیحات'}</span></div><div className="row-progress"><div className="progress-track"><div style={{ width: `${50 + Math.random() * 40}%` }} /></div><span>در حال انجام</span></div><span className="row-date">{formatDate(project.createdAt)}</span>{icons.arrow}</div> }
function formatDate(date) { if (!date) return '—'; return new Intl.DateTimeFormat('fa-IR', { month: 'short', day: 'numeric' }).format(new Date(date)) }
function EmptyView({ icon, title, text, action, onAction }) { return <section className="page"><div className="empty-page"><div className="empty-icon">{icon}</div><h1>{title}</h1><p>{text}</p>{action && <button className="primary-btn" onClick={onAction}>{icons.plus}{action}</button>}</div></section> }

function Auth({ onSuccess }) {
  const [mode, setMode] = useState('login'); const [form, setForm] = useState({ fullname: '', email: '', password: '' }); const [error, setError] = useState(''); const [busy, setBusy] = useState(false)
  const submit = async e => { e.preventDefault(); setBusy(true); setError(''); try { const result = await api(mode === 'login' ? '/Auth/Login' : '/Auth/Register', { method: 'POST', body: JSON.stringify(mode === 'login' ? { email: form.email, password: form.password } : { fullname: form.fullname, email: form.email, password: form.password, role: 1 }) }); if (mode === 'register') { setMode('login'); setError('حساب ساخته شد؛ حالا وارد شوید.'); } else { localStorage.setItem('taskflow_token', result.token); localStorage.setItem('taskflow_user', JSON.stringify(result)); onSuccess(result) } } catch (err) { setError(err.message) } finally { setBusy(false) } }
  return <div className="auth-screen"><div className="auth-decoration"><div className="orb orb-one" /><div className="orb orb-two" /><div className="auth-copy"><div className="brand light"><span className="brand-mark">✓</span><span>Task<span>Flow</span></span></div><h1>کارها را به<br /><em>نتیجه</em> تبدیل کن.</h1><p>یک فضای ساده و متمرکز برای مدیریت پروژه‌ها، هماهنگی تیم و رسیدن به هدف‌ها.</p><div className="quote">«تمرکز یعنی نه گفتن به صد کار خوب دیگر.»<small>— استیو جابز</small></div></div></div><div className="auth-card"><div className="mobile-brand"><div className="brand"><span className="brand-mark">✓</span><span>Task<span>Flow</span></span></div></div><div className="auth-card-head"><p className="eyebrow">خوش آمدید</p><h2>{mode === 'login' ? 'ورود به فضای کاری' : 'ساخت حساب کاربری'}</h2><p>{mode === 'login' ? 'برای ادامه اطلاعات خود را وارد کنید.' : 'چند قدم تا نظم بیشتر در کارها فاصله دارید.'}</p></div><form onSubmit={submit}>{mode === 'register' && <label>نام و نام خانوادگی<input required value={form.fullname} onChange={e => setForm({ ...form, fullname: e.target.value })} placeholder="مثلاً علی رضایی" /></label>}<label>ایمیل<input required type="email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} placeholder="you@example.com" dir="ltr" /></label><label>رمز عبور<input required type="password" minLength="6" value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} placeholder="حداقل ۶ کاراکتر" dir="ltr" /></label>{error && <div className={`form-message ${error.includes('موفق') ? 'success' : ''}`}>{error}</div>}<button className="primary-btn auth-submit" disabled={busy}>{busy ? 'در حال بررسی...' : mode === 'login' ? 'ورود' : 'ثبت‌نام'} {icons.arrow}</button></form><p className="switch-auth">{mode === 'login' ? 'حساب کاربری ندارید؟' : 'قبلاً ثبت‌نام کرده‌اید؟'} <button onClick={() => { setMode(mode === 'login' ? 'register' : 'login'); setError('') }}>{mode === 'login' ? 'ثبت‌نام کنید' : 'وارد شوید'}</button></p><small className="api-note">اتصال API: {API_URL}</small></div></div>
}

function ProjectModal({ onClose, onCreated }) { const [form, setForm] = useState({ title: '', description: '' }); const [error, setError] = useState(''); const [busy, setBusy] = useState(false); const submit = async e => { e.preventDefault(); setBusy(true); try { const result = await api('/Project/CreateProject', { method: 'POST', body: JSON.stringify(form) }); onCreated(result) } catch (err) { setError(err.message) } finally { setBusy(false) } }; return <Modal title="پروژه جدید" subtitle="یک فضای تازه برای پیش‌برد کارها بسازید." onClose={onClose}><form onSubmit={submit} className="modal-form"><label>عنوان پروژه<input required autoFocus value={form.title} onChange={e => setForm({ ...form, title: e.target.value })} placeholder="مثلاً طراحی وب‌سایت" /></label><label>توضیحات <span className="optional">اختیاری</span><textarea rows="4" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="هدف این پروژه چیست؟" /></label>{error && <div className="form-message">{error}</div>}<div className="modal-actions"><button type="button" className="secondary-btn" onClick={onClose}>انصراف</button><button className="primary-btn" disabled={busy}>{busy ? 'در حال ساخت...' : 'ساخت پروژه'} {icons.arrow}</button></div></form></Modal> }
function TaskModal({ project, projects, onClose, onCreated }) { const [form, setForm] = useState({ title: '', description: '', status: 1, priority: 2, deadLine: '', projectId: project?.projectId || projects[0]?.projectId || '', assignedToUserId: '00000000-0000-0000-0000-000000000000' }); const [error, setError] = useState(''); const [busy, setBusy] = useState(false); const submit = async e => { e.preventDefault(); setBusy(true); try { await api('/Task/CreateTask', { method: 'POST', body: JSON.stringify({ ...form, status: Number(form.status), priority: Number(form.priority), deadLine: form.deadLine ? new Date(form.deadLine).toISOString() : new Date().toISOString() }) }); onCreated() } catch (err) { setError(err.message) } finally { setBusy(false) } }; return <Modal title="تسک جدید" subtitle="کار بعدی را با جزئیات مشخص کنید." onClose={onClose}><form onSubmit={submit} className="modal-form"><label>عنوان تسک<input required autoFocus value={form.title} onChange={e => setForm({ ...form, title: e.target.value })} placeholder="مثلاً آماده‌سازی گزارش هفتگی" /></label><div className="form-row"><label>پروژه<select required value={form.projectId} onChange={e => setForm({ ...form, projectId: e.target.value })}>{projects.map(p => <option value={p.projectId} key={p.projectId}>{p.title}</option>)}</select></label><label>اولویت<select value={form.priority} onChange={e => setForm({ ...form, priority: e.target.value })}><option value="1">کم</option><option value="2">متوسط</option><option value="3">زیاد</option></select></label></div><label>توضیحات <span className="optional">اختیاری</span><textarea rows="3" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="جزئیات این کار..." /></label><label>موعد انجام<input type="date" value={form.deadLine} onChange={e => setForm({ ...form, deadLine: e.target.value })} /></label>{error && <div className="form-message">{error}</div>}<div className="modal-actions"><button type="button" className="secondary-btn" onClick={onClose}>انصراف</button><button className="primary-btn" disabled={busy}>{busy ? 'در حال ساخت...' : 'ساخت تسک'} {icons.arrow}</button></div></form></Modal> }
function Modal({ title, subtitle, onClose, children }) { return <div className="modal-backdrop" onMouseDown={e => e.target === e.currentTarget && onClose()}><div className="modal"><button className="modal-close" onClick={onClose}>{icons.close}</button><div className="modal-head"><div className="modal-symbol">✦</div><div><h2>{title}</h2><p>{subtitle}</p></div></div>{children}</div></div> }

createRoot(document.getElementById('root')).render(<App />)
