<script setup lang="ts">
import { computed, ref } from 'vue'

const props = withDefaults(defineProps<{
  productId: number
  productTitle?: string
  variant?: 'corner' | 'inline'
}>(), {
  variant: 'corner',
})

const { isLoggedIn } = useAuth()
const { has, toggle } = useFavorites()
const router = useRouter()
const route = useRoute()
const toast = useToast()

const busy = ref(false)
const isFavorited = computed(() => has(props.productId))

const label = computed(() =>
  isFavorited.value ? 'Remove from favorites' : 'Add to favorites',
)

const handleClick = async (event: MouseEvent) => {
  event.preventDefault()
  event.stopPropagation()

  if (!isLoggedIn.value) {
    toast.error('Sign in to save favorites.')
    router.push(`/login?redirect=${encodeURIComponent(route.fullPath)}`)
    return
  }

  if (busy.value) return
  busy.value = true
  const wasFavorited = isFavorited.value
  try {
    await toggle(props.productId)
    const name = props.productTitle ?? 'item'
    toast.success(
      wasFavorited
        ? `Removed "${name}" from favorites.`
        : `Saved "${name}" to favorites.`,
    )
  } catch {
    toast.error('Could not update favorites. Please try again.')
  } finally {
    busy.value = false
  }
}
</script>

<template>
  <button
    type="button"
    class="fav-toggle"
    :class="[`fav-toggle--${variant}`, { 'is-active': isFavorited }]"
    :aria-label="label"
    :aria-pressed="isFavorited"
    :disabled="busy"
    @click="handleClick"
  >
    <svg
      viewBox="0 0 24 24"
      :fill="isFavorited ? 'currentColor' : 'none'"
      stroke="currentColor"
      stroke-width="2"
      stroke-linecap="round"
      stroke-linejoin="round"
    >
      <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
    </svg>
  </button>
</template>

<style scoped>
.fav-toggle {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  background: none;
  border: none;
  cursor: pointer;
  color: #777;
  transition: color 0.2s ease, transform 0.2s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.fav-toggle:disabled {
  cursor: wait;
  opacity: 0.6;
}

.fav-toggle:hover:not(:disabled) {
  color: #b22222;
  transform: scale(1.1);
}

.fav-toggle.is-active {
  color: #b22222;
}

.fav-toggle svg {
  width: 20px;
  height: 20px;
}

/* Corner variant — overlaid on a product card image */
.fav-toggle--corner {
  position: absolute;
  top: 0.65rem;
  right: 0.65rem;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.95);
  box-shadow: 1.5px 1.5px 0 rgba(0, 49, 83, 0.18);
  z-index: 2;
}

.fav-toggle--corner.is-active {
  background: rgba(255, 240, 240, 0.95);
}

/* Inline variant — used in cart lines */
.fav-toggle--inline {
  width: 32px;
  height: 32px;
  padding: 0.25rem;
}
</style>
