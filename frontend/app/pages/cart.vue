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

    <div class="cart-tabs" role="tablist" aria-label="Cart type">
      <button
        type="button"
        class="cart-tab"
        role="tab"
        :aria-selected="!isPreorderView"
        :class="{ active: !isPreorderView }"
        @click="view = 'standard'"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/>
          <line x1="3" y1="6" x2="21" y2="6"/>
          <path d="M16 10a4 4 0 0 1-8 0"/>
        </svg>
        <span class="cart-tab-label">Standard Cart</span>
        <span class="cart-tab-count">{{ standardCount }}</span>
      </button>
      <button
        type="button"
        class="cart-tab cart-tab--preorder"
        role="tab"
        :aria-selected="isPreorderView"
        :class="{ active: isPreorderView }"
        @click="view = 'preorder'"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
          <line x1="16" y1="2" x2="16" y2="6"/>
          <line x1="8" y1="2" x2="8" y2="6"/>
          <line x1="3" y1="10" x2="21" y2="10"/>
          <path d="M8 14h.01"/>
          <path d="M12 14h.01"/>
          <path d="M16 14h.01"/>
        </svg>
        <span class="cart-tab-label">Pre-order Cart</span>
        <span class="cart-tab-count">{{ preorderCount }}</span>
      </button>
    </div>

    <div v-if="isPreorderView && activeItems.length > 0" class="preorder-banner">
      <span class="preorder-banner-stamp" aria-hidden="true"></span>
      <div class="preorder-banner-text">
        <p class="preorder-banner-title">Pre-order Reservations</p>
        <p class="preorder-banner-desc">
          These items haven't arrived at the gallery yet. Pay a deposit today
          (% varies by card rarity) and the remaining balance when they ship.
        </p>
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

.cart-tabs {
  display: inline-flex;
  gap: 0.4rem;
  margin-bottom: 1.5rem;
}

.cart-tab {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.85rem;
  background: rgba(255, 255, 255, 0.5);
  border: 1.5px solid rgba(0, 49, 83, 0.15);
  border-radius: 999px;
  font-family: var(--font-sans);
  color: #777;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.23, 1, 0.32, 1);
}

.cart-tab svg {
  width: 15px;
  height: 15px;
  flex-shrink: 0;
}

.cart-tab-label {
  font-weight: 600;
  font-size: 0.8rem;
  letter-spacing: 0.3px;
}

.cart-tab-count {
  font-family: var(--font-sans);
  font-size: 0.7rem;
  font-weight: 700;
  min-width: 20px;
  height: 20px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 999px;
  background: rgba(0, 49, 83, 0.1);
  color: var(--color-prussian-blue);
  padding: 0 0.4rem;
}

.cart-tab:hover:not(.active) {
  border-color: rgba(0, 49, 83, 0.3);
  background: rgba(255, 255, 255, 0.75);
  color: var(--color-prussian-blue);
}

.cart-tab.active {
  background: #fff;
  color: var(--color-prussian-blue);
  border-color: var(--color-prussian-blue);
  box-shadow: 0 2px 6px rgba(0, 49, 83, 0.1);
}

.cart-tab--preorder.active {
  border-color: #b22222;
  color: #b22222;
  box-shadow: 0 2px 6px rgba(178, 34, 34, 0.15);
}

.cart-tab--preorder.active .cart-tab-count {
  background: rgba(178, 34, 34, 0.12);
  color: #b22222;
}

.preorder-banner {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  padding: 1.25rem 1.5rem;
  margin-bottom: 1.5rem;
  background: linear-gradient(
    180deg,
    rgba(178, 34, 34, 0.09) 0%,
    rgba(178, 34, 34, 0.02) 100%
  );
  border: 1.5px solid rgba(178, 34, 34, 0.25);
  border-radius: 14px;
  position: relative;
  overflow: hidden;
}

.preorder-banner::before {
  content: "";
  position: absolute;
  top: 0;
  left: 1.5rem;
  right: 1.5rem;
  height: 2px;
  background: linear-gradient(90deg, transparent, #b22222 30%, #b22222 70%, transparent);
  box-shadow: 0 0 10px rgba(178, 34, 34, 0.4);
}

.preorder-banner-stamp {
  flex-shrink: 0;
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, #fff 0 22%, #1a1a1a 22% 32%, transparent 32%),
    linear-gradient(180deg, #b22222 0 47%, #1a1a1a 47% 53%, #fff 53% 100%);
  border: 1.5px solid var(--color-prussian-blue);
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.2);
  transform: rotate(-8deg);
}

.preorder-banner-text {
  flex: 1;
  min-width: 0;
}

.preorder-banner-title {
  font-family: var(--font-serif);
  color: #b22222;
  font-size: 1.1rem;
  font-weight: 700;
  margin: 0 0 0.25rem;
  letter-spacing: 0.3px;
}

.preorder-banner-desc {
  font-family: var(--font-sans);
  color: #5c5c5c;
  font-size: 0.85rem;
  line-height: 1.5;
  margin: 0;
}

@media (max-width: 600px) {
  .preorder-banner {
    flex-direction: column;
    text-align: center;
    align-items: center;
  }
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
