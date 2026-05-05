<script setup lang="ts">
import { onMounted, onUnmounted, ref, watch } from 'vue'

interface AvatarOption {
  src: string
  label: string
}

const props = defineProps<{
  open: boolean
  current?: string
}>()

const emit = defineEmits<{
  (e: 'update:open', value: boolean): void
  (e: 'select', src: string): void
}>()

const pokeballs: AvatarOption[] = [
  { src: '/images/red_pokeball.jpg', label: 'Poké Ball' },
  { src: '/images/blue_pokeball.jpg', label: 'Great Ball' },
  { src: '/images/black_pokeball.jpg', label: 'Ultra Ball' },
]

const guardians: AvatarOption[] = [
  { src: '/images/pikachu_card.jpg', label: 'Pikachu' },
  { src: '/images/pikachu_vangogh.webp', label: 'Pikachu, after Van Gogh' },
  { src: '/images/kuma_sleep.jpeg', label: 'Sleeping Cubchoo' },
]

const selected = ref<string | undefined>(props.current)

watch(
  () => props.open,
  (open) => {
    if (open) selected.value = props.current
    if (!import.meta.client) return
    document.body.style.overflow = open ? 'hidden' : ''
  },
)

const close = () => emit('update:open', false)

const handleConfirm = () => {
  if (!selected.value) return
  emit('select', selected.value)
  close()
}

const onKeydown = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && props.open) close()
  if (e.key === 'Enter' && props.open) handleConfirm()
}

onMounted(() => {
  if (import.meta.client) document.addEventListener('keydown', onKeydown)
})

onUnmounted(() => {
  if (import.meta.client) {
    document.removeEventListener('keydown', onKeydown)
    document.body.style.overflow = ''
  }
})
</script>

<template>
  <Teleport to="body">
    <Transition name="picker-fade">
      <div v-if="open" class="picker-overlay" @click.self="close">
        <div class="picker-card" role="dialog" aria-modal="true" aria-label="Choose avatar">
          <span class="picker-eyebrow">— Pokédex Photo Booth —</span>
          <h3 class="picker-title">Choose Your Avatar</h3>
          <p class="picker-subtitle">Pick the face you want every Pokégogh patron to remember.</p>

          <section class="picker-section">
            <h4 class="picker-section-title">Choose a Pokéball</h4>
            <ul class="picker-grid">
              <li v-for="(opt, i) in pokeballs" :key="opt.src" :style="{ '--i': i }">
                <button
                  type="button"
                  class="picker-tile"
                  :class="{ selected: selected === opt.src }"
                  :aria-pressed="selected === opt.src"
                  @click="selected = opt.src"
                >
                  <img :src="opt.src" :alt="opt.label" />
                  <span class="picker-check" aria-hidden="true">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="20 6 9 17 4 12"></polyline>
                    </svg>
                  </span>
                  <span class="picker-tile-label">{{ opt.label }}</span>
                </button>
              </li>
            </ul>
          </section>

          <section class="picker-section">
            <h4 class="picker-section-title">Or a guardian</h4>
            <ul class="picker-grid">
              <li v-for="(opt, i) in guardians" :key="opt.src" :style="{ '--i': i }">
                <button
                  type="button"
                  class="picker-tile"
                  :class="{ selected: selected === opt.src }"
                  :aria-pressed="selected === opt.src"
                  @click="selected = opt.src"
                >
                  <img :src="opt.src" :alt="opt.label" />
                  <span class="picker-check" aria-hidden="true">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
                      <polyline points="20 6 9 17 4 12"></polyline>
                    </svg>
                  </span>
                  <span class="picker-tile-label">{{ opt.label }}</span>
                </button>
              </li>
            </ul>
          </section>

          <div class="picker-actions">
            <button type="button" class="btn-cancel" @click="close">Cancel</button>
            <button
              type="button"
              class="btn-confirm"
              :disabled="!selected || selected === current"
              @click="handleConfirm"
            >
              Save Avatar
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.picker-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(11, 21, 28, 0.7);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
  overflow-y: auto;
}

.picker-card {
  position: relative;
  background-color: var(--color-linen);
  border: 2px solid var(--color-prussian-blue);
  border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
  box-shadow: 6px 6px 0 rgba(0, 49, 83, 1);
  padding: clamp(1.5rem, 4vw, 2.25rem);
  width: 100%;
  max-width: 640px;
  max-height: calc(100vh - 2rem);
  overflow-y: auto;
}

.picker-card::before {
  content: '';
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(90deg, rgba(200, 195, 180, 0.08) 50%, transparent 50%),
    linear-gradient(rgba(200, 195, 180, 0.08) 50%, transparent 50%);
  background-size: 4px 4px;
  pointer-events: none;
  border-radius: inherit;
}

.picker-card > * {
  position: relative;
  z-index: 1;
}

.picker-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #c2821b;
  letter-spacing: 1.5px;
  font-size: 0.8rem;
  display: block;
  text-align: center;
  margin-bottom: 0.5rem;
}

.picker-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1.4rem, 3.5vw, 1.75rem);
  text-align: center;
  margin-bottom: 0.4rem;
}

.picker-title::after {
  content: '';
  display: block;
  width: 50px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 0.55rem auto 0;
  border-radius: 2px;
}

.picker-subtitle {
  font-family: var(--font-sans);
  font-size: 0.88rem;
  color: #6f6f6f;
  text-align: center;
  margin-bottom: 1.5rem;
}

.picker-section {
  margin-bottom: 1.25rem;
}

.picker-section-title {
  font-family: var(--font-sans);
  font-size: 0.72rem;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  color: var(--color-prussian-blue);
  margin-bottom: 0.7rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.picker-section-title::after {
  content: '';
  flex: 1;
  border-bottom: 1px dashed rgba(0, 49, 83, 0.25);
}

.picker-grid {
  list-style: none;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(96px, 1fr));
  gap: 0.85rem;
  padding: 0;
  margin: 0;
}

.picker-grid > li {
  animation: tile-rise 0.4s calc(var(--i) * 0.05s) ease both;
}

.picker-tile {
  position: relative;
  width: 100%;
  background: #fff;
  border: 2px solid rgba(0, 49, 83, 0.2);
  border-radius: 14px;
  padding: 0.5rem;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.4rem;
  transition: transform 0.25s cubic-bezier(0.34, 1.56, 0.64, 1),
    border-color 0.2s ease, box-shadow 0.25s ease;
}

.picker-tile img {
  width: 100%;
  aspect-ratio: 1 / 1;
  object-fit: cover;
  border-radius: 10px;
  display: block;
}

.picker-tile-label {
  font-family: var(--font-sans);
  font-size: 0.7rem;
  font-weight: 600;
  color: var(--color-prussian-blue);
  text-align: center;
  line-height: 1.15;
}

.picker-tile:hover {
  transform: translateY(-3px) rotate(-1deg);
  border-color: var(--color-prussian-blue);
  box-shadow: 3px 4px 0 rgba(0, 49, 83, 0.18);
}

.picker-tile.selected {
  border-color: var(--color-sunflower-yellow);
  box-shadow: 0 0 0 3px rgba(255, 197, 18, 0.35),
    3px 4px 0 rgba(0, 49, 83, 0.25);
  transform: translateY(-2px);
}

.picker-check {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: var(--color-cypress-green);
  color: #fff;
  display: grid;
  place-items: center;
  opacity: 0;
  transform: scale(0.5);
  transition: opacity 0.2s ease, transform 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.picker-check svg {
  width: 12px;
  height: 12px;
}

.picker-tile.selected .picker-check {
  opacity: 1;
  transform: scale(1);
}

.picker-actions {
  display: flex;
  gap: 0.85rem;
  justify-content: center;
  flex-wrap: wrap;
  margin-top: 1.25rem;
}

.btn-cancel,
.btn-confirm {
  flex: 1 1 140px;
  padding: 0.85rem 1.5rem;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  cursor: pointer;
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}

.btn-cancel {
  background-color: transparent;
  border: 2px solid var(--color-prussian-blue);
  color: var(--color-prussian-blue);
  box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.15);
}

.btn-cancel:hover {
  background-color: rgba(0, 49, 83, 0.05);
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0 rgba(0, 49, 83, 0.3);
}

.btn-confirm {
  background-color: var(--color-sunflower-yellow);
  border: 2px solid var(--color-prussian-blue);
  color: var(--color-prussian-blue);
  box-shadow: 3px 3px 0 rgba(0, 49, 83, 1);
}

.btn-confirm:hover:not(:disabled) {
  background-color: #ffcf33;
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0 rgba(0, 49, 83, 1);
}

.btn-confirm:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.picker-fade-enter-active,
.picker-fade-leave-active {
  transition: opacity 0.22s ease;
}
.picker-fade-enter-from,
.picker-fade-leave-to {
  opacity: 0;
}
.picker-fade-enter-active .picker-card {
  animation: picker-pop 0.36s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes picker-pop {
  from {
    opacity: 0;
    transform: translateY(20px) scale(0.94);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@keyframes tile-rise {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
