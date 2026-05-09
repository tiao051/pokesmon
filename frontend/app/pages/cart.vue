<script setup>
import { computed, ref } from 'vue'

const {
  standardItems,
  standardCount,
  standardSubtotal,
  preorderItems,
  preorderCount,
  preorderSubtotal,
  preorderDeposit,
  preorderRemaining,
} = useCart()
const { isLoggedIn } = useAuth()
const router = useRouter()
const route = useRoute()

const initialView = (() => {
  if (route.query.view === 'preorder') return 'preorder'
  if (route.query.view === 'standard') return 'standard'
  // Smart default: if user only has preorder items, open preorder tab
  if (preorderCount.value > 0 && standardCount.value === 0) return 'preorder'
  return 'standard'
})()

const view = ref(initialView) // 'standard' | 'preorder'
const isPreorderView = computed(() => view.value === 'preorder')

const swapView = () => {
  view.value = isPreorderView.value ? 'standard' : 'preorder'
}

const swapButtonLabel = computed(() =>
  isPreorderView.value ? 'View Standard Cart' : 'View Pre-order Cart',
)
const currentTypeLabel = computed(() =>
  isPreorderView.value ? 'Pre-order Cart' : 'Standard Cart',
)

const activeItems = computed(() =>
  isPreorderView.value ? preorderItems.value : standardItems.value,
)
const activeCount = computed(() =>
  isPreorderView.value ? preorderCount.value : standardCount.value,
)

const headerSubtitle = computed(() => {
  const c = activeCount.value
  if (c === 0) return ''
  const noun = c === 1 ? 'artifact' : 'artifacts'
  return isPreorderView.value
    ? `${c} ${noun} reserved for future arrival`
    : `${c} ${noun} awaiting confirmation`
})

const emptyMessage = computed(() =>
  isPreorderView.value
    ? 'No pre-orders reserved yet. Browse upcoming arrivals to secure your place.'
    : "No artifacts have been reserved yet. Begin your collection from the museum's catalog.",
)
const emptyCtaLabel = computed(() =>
  isPreorderView.value ? 'Browse Pre-orders' : 'Browse The Collection',
)
const emptyCtaTo = computed(() =>
  isPreorderView.value ? '/products?isPreorder=true' : '/products',
)

const proceedToCheckout = () => {
  const target = `/checkout?type=${isPreorderView.value ? 'preorder' : 'standard'}`
  if (!isLoggedIn.value) {
    router.push(`/login?redirect=${encodeURIComponent(target)}`)
  } else {
    router.push(target)
  }
}

useHead({ title: 'Pending Acquisitions — PokéGogh' })
</script>

<template>
  <main class="cart-page">
    <header class="page-header">
      <span class="eyebrow gold-italic">Reserve a Place in the Collection</span>
      <h1 class="page-title">Your Pending Acquisitions</h1>
      <p v-if="headerSubtitle" class="page-subtitle">{{ headerSubtitle }}</p>
    </header>

    <div class="controls-bar">
      <div class="type-swap">
        <span class="current-type">{{ currentTypeLabel }}</span>
        <button
          type="button"
          class="swap-button"
          :aria-label="swapButtonLabel"
          @click="swapView"
        >
          <svg
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <polyline points="17 1 21 5 17 9" />
            <path d="M3 11V9a4 4 0 0 1 4-4h14" />
            <polyline points="7 23 3 19 7 15" />
            <path d="M21 13v2a4 4 0 0 1-4 4H3" />
          </svg>
          {{ swapButtonLabel }}
        </button>
      </div>

      <div v-if="standardCount + preorderCount > 0" class="counts">
        <span class="count-pill" :class="{ active: !isPreorderView }">
          Standard · {{ standardCount }}
        </span>
        <span class="count-pill" :class="{ active: isPreorderView }">
          Pre-order · {{ preorderCount }}
        </span>
      </div>
    </div>

    <EmptyState
      v-if="activeItems.length === 0"
      :title="isPreorderView ? 'No reservations yet' : 'Your gallery awaits'"
      :message="emptyMessage"
      :cta-label="emptyCtaLabel"
      :cta-to="emptyCtaTo"
    />

    <div v-else class="cart-grid">
      <div class="cart-lines">
        <CartLineItem
          v-for="item in activeItems"
          :key="item.product.id"
          :item="item"
        />
      </div>

      <div class="cart-summary-side">
        <OrderSummary
          v-if="!isPreorderView"
          :subtotal="standardSubtotal"
          :shipping="0"
          cta-label="Proceed to Acquisition"
          @cta-click="proceedToCheckout"
        >
          <template #extra>
            <p v-if="!isLoggedIn" class="auth-hint">
              You will be asked to sign in before completing your acquisition.
            </p>
          </template>
        </OrderSummary>

        <OrderSummary
          v-else
          mode="preorder"
          :subtotal="preorderSubtotal"
          :deposit="preorderDeposit"
          :remaining="preorderRemaining"
          cta-label="Reserve with Deposit"
          @cta-click="proceedToCheckout"
        >
          <template #extra>
            <p v-if="!isLoggedIn" class="auth-hint">
              You will be asked to sign in before reserving your pre-order.
            </p>
          </template>
        </OrderSummary>
      </div>
    </div>
  </main>
</template>

<style scoped>
.cart-page {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 2rem clamp(4rem, 7vw, 6rem);
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

.eyebrow.gold-italic {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 1.05rem;
  letter-spacing: 0.5px;
  display: block;
  margin-bottom: 0.75rem;
}

.page-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(2.25rem, 5vw, 3rem);
  margin-bottom: 1rem;
  position: relative;
  display: inline-block;
}
.page-title::after {
  content: '';
  display: block;
  width: 80px;
  height: 4px;
  background-color: var(--color-cypress-green);
  margin: 0.85rem auto 0;
  border-radius: 2px;
}

.page-subtitle {
  font-family: var(--font-sans);
  color: #555;
}

.controls-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px dashed rgba(0, 49, 83, 0.2);
  flex-wrap: wrap;
}

.type-swap {
  display: flex;
  align-items: center;
  gap: 0.85rem;
}

.current-type {
  font-family: var(--font-serif);
  font-style: italic;
  font-size: 0.95rem;
  color: var(--color-prussian-blue);
}

.swap-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.78rem;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  color: var(--color-prussian-blue);
  background: var(--color-sunflower-yellow);
  border: 1.5px solid var(--color-prussian-blue);
  border-radius: 999px;
  cursor: pointer;
  box-shadow: 2px 2px 0 var(--color-prussian-blue);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.swap-button:hover {
  transform: translate(-1px, -1px);
  box-shadow: 3px 3px 0 var(--color-prussian-blue);
}

.swap-button:active {
  transform: translate(1px, 1px);
  box-shadow: 1px 1px 0 var(--color-prussian-blue);
}

.swap-button svg {
  width: 14px;
  height: 14px;
}

.counts {
  display: inline-flex;
  gap: 0.5rem;
}

.count-pill {
  font-family: var(--font-sans);
  font-size: 0.78rem;
  font-weight: 600;
  letter-spacing: 0.5px;
  padding: 0.35rem 0.85rem;
  border-radius: 999px;
  background: rgba(0, 49, 83, 0.06);
  color: #777;
  border: 1.5px solid transparent;
  transition: color 0.2s ease, border-color 0.2s ease, background-color 0.2s ease;
}

.count-pill.active {
  color: var(--color-prussian-blue);
  background: #fff;
  border-color: rgba(0, 49, 83, 0.2);
}

.cart-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  align-items: start;
}

@media (min-width: 900px) {
  .cart-grid {
    grid-template-columns: 1.6fr 1fr;
    gap: 3rem;
  }
}

.cart-lines {
  background-color: rgba(255, 255, 255, 0.4);
  border-radius: 8px;
  padding: 0 1.5rem;
}

.cart-summary-side {
  position: sticky;
  top: 100px;
}

.auth-hint {
  font-family: var(--font-serif);
  font-style: italic;
  color: #B0BCC8;
  font-size: 0.85rem;
  text-align: center;
  margin: -0.5rem 0 1.25rem;
  position: relative;
  z-index: 1;
}
</style>
