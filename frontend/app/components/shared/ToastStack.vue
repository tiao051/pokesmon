<script setup>
const { toasts, dismiss } = useToast()
</script>

<template>
  <Teleport to="body">
    <div class="toast-stack" aria-live="polite" aria-atomic="true">
      <TransitionGroup name="toast-slide" tag="div">
        <div
          v-for="t in toasts"
          :key="t.id"
          class="toast"
          :class="`toast-${t.type}`"
          role="status"
        >
          <span class="toast-icon" aria-hidden="true">
            <svg v-if="t.type === 'success'" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"/></svg>
            <svg v-else-if="t.type === 'error'" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
            <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>
          </span>
          <p class="toast-message">{{ t.message }}</p>
          <button class="toast-close" aria-label="Dismiss" @click="dismiss(t.id)">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<style scoped>
.toast-stack {
  position: fixed;
  top: clamp(1rem, 3vw, 2rem);
  right: clamp(1rem, 3vw, 2rem);
  z-index: 1100;
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
  max-width: min(380px, calc(100vw - 2rem));
  pointer-events: none;
}

.toast-stack > div {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
}

.toast {
  pointer-events: auto;
  position: relative;
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: 0.85rem;
  padding: 0.85rem 1rem 0.85rem 1.35rem;
  background-color: #FBF9F2;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.25);
  font-family: var(--font-sans);
  color: var(--color-prussian-blue);
  overflow: hidden;
}

.toast::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 5px;
}

.toast-success::before { background-color: var(--color-cypress-green); }
.toast-error::before { background-color: #B22222; }
.toast-info::before { background-color: #C2821B; }

.toast-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  flex-shrink: 0;
}

.toast-success .toast-icon {
  background-color: rgba(15, 94, 60, 0.12);
  color: var(--color-cypress-green);
}
.toast-error .toast-icon {
  background-color: rgba(178, 34, 34, 0.12);
  color: #B22222;
}
.toast-info .toast-icon {
  background-color: rgba(194, 130, 27, 0.12);
  color: #C2821B;
}

.toast-icon svg {
  width: 18px;
  height: 18px;
}

.toast-message {
  font-size: 0.9rem;
  line-height: 1.45;
  margin: 0;
  color: var(--color-prussian-blue);
}

.toast-close {
  background: none;
  border: none;
  color: #888;
  cursor: pointer;
  padding: 0.25rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: color 0.2s ease, background-color 0.2s ease;
}
.toast-close:hover {
  color: var(--color-prussian-blue);
  background-color: rgba(0, 49, 83, 0.08);
}
.toast-close svg {
  width: 16px;
  height: 16px;
}

.toast-slide-enter-active,
.toast-slide-leave-active {
  transition: all 0.32s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.toast-slide-enter-from {
  opacity: 0;
  transform: translateX(20px) scale(0.96);
}
.toast-slide-leave-to {
  opacity: 0;
  transform: translateX(20px) scale(0.96);
}
.toast-slide-leave-active {
  position: absolute;
  width: calc(100% - 0px);
}
</style>
