export type ToastType = 'success' | 'error' | 'info'

export interface Toast {
  id: number
  type: ToastType
  message: string
}

const MAX_TOASTS = 3
const DEFAULT_DURATION = 3500

let nextId = 1

export const useToast = () => {
  const toasts = useState<Toast[]>('toasts', () => [])

  const dismiss = (id: number) => {
    toasts.value = toasts.value.filter((t) => t.id !== id)
  }

  const show = (type: ToastType, message: string, duration = DEFAULT_DURATION) => {
    const id = nextId++
    const next = [...toasts.value, { id, type, message }]
    while (next.length > MAX_TOASTS) next.shift()
    toasts.value = next
    if (import.meta.client) {
      window.setTimeout(() => dismiss(id), duration)
    }
  }

  return {
    toasts,
    dismiss,
    success: (msg: string, d?: number) => show('success', msg, d),
    error: (msg: string, d?: number) => show('error', msg, d),
    info: (msg: string, d?: number) => show('info', msg, d),
  }
}
