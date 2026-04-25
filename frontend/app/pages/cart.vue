<script setup>
const { items, subtotal, count } = useCart()
const { isLoggedIn } = useAuth()
const router = useRouter()

const proceedToCheckout = () => {
  if (!isLoggedIn.value) {
    router.push('/login?redirect=/checkout')
  } else {
    router.push('/checkout')
  }
}

useHead({ title: 'Pending Acquisitions — PokéGogh' })
</script>

<template>
  <main class="cart-page">
    <header class="page-header">
      <span class="eyebrow gold-italic">Reserve a Place in the Collection</span>
      <h1 class="page-title">Your Pending Acquisitions</h1>
      <p v-if="count > 0" class="page-subtitle">
        {{ count }} {{ count === 1 ? 'artifact' : 'artifacts' }} awaiting confirmation
      </p>
    </header>

    <EmptyState
      v-if="items.length === 0"
      title="Your gallery awaits"
      message="No artifacts have been reserved yet. Begin your collection from the museum's catalog."
      cta-label="Browse The Collection"
      cta-to="/products"
    />

    <div v-else class="cart-grid">
      <div class="cart-lines">
        <CartLineItem
          v-for="item in items"
          :key="item.product.id"
          :item="item"
        />
      </div>

      <div class="cart-summary-side">
        <OrderSummary
          :subtotal="subtotal"
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
      </div>
    </div>
  </main>
</template>

<style scoped>
.cart-page {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: clamp(3rem, 6vw, 5rem) 2rem clamp(4rem, 7vw, 6rem);
}

.page-header {
  text-align: center;
  margin-bottom: 3rem;
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
