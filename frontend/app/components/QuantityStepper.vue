<script setup>
const props = defineProps({
  modelValue: { type: Number, required: true },
  min: { type: Number, default: 1 },
  max: { type: Number, default: 99 },
})

const emit = defineEmits(['update:modelValue'])

const decrement = () => {
  if (props.modelValue > props.min) emit('update:modelValue', props.modelValue - 1)
}

const increment = () => {
  if (props.modelValue < props.max) emit('update:modelValue', props.modelValue + 1)
}
</script>

<template>
  <div class="qty-stepper">
    <button
      type="button"
      class="qty-btn"
      :disabled="modelValue <= min"
      aria-label="Decrease quantity"
      @click="decrement"
    >
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><line x1="5" y1="12" x2="19" y2="12"/></svg>
    </button>
    <span class="qty-value">{{ modelValue }}</span>
    <button
      type="button"
      class="qty-btn"
      :disabled="modelValue >= max"
      aria-label="Increase quantity"
      @click="increment"
    >
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
    </button>
  </div>
</template>

<style scoped>
.qty-stepper {
  display: inline-flex;
  align-items: center;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  background-color: #fff;
  box-shadow: 2px 2px 0px rgba(0, 49, 83, 0.15);
  overflow: hidden;
}

.qty-btn {
  background: transparent;
  border: none;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: var(--color-prussian-blue);
  transition: background-color 0.2s ease;
}

.qty-btn:hover:not(:disabled) {
  background-color: rgba(0, 49, 83, 0.08);
}

.qty-btn:disabled {
  opacity: 0.35;
  cursor: not-allowed;
}

.qty-btn svg {
  width: 16px;
  height: 16px;
}

.qty-value {
  font-family: var(--font-sans);
  font-weight: 600;
  color: var(--color-prussian-blue);
  font-size: 1rem;
  min-width: 40px;
  text-align: center;
  user-select: none;
}
</style>
