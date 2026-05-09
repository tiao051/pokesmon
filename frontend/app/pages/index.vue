<script setup>
	import {ref, onMounted, onUnmounted, computed} from "vue";
	import {productService} from "../services/productService";

	const RAIL_LIMIT = 8;
	const HOME_CACHE_TTL_MS = 120_000;

	const dayOfYearSeed = () => {
		const now = new Date();
		const start = new Date(now.getFullYear(), 0, 0);
		return Math.floor((now.getTime() - start.getTime()) / 86_400_000);
	};

	const useLazyRail = (key, fetcher) => {
		const cache = useState(`home-rail-${key}`, () => ({
			data: null,
			expiresAt: 0,
		}));
		return useAsyncData(
			`home-${key}`,
			async () => {
				const data = await fetcher();
				cache.value = {
					data,
					expiresAt: Date.now() + HOME_CACHE_TTL_MS,
				};
				return data;
			},
			{
				lazy: true,
				getCachedData: () => {
					const c = cache.value;
					if (!c.data || Date.now() > c.expiresAt) return undefined;
					return c.data;
				},
			},
		);
	};

	const {
		data: newestData,
		pending: newestPending,
		error: newestError,
	} = useLazyRail("newest", () =>
		productService.getProducts({limit: RAIL_LIMIT, sort: "newest"}),
	);

	const {
		data: topPricedData,
		pending: topPricedPending,
		error: topPricedError,
	} = useLazyRail("top-priced", () =>
		productService.getProducts({limit: RAIL_LIMIT}),
	);

	const {
		data: sealedData,
		pending: sealedPending,
		error: sealedError,
	} = useLazyRail("sealed", () =>
		productService.getProducts({limit: RAIL_LIMIT, sealed: true}),
	);

	const {
		data: curatorData,
		pending: curatorPending,
		error: curatorError,
	} = useLazyRail("curator", async () => {
		const categories = await productService.getCategories();
		const setNames = categories.setNames ?? [];
		const set = setNames.length
			? setNames[dayOfYearSeed() % setNames.length]
			: null;
		if (!set) return {items: [], curatorSet: null};
		const result = await productService.getProducts({
			limit: RAIL_LIMIT,
			setName: set,
		});
		return {items: result.items, curatorSet: set};
	});

	const newestProducts = computed(() => newestData.value?.items ?? []);
	const topPricedProducts = computed(() => topPricedData.value?.items ?? []);
	const sealedProducts = computed(() => sealedData.value?.items ?? []);
	const curatorProducts = computed(() => curatorData.value?.items ?? []);
	const curatorSet = computed(() => curatorData.value?.curatorSet ?? null);

	useHead({
		link: [
			{
				rel: "preload",
				as: "image",
				href: "/images/pikachu_vangogh.webp",
				type: "image/webp",
				fetchpriority: "high",
			},
		],
	});

	const newsItems = ref([
		{
			id: 1,
			tag: "New Arrival",
			title: 'The "Sunflowers Eevee" canvas painting has officially been restocked in the Museum Shop.',
			date: "April 15, 2026",
		},
		{
			id: 2,
			tag: "Exhibition",
			title: "Join our special guided tour exploring the brushstrokes of the Pokémon masterclass collection.",
			date: "April 14, 2026",
		},
		{
			id: 3,
			tag: "Limited Release",
			title: "Pre-order the exclusive Snorlax Bedroom diorama miniature set starting this weekend.",
			date: "April 12, 2026",
		},
		{
			id: 4,
			tag: "Museum News",
			title: "Our gallery hours are extending! Now open until 9 PM on weekends for night viewings.",
			date: "April 10, 2026",
		},
		{
			id: 5,
			tag: "Shop Update",
			title: "New post-impressionist mugs featuring Charizard and Bulbasaur are now available.",
			date: "April 05, 2026",
		},
	]);

	const currentNewsIndex = ref(0);
	let newsInterval = null;

	const nextNews = () => {
		currentNewsIndex.value =
			(currentNewsIndex.value + 1) % newsItems.value.length;
	};

	onMounted(() => {
		newsInterval = setInterval(nextNews, 3000);
	});

	onUnmounted(() => {
		if (newsInterval) clearInterval(newsInterval);
	});
</script>

<template>
	<main>
		<!-- Hero Section -->
		<section class="hero full-width-hero">
			<img
				src="/images/pikachu_vangogh.webp"
				alt="Starry Pikachu"
				class="hero-bg full-width"
				width="1920"
				height="1080"
				fetchpriority="high"
				loading="eager"
				decoding="async"
			/>
			<div class="hero-overlay"></div>

			<div class="hero-content full-width-content">
				<div class="hero-text-box overlay-text">
					<span class="eyebrow gold-text">New Acquisition</span>
					<h2 class="hero-title white-text">Starry<br />Pikachu</h2>
					<p class="hero-desc white-text-muted">
						A post-impressionist masterpiece capturing the electric
						essence of the forest guardian beneath a swirling,
						cosmic firmament.
					</p>

					<div class="hero-buttons">
						<NuxtLink
							to="/products"
							class="btn-primary rounded-pill hero-cta"
							>Explore The Collection</NuxtLink
						>
						<NuxtLink
							to="/products/starry-pikachu-promo"
							class="btn-secondary rounded-pill outline-glass hero-cta"
							>View Details</NuxtLink
						>
					</div>

				</div>
			</div>
		</section>

		<!-- What's New Separator -->
		<section class="whats-new-section">
			<div class="whats-new-belt" aria-hidden="true">
				<span class="line"></span>
				<NuxtImg
					src="/images/red_pokeball.jpg"
					alt=""
					class="bp"
					width="22"
					height="22"
					loading="lazy"
					decoding="async"
				/>
				<NuxtImg
					src="/images/blue_pokeball.jpg"
					alt=""
					class="bp"
					width="22"
					height="22"
					loading="lazy"
					decoding="async"
				/>
				<NuxtImg
					src="/images/black_pokeball.jpg"
					alt=""
					class="bp"
					width="22"
					height="22"
					loading="lazy"
					decoding="async"
				/>
				<span class="line"></span>
			</div>
			<h2 class="whats-new-text">What's new?</h2>
		</section>

		<!-- News Banner Auto-Slider Section -->
		<section class="news-banner-section">
			<div class="news-banner-background"></div>
			<div class="news-banner-container">
				<transition name="slide-fade" mode="out-in">
					<div :key="currentNewsIndex" class="news-banner-content">
						<span class="gold-text">{{
							newsItems[currentNewsIndex].tag
						}}</span>
						<h3 class="news-banner-title">
							{{ newsItems[currentNewsIndex].title }}
						</h3>
						<p class="white-text-muted">
							{{ newsItems[currentNewsIndex].date }}
						</p>
					</div>
				</transition>

				<!-- Pagination Dots -->
				<div class="news-banner-indicators">
					<span
						v-for="(item, index) in newsItems"
						:key="index"
						class="banner-dot"
						:class="{active: index === currentNewsIndex}"
						@click="currentNewsIndex = index"
					></span>
				</div>
			</div>
		</section>

		<!-- Shop Section -->
		<section class="shop-section">
			<NuxtImg
				src="/images/big_pokeball.jpg"
				alt=""
				class="shop-pokeball-stamp"
				width="56"
				height="56"
				loading="lazy"
				decoding="async"
			/>
			<h3 class="section-title">Featured from the Collection</h3>

			<ProductRail
				theme="display"
				eyebrow="Wing 01 · Recently Acquired"
				title="Now on Display"
				subtitle="Latest acquisitions, freshly curated"
				:products="newestProducts"
				:pending="newestPending"
				:error="newestError"
			/>

			<ProductRail
				theme="master"
				eyebrow="Wing 02 · Premier Selection"
				title="Master Collection"
				subtitle="The gallery's most prized pieces"
				:products="topPricedProducts"
				:pending="topPricedPending"
				:error="topPricedError"
				view-all-link="/products"
			/>

			<ProductRail
				theme="vault"
				eyebrow="Wing 03 · Sealed Archive"
				title="Vault Treasures"
				subtitle="Sealed and untouched, just as they arrived"
				:products="sealedProducts"
				:pending="sealedPending"
				:error="sealedError"
				view-all-link="/products?sealed=true"
			/>

			<ProductRail
				v-if="curatorPending || curatorSet"
				theme="curator"
				eyebrow="Wing 04 · Daily Curation"
				title="Curator's Eye"
				:subtitle="curatorSet ? `A daily look into the ${curatorSet} expansion` : 'A daily look into our curated expansions'"
				:products="curatorProducts"
				:pending="curatorPending"
				:error="curatorError"
				:view-all-link="curatorSet ? `/products?setName=${encodeURIComponent(curatorSet)}` : undefined"
			/>

			<div class="load-more-container">
				<NuxtLink
					to="/products"
					class="btn-primary rounded-pill view-all-cta"
					>View The Full Collection</NuxtLink
				>
			</div>
		</section>
	</main>
</template>

<style scoped>
	.hero-cta,
	.view-all-cta {
		text-decoration: none;
		display: inline-block;
	}

	.view-all-cta:hover,
	.hero-buttons .btn-primary.rounded-pill:hover {
		background: linear-gradient(
			135deg,
			#f5b341 0%,
			#e6a039 50%,
			#b07212 100%
		);
		transform: translateY(-2px) scale(1.02);
		letter-spacing: 1.5px;
		box-shadow:
			inset 1px 2px 4px rgba(255, 255, 255, 0.55),
			0 0 22px rgba(245, 176, 65, 0.45),
			0 6px 16px rgba(0, 0, 0, 0.25);
	}

	/* What's New — trainer belt above the title */
	.whats-new-belt {
		display: flex;
		justify-content: center;
		align-items: center;
		gap: 0.75rem;
		margin-bottom: 1.25rem;
	}
	.whats-new-belt .line {
		width: 60px;
		height: 1px;
		background: linear-gradient(
			90deg,
			transparent,
			rgba(0, 49, 83, 0.35),
			transparent
		);
	}
	.whats-new-belt .bp {
		width: 22px;
		height: 22px;
		border-radius: 50%;
		object-fit: cover;
		border: 1.5px solid var(--color-prussian-blue);
		box-shadow: 1.5px 1.5px 0 rgba(0, 49, 83, 0.2);
	}

	/* Shop Section — pokeball stamp before section title */
	.shop-section {
		position: relative;
	}
	.shop-pokeball-stamp {
		display: block;
		width: 56px;
		height: 56px;
		margin: 0 auto 1.25rem;
		border-radius: 50%;
		object-fit: cover;
		border: 2px solid var(--color-prussian-blue);
		box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.25);
		transform: rotate(-6deg);
	}

</style>
