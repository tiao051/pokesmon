<script setup lang="ts">
	import { ref, computed, watch } from 'vue'
	import { useRoute } from 'vue-router'
	import { productService } from '../services/productService'

	const route = useRoute()
	const query = computed(() => route.query.q?.toString() || '')

	const selectedCategory = ref('')
	const maxPrice = ref(500)

	const currentPage = ref(1)
	const itemsPerPage = 8

	watch([query, selectedCategory, maxPrice], () => {
	  currentPage.value = 1
	})

	const {
	  data: catData,
	} = await useAsyncData('search-categories', () =>
	  productService.getCategories(),
	)

	const categories = computed(() => catData.value?.productTypes ?? [])

	const toggleCategory = (cat: string) => {
	  selectedCategory.value = selectedCategory.value === cat ? '' : cat
	}

	const searchParams = computed(() => ({
	  search: query.value || undefined,
	  productType: selectedCategory.value || undefined,
	  limit: itemsPerPage,
	  page: currentPage.value,
	}))

	const {
	  data: searchData,
	  pending: searchLoading,
	  error: searchError,
	} = await useAsyncData(
	  'search-results',
	  () => productService.getProducts(searchParams.value),
	  { watch: [searchParams] },
	)

	const filteredProducts = computed(() => searchData.value?.items ?? [])
	const totalResults = computed(() => searchData.value?.total ?? 0)
	const totalPages = computed(() => Math.ceil(totalResults.value / itemsPerPage))

	const visiblePages = computed(() => {
	  const total = totalPages.value;
	  const current = currentPage.value;
	  const delta = 1;
	  const range: number[] = [];
	  const rangeWithDots: (number | string)[] = [];
	  let l;

	  for (let i = 1; i <= total; i++) {
	    if (
	      i === 1 ||
	      i === total ||
	      (i >= current - delta && i <= current + delta)
	    ) {
	      range.push(i);
	    }
	  }

	  for (let i of range) {
	    if (l) {
	      if (i - l === 2) {
	        rangeWithDots.push(l + 1);
	      } else if (i - l !== 1) {
	        rangeWithDots.push("...");
	      }
	    }
	    rangeWithDots.push(i);
	    l = i;
	  }

	  return rangeWithDots;
	});
</script>

<template>
	<div class="search-page">
		<div class="search-header-banner">
			<div class="banner-content">
				<h1 class="search-title">Search Results</h1>
				<p class="search-subtitle" v-if="query">
					Results for "<span class="highlight">{{ query }}</span
					>"
				</p>
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
							<label
								v-for="cat in categories"
								:key="cat"
								class="checkbox-label"
							>
								<input
									type="checkbox"
									:checked="selectedCategory === cat"
									class="custom-checkbox"
									@change="toggleCategory(cat)"
								/>
								<span class="checkbox-text">{{ cat }}</span>
							</label>
						</div>
					</div>

					<div class="filter-group">
						<h3>
							Max Price:
							<span class="price-val">${{ maxPrice }}</span>
						</h3>
						<input
							type="range"
							min="0"
							max="500"
							step="10"
							v-model.number="maxPrice"
							class="price-slider"
						/>
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
					<p v-if="searchLoading" class="results-count">
						Searching the gallery...
					</p>
					<p v-else class="results-count">
						Showing {{ filteredProducts.length }} of
						{{ totalResults }} products
					</p>
				</div>

				<!-- Loading -->
				<div
					v-if="searchLoading && !filteredProducts.length"
					class="product-grid"
				>
					<div
						v-for="n in 4"
						:key="n"
						class="product-card-skeleton"
					></div>
				</div>

				<!-- Error -->
				<p v-else-if="searchError" class="search-error">
					The curators are having trouble with the catalog. Please try
					again.
				</p>

				<div
					v-else-if="filteredProducts.length > 0"
					class="product-grid"
				>
					<ProductCard
						v-for="product in filteredProducts"
						:key="product.id"
						:product="product"
					/>
				</div>
				<div v-else class="empty-state-wrapper">
					<EmptyState
						title="No Masterpieces Found"
						message="Try adjusting your filters or searching for different keywords to find what you're looking for."
					/>
				</div>

				<!-- Pagination -->
				<div v-if="totalPages > 1" class="pagination">
					<button
						class="page-btn prev-btn"
						:disabled="currentPage === 1"
						@click="currentPage--"
						aria-label="Previous page"
					>
						<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
							<polyline points="15 18 9 12 15 6"></polyline>
						</svg>
					</button>

					<div class="page-numbers">
						<template v-for="(p, idx) in visiblePages" :key="idx">
							<span v-if="p === '...'" class="page-ellipsis"
								>...</span
							>
							<button
								v-else
								class="page-number"
								:class="{active: currentPage === p}"
								@click="currentPage = p as number"
							>
								{{ p }}
							</button>
						</template>
					</div>

					<button
						class="page-btn next-btn"
						:disabled="currentPage === totalPages"
						@click="currentPage++"
						aria-label="Next page"
					>
						<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
							<polyline points="9 18 15 12 9 6"></polyline>
						</svg>
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
		background-image: linear-gradient(
			135deg,
			rgba(0, 49, 83, 1) 0%,
			rgba(15, 94, 60, 0.8) 100%
		);
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
		box-shadow: 0 4px 20px rgba(0, 0, 0, 0.03);
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
		border-bottom: 1px solid rgba(0, 0, 0, 0.1);
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
		display: flex;
		justify-content: center;
		align-items: center;
		min-height: clamp(280px, 40vh, 460px);
	}

	.empty-state-wrapper :deep(.empty-state) {
		margin: 0 auto;
		width: 100%;
	}

	.pagination {
		display: flex;
		justify-content: center;
		align-items: center;
		gap: 1.5rem;
		margin-top: 6rem;
		padding-top: 4rem;
		border-top: 1px solid rgba(0, 49, 83, 0.1);
		position: relative;
	}

	.pagination::before {
		content: "";
		position: absolute;
		top: -1px;
		left: 50%;
		transform: translateX(-50%);
		width: 100px;
		height: 3px;
		background: var(--color-sunflower-yellow);
		border-radius: 2px;
	}

	.page-btn {
		width: 48px;
		height: 48px;
		border-radius: 50%;
		border: 2px solid var(--color-prussian-blue);
		background: #fff;
		color: var(--color-prussian-blue);
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
		transition: all 0.4s cubic-bezier(0.23, 1, 0.32, 1);
		box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.08);
	}

	.page-btn:hover:not(:disabled) {
		background-color: var(--color-prussian-blue);
		color: #fff;
		transform: translateY(-3px);
		box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.12);
	}

	.page-btn:active:not(:disabled) {
		transform: translateY(-1px);
		box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.08);
	}

	.page-btn:disabled {
		opacity: 0.25;
		cursor: not-allowed;
		filter: grayscale(1);
		border-color: #ddd;
		box-shadow: none;
	}

	.page-btn svg {
		width: 20px;
		height: 20px;
	}

	.page-numbers {
		display: flex;
		gap: 0.75rem;
		align-items: center;
	}

	.page-number {
		min-width: 42px;
		height: 42px;
		padding: 0 0.4rem;
		border-radius: 10px;
		border: 2px solid transparent;
		background: transparent;
		color: var(--color-prussian-blue);
		font-family: var(--font-serif);
		font-weight: 700;
		font-size: 1.1rem;
		cursor: pointer;
		transition: all 0.3s ease;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.page-number:hover:not(.active) {
		color: var(--color-cypress-green);
		background: rgba(0, 49, 83, 0.04);
	}

	.page-number.active {
		background-color: var(--color-sunflower-yellow);
		border-color: var(--color-prussian-blue);
		color: var(--color-prussian-blue);
		box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.12);
		transform: translateY(-2px);
	}

	.page-ellipsis {
		font-family: var(--font-serif);
		color: #aaa;
		font-size: 1.1rem;
		font-weight: 700;
		letter-spacing: 1px;
	}

	@media (max-width: 600px) {
		.pagination {
			gap: 1rem;
		}
		.page-numbers {
			gap: 0.5rem;
		}
		.page-btn {
			width: 40px;
			height: 40px;
		}
		.page-number {
			min-width: 36px;
			height: 36px;
			font-size: 0.95rem;
		}
	}

	/* Skeleton loading */
	.product-card-skeleton {
		aspect-ratio: 4/5;
		border-radius: 12px;
		background: linear-gradient(
			90deg,
			#e8e5d8 25%,
			#ddd9c8 50%,
			#e8e5d8 75%
		);
		background-size: 200% 100%;
		animation: shimmer 1.5s infinite;
		border: 1px solid rgba(0, 49, 83, 0.08);
	}

	.search-error {
		font-family: var(--font-serif);
		font-style: italic;
		color: #c2821b;
		font-size: 0.95rem;
		text-align: center;
		padding: 2rem 0;
	}

	@keyframes shimmer {
		0% {
			background-position: 200% 0;
		}
		100% {
			background-position: -200% 0;
		}
	}
</style>
