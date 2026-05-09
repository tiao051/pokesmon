<script setup>
	import {ref, computed, watch} from "vue";
	import {useRoute} from "vue-router";
	import {productService} from "../../services/productService";

	const route = useRoute();

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

	const selectedType = ref(route.query.productType?.toString() ?? "");
	const selectedSet = ref(route.query.setName?.toString() ?? "");
	const onlySealed = ref(route.query.sealed === "true");
	const onlyPreorder = ref(route.query.isPreorder === "true");

	const page = ref(1);
	const limit = 20;

	watch([selectedType, selectedSet, onlySealed, onlyPreorder], () => {
		page.value = 1;
	});

	const queryParams = computed(() => {
		const params = {limit, page: page.value};
		if (selectedType.value) params.productType = selectedType.value;
		if (selectedSet.value) params.setName = selectedSet.value;
		if (onlySealed.value) params.sealed = true;
		if (onlyPreorder.value) params.isPreorder = true;
		return params;
	});

	const {
		data: productsData,
		pending: productsLoading,
		error: productsError,
	} = await useAsyncData(
		"products-list",
		() => productService.getProducts(queryParams.value),
		{watch: [queryParams]},
	);

	const filteredProducts = computed(() => productsData.value?.items ?? []);
	const filteredCount = computed(() => productsData.value?.total ?? 0);

	useHead({title: "The Collection — PokéGogh"});
</script>

<template>
	<main class="collection-page">
		<header class="page-header">
			<NuxtImg
				src="/images/big_pokeball.jpg"
				alt=""
				class="header-pokeball"
				width="56"
				height="56"
				loading="eager"
				decoding="async"
			/>
			<span class="eyebrow gold-italic">PokéGogh Museum Catalog</span>
			<h1 class="page-title">The Collection</h1>
			<p class="page-subtitle">
				Browse the museum's complete inventory of curated
				post-impressionist Pokémon artifacts.
			</p>
			<div class="header-trainer-belt" aria-hidden="true">
				<NuxtImg
					src="/images/red_pokeball.jpg"
					alt=""
					class="belt-pokeball"
					width="22"
					height="22"
					loading="lazy"
					decoding="async"
				/>
				<NuxtImg
					src="/images/blue_pokeball.jpg"
					alt=""
					class="belt-pokeball"
					width="22"
					height="22"
					loading="lazy"
					decoding="async"
				/>
				<NuxtImg
					src="/images/black_pokeball.jpg"
					alt=""
					class="belt-pokeball"
					width="22"
					height="22"
					loading="lazy"
					decoding="async"
				/>
			</div>
		</header>

		<!-- Filter Bar -->
		<div class="filter-bar">
			<!-- Loading skeleton -->
			<template v-if="catsLoading">
				<div class="filter-left">
					<div class="skeleton select-skeleton"></div>
					<div class="skeleton select-skeleton"></div>
				</div>
				<div class="skeleton count-skeleton"></div>
			</template>

			<!-- Error fallback -->
			<p v-else-if="catsError" class="inline-error">
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
			<div v-for="n in 8" :key="n" class="skeleton skeleton-card"></div>
		</div>

		<p v-else-if="productsError" class="inline-error">
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

		<AppPagination
			:current-page="page"
			:total-items="filteredCount"
			:page-size="limit"
			@update:current-page="(p) => (page = p)"
		/>
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

	/* Skeleton loading for selects (shimmer comes from global .skeleton) */
	.select-skeleton {
		width: 220px;
		height: 60px;
		border-radius: 8px;
	}

	.count-skeleton {
		width: 70px;
		height: 52px;
		border-radius: 8px;
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

</style>
