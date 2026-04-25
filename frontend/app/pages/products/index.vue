<script setup>
import { ref, computed } from 'vue'
import { products as allProducts, categories } from '~/data/products'

const filterOptions = ['All', ...categories]
const filter = ref('All')

const filteredProducts = computed(() => {
  if (filter.value === 'All') return allProducts
  return allProducts.filter((p) => p.category === filter.value)
})

useHead({ title: 'The Collection — PokéGogh' })
</script>

<template>
  <main class="collection-page">
    <header class="page-header">
      <img src="/images/big_pokeball.jpg" alt="" class="header-pokeball" />
      <span class="eyebrow gold-italic">PokéGogh Museum Catalog</span>
      <h1 class="page-title">The Collection</h1>
      <p class="page-subtitle">Browse the museum's complete inventory of curated post-impressionist Pokémon artifacts.</p>
      <div class="header-trainer-belt" aria-hidden="true">
        <img src="/images/red_pokeball.jpg" alt="" class="belt-pokeball" />
        <img src="/images/blue_pokeball.jpg" alt="" class="belt-pokeball" />
        <img src="/images/black_pokeball.jpg" alt="" class="belt-pokeball" />
      </div>
    </header>

    <div class="categories">
      <button
        v-for="cat in filterOptions"
        :key="cat"
        class="category-btn"
        :class="{ active: filter === cat }"
        @click="filter = cat"
      >
        <span class="cat-pokeball" aria-hidden="true"></span>
        {{ cat }}
      </button>
    </div>

    <div v-if="filteredProducts.length" class="product-grid">
      <ProductCard
        v-for="product in filteredProducts"
        :key="product.id"
        :product="product"
      />
    </div>
    <EmptyState
      v-else
      title="The gallery is currently quiet"
      message="No artifacts match this filter. Try another wing of the museum."
    />
  </main>
</template>

<style scoped>
.collection-page {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: clamp(3rem, 6vw, 5rem) 2rem clamp(4rem, 7vw, 6rem);
}

.page-header {
  position: relative;
  text-align: center;
  margin-bottom: 3rem;
  padding-bottom: 1rem;
}

.header-pokeball {
  display: block;
  width: 56px;
  height: 56px;
  margin: 0 auto 1.1rem;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid var(--color-prussian-blue);
  box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.25);
  transform: rotate(-6deg);
}

.header-trainer-belt {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 0.85rem;
  margin-top: 1.5rem;
}
.header-trainer-belt::before,
.header-trainer-belt::after {
  content: '';
  width: 60px;
  height: 1px;
  background: linear-gradient(90deg, transparent, rgba(0, 49, 83, 0.35), transparent);
}

.belt-pokeball {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  object-fit: cover;
  border: 1.5px solid var(--color-prussian-blue);
  box-shadow: 1.5px 1.5px 0 rgba(0, 49, 83, 0.2);
}

.category-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.cat-pokeball {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background:
    radial-gradient(circle at center, #FBF9F2 0 22%, #1A1A1A 22% 36%, transparent 36%),
    linear-gradient(180deg, #B22222 0 47%, #1A1A1A 47% 53%, #FBF9F2 53% 100%);
  border: 1px solid currentColor;
  opacity: 0.55;
  flex-shrink: 0;
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.category-btn.active .cat-pokeball {
  opacity: 1;
  transform: scale(1.1);
  background:
    radial-gradient(circle at center, var(--color-linen) 0 22%, #1A1A1A 22% 36%, transparent 36%),
    linear-gradient(180deg, #1A1A1A 0 47%, #FFC512 47% 53%, var(--color-linen) 53% 100%);
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
  font-size: clamp(2.5rem, 6vw, 3.5rem);
  margin-bottom: 1.25rem;
  position: relative;
  display: inline-block;
}

.page-title::after {
  content: '';
  display: block;
  width: 80px;
  height: 4px;
  background-color: var(--color-cypress-green);
  margin: 1rem auto 0;
  border-radius: 2px;
}

.page-subtitle {
  font-family: var(--font-sans);
  color: #555;
  max-width: 540px;
  margin: 0 auto;
  font-size: 1rem;
  line-height: 1.6;
}
</style>
