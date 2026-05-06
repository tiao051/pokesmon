<script setup>
import { ref, computed } from 'vue'
import { formatPrice } from '../types/product'

const props = defineProps({
  product: {
    type: Object,
    required: true
  }
})

const { add } = useCart()
const toast = useToast()

const isLoaded = ref(false)
const onLoad = () => {
  isLoaded.value = true
}

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

const imageSrc = computed(() => {
  if (props.product.imageBase64) {
    const base64 = props.product.imageBase64
    if (base64.startsWith('data:')) return base64
    return `data:image/png;base64,${base64}`
  }
  return props.product.image || '/images/loading_gif.gif'
})
</script>

<template>
  <NuxtLink :to="`/products/${product.slug}`" class="product-card-link">
    <article class="product-card">
      <div class="product-image-container">
        <!-- Main Image -->
        <img 
          :src="imageSrc" 
          :alt="product.title" 
          class="product-image" 
          :class="{ 'is-hidden': !isLoaded }"
          @load="onLoad"
          @error="onLoad"
          loading="lazy" 
        />
        
        <!-- Loading Placeholder -->
        <div v-if="!isLoaded" class="loading-overlay">
          <img 
            src="/images/loading_gif.gif" 
            alt="Loading..." 
            class="loading-gif" 
          />
        </div>

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

.product-image-container {
  position: relative;
  aspect-ratio: 4/5;
  background-color: var(--color-linen);
  border-radius: 8px;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
}

.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
  transition: opacity 0.3s ease;
}

.product-image.is-hidden {
  opacity: 0;
  position: absolute;
}

.loading-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--color-linen);
  z-index: 1;
}

.loading-gif {
  width: 50%;
  height: auto;
  object-fit: contain;
}
</style>
