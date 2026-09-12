import { useMemo, useState } from 'react'
import { Bell, ChevronDown, CircleAlert, Clock3, Search, TicketCheck } from 'lucide-react'

type Status = 'Pendiente' | 'En pausa' | 'Resuelto' | 'Atrasado'
type Ticket = { id: number; title: string; requester: string; technician: string; priority: 'Crítica' | 'Alta' | 'Media' | 'Baja'; status: Status; due: string }
const seed: Ticket[] = [
  { id: 1042, title: 'VPN sin conexión', requester: 'María López', technician: 'Carlos Ruiz', priority: 'Alta', status: 'Pendiente', due: 'Hoy, 16:00' },
  { id: 1041, title: 'Acceso a carpeta compartida', requester: 'Diego Hernández', technician: 'Sin asignar', priority: 'Media', status: 'En pausa', due: '13 sep, 10:00' },
  { id: 1038, title: 'Servidor de archivos no responde', requester: 'Operaciones', technician: 'Carlos Ruiz', priority: 'Crítica', status: 'Atrasado', due: 'Ayer, 18:00' },
  { id: 1035, title: 'Monitor externo sin señal', requester: 'Andrea Soto', technician: 'Lucía Díaz', priority: 'Baja', status: 'Resuelto', due: '11 sep, 14:30' },
]
const tone = { 'Crítica': 'critical', 'Alta': 'high', 'Media': 'medium', 'Baja': 'low', 'Pendiente': 'pending', 'En pausa': 'paused', 'Resuelto': 'resolved', 'Atrasado': 'overdue' }

export default function App() {
  const [filter, setFilter] = useState<Status | 'Todos'>('Todos'); const [query, setQuery] = useState('')
  const tickets = useMemo(() => seed.filter(t => (filter === 'Todos' || t.status === filter) && `${t.title} ${t.requester}`.toLowerCase().includes(query.toLowerCase())), [filter, query])
  const metrics = [{ label: 'Abiertos', value: '12', note: '3 requieren atención', icon: TicketCheck }, { label: 'Pendientes', value: '8', note: 'En progreso o sin asignar', icon: Clock3 }, { label: 'Críticos', value: '2', note: '1 vencido', icon: CircleAlert }, { label: 'Resueltos', value: '24', note: 'Este mes', icon: TicketCheck }]
  return <main className="shell">
    <aside><div className="brand"><span>i</span> Incidentes</div><nav><a className="active">Resumen</a><a>Tickets</a><a>Equipo</a><a>Configuración</a></nav><div className="profile"><div className="avatar">AM</div><div><b>Ana Martínez</b><small>Administradora</small></div><ChevronDown size={16}/></div></aside>
    <section className="content"><header><div><p className="eyebrow">OPERACIONES · 12 DE SEPTIEMBRE</p><h1>Resumen de incidentes</h1><p className="subtle">Supervisa y atiende los tickets del equipo.</p></div><div className="header-actions"><button className="icon"><Bell size={19}/></button><button className="primary">+ Nuevo incidente</button></div></header>
      <div className="metrics">{metrics.map(({ label, value, note, icon: Icon }) => <article className="metric" key={label}><div><p>{label}</p><strong>{value}</strong><small>{note}</small></div><Icon size={19}/></article>)}</div>
      <section className="workspace"><div className="section-head"><div><h2>Tickets recientes</h2><p className="subtle">Actividad más reciente del equipo</p></div><button className="plain">Ver todos →</button></div><div className="toolbar"><label className="search"><Search size={17}/><input placeholder="Buscar por ticket o solicitante" value={query} onChange={e => setQuery(e.target.value)}/></label><div className="filters">{(['Todos', 'Pendiente', 'En pausa', 'Atrasado', 'Resuelto'] as const).map(x => <button className={filter === x ? 'selected' : ''} onClick={() => setFilter(x)} key={x}>{x}</button>)}</div></div>
        <div className="table-wrap"><table><thead><tr><th>Ticket</th><th>Solicitante</th><th>Asignado a</th><th>Prioridad</th><th>Estado</th><th>Fecha límite</th></tr></thead><tbody>{tickets.map(t => <tr key={t.id}><td><span className="ticket-id">#{t.id}</span><b>{t.title}</b></td><td>{t.requester}</td><td>{t.technician}</td><td><span className={`tag ${tone[t.priority]}`}>{t.priority}</span></td><td><span className={`status ${tone[t.status]}`}><i/> {t.status}</span></td><td>{t.due}</td></tr>)}</tbody></table>{!tickets.length && <p className="empty">No hay tickets con estos filtros.</p>}</div>
      </section></section>
  </main>
}

