<script setup lang="ts">
	import { ref, computed, watch, onBeforeUnmount } from 'vue'
	import { useRoute } from 'vue-router'
	import { productService } from '../services/productService'

	const PRICE_MAX = 2000
	const PRICE_COMMIT_DEBOUNCE_MS = 300
	const itemsPerPage = 8

	const route = useRoute()
	const query = computed(() => route.query.q?.toString() || '')

	const selectedCategory = ref('')

	const maxPrice = ref(PRICE_MAX)
	const appliedMaxPrice = ref(PRICE_MAX)
	const isDraggingPrice = ref(false)

	let priceCommitTimer: ReturnType<typeof setTimeout> | null = null
	const commitMaxPrice = () => {
	  if (priceCommitTimer) clearTimeout(priceCommitTimer)
	  priceCommitTimer = setTimeout(() => {
	    appliedMaxPrice.value = maxPrice.value
	  }, PRICE_COMMIT_DEBOUNCE_MS)
	}
	onBeforeUnmount(() => {
	  if (priceCommitTimer) clearTimeout(priceCommitTimer)
	})

	const currentPage = ref(1)

	watch([query, selectedCategory, appliedMaxPrice], () => {
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
	  maxPrice: appliedMaxPrice.value < PRICE_MAX ? appliedMaxPrice.value : undefined,
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
						<h3 :class="{ 'is-pending': isDraggingPrice }">
							Max Price:
							<span class="price-val">${{ maxPrice }}</span>
						</h3>
						<p
							class="price-hint"
							:class="{ 'is-visible': isDraggingPrice }"
							aria-live="polite"
						>
							Release to apply
						</p>
						<input
							type="range"
							min="0"
							:max="PRICE_MAX"
							step="10"
							v-model.number="maxPrice"
							class="price-slider"
							@pointerdown="isDraggingPrice = true"
							@pointerup="isDraggingPrice = false"
							@pointercancel="isDraggingPrice = false"
							@change="commitMaxPrice"
						/>
						<div class="price-labels">
							<span>$0</span>
							<span>${{ PRICE_MAX }}</span>
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
						class="skeleton skeleton-card"
					></div>
				</div>

				<!-- Error -->
				<p v-else-if="searchError" class="inline-error">
					The curators are having trouble with the catalog. Please try
					again.
				</p>

				<div
					v-else-if="filteredProducts.length > 0"
					class="product-grid"
					:class="{ 'is-refetching': searchLoading }"
					:aria-busy="searchLoading"
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

				<AppPagination
					:current-page="currentPage"
					:total-items="totalResults"
					:page-size="itemsPerPage"
					@update:current-page="(p) => (currentPage = p)"
				/>
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
		transition: opacity 0.15s ease;
	}

	.filter-group h3.is-pending .price-val {
		opacity: 0.5;
	}

	.price-hint {
		margin: 0;
		height: 0;
		overflow: hidden;
		line-height: 0.9rem;
		font-family: var(--font-sans);
		font-size: 0.7rem;
		font-weight: 400;
		text-transform: uppercase;
		letter-spacing: 0.08em;
		color: #999;
		opacity: 0;
		transition: height 0.2s ease, margin-bottom 0.2s ease,
			opacity 0.2s ease;
		pointer-events: none;
	}

	.price-hint.is-visible {
		height: 0.9rem;
		margin-bottom: 0.4rem;
		opacity: 1;
	}

	.price-slider {
		width: 100%;
		accent-color: var(--color-prussian-blue);
		margin-bottom: 0.5rem;
		transition: accent-color 0.15s ease;
	}

	.price-slider:active {
		accent-color: var(--color-cypress-green);
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
		transition: opacity 0.2s ease;
	}

	.product-grid.is-refetching {
		opacity: 0.5;
		pointer-events: none;
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

</style>
