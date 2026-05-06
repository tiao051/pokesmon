<script setup>
	import {ref, computed, watch} from "vue";
	import {productService} from "../../services/productService";

	const {
		data: apiCategories,
		pending: catsLoading,
		error: catsError,
	} = await useAsyncData("product-categories", () =>
		productService.getCategories(),
	);

	const productTypes = computed(
		() => apiCategories.value?.productTypes ?? [],
	);
	const setNames = computed(() => apiCategories.value?.setNames ?? []);

	const selectedType = ref("");
	const selectedSet = ref("");

	const page = ref(1);
	const limit = 20;

	watch([selectedType, selectedSet], () => {
		page.value = 1;
	});

	const queryParams = computed(() => {
		const params = {limit, page: page.value};
		if (selectedType.value) params.productType = selectedType.value;
		if (selectedSet.value) params.setName = selectedSet.value;
		return params;
	});

	const {
		data: productsData,
		pending: productsLoading,
		error: productsError,
		refresh: refreshProducts,
	} = await useAsyncData(
		"products-list",
		() => productService.getProducts(queryParams.value),
		{watch: [queryParams]},
	);

	const filteredProducts = computed(() => productsData.value?.items ?? []);
	const filteredCount = computed(() => productsData.value?.total ?? 0);
	const totalPages = computed(() => Math.ceil(filteredCount.value / limit));

	const visiblePages = computed(() => {
		const total = totalPages.value;
		const current = page.value;
		const delta = 1;
		const range = [];
		const rangeWithDots = [];
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

	useHead({title: "The Collection — PokéGogh"});
</script>

<template>
	<main class="collection-page">
		<header class="page-header">
			<img
				src="/images/big_pokeball.jpg"
				alt=""
				class="header-pokeball"
			/>
			<span class="eyebrow gold-italic">PokéGogh Museum Catalog</span>
			<h1 class="page-title">The Collection</h1>
			<p class="page-subtitle">
				Browse the museum's complete inventory of curated
				post-impressionist Pokémon artifacts.
			</p>
			<div class="header-trainer-belt" aria-hidden="true">
				<img
					src="/images/red_pokeball.jpg"
					alt=""
					class="belt-pokeball"
				/>
				<img
					src="/images/blue_pokeball.jpg"
					alt=""
					class="belt-pokeball"
				/>
				<img
					src="/images/black_pokeball.jpg"
					alt=""
					class="belt-pokeball"
				/>
			</div>
		</header>

		<!-- Filter Bar -->
		<div class="filter-bar">
			<!-- Loading skeleton -->
			<template v-if="catsLoading">
				<div class="filter-left">
					<div class="select-skeleton"></div>
					<div class="select-skeleton"></div>
				</div>
				<div class="count-skeleton"></div>
			</template>

			<!-- Error fallback -->
			<p v-else-if="catsError" class="categories-error">
				Unable to load exhibition wings. Showing all artifacts.
			</p>

			<!-- Loaded selects + count -->
			<template v-else>
				<div class="filter-left">
					<div class="select-wrapper">
						<label for="filter-type" class="select-label"
							>Product Type</label
						>
						<select
							id="filter-type"
							v-model="selectedType"
							class="museum-select"
						>
							<option value="">All Types</option>
							<option
								v-for="t in productTypes"
								:key="t"
								:value="t"
							>
								{{ t }}
							</option>
						</select>
					</div>
					<div class="select-wrapper">
						<label for="filter-set" class="select-label"
							>Series / Set</label
						>
						<select
							id="filter-set"
							v-model="selectedSet"
							class="museum-select"
						>
							<option value="">All Sets</option>
							<option v-for="s in setNames" :key="s" :value="s">
								{{ s }}
							</option>
						</select>
					</div>
				</div>
				<div class="filter-right">
					<span class="count-eyebrow">Artifacts</span>
					<span class="count-value">{{ filteredCount }}</span>
				</div>
			</template>
		</div>

		<!-- Products -->
		<div
			v-if="productsLoading && !filteredProducts.length"
			class="product-grid"
		>
			<div v-for="n in 8" :key="n" class="product-card-skeleton"></div>
		</div>

		<p v-else-if="productsError" class="categories-error">
			Unable to load the collection. Please try again.
		</p>

		<div v-else-if="filteredProducts.length" class="product-grid">
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

		<!-- Pagination -->
		<div v-if="totalPages > 1" class="pagination">
			<button
				class="page-btn prev-btn"
				:disabled="page === 1"
				@click="page--"
				aria-label="Previous page"
			>
				<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
					<polyline points="15 18 9 12 15 6"></polyline>
				</svg>
			</button>

			<div class="page-numbers">
				<template v-for="(p, idx) in visiblePages" :key="idx">
					<span v-if="p === '...'" class="page-ellipsis">...</span>
					<button
						v-else
						class="page-number"
						:class="{ active: page === p }"
						@click="page = p"
					>
						{{ p }}
					</button>
				</template>
			</div>

			<button
				class="page-btn next-btn"
				:disabled="page === totalPages"
				@click="page++"
				aria-label="Next page"
			>
				<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
					<polyline points="9 18 15 12 9 6"></polyline>
				</svg>
			</button>
		</div>
	</main>
</template>

<style scoped>
	.collection-page {
		width: 100%;
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
		content: "";
		width: 60px;
		height: 1px;
		background: linear-gradient(
			90deg,
			transparent,
			rgba(0, 49, 83, 0.35),
			transparent
		);
	}

	.belt-pokeball {
		width: 24px;
		height: 24px;
		border-radius: 50%;
		object-fit: cover;
		border: 1.5px solid var(--color-prussian-blue);
		box-shadow: 1.5px 1.5px 0 rgba(0, 49, 83, 0.2);
	}

	/* Filter Bar */
	.filter-bar {
		width: 100%;
		display: flex;
		flex-wrap: wrap;
		gap: 1.25rem;
		justify-content: space-between;
		align-items: flex-end;
		margin-bottom: 2.5rem;
	}

	.filter-left {
		display: flex;
		flex-wrap: wrap;
		gap: 1.25rem;
		align-items: flex-end;
	}

	.filter-right {
		display: flex;
		flex-direction: column;
		align-items: flex-end;
		gap: 0.15rem;
	}

	.count-eyebrow {
		font-family: var(--font-serif);
		font-style: italic;
		font-size: 0.8rem;
		color: #c2821b;
		letter-spacing: 0.5px;
	}

	.count-value {
		font-family: var(--font-serif);
		font-size: 2rem;
		font-weight: 700;
		color: var(--color-prussian-blue);
		line-height: 1;
	}

	.select-wrapper {
		display: flex;
		flex-direction: column;
		gap: 0.4rem;
		min-width: 220px;
	}

	.select-label {
		font-family: var(--font-serif);
		font-style: italic;
		font-size: 0.85rem;
		color: #c2821b;
		letter-spacing: 0.5px;
		padding-left: 0.25rem;
	}

	.museum-select {
		font-family: var(--font-sans);
		font-size: 0.95rem;
		color: var(--color-prussian-blue);
		background-color: var(--color-linen);
		border: 1.5px solid var(--color-prussian-blue);
		border-radius: 8px;
		padding: 0.65rem 2.25rem 0.65rem 1rem;
		cursor: pointer;
		appearance: none;
		-webkit-appearance: none;
		background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='8' viewBox='0 0 12 8'%3E%3Cpath d='M1 1.5l5 5 5-5' stroke='%23003153' stroke-width='1.5' fill='none' stroke-linecap='round' stroke-linejoin='round'/%3E%3C/svg%3E");
		background-repeat: no-repeat;
		background-position: right 0.85rem center;
		transition:
			border-color 0.2s ease,
			box-shadow 0.2s ease;
		box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.12);
	}

	.museum-select:hover {
		border-color: var(--color-cypress-green);
		box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.2);
	}

	.museum-select:focus {
		outline: none;
		border-color: var(--color-sunflower-yellow);
		box-shadow:
			0 0 0 3px rgba(255, 197, 18, 0.3),
			2px 2px 0 rgba(0, 49, 83, 0.2);
	}

	/* Skeleton loading for selects */
	.select-skeleton {
		width: 220px;
		height: 60px;
		border-radius: 8px;
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

	.count-skeleton {
		width: 70px;
		height: 52px;
		border-radius: 8px;
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

	.eyebrow.gold-italic {
		font-family: var(--font-serif);
		font-style: italic;
		color: #c2821b;
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
		content: "";
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

	.categories-error {
		font-family: var(--font-serif);
		font-style: italic;
		color: #c2821b;
		font-size: 0.95rem;
		text-align: center;
		width: 100%;
	}

	/* Pagination */
	.pagination {
		display: flex;
		justify-content: center;
		align-items: center;
		gap: 2rem;
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
		width: 120px;
		height: 3px;
		background: var(--color-sunflower-yellow);
		border-radius: 2px;
	}

	.page-btn {
		width: 52px;
		height: 52px;
		border-radius: 50%;
		border: 2px solid var(--color-prussian-blue);
		background: #fff;
		color: var(--color-prussian-blue);
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
		transition: all 0.4s cubic-bezier(0.23, 1, 0.32, 1);
		box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.1);
	}

	.page-btn:hover:not(:disabled) {
		background-color: var(--color-prussian-blue);
		color: #fff;
		transform: translateY(-3px) scale(1.05);
		box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.15);
	}

	.page-btn:active:not(:disabled) {
		transform: translateY(-1px);
		box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.1);
	}

	.page-btn:disabled {
		opacity: 0.25;
		cursor: not-allowed;
		filter: grayscale(1);
		border-color: #ccc;
		box-shadow: none;
	}

	.page-btn svg {
		width: 22px;
		height: 22px;
	}

	.page-numbers {
		display: flex;
		gap: 1rem;
		align-items: center;
	}

	.page-number {
		min-width: 46px;
		height: 46px;
		padding: 0 0.5rem;
		border-radius: 12px;
		border: 2px solid transparent;
		background: transparent;
		color: var(--color-prussian-blue);
		font-family: var(--font-serif);
		font-weight: 700;
		font-size: 1.2rem;
		cursor: pointer;
		transition: all 0.3s ease;
		display: flex;
		align-items: center;
		justify-content: center;
		position: relative;
	}

	.page-number::after {
		content: "";
		position: absolute;
		bottom: 6px;
		left: 50%;
		transform: translateX(-50%) scaleX(0);
		width: 20px;
		height: 2px;
		background: var(--color-cypress-green);
		transition: transform 0.3s ease;
	}

	.page-number:hover:not(.active) {
		color: var(--color-cypress-green);
		background: rgba(0, 49, 83, 0.04);
	}

	.page-number:hover::after {
		transform: translateX(-50%) scaleX(1);
	}

	.page-number.active {
		background-color: var(--color-sunflower-yellow);
		border-color: var(--color-prussian-blue);
		color: var(--color-prussian-blue);
		box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.15);
		transform: translateY(-2px);
	}

	.page-ellipsis {
		font-family: var(--font-serif);
		color: #999;
		font-size: 1.2rem;
		font-weight: 700;
		padding: 0 0.25rem;
		letter-spacing: 2px;
	}

	@media (max-width: 600px) {
		.pagination {
			gap: 1rem;
		}
		.page-numbers {
			gap: 0.5rem;
		}
		.page-btn {
			width: 44px;
			height: 44px;
		}
		.page-number {
			min-width: 38px;
			height: 38px;
			font-size: 1rem;
		}
	}
</style>
