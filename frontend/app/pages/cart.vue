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

const view = ref(initialView)
const isPreorderView = computed(() => view.value === 'preorder')

const activeItems = computed(() =>
  isPreorderView.value ? preorderItems.value : standardItems.value,
)

const headerSubtitle = computed(() => {
  const c = isPreorderView.value ? preorderCount.value : standardCount.value
  if (c === 0) return ''
  const noun = c === 1 ? 'item' : 'items'
  return isPreorderView.value
    ? `${c} ${noun} reserved for arrival`
    : `${c} ${noun} ready to check out`
})

const emptyMessage = computed(() =>
  isPreorderView.value
    ? 'No pre-orders yet. Browse upcoming items to reserve your spot.'
    : 'Your cart is empty. Start shopping to add items.',
)
const emptyCtaLabel = computed(() =>
  isPreorderView.value ? 'Browse Pre-orders' : 'Browse Items',
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

useHead({ title: 'Your Cart — PokéGogh' })
</script>

<template>
  <main class="cart-page">
    <header v-if="activeItems.length > 0" class="cart-hero">
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/cherish-ball.png"
        alt=""
        aria-hidden="true"
        class="cart-hero-stamp"
        loading="eager"
        decoding="async"
      />
      <div class="cart-hero-text">
        <p class="cart-hero-eyebrow">— Items Ready to Check Out —</p>
        <h1 class="cart-hero-title">Your Cart</h1>
        <p v-if="headerSubtitle" class="cart-hero-subtitle">
          {{ headerSubtitle }} — Delibird is watching over your bag.
        </p>
      </div>
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/225.png"
        alt="Delibird, our shop's courier"
        class="cart-hero-mascot"
        loading="lazy"
        decoding="async"
      />
    </header>

    <header v-else class="page-header">
      <span class="eyebrow gold-italic">Items Ready to Check Out</span>
      <h1 class="page-title">Your Cart</h1>
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
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/timer-ball.png"
        alt=""
        aria-hidden="true"
        class="preorder-banner-stamp"
        loading="lazy"
        decoding="async"
      />
      <div class="preorder-banner-text">
        <p class="preorder-banner-title">Pre-order Items</p>
        <p class="preorder-banner-desc">
          These items haven't arrived yet. Pay a deposit today
          (% based on card rarity) and the rest when they ship.
        </p>
      </div>
    </div>

    <div v-if="activeItems.length === 0" class="cart-empty">
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/446.png"
        alt="A hungry Munchlax"
        class="cart-empty-mascot"
        loading="lazy"
        decoding="async"
      />
      <h2 class="cart-empty-title">
        {{ isPreorderView ? 'Munchlax says: nothing reserved yet' : 'Munchlax is waiting to be fed' }}
      </h2>
      <p class="cart-empty-msg">{{ emptyMessage }}</p>
      <NuxtLink :to="emptyCtaTo" class="cart-empty-cta">{{ emptyCtaLabel }}</NuxtLink>
    </div>

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
          cta-label="Go to Checkout"
          @cta-click="proceedToCheckout"
        >
          <template #extra>
            <p v-if="!isLoggedIn" class="auth-hint">
              You'll be asked to sign in before checkout.
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
              You'll be asked to sign in before pre-ordering.
            </p>
          </template>
        </OrderSummary>
      </div>
    </div>
  </main>
</template>

<style scoped>
.cart-page {
  max-width: clamp(var(--container-max), 90vw, 1500px);
  margin: 0 auto;
  padding: 0 2rem clamp(4rem, 7vw, 6rem);
}

.page-header {
  text-align: center;
  margin-bottom: 2rem;
}

/* Cart hero — Cherish Ball stamp + Delibird mascot */
.cart-hero {
  position: relative;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 1.75rem 1.75rem 1.75rem 1.75rem;
  margin-bottom: 1.5rem;
  background:
    radial-gradient(circle at top right, rgba(0, 49, 83, 0.08) 0%, transparent 60%),
    linear-gradient(180deg, rgba(0, 49, 83, 0.07) 0%, rgba(0, 49, 83, 0.01) 100%);
  border: 1.5px solid rgba(0, 49, 83, 0.18);
  border-radius: 18px;
  overflow: hidden;
  min-height: 160px;
}

.cart-hero::before {
  content: "";
  position: absolute;
  top: 0;
  left: 1.5rem;
  right: 1.5rem;
  height: 3px;
  background: linear-gradient(90deg, transparent, var(--color-prussian-blue) 30%, var(--color-prussian-blue) 70%, transparent);
  box-shadow: 0 0 12px rgba(0, 49, 83, 0.4);
}

.cart-hero-stamp {
  position: relative;
  z-index: 2;
  flex-shrink: 0;
  width: 72px;
  height: 72px;
  object-fit: contain;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
  filter: drop-shadow(2px 3px 4px rgba(0, 49, 83, 0.3));
  transform: rotate(-8deg);
}

.cart-hero-text {
  position: relative;
  z-index: 2;
  flex: 1;
  min-width: 0;
}

.cart-hero-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 0.95rem;
  letter-spacing: 1px;
  margin: 0 0 0.3rem;
}

.cart-hero-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1.75rem, 4vw, 2.4rem);
  margin: 0;
  line-height: 1.15;
  letter-spacing: -0.5px;
}

.cart-hero-title::after {
  content: "";
  display: block;
  width: 60px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin-top: 0.6rem;
  border-radius: 2px;
}

.cart-hero-subtitle {
  font-family: var(--font-sans);
  color: #555;
  font-size: 0.9rem;
  line-height: 1.5;
  margin: 0.85rem 0 0;
  max-width: 520px;
}

/* Delibird mascot — courier carrying your acquisitions */
.cart-hero-mascot {
  position: relative;
  z-index: 2;
  flex-shrink: 0;
  width: 140px;
  height: 140px;
  object-fit: contain;
  transform-origin: center bottom;
  filter: drop-shadow(2px 4px 8px rgba(0, 49, 83, 0.25));
  animation: delibird-hop 3.5s ease-in-out infinite;
}

@keyframes delibird-hop {
  0%, 100% { transform: translateY(0) rotate(4deg); }
  50%      { transform: translateY(-6px) rotate(1deg); }
}

@media (max-width: 768px) {
  .cart-hero-mascot { width: 105px; height: 105px; }
  .cart-hero-stamp  { width: 60px;  height: 60px; }
}

@media (max-width: 600px) {
  .cart-hero {
    flex-direction: column;
    text-align: center;
    padding: 1.5rem 1.25rem 1rem;
  }
  .cart-hero-title::after {
    margin-left: auto;
    margin-right: auto;
  }
  .cart-hero-subtitle {
    margin-left: auto;
    margin-right: auto;
  }
  .cart-hero-mascot { width: 95px; height: 95px; }
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
  width: 56px;
  height: 56px;
  object-fit: contain;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
  filter: drop-shadow(2px 3px 4px rgba(178, 34, 34, 0.3));
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

.cart-empty {
  text-align: center;
  padding: clamp(2rem, 5vw, 3rem) 1.5rem clamp(2.5rem, 6vw, 4rem);
  background: rgba(255, 255, 255, 0.5);
  border: 2px dashed rgba(0, 49, 83, 0.18);
  border-radius: 16px;
  margin-top: 1.5rem;
}

.cart-empty-mascot {
  width: 200px;
  height: 200px;
  object-fit: contain;
  margin: 0 auto 0.5rem;
  display: block;
  filter: drop-shadow(2px 4px 8px rgba(0, 49, 83, 0.18));
  animation: munchlax-bob 3s ease-in-out infinite;
}

@keyframes munchlax-bob {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-6px); }
}

.cart-empty-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: 1.4rem;
  margin: 0 0 0.5rem;
}

.cart-empty-msg {
  font-family: var(--font-sans);
  color: #666;
  font-size: 0.95rem;
  line-height: 1.5;
  margin: 0 auto 1.5rem;
  max-width: 380px;
}

.cart-empty-cta {
  display: inline-block;
  padding: 0.85rem 1.75rem;
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.9rem;
  letter-spacing: 1px;
  text-transform: uppercase;
  text-decoration: none;
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.18);
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.cart-empty-cta:hover {
  transform: translateY(-2px) scale(1.02);
  box-shadow: inset 1px 2px 4px rgba(255,255,255,0.55), 0 0 22px rgba(245, 176, 65, 0.45), 0 6px 16px rgba(0, 0, 0, 0.25);
}
</style>
