<script setup>
import { formatPrice } from '~/data/products'

const props = defineProps({
  product: {
    type: Object,
    required: true
  }
})

const { add } = useCart()
const toast = useToast()

const handleAdd = () => {
  const result = add(props.product, 1)
  if (result.reason === 'sold-out') {
    toast.error(`"${props.product.title}" is no longer in the gallery.`)
    return
  }
  if (result.reason === 'cap-reached') {
    toast.error(`The full reserve of "${props.product.title}" is already in your cart.`)
    return
  }
  toast.success(`Added "${props.product.title}" to your cart.`)
}
</script>

<template>
  <NuxtLink :to="`/products/${product.slug}`" class="product-card-link">
    <article class="product-card">
      <div class="product-image-container">
        <img :src="product.image" :alt="product.title" class="product-image" loading="lazy" />
        <button
          class="add-to-cart-quick"
          aria-label="Acquire"
          @click.prevent.stop="handleAdd"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
        </button>
      </div>
      <div class="product-info">
        <h4 class="product-title">{{ product.title }}</h4>
        <p class="product-price">{{ formatPrice(product.price) }}</p>
      </div>
    </article>
  </NuxtLink>
</template>

<style scoped>
.product-card-link {
  display: block;
  text-decoration: none;
  color: inherit;
}
</style>
