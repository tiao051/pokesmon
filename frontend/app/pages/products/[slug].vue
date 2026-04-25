<script setup>
import { ref, computed } from 'vue'
import { findProductBySlug, formatPrice, products } from '~/data/products'

const route = useRoute()
const router = useRouter()

const product = computed(() => findProductBySlug(route.params.slug))

const placeholderImage = 'https://placehold.co/400x500/003153/FFC512?text=151+Booster+Box'
const showRealImage = ref(true)
const toggleImage = () => { showRealImage.value = !showRealImage.value }

if (!product.value) {
  throw createError({
    statusCode: 404,
    statusMessage: 'This artifact is not in the collection',
    fatal: true,
  })
}

const { add } = useCart()
const toast = useToast()
const quantity = ref(1)

const tryAdd = () => {
  const result = add(product.value, quantity.value)
  if (result.reason === 'sold-out') {
    toast.error(`"${product.value.title}" is no longer in the gallery.`)
    return false
  }
  if (result.reason === 'cap-reached') {
    toast.error(`The full reserve of "${product.value.title}" is already in your cart.`)
    return false
  }
  toast.success(`Added ${result.added} × "${product.value.title}" to your cart.`)
  return true
}

const acquire = () => {
  tryAdd()
}

const acquireNow = () => {
  if (tryAdd()) router.push('/cart')
}

const related = computed(() =>
  products
    .filter((p) => p.category === product.value.category && p.id !== product.value.id)
    .slice(0, 4),
)

useHead(() => ({
  title: product.value ? `${product.value.title} — PokéGogh` : 'PokéGogh',
}))
</script>

<template>
  <main v-if="product" class="detail-page">
    <NuxtLink to="/products" class="breadcrumb-link">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="19" y1="12" x2="5" y2="12"/><polyline points="12 19 5 12 12 5"/></svg>
      Back to The Collection
    </NuxtLink>

    <article class="detail-grid">
      <div class="detail-image-side">
        <div class="detail-image-frame">
          <button
            type="button"
            class="detail-pokeball-stamp"
            :aria-label="showRealImage ? 'Show placeholder image' : 'Show real image'"
            @click="toggleImage"
          >
            <img src="/images/big_pokeball.jpg" alt="" />
          </button>
          <img :src="showRealImage ? product.image : placeholderImage" :alt="product.title" />
        </div>
      </div>

      <div class="detail-info-side">
        <span class="eyebrow gold-italic">{{ product.category }}</span>
        <h1 class="detail-title">{{ product.title }}</h1>
        <p class="detail-price">{{ formatPrice(product.price) }}</p>

        <ul v-if="product.tags?.length" class="detail-tags">
          <li v-for="tag in product.tags" :key="tag" class="detail-tag">
            <span class="tag-energy" aria-hidden="true"></span>
            <span class="tag-label">{{ tag }}</span>
          </li>
        </ul>

        <p class="detail-description">{{ product.description }}</p>

        <div class="detail-meta">
          <div class="meta-row">
            <span class="meta-label">Availability</span>
            <span class="meta-value" :class="{ 'in-stock': product.stock > 0, 'out-of-stock': product.stock === 0 }">
              <span class="meta-pokeball" aria-hidden="true"></span>
              {{ product.stock > 0 ? `${product.stock} remaining in the gallery` : 'Unavailable' }}
            </span>
          </div>
        </div>

        <div class="detail-actions">
          <div class="qty-row">
            <span class="qty-label">Quantity</span>
            <QuantityStepper v-model="quantity" :min="1" :max="Math.max(product.stock, 1)" />
          </div>

          <div class="action-buttons">
            <button
              type="button"
              class="btn-acquire btn-secondary-museum"
              :class="{ 'is-sold-out': product.stock === 0 }"
              @click="acquire"
            >
              Add to Collection
            </button>
            <button
              type="button"
              class="btn-acquire btn-primary-museum"
              :class="{ 'is-sold-out': product.stock === 0 }"
              @click="acquireNow"
            >
              Take it Home
            </button>
          </div>
        </div>
      </div>
    </article>

    <section v-if="related.length" class="related-section">
      <h2 class="related-title">Companions in the Collection</h2>
      <div class="product-grid">
        <ProductCard v-for="r in related" :key="r.id" :product="r" />
      </div>
    </section>
  </main>
</template>

<style scoped>
.detail-page {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: clamp(2rem, 4vw, 3rem) 2rem clamp(4rem, 7vw, 6rem);
}

.breadcrumb-link {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-prussian-blue);
  text-decoration: none;
  font-family: var(--font-sans);
  font-weight: 500;
  font-size: 0.95rem;
  margin-bottom: 2.5rem;
  transition: color 0.2s ease;
}
.breadcrumb-link:hover { color: var(--color-cypress-green); }
.breadcrumb-link svg { width: 18px; height: 18px; }

.detail-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  margin-bottom: 5rem;
}

@media (min-width: 900px) {
  .detail-grid {
    grid-template-columns: 1.1fr 1fr;
    gap: 4rem;
    align-items: start;
  }
}

.detail-image-frame {
  position: relative;
  aspect-ratio: 4/5;
  background-color: #E8E5D8;
  border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
  overflow: hidden;
  border: 2px solid var(--color-prussian-blue);
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.2), 0 8px 30px rgba(0,0,0,0.1);
}

.detail-image-frame::after {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: inherit;
  pointer-events: none;
  box-shadow:
    inset 0 0 0 6px rgba(251, 249, 242, 0.9),
    inset 0 0 0 7.5px rgba(255, 197, 18, 0.55);
  z-index: 3;
}

.detail-image-frame img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  position: relative;
  z-index: 1;
}

.detail-pokeball-stamp {
  position: absolute;
  top: 1rem;
  right: 1rem;
  width: 52px;
  height: 52px;
  padding: 0;
  border-radius: 50%;
  overflow: hidden;
  background: var(--color-linen);
  border: 2px solid var(--color-prussian-blue);
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.35);
  transform: rotate(-10deg);
  z-index: 4;
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}
.detail-pokeball-stamp:hover {
  transform: rotate(-10deg) scale(1.08);
  box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.45);
}
.detail-pokeball-stamp:focus-visible {
  outline: 2px solid var(--color-sunflower-yellow);
  outline-offset: 2px;
}
.detail-pokeball-stamp img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.detail-featured-badge {
  position: absolute;
  top: 1rem;
  left: 1.1rem;
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, #FBF9F2 0 22%, #1A1A1A 22% 32%, transparent 32%),
    linear-gradient(180deg, #1A1A1A 0 47%, #FFC512 47% 53%, #FBF9F2 53% 100%);
  border: 1.5px solid var(--color-prussian-blue);
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.35);
  transform: rotate(8deg);
  z-index: 4;
}
.detail-featured-badge::before {
  content: '★';
  position: absolute;
  top: -8px;
  right: -8px;
  width: 22px;
  height: 22px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  border: 1.5px solid var(--color-prussian-blue);
  border-radius: 50%;
  font-size: 0.75rem;
  font-weight: 700;
  box-shadow: 1px 1px 0 rgba(0, 49, 83, 0.3);
}

.detail-info-side {
  display: flex;
  flex-direction: column;
}

.eyebrow.gold-italic {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 1.1rem;
  letter-spacing: 0.5px;
  margin-bottom: 0.5rem;
}

.detail-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(2rem, 5vw, 3rem);
  line-height: 1.15;
  margin-bottom: 1rem;
}

.detail-price {
  font-family: var(--font-sans);
  color: var(--color-cypress-green);
  font-weight: 600;
  font-size: 1.5rem;
  margin-bottom: 1.25rem;
}

.detail-tags {
  list-style: none;
  margin: 0 0 1.75rem;
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.detail-tag {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0.3rem 0.8rem 0.3rem 0.55rem;
  background-color: rgba(255, 197, 18, 0.12);
  border: 1.5px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  font-family: var(--font-sans);
  font-weight: 600;
  font-size: 0.72rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--color-prussian-blue);
  box-shadow: 1.5px 1.5px 0 rgba(0, 49, 83, 0.18);
}

.tag-energy {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background:
    radial-gradient(circle at 35% 30%, #FFE070 0 30%, transparent 31%),
    var(--color-sunflower-yellow);
  border: 1px solid var(--color-prussian-blue);
  flex-shrink: 0;
}

.meta-pokeball {
  display: inline-block;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, #FBF9F2 0 22%, #1A1A1A 22% 34%, transparent 34%),
    linear-gradient(180deg, #B22222 0 47%, #1A1A1A 47% 53%, #FBF9F2 53% 100%);
  border: 1px solid var(--color-prussian-blue);
  vertical-align: -2px;
  margin-right: 0.4rem;
}
.meta-value.out-of-stock .meta-pokeball {
  filter: grayscale(0.6);
  opacity: 0.6;
}

.detail-description {
  font-family: var(--font-sans);
  color: #444;
  line-height: 1.7;
  font-size: 1rem;
  margin-bottom: 2rem;
}

.detail-meta {
  border-top: 1px solid rgba(0, 49, 83, 0.1);
  border-bottom: 1px solid rgba(0, 49, 83, 0.1);
  padding: 1.25rem 0;
  margin-bottom: 2rem;
}

.meta-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-family: var(--font-sans);
}

.meta-label {
  color: #777;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.8rem;
}

.meta-value {
  font-weight: 600;
  color: var(--color-prussian-blue);
}
.meta-value.in-stock { color: var(--color-cypress-green); }
.meta-value.out-of-stock { color: #B22222; }

.qty-row {
  display: flex;
  align-items: center;
  gap: 1.25rem;
  margin-bottom: 1.5rem;
}

.qty-label {
  font-family: var(--font-sans);
  font-weight: 600;
  color: var(--color-prussian-blue);
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.85rem;
}

.action-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
}

.btn-acquire {
  flex: 1;
  min-width: 160px;
  padding: 0.95rem 1.75rem;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 1rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  cursor: pointer;
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.btn-primary-museum {
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.2);
}
.btn-primary-museum:hover:not(:disabled) {
  background: linear-gradient(135deg, #F5B341 0%, #E6A039 50%, #B07212 100%);
  transform: translateY(-2px) scale(1.02);
  letter-spacing: 1.5px;
  box-shadow:
    inset 1px 2px 4px rgba(255,255,255,0.55),
    0 0 22px rgba(245, 176, 65, 0.45),
    0 6px 16px rgba(0, 0, 0, 0.25);
}

.btn-secondary-museum {
  background-color: transparent;
  border: 2px solid var(--color-prussian-blue);
  color: var(--color-prussian-blue);
  box-shadow: 3px 3px 0px rgba(0, 49, 83, 0.15);
}
.btn-secondary-museum:hover:not(:disabled) {
  background-color: var(--color-prussian-blue);
  color: var(--color-linen);
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0px rgba(0, 49, 83, 0.4);
}

.btn-acquire:disabled,
.btn-acquire.is-sold-out {
  opacity: 0.45;
  filter: grayscale(0.35);
}
.btn-acquire.is-sold-out:hover:not(:disabled) {
  transform: none;
  letter-spacing: 1px;
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.2);
}
.btn-acquire.btn-secondary-museum.is-sold-out:hover:not(:disabled) {
  background-color: transparent;
  color: var(--color-prussian-blue);
  transform: none;
  box-shadow: 3px 3px 0px rgba(0, 49, 83, 0.15);
}

.related-section {
  margin-top: 4rem;
  padding-top: 4rem;
  border-top: 1px solid rgba(0, 49, 83, 0.1);
}

.related-title {
  font-family: var(--font-serif);
  font-size: clamp(1.75rem, 4vw, 2.25rem);
  color: var(--color-prussian-blue);
  text-align: center;
  margin-bottom: 3rem;
  position: relative;
}
.related-title::after {
  content: '';
  display: block;
  width: 60px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 1rem auto 0;
  border-radius: 2px;
}
</style>
