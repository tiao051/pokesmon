<script setup>
import { computed, ref, reactive, onMounted } from 'vue'

const router = useRouter()
const route = useRoute()
const {
  standardItems,
  standardSubtotal,
  standardCount,
  preorderItems,
  preorderSubtotal,
  preorderDeposit,
  preorderRemaining,
  preorderCount,
  clear,
} = useCart()
const { isLoggedIn, email: authEmail } = useAuth()

const isPreorder = computed(() => route.query.type === 'preorder')

const activeItems = computed(() =>
  isPreorder.value ? preorderItems.value : standardItems.value,
)
const activeSubtotal = computed(() =>
  isPreorder.value ? preorderSubtotal.value : standardSubtotal.value,
)
const activeCount = computed(() =>
  isPreorder.value ? preorderCount.value : standardCount.value,
)

onMounted(() => {
  if (activeCount.value === 0) {
    router.replace('/cart')
    return
  }
  if (!isLoggedIn.value) {
    const target = `/checkout${isPreorder.value ? '?type=preorder' : ''}`
    router.replace(`/login?redirect=${encodeURIComponent(target)}`)
  }
})

useHead({ title: 'Checkout — PokéGogh' })

const form = reactive({
  fullName: '',
  email: authEmail.value || '',
  phone: '',
  addressLine1: '',
  addressLine2: '',
  city: '',
  country: '',
  cardNumber: '',
  cardExpiry: '',
  cardCvc: '',
})

const submitting = ref(false)

const placeOrder = () => {
  if (submitting.value) return
  submitting.value = true
  const orderRef = 'PG-' + Date.now().toString().slice(-8)
  setTimeout(() => {
    clear()
    router.push(`/checkout/success?ref=${orderRef}`)
  }, 600)
}
</script>

<template>
  <main v-if="activeItems.length > 0" class="checkout-page">
    <header class="checkout-hero" :class="{ 'is-preorder': isPreorder }">
      <img
        :src="isPreorder
          ? 'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/timer-ball.png'
          : 'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/cherish-ball.png'"
        alt=""
        aria-hidden="true"
        class="checkout-hero-stamp"
        loading="eager"
        decoding="async"
      />
      <div class="checkout-hero-text">
        <p class="checkout-hero-eyebrow">
          — {{ isPreorder ? 'Final Step · Pre-order' : 'Final Step' }} —
        </p>
        <h1 class="checkout-hero-title">
          {{ isPreorder ? 'Reserve Your Pre-order' : 'Checkout' }}
        </h1>
        <p class="checkout-hero-subtitle">
          {{ isPreorder
            ? 'Audino is ready to take your deposit and watch over your reservation until it ships.'
            : 'Audino is ready to wrap up your order — last details and you\'re done.' }}
        </p>
      </div>
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/531.png"
        alt="Audino, the shop's caring helper"
        class="checkout-hero-mascot"
        loading="lazy"
        decoding="async"
      />
    </header>

    <form class="checkout-grid" @submit.prevent="placeOrder">
      <div class="checkout-form-side">
        <img
          src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/premier-ball.png"
          alt=""
          aria-hidden="true"
          class="form-pokeball-stamp"
          loading="lazy"
          decoding="async"
        />

        <span class="form-eyebrow">— Shipping Details —</span>

        <section class="form-section">
          <header class="form-section-header">
            <span class="section-badge">I</span>
            <h2 class="section-heading">Your Info</h2>
          </header>
          <div class="form-grid">
            <div class="form-group full">
              <label for="fullName">Full Name</label>
              <input id="fullName" v-model="form.fullName" type="text" required placeholder="Nguyễn Hướng Dương" />
            </div>
            <div class="form-group">
              <label for="email">Email Address</label>
              <input id="email" v-model="form.email" type="email" required placeholder="khach@bao-tang.vn" />
            </div>
            <div class="form-group">
              <label for="phone">Phone</label>
              <input id="phone" v-model="form.phone" type="tel" required placeholder="+84 90 123 4567" />
            </div>
          </div>
        </section>

        <section class="form-section">
          <header class="form-section-header">
            <span class="section-badge">II</span>
            <h2 class="section-heading">Shipping Address</h2>
          </header>
          <div class="form-grid">
            <div class="form-group full">
              <label for="addressLine1">Street Address</label>
              <input id="addressLine1" v-model="form.addressLine1" type="text" required placeholder="Số 1, Đường Hướng Dương" />
            </div>
            <div class="form-group full">
              <label for="addressLine2">Apartment / Suite (optional)</label>
              <input id="addressLine2" v-model="form.addressLine2" type="text" placeholder="Phòng 47" />
            </div>
            <div class="form-group">
              <label for="city">City</label>
              <input id="city" v-model="form.city" type="text" required placeholder="Hà Nội" />
            </div>
            <div class="form-group">
              <label for="country">Country</label>
              <input id="country" v-model="form.country" type="text" required placeholder="Việt Nam" />
            </div>
          </div>
        </section>

        <section class="form-section">
          <header class="form-section-header">
            <span class="section-badge">III</span>
            <h2 class="section-heading">Payment</h2>
          </header>
          <p class="section-note">This is a demo form. No real payment is processed.</p>
          <div class="form-grid">
            <div class="form-group full">
              <label for="cardNumber">Card Number</label>
              <input id="cardNumber" v-model="form.cardNumber" type="text" required placeholder="4242 4242 4242 4242" inputmode="numeric" />
            </div>
            <div class="form-group">
              <label for="cardExpiry">Expiry</label>
              <input id="cardExpiry" v-model="form.cardExpiry" type="text" required placeholder="MM / YY" inputmode="numeric" />
            </div>
            <div class="form-group">
              <label for="cardCvc">CVC</label>
              <input id="cardCvc" v-model="form.cardCvc" type="text" required placeholder="123" inputmode="numeric" />
            </div>
          </div>
        </section>

        <button
          type="submit"
          class="checkout-submit-mobile"
          :disabled="submitting"
        >
          {{ submitting ? 'Confirming…' : (isPreorder ? 'Reserve with Deposit' : 'Place Order') }}
        </button>
      </div>

      <div class="checkout-summary-side">
        <OrderSummary
          :mode="isPreorder ? 'preorder' : 'standard'"
          :subtotal="activeSubtotal"
          :shipping="0"
          :deposit="preorderDeposit"
          :remaining="preorderRemaining"
          :cta-label="submitting ? 'Confirming…' : (isPreorder ? 'Reserve with Deposit' : 'Place Order')"
          :cta-disabled="submitting"
          @cta-click="placeOrder"
        >
          <template #extra>
            <ul class="summary-items">
              <li v-for="item in activeItems" :key="item.product.id" class="summary-item">
                <span class="summary-item-qty">{{ item.quantity }}×</span>
                <span class="summary-item-title">{{ item.product.title }}</span>
              </li>
            </ul>
          </template>
        </OrderSummary>
      </div>
    </form>
  </main>
</template>

<style scoped>
.checkout-page {
  max-width: clamp(var(--container-max), 90vw, 1500px);
  margin: 0 auto;
  padding: clamp(3rem, 6vw, 5rem) 2rem clamp(4rem, 7vw, 6rem);
}

/* Checkout hero — mirror favorites/cart pattern */
.checkout-hero {
  position: relative;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 1.75rem 1.75rem 1.75rem 1.75rem;
  margin-bottom: 2rem;
  background:
    radial-gradient(circle at top right, rgba(0, 49, 83, 0.08) 0%, transparent 60%),
    linear-gradient(180deg, rgba(0, 49, 83, 0.07) 0%, rgba(0, 49, 83, 0.01) 100%);
  border: 1.5px solid rgba(0, 49, 83, 0.18);
  border-radius: 18px;
  overflow: hidden;
  min-height: 160px;
}

.checkout-hero::before {
  content: "";
  position: absolute;
  top: 0;
  left: 1.5rem;
  right: 1.5rem;
  height: 3px;
  background: linear-gradient(90deg, transparent, var(--color-prussian-blue) 30%, var(--color-prussian-blue) 70%, transparent);
  box-shadow: 0 0 12px rgba(0, 49, 83, 0.4);
}

.checkout-hero.is-preorder {
  background:
    radial-gradient(circle at top right, rgba(255, 105, 180, 0.06) 0%, transparent 60%),
    linear-gradient(180deg, rgba(178, 34, 34, 0.1) 0%, rgba(178, 34, 34, 0.02) 100%);
  border-color: rgba(178, 34, 34, 0.25);
}

.checkout-hero.is-preorder::before {
  background: linear-gradient(90deg, transparent, #b22222 30%, #b22222 70%, transparent);
  box-shadow: 0 0 12px rgba(178, 34, 34, 0.4);
}

.checkout-hero-stamp {
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

.checkout-hero-text {
  position: relative;
  z-index: 2;
  flex: 1;
  min-width: 0;
}

.checkout-hero-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 0.95rem;
  letter-spacing: 1px;
  margin: 0 0 0.3rem;
}

.checkout-hero.is-preorder .checkout-hero-eyebrow {
  color: #b22222;
}

.checkout-hero-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1.75rem, 4vw, 2.4rem);
  margin: 0;
  line-height: 1.15;
  letter-spacing: -0.5px;
}

.checkout-hero-title::after {
  content: "";
  display: block;
  width: 60px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin-top: 0.6rem;
  border-radius: 2px;
}

.checkout-hero-subtitle {
  font-family: var(--font-sans);
  color: #555;
  font-size: 0.9rem;
  line-height: 1.5;
  margin: 0.85rem 0 0;
  max-width: 520px;
}

/* Audino mascot — caring helper */
.checkout-hero-mascot {
  position: relative;
  z-index: 2;
  flex-shrink: 0;
  width: 140px;
  height: 140px;
  object-fit: contain;
  transform-origin: center bottom;
  filter: drop-shadow(2px 4px 8px rgba(255, 105, 180, 0.25));
  animation: audino-sway 4s ease-in-out infinite;
}

@keyframes audino-sway {
  0%, 100% { transform: translateY(0) rotate(-3deg); }
  50%      { transform: translateY(-5px) rotate(3deg); }
}

@media (max-width: 768px) {
  .checkout-hero-mascot { width: 105px; height: 105px; }
  .checkout-hero-stamp  { width: 60px;  height: 60px; }
}

@media (max-width: 600px) {
  .checkout-hero {
    flex-direction: column;
    text-align: center;
    padding: 1.5rem 1.25rem 1rem;
  }
  .checkout-hero-title::after {
    margin-left: auto;
    margin-right: auto;
  }
  .checkout-hero-subtitle {
    margin-left: auto;
    margin-right: auto;
  }
  .checkout-hero-mascot { width: 95px; height: 95px; }
}

.checkout-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  align-items: start;
}

@media (min-width: 900px) {
  .checkout-grid {
    grid-template-columns: 1.6fr 1fr;
  }
  .checkout-summary-side {
    position: sticky;
    top: 100px;
  }
  .checkout-submit-mobile {
    display: none;
  }
}

.checkout-form-side {
  position: relative;
  background-color: #FBF9F2;
  padding: clamp(1.75rem, 4vw, 2.5rem);
  border: 2px solid var(--color-prussian-blue);
  border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.15);
  overflow: hidden;
}

.checkout-form-side::before {
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

.form-pokeball-stamp,
.form-eyebrow,
.form-section,
.checkout-submit-mobile {
  position: relative;
  z-index: 1;
}

.form-pokeball-stamp {
  position: absolute;
  top: 1rem;
  right: 1.25rem;
  width: 44px;
  height: 44px;
  object-fit: contain;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
  filter: drop-shadow(1.5px 2px 3px rgba(0, 49, 83, 0.25));
  transform: rotate(-12deg);
  z-index: 2;
  pointer-events: none;
}

.form-eyebrow {
  display: block;
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 0.9rem;
  letter-spacing: 1.5px;
  margin-bottom: 1.5rem;
}

.form-section {
  margin-bottom: 0;
}
.form-section + .form-section {
  margin-top: 2.25rem;
  padding-top: 2.25rem;
  border-top: 1px dashed rgba(0, 49, 83, 0.25);
}

.form-section-header {
  display: flex;
  align-items: center;
  gap: 0.85rem;
  margin-bottom: 1.25rem;
}

.section-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  font-family: var(--font-serif);
  font-style: italic;
  font-weight: 700;
  font-size: 0.95rem;
  border: 2px solid var(--color-prussian-blue);
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.25);
  flex-shrink: 0;
  letter-spacing: 0.5px;
}

.section-heading {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1.4rem, 3.5vw, 1.6rem);
  position: relative;
  display: inline-block;
  margin: 0;
}
.section-heading::after {
  content: '';
  display: block;
  width: 38px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin-top: 0.4rem;
  border-radius: 2px;
}

.section-note {
  font-family: var(--font-serif);
  font-style: italic;
  color: #888;
  font-size: 0.9rem;
  margin: 0 0 1.5rem;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem 1.25rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}
.form-group.full { grid-column: 1 / -1; }

.form-group label {
  font-family: var(--font-sans);
  font-size: 0.78rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 1.2px;
  color: rgba(0, 49, 83, 0.7);
}

.form-group input {
  padding: 0.55rem 0.15rem;
  border: none;
  border-bottom: 1.5px solid rgba(0, 49, 83, 0.25);
  border-radius: 0;
  background: transparent;
  box-shadow: none;
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: var(--color-prussian-blue);
  transition: border-color 0.2s ease, border-width 0.2s ease;
}
.form-group input::placeholder {
  color: rgba(0, 49, 83, 0.3);
  font-family: var(--font-serif);
  font-style: italic;
}
.form-group input:focus {
  outline: none;
  border-bottom-color: var(--color-prussian-blue);
  border-bottom-width: 2px;
}
.form-group input:focus::placeholder {
  color: rgba(0, 49, 83, 0.45);
}

.checkout-submit-mobile {
  display: block;
  width: 100%;
  padding: 1rem;
  margin-top: 2.5rem;
  background-color: var(--color-prussian-blue);
  color: var(--color-linen);
  border: 2px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 1rem;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  cursor: pointer;
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.35);
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}
.checkout-submit-mobile:hover:not(:disabled) {
  background-color: #002240;
  transform: translate(-2px, -2px);
  box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.55);
}
.checkout-submit-mobile:active:not(:disabled) {
  transform: translate(2px, 2px);
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.4);
}
.checkout-submit-mobile:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.summary-items {
  list-style: none;
  margin: -0.5rem 0 1.25rem;
  padding: 0 0 1.25rem;
  border-bottom: 1px dashed rgba(0, 49, 83, 0.25);
  position: relative;
  z-index: 1;
}
.summary-item {
  display: flex;
  gap: 0.5rem;
  font-family: var(--font-sans);
  font-size: 0.85rem;
  color: #5C5C5C;
  margin-bottom: 0.45rem;
  line-height: 1.4;
}
.summary-item-qty {
  font-weight: 700;
  color: #C2821B;
  flex-shrink: 0;
}
.summary-item-title {
  color: var(--color-prussian-blue);
  font-weight: 500;
}
</style>
