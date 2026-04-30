export interface AuthState {
  isLoggedIn: boolean
  email?: string
  username?: string
}

const STORAGE_KEY = 'pokegogh-auth'
let hydrated = false

export const useAuth = () => {
  const state = useState<AuthState>('auth', () => ({ isLoggedIn: false }))

  if (import.meta.client && !hydrated) {
    hydrated = true
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (raw) {
        const parsed = JSON.parse(raw) as AuthState
        if (parsed && typeof parsed.isLoggedIn === 'boolean') state.value = parsed
      }
    } catch {
      // ignore
    }
    watch(
      state,
      (v) => {
        try {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(v))
        } catch {
          // ignore
        }
      },
      { deep: true },
    )
  }

  const signIn = (email: string, username?: string) => {
    state.value = { isLoggedIn: true, email, username }
  }

  const signOut = () => {
    state.value = { isLoggedIn: false }
  }

  return {
    isLoggedIn: computed(() => state.value.isLoggedIn),
    email: computed(() => state.value.email),
    username: computed(() => state.value.username),
    signIn,
    signOut,
  }
}
