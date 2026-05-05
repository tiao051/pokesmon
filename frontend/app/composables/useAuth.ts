export interface Location {
  id: string
  label: string
  address: string
  city: string
  country: string
  isDefault: boolean
}

export type LocationInput = Omit<Location, 'id' | 'isDefault'>
export type LocationPatch = Partial<LocationInput>

export interface AuthState {
  isLoggedIn: boolean
  email?: string
  username?: string
  avatar?: string
  displayName?: string
  phone?: string
  locations?: Location[]
}

export type ProfilePatch = Partial<Pick<AuthState, 'displayName' | 'phone'>>

const STORAGE_KEY = 'pokegogh-auth'
const SIMULATED_LATENCY_MS = 350
let hydrated = false

const generateId = (): string => {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID()
  }
  return `loc-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`
}

const SAMPLE_LOCATIONS: Location[] = [
  {
    id: 'sample-pallet',
    label: 'Pallet Town Cabin',
    address: 'Số 1, Đường Hướng Dương',
    city: 'Pallet Town',
    country: 'Kanto',
    isDefault: true,
  },
  {
    id: 'sample-pewter',
    label: 'Pewter Loft',
    address: 'Stone Street 22, Apt 7',
    city: 'Pewter City',
    country: 'Kanto',
    isDefault: false,
  },
]

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
    if (state.value.locations === undefined) {
      state.value = { ...state.value, locations: [...SAMPLE_LOCATIONS] }
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
    state.value = {
      isLoggedIn: true,
      email,
      username,
      locations: state.value.locations ?? [...SAMPLE_LOCATIONS],
    }
  }

  const signOut = () => {
    state.value = { isLoggedIn: false }
  }

  const setAvatar = (avatar: string) => {
    state.value = { ...state.value, avatar }
  }

  const updateProfile = (patch: ProfilePatch) => {
    const next: AuthState = { ...state.value }
    for (const key of ['displayName', 'phone'] as const) {
      if (key in patch) {
        const value = patch[key]?.trim()
        if (value) next[key] = value
        else delete next[key]
      }
    }
    state.value = next
  }

  const changePassword = async (current: string, next: string) => {
    if (!current) throw new Error('Current password is required')
    if (!next || next.length < 8) throw new Error('New password must be at least 8 characters')
    if (current === next) throw new Error('New password must differ from current password')
    await new Promise((resolve) => setTimeout(resolve, 700))
    return true
  }

  const requestPasswordReset = async (email: string) => {
    const trimmed = email.trim()
    if (!trimmed) throw new Error('Email is required')
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmed)) {
      throw new Error('Please enter a valid email address')
    }
    await new Promise((resolve) => setTimeout(resolve, 800))
    return true
  }

  // Locations API.
  // Backed by localStorage today; replace each body with a $fetch call when the
  // backend is wired (e.g. POST /api/locations, PATCH /api/locations/:id).
  // Method signatures are the contract — keep them stable.

  const validateLocationInput = (input: LocationInput | LocationPatch) => {
    for (const key of ['label', 'address', 'city', 'country'] as const) {
      const v = input[key]
      if (v !== undefined && !v.trim()) {
        throw new Error(`${key} cannot be empty`)
      }
    }
  }

  const addLocation = async (input: LocationInput): Promise<Location> => {
    validateLocationInput(input)
    await new Promise((resolve) => setTimeout(resolve, SIMULATED_LATENCY_MS))
    const existing = state.value.locations ?? []
    const created: Location = {
      id: generateId(),
      label: input.label.trim(),
      address: input.address.trim(),
      city: input.city.trim(),
      country: input.country.trim(),
      isDefault: existing.length === 0,
    }
    state.value = { ...state.value, locations: [...existing, created] }
    return created
  }

  const updateLocation = async (id: string, patch: LocationPatch): Promise<Location> => {
    validateLocationInput(patch)
    await new Promise((resolve) => setTimeout(resolve, SIMULATED_LATENCY_MS))
    const existing = state.value.locations ?? []
    const idx = existing.findIndex((l) => l.id === id)
    if (idx === -1) throw new Error('Location not found')
    const current = existing[idx]!
    const updated: Location = {
      ...current,
      ...(patch.label !== undefined && { label: patch.label.trim() }),
      ...(patch.address !== undefined && { address: patch.address.trim() }),
      ...(patch.city !== undefined && { city: patch.city.trim() }),
      ...(patch.country !== undefined && { country: patch.country.trim() }),
    }
    const nextList = [...existing]
    nextList[idx] = updated
    state.value = { ...state.value, locations: nextList }
    return updated
  }

  const removeLocation = async (id: string): Promise<void> => {
    await new Promise((resolve) => setTimeout(resolve, SIMULATED_LATENCY_MS))
    const existing = state.value.locations ?? []
    const target = existing.find((l) => l.id === id)
    if (!target) return
    let nextList = existing.filter((l) => l.id !== id)
    if (target.isDefault && nextList.length > 0) {
      nextList = nextList.map((l, i) => ({ ...l, isDefault: i === 0 }))
    }
    state.value = { ...state.value, locations: nextList }
  }

  const setDefaultLocation = async (id: string): Promise<void> => {
    await new Promise((resolve) => setTimeout(resolve, SIMULATED_LATENCY_MS))
    const existing = state.value.locations ?? []
    if (!existing.some((l) => l.id === id)) throw new Error('Location not found')
    state.value = {
      ...state.value,
      locations: existing.map((l) => ({ ...l, isDefault: l.id === id })),
    }
  }

  return {
    isLoggedIn: computed(() => state.value.isLoggedIn),
    email: computed(() => state.value.email),
    username: computed(() => state.value.username),
    avatar: computed(() => state.value.avatar),
    storedDisplayName: computed(() => state.value.displayName),
    phone: computed(() => state.value.phone),
    locations: computed<Location[]>(() => state.value.locations ?? []),
    defaultLocation: computed<Location | undefined>(() =>
      (state.value.locations ?? []).find((l) => l.isDefault),
    ),
    signIn,
    signOut,
    setAvatar,
    updateProfile,
    changePassword,
    requestPasswordReset,
    addLocation,
    updateLocation,
    removeLocation,
    setDefaultLocation,
  }
}
