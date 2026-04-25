<script setup>
import { onMounted, onUnmounted, watch } from 'vue'

const props = defineProps({
  open: { type: Boolean, default: false },
  title: { type: String, default: 'Are you sure?' },
  message: { type: String, default: '' },
  confirmLabel: { type: String, default: 'Confirm' },
  cancelLabel: { type: String, default: 'Cancel' },
})

const emit = defineEmits(['confirm', 'cancel', 'update:open'])

const close = () => emit('update:open', false)

const handleCancel = () => {
  close()
  emit('cancel')
}

const handleConfirm = () => {
  close()
  emit('confirm')
}

const onKeydown = (e) => {
  if (e.key === 'Escape' && props.open) handleCancel()
}

watch(
  () => props.open,
  (open) => {
    if (!import.meta.client) return
    document.body.style.overflow = open ? 'hidden' : ''
  },
)

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
    <Transition name="confirm-fade">
      <div v-if="open" class="confirm-overlay" @click.self="handleCancel">
        <div class="confirm-card" role="dialog" aria-modal="true" :aria-label="title">
          <span class="confirm-eyebrow">— a moment —</span>
          <h3 class="confirm-title">{{ title }}</h3>
          <p v-if="message" class="confirm-message">{{ message }}</p>
          <div class="confirm-actions">
            <button type="button" class="btn-cancel" @click="handleCancel">{{ cancelLabel }}</button>
            <button type="button" class="btn-confirm" @click="handleConfirm">{{ confirmLabel }}</button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.confirm-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(11, 21, 28, 0.65);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.confirm-card {
  background-color: var(--color-linen);
  border: 2px solid var(--color-prussian-blue);
  border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 1);
  padding: 2.25rem clamp(1.5rem, 4vw, 2.5rem);
  max-width: 440px;
  width: 100%;
  text-align: center;
  position: relative;
}

.confirm-card::before {
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

.confirm-eyebrow,
.confirm-title,
.confirm-message,
.confirm-actions {
  position: relative;
  z-index: 1;
}

.confirm-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  letter-spacing: 1.5px;
  font-size: 0.85rem;
  display: block;
  margin-bottom: 1rem;
}

.confirm-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1.4rem, 3.5vw, 1.75rem);
  margin-bottom: 0.75rem;
  position: relative;
  display: inline-block;
}
.confirm-title::after {
  content: '';
  display: block;
  width: 40px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 0.65rem auto 0;
  border-radius: 2px;
}

.confirm-message {
  font-family: var(--font-sans);
  color: #555;
  margin-bottom: 1.75rem;
  line-height: 1.6;
  font-size: 0.95rem;
}

.confirm-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
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
  background-color: var(--color-prussian-blue);
  border: 2px solid var(--color-prussian-blue);
  color: var(--color-linen);
  box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.4);
}
.btn-confirm:hover {
  background-color: #002240;
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0 rgba(0, 49, 83, 0.55);
}

.confirm-fade-enter-active,
.confirm-fade-leave-active {
  transition: opacity 0.22s ease;
}
.confirm-fade-enter-from,
.confirm-fade-leave-to {
  opacity: 0;
}

.confirm-fade-enter-active .confirm-card {
  animation: confirm-pop-in 0.32s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes confirm-pop-in {
  from {
    opacity: 0;
    transform: translateY(18px) scale(0.94);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
</style>
