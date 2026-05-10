<script setup>
import { computed } from 'vue'
import { formatPrice } from '../../utils/format'

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
  isPreorder.value ? 'Pay Today (Deposit)' : 'Total',
)
const headlineValue = computed(() =>
  isPreorder.value ? props.deposit : total.value,
)
const titleText = computed(() =>
  isPreorder.value ? 'Pre-order Summary' : 'Order Summary',
)
const eyebrowText = computed(() =>
  isPreorder.value ? '— Pre-order Receipt —' : '— Order Receipt —',
)

const POKEAPI_ITEM_BASE = 'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items'
const stampSrc = computed(() =>
  isPreorder.value
    ? `${POKEAPI_ITEM_BASE}/timer-ball.png`
    : `${POKEAPI_ITEM_BASE}/heal-ball.png`,
)

const handleClick = () => {
  if (!props.ctaDisabled) emit('cta-click')
}
</script>

<template>
  <aside class="order-summary">
    <img
      :src="stampSrc"
      alt=""
      aria-hidden="true"
      class="summary-stamp"
      loading="lazy"
      decoding="async"
    />

    <div class="summary-header">
      <span class="summary-eyebrow">{{ eyebrowText }}</span>
      <h3 class="summary-title">{{ titleText }}</h3>
    </div>

    <div class="summary-rows">
      <div class="summary-row">
        <span class="row-label">Subtotal</span>
        <span class="row-value">{{ formatPrice(subtotal) }}</span>
      </div>

      <template v-if="isPreorder">
        <div class="summary-row">
          <span class="row-label">Pay on Delivery</span>
          <span class="row-value">{{ formatPrice(remaining) }}</span>
        </div>
      </template>
      <template v-else>
        <div class="summary-row">
          <span class="row-label">Shipping</span>
          <span class="row-value">{{ shipping > 0 ? formatPrice(shipping) : 'Free' }}</span>
        </div>
        <div v-if="tax > 0" class="summary-row">
          <span class="row-label">Tax</span>
          <span class="row-value">{{ formatPrice(tax) }}</span>
        </div>
      </template>
    </div>

    <div class="summary-total">
      <span class="total-label">{{ headlineLabel }}</span>
      <span class="total-value">{{ formatPrice(headlineValue) }}</span>
    </div>

    <p v-if="isPreorder" class="preorder-note">
      Deposit % is based on card rarity. You pay the rest when the item ships.
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

/* Pokeball seal stamp — pixel-art ball from PokeAPI */
.summary-stamp {
  position: absolute;
  top: 0.85rem;
  right: 1.1rem;
  width: 48px;
  height: 48px;
  object-fit: contain;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
  filter: drop-shadow(1.5px 2px 3px rgba(0, 49, 83, 0.3));
  transform: rotate(-10deg);
  z-index: 2;
  pointer-events: none;
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
