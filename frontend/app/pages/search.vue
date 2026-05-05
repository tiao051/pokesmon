<script setup>
import { ref, computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { products, categories } from '~/data/products'

const route = useRoute()
const query = computed(() => route.query.q?.toString() || '')

const selectedCategories = ref([])
const maxPrice = ref(300)

const currentPage = ref(1)
const itemsPerPage = 8

watch([query, selectedCategories, maxPrice], () => {
  currentPage.value = 1
})

const filteredProducts = computed(() => {
  let result = products

  // Fuzzy Search
  if (query.value) {
    result = result.filter(p => fuzzySearch(query.value, p.title) || fuzzySearch(query.value, p.description))
  }

  // Filter by category
  if (selectedCategories.value.length > 0) {
    result = result.filter(p => selectedCategories.value.includes(p.category))
  }

  // Filter by price
  result = result.filter(p => p.price <= maxPrice.value)

  return result
})

const totalPages = computed(() => Math.ceil(filteredProducts.value.length / itemsPerPage))

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredProducts.value.slice(start, end)
})
</script>

<template>
  <div class="search-page">
    <div class="search-header-banner">
      <div class="banner-content">
        <h1 class="search-title">Search Results</h1>
        <p class="search-subtitle" v-if="query">Results for "<span class="highlight">{{ query }}</span>"</p>
        <p class="search-subtitle" v-else>Browse all products</p>
      </div>
    </div>

    <div class="search-container">
      <!-- Filters Sidebar -->
      <aside class="search-sidebar">
        <div class="filter-panel">
          <h2 class="filter-title">Filters</h2>
          
          <div class="filter-group">
            <h3>Categories</h3>
            <div class="checkbox-list">
              <label v-for="cat in categories" :key="cat" class="checkbox-label">
                <input type="checkbox" :value="cat" v-model="selectedCategories" class="custom-checkbox" />
                <span class="checkbox-text">{{ cat }}</span>
              </label>
            </div>
          </div>

          <div class="filter-group">
            <h3>Max Price: <span class="price-val">${{ maxPrice }}</span></h3>
            <input type="range" min="0" max="500" step="10" v-model.number="maxPrice" class="price-slider" />
            <div class="price-labels">
              <span>$0</span>
              <span>$500</span>
            </div>
          </div>
        </div>
      </aside>

      <!-- Main Results -->
      <main class="search-main">
        <div class="results-meta">
          <p class="results-count">Showing {{ filteredProducts.length }} products</p>
        </div>

        <div v-if="paginatedProducts.length > 0" class="product-grid">
          <ProductCard v-for="product in paginatedProducts" :key="product.id" :product="product" />
        </div>
        <div v-else class="empty-state-wrapper">
          <EmptyState 
            title="No Masterpieces Found" 
            message="Try adjusting your filters or searching for different keywords to find what you're looking for." 
          />
        </div>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="pagination">
          <button class="page-btn" :disabled="currentPage === 1" @click="currentPage--">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="15 18 9 12 15 6"></polyline></svg>
          </button>
          <div class="page-numbers">
            <button 
              v-for="page in totalPages" 
              :key="page" 
              class="page-number" 
              :class="{ active: currentPage === page }"
              @click="currentPage = page"
            >
              {{ page }}
            </button>
          </div>
          <button class="page-btn" :disabled="currentPage === totalPages" @click="currentPage++">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="9 18 15 12 9 6"></polyline></svg>
          </button>
        </div>
      </main>
    </div>
  </div>
</template>

<style scoped>
.search-page {
  min-height: 80vh;
  padding-bottom: 6rem;
}

.search-header-banner {
  background-color: var(--color-prussian-blue);
  background-image: linear-gradient(135deg, rgba(0,49,83,1) 0%, rgba(15,94,60,0.8) 100%);
  padding: 4rem 2rem;
  text-align: center;
  color: var(--color-linen);
  margin-bottom: 3rem;
}

.search-title {
  font-size: clamp(2rem, 5vw, 3rem);
  color: #fff;
  margin-bottom: 0.5rem;
}

.search-subtitle {
  font-family: var(--font-sans);
  font-size: 1.2rem;
  opacity: 0.9;
}

.highlight {
  color: var(--color-sunflower-yellow);
  font-style: italic;
  font-family: var(--font-serif);
}

.search-container {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 0 2rem;
  display: grid;
  grid-template-columns: 1fr;
  gap: 2rem;
}

@media (min-width: 768px) {
  .search-container {
    grid-template-columns: 280px 1fr;
    gap: 3rem;
  }
}

.filter-panel {
  background-color: #fff;
  border: 1px solid rgba(0, 49, 83, 0.1);
  border-radius: 12px;
  padding: 1.5rem;
  position: sticky;
  top: 100px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.03);
}

.filter-title {
  font-size: 1.5rem;
  margin-bottom: 1.5rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid var(--color-sunflower-yellow);
}

.filter-group {
  margin-bottom: 2rem;
}

.filter-group h3 {
  font-size: 1.1rem;
  margin-bottom: 1rem;
  font-family: var(--font-sans);
  color: var(--color-prussian-blue);
}

.checkbox-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
}

.custom-checkbox {
  width: 18px;
  height: 18px;
  accent-color: var(--color-cypress-green);
}

.checkbox-text {
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: #4a4a4a;
}

.price-val {
  color: var(--color-cypress-green);
  font-weight: 600;
}

.price-slider {
  width: 100%;
  accent-color: var(--color-cypress-green);
  margin-bottom: 0.5rem;
}

.price-labels {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: #888;
  font-family: var(--font-sans);
}

.results-meta {
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(0,0,0,0.1);
}

.results-count {
  font-family: var(--font-sans);
  color: #666;
}

.product-grid {
  display: grid;
  grid-template-columns: repeat(1, 1fr);
  gap: 2rem;
}

@media (min-width: 500px) {
  .product-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (min-width: 1024px) {
  .product-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

.empty-state-wrapper {
  padding: 4rem 0;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 4rem;
}

.page-btn {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 1px solid rgba(0,49,83,0.2);
  background: white;
  color: var(--color-prussian-blue);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.page-btn:hover:not(:disabled) {
  background-color: var(--color-prussian-blue);
  color: white;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-btn svg {
  width: 20px;
  height: 20px;
}

.page-numbers {
  display: flex;
  gap: 0.5rem;
}

.page-number {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  border: 1px solid transparent;
  background: transparent;
  color: #555;
  font-family: var(--font-sans);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.page-number:hover {
  background-color: rgba(0,49,83,0.05);
}

.page-number.active {
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  font-weight: 700;
}
</style>
