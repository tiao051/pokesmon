<script setup>
defineProps({
  product: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <div class="product-card">
    <div class="product-image-container">
      <img :src="product.image" :alt="product.title" class="product-image" loading="lazy" />
      <button class="add-to-cart-quick" aria-label="Add to cart">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
      </button>
    </div>
    <div class="product-info">
      <h4 class="product-title">{{ product.title }}</h4>
      <p class="product-price">{{ product.price }}</p>
    </div>
  </div>
</template>
