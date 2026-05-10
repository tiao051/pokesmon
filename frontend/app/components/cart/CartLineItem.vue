<script setup lang="ts">
import { computed, ref, toRef } from 'vue'
import type { CartItem } from '../../composables/cart/useCart'
import { useProductImage } from '../../composables/products/useProductImage'
import { formatPrice } from '../../utils/format'

const props = defineProps<{ item: CartItem }>()

const { updateQty, remove } = useCart()
const showConfirm = ref(false)

const handleQtyUpdate = (newQty: number) => {
  updateQty(props.item.product.id, newQty)
}

const askRemove = () => {
  showConfirm.value = true
}

const confirmRemove = () => {
  remove(props.item.product.id)
}

const productRef = computed(() => props.item.product)
const { src: thumbSrc, isDataUrl: thumbIsDataUrl } = useProductImage(productRef)

const lineTotal = computed(() => formatPrice(props.item.product.price * props.item.quantity))
</script>

<template>
  <div class="cart-line">
    <NuxtLink :to="`/products/${item.product.slug}`" class="cart-thumb">
      <img
        v-if="thumbIsDataUrl"
        :src="thumbSrc"
        :alt="item.product.title"
        width="100"
        height="125"
        loading="lazy"
        decoding="async"
      />
      <NuxtImg
        v-else-if="thumbSrc"
        :src="thumbSrc"
        :alt="item.product.title"
        width="100"
        height="125"
        sizes="xs:100px sm:100px"
        loading="lazy"
        decoding="async"
        placeholder
      />
    </NuxtLink>

    <div class="cart-line-info">
      <div class="cart-line-tags">
        <span class="cart-line-category">{{ item.product.category }}</span>
        <span v-if="item.product.isPreorder" class="cart-line-preorder-badge">Pre-order</span>
      </div>
      <NuxtLink :to="`/products/${item.product.slug}`" class="cart-line-title">{{ item.product.title }}</NuxtLink>
      <p class="cart-line-price">{{ formatPrice(item.product.price) }} each</p>
    </div>

    <div class="cart-line-qty">
      <QuantityStepper
        :model-value="item.quantity"
        :min="1"
        :max="item.product.stock"
        @update:model-value="handleQtyUpdate"
      />
    </div>

    <div class="cart-line-total">
      <span class="line-total-label">Subtotal</span>
      <span class="line-total-value">{{ lineTotal }}</span>
    </div>

    <div class="cart-line-actions">
      <FavoriteToggle
        :product-id="item.product.id"
        :product-title="item.product.title"
        variant="inline"
      />
      <button class="cart-line-remove" aria-label="Remove from cart" @click="askRemove">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-2 14a2 2 0 0 1-2 2H9a2 2 0 0 1-2-2L5 6"/><path d="M10 11v6"/><path d="M14 11v6"/><path d="M9 6V4a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2v2"/></svg>
      </button>
    </div>

    <ConfirmDialog
      v-model:open="showConfirm"
      title="Remove this item?"
      :message="`Remove “${item.product.title}” from your cart?`"
      confirm-label="Yes, Remove"
      cancel-label="Keep It"
      @confirm="confirmRemove"
    />
  </div>
</template>

<style scoped>
.cart-line {
  display: grid;
  grid-template-columns: 100px 1fr auto auto auto;
  gap: 1.5rem;
  align-items: center;
  padding: 1.5rem 0;
  border-bottom: 1px solid rgba(0, 49, 83, 0.1);
}

.cart-line:last-child {
  border-bottom: none;
}

@media (max-width: 768px) {
  .cart-line {
    grid-template-columns: 80px 1fr auto;
    grid-template-areas:
      "thumb info actions"
      "thumb qty total";
    gap: 0.75rem 1rem;
  }
  .cart-thumb { grid-area: thumb; }
  .cart-line-info { grid-area: info; }
  .cart-line-qty { grid-area: qty; align-self: end; }
  .cart-line-actions { grid-area: actions; }
}

.cart-line-actions {
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
}

.cart-thumb {
  width: 100px;
  height: 125px;
  background-color: var(--color-linen);
  border: 1px solid var(--color-prussian-blue);
  border-radius: 12px 4px 10px 4px / 4px 10px 4px 12px;
  overflow: hidden;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.cart-thumb :deep(img) {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.cart-line-info {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  min-width: 0;
}

.cart-line-tags {
  display: inline-flex;
  align-items: center;
  gap: 0.65rem;
  flex-wrap: wrap;
}

.cart-line-category {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
}

.cart-line-preorder-badge {
  font-family: var(--font-sans);
  font-size: 0.65rem;
  font-weight: 700;
  letter-spacing: 1.2px;
  text-transform: uppercase;
  padding: 0.2rem 0.55rem;
  background: rgba(178, 34, 34, 0.1);
  color: #b22222;
  border: 1px solid rgba(178, 34, 34, 0.3);
  border-radius: 4px;
  line-height: 1;
}

.cart-line-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1rem, 2vw, 1.15rem);
  font-weight: 600;
  text-decoration: none;
  line-height: 1.3;
  transition: color 0.2s ease;
}

.cart-line-title:hover {
  color: var(--color-cypress-green);
}

.cart-line-price {
  font-family: var(--font-sans);
  color: #777;
  font-size: 0.85rem;
}

.cart-line-total {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  text-align: right;
}

.line-total-label {
  font-family: var(--font-sans);
  font-size: 0.7rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #777;
  margin-bottom: 0.25rem;
}

.line-total-value {
  font-family: var(--font-sans);
  color: var(--color-cypress-green);
  font-weight: 700;
  font-size: 1.05rem;
}

.cart-line-remove {
  background: none;
  border: none;
  color: #999;
  cursor: pointer;
  padding: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.2s ease;
}

.cart-line-remove:hover {
  color: #B22222;
}

.cart-line-remove svg {
  width: 20px;
  height: 20px;
}
</style>
