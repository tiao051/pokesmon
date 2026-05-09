<script setup>
import { computed } from 'vue'
import { formatPrice } from '../types/product'

const props = defineProps({
  mode: { type: String, default: 'standard' }, // 'standard' | 'preorder'
  subtotal: { type: Number, required: true },
  shipping: { type: Number, default: 0 },
  tax: { type: Number, default: 0 },
  deposit: { type: Number, default: 0 },
  remaining: { type: Number, default: 0 },
  ctaLabel: { type: String, default: '' },
  ctaTo: { type: String, default: '' },
  ctaDisabled: { type: Boolean, default: false },
})

const emit = defineEmits(['cta-click'])

const isPreorder = computed(() => props.mode === 'preorder')
const total = computed(() => props.subtotal + props.shipping + props.tax)
const headlineLabel = computed(() =>
  isPreorder.value ? 'Deposit Today' : 'Acquisition Total',
)
const headlineValue = computed(() =>
  isPreorder.value ? props.deposit : total.value,
)
const titleText = computed(() =>
  isPreorder.value ? 'Reservation Summary' : 'Acquisition Summary',
)
const eyebrowText = computed(() =>
  isPreorder.value ? "— Trainer's Reservation —" : "— Trainer's Receipt —",
)

const handleClick = () => {
  if (!props.ctaDisabled) emit('cta-click')
}
</script>

<template>
  <aside class="order-summary" :class="{ 'is-preorder': isPreorder }">
    <div class="summary-stamp" aria-hidden="true"></div>

    <div class="summary-header">
      <span class="summary-eyebrow">{{ eyebrowText }}</span>
      <h3 class="summary-title">{{ titleText }}</h3>
    </div>

    <div class="summary-rows">
      <div class="summary-row">
        <span class="row-label">{{ isPreorder ? 'Reservation Subtotal' : 'Subtotal' }}</span>
        <span class="row-value">{{ formatPrice(subtotal) }}</span>
      </div>

      <template v-if="isPreorder">
        <div class="summary-row">
          <span class="row-label">Due at Delivery</span>
          <span class="row-value">{{ formatPrice(remaining) }}</span>
        </div>
      </template>
      <template v-else>
        <div class="summary-row">
          <span class="row-label">Pokémart Postage</span>
          <span class="row-value">{{ shipping > 0 ? formatPrice(shipping) : 'Complimentary' }}</span>
        </div>
        <div v-if="tax > 0" class="summary-row">
          <span class="row-label">Estimated Tax</span>
          <span class="row-value">{{ formatPrice(tax) }}</span>
        </div>
      </template>
    </div>

    <div class="summary-total">
      <span class="total-label">{{ headlineLabel }}</span>
      <span class="total-value">{{ formatPrice(headlineValue) }}</span>
    </div>

    <p v-if="isPreorder" class="preorder-note">
      Deposit varies by card rarity. The remaining balance is collected upon delivery.
    </p>

    <slot name="extra" />

    <NuxtLink
      v-if="ctaLabel && ctaTo && !ctaDisabled"
      :to="ctaTo"
      class="summary-cta"
    >
      {{ ctaLabel }}
    </NuxtLink>
    <button
      v-else-if="ctaLabel"
      type="button"
      class="summary-cta"
      :disabled="ctaDisabled"
      @click="handleClick"
    >
      {{ ctaLabel }}
    </button>
  </aside>
</template>

<style scoped>
.order-summary {
  background-color: #FBF9F2;
  color: var(--color-brush-dark);
  padding: 2rem 1.85rem;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
  position: relative;
  overflow: hidden;
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.15);
}

/* Subtle linen-canvas texture, matching the body */
.order-summary::before {
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

/* Pokeball stamp matching the patron's manuscript on the left */
.summary-stamp {
  position: absolute;
  top: 1rem;
  right: 1.25rem;
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, #FBF9F2 0 22%, #1A1A1A 22% 32%, transparent 32%),
    linear-gradient(180deg, #B22222 0 47%, #1A1A1A 47% 53%, #FBF9F2 53% 100%);
  border: 1.5px solid var(--color-prussian-blue);
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.25);
  transform: rotate(-8deg);
  z-index: 2;
}

.summary-header,
.summary-rows,
.summary-total,
.summary-cta {
  position: relative;
  z-index: 1;
}

.summary-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 0.9rem;
  letter-spacing: 1.5px;
  display: block;
  margin-bottom: 0.4rem;
}

.summary-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: 1.5rem;
  margin-bottom: 1.5rem;
  position: relative;
  display: inline-block;
}
.summary-title::after {
  content: '';
  display: block;
  width: 50px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin-top: 0.5rem;
  border-radius: 2px;
}

.summary-rows {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
  padding-bottom: 1.25rem;
  border-bottom: 1px dashed rgba(0, 49, 83, 0.25);
  margin-bottom: 1.25rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  font-family: var(--font-sans);
  font-size: 0.95rem;
}

.row-label {
  color: #5C5C5C;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.78rem;
}

.row-value {
  color: var(--color-prussian-blue);
  font-weight: 600;
}

.summary-total {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  margin-bottom: 1.75rem;
  padding-top: 0.25rem;
}

.total-label {
  font-family: var(--font-serif);
  font-style: italic;
  font-size: 1.05rem;
  color: var(--color-prussian-blue);
  letter-spacing: 0.3px;
}

.total-value {
  font-family: var(--font-serif);
  color: var(--color-cypress-green);
  font-weight: 700;
  font-size: 2rem;
  letter-spacing: -0.5px;
}

.summary-cta {
  display: block;
  width: 100%;
  padding: 1rem;
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 1rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  text-align: center;
  text-decoration: none;
  cursor: pointer;
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.2);
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.summary-cta:hover:not(:disabled) {
  background: linear-gradient(135deg, #F5B341 0%, #E6A039 50%, #B07212 100%);
  transform: translateY(-2px) scale(1.02);
  letter-spacing: 1.5px;
  box-shadow:
    inset 1px 2px 4px rgba(255,255,255,0.55),
    0 0 22px rgba(245, 176, 65, 0.45),
    0 6px 16px rgba(0, 0, 0, 0.25);
}

.summary-cta:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.preorder-note {
  font-family: var(--font-serif);
  font-style: italic;
  font-size: 0.78rem;
  color: #7a6a55;
  text-align: center;
  margin: -0.5rem 0 1.25rem;
  position: relative;
  z-index: 1;
  line-height: 1.5;
}
</style>
