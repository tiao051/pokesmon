<script setup>
	import {ref, onMounted, onUnmounted, computed} from "vue";
	import {productService} from "../services/products.service";

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
		data: preorderData,
		pending: preorderPending,
		error: preorderError,
	} = useLazyRail("preorder", () =>
		productService.getProducts({limit: RAIL_LIMIT, isPreorder: true}),
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
	const preorderProducts = computed(() => preorderData.value?.items ?? []);
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
			title: 'The "Sunflowers Eevee" canvas painting is back in stock!',
			date: "April 15, 2026",
		},
		{
			id: 2,
			tag: "Event",
			title: "Join our special tour of the Pokémon masterclass collection.",
			date: "April 14, 2026",
		},
		{
			id: 3,
			tag: "Limited Release",
			title: "Pre-order the exclusive Snorlax Bedroom diorama set starting this weekend.",
			date: "April 12, 2026",
		},
		{
			id: 4,
			tag: "Shop News",
			title: "We're open later! Now until 9 PM on weekends.",
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
					<span class="eyebrow gold-text">Just In</span>
					<h2 class="hero-title white-text">Starry<br />Pikachu</h2>
					<p class="hero-desc white-text-muted">
						A post-impressionist piece showing the electric forest
						guardian under a swirling starry sky.
					</p>

					<div class="hero-buttons">
						<NuxtLink
							to="/products"
							class="btn-primary rounded-pill hero-cta"
							>Browse All Items</NuxtLink
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
			<header class="shop-hero">
				<img
					src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/master-ball.png"
					alt=""
					aria-hidden="true"
					class="shop-hero-stamp"
					loading="lazy"
					decoding="async"
				/>
				<div class="shop-hero-text">
					<p class="shop-hero-eyebrow">— Today's Picks · Refreshed Daily —</p>
					<h3 class="shop-hero-title">Featured Items</h3>
					<p class="shop-hero-subtitle">
						Five sections, picked from across our shop. Mew watches over every order, old and new.
					</p>
				</div>
				<img
					src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/151.png"
					alt="Mew, our shop's mythical guardian"
					class="shop-hero-mascot"
					loading="lazy"
					decoding="async"
				/>
			</header>

			<ProductRail
				v-if="preorderPending || preorderProducts.length"
				theme="preorder"
				eyebrow="Pre-orders · Coming Soon"
				title="Coming Soon"
				subtitle="Reserve items still on the way to our shop"
				:products="preorderProducts"
				:pending="preorderPending"
				:error="preorderError"
				view-all-link="/products?isPreorder=true"
			/>

			<ProductRail
				theme="display"
				eyebrow="Just Arrived"
				title="New Arrivals"
				subtitle="The latest items, freshly added"
				:products="newestProducts"
				:pending="newestPending"
				:error="newestError"
			/>

			<ProductRail
				theme="master"
				eyebrow="Top Picks"
				title="Best of Our Shop"
				subtitle="Our most-loved items"
				:products="topPricedProducts"
				:pending="topPricedPending"
				:error="topPricedError"
				view-all-link="/products"
			/>

			<ProductRail
				theme="vault"
				eyebrow="Sealed Items"
				title="Sealed &amp; Untouched"
				subtitle="Sealed in original packaging, never opened"
				:products="sealedProducts"
				:pending="sealedPending"
				:error="sealedError"
				view-all-link="/products?sealed=true"
			/>

			<ProductRail
				v-if="curatorPending || curatorSet"
				theme="curator"
				eyebrow="Today's Set"
				title="Set of the Day"
				:subtitle="curatorSet ? `Today's pick: the ${curatorSet} set` : 'A new set picked daily'"
				:products="curatorProducts"
				:pending="curatorPending"
				:error="curatorError"
				:view-all-link="curatorSet ? `/products?setName=${encodeURIComponent(curatorSet)}` : undefined"
			/>

			<div class="load-more-container">
				<NuxtLink
					to="/products"
					class="btn-primary rounded-pill view-all-cta"
					>See All Items</NuxtLink
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

	/* Shop Section hero — Master Ball stamp + Mew mascot */
	.shop-section {
		position: relative;
	}

	.shop-hero {
		position: relative;
		display: flex;
		align-items: center;
		gap: 1.75rem;
		max-width: clamp(var(--container-max), 90vw, 1500px);
		margin: 0 auto 3rem;
		padding: 2rem 2rem 2rem 1.75rem;
		background:
			radial-gradient(circle at top right, rgba(255, 197, 18, 0.14) 0%, transparent 60%),
			radial-gradient(circle at bottom left, rgba(0, 49, 83, 0.06) 0%, transparent 50%),
			linear-gradient(180deg, rgba(194, 130, 27, 0.08) 0%, rgba(194, 130, 27, 0.015) 100%);
		border: 1.5px solid rgba(194, 130, 27, 0.3);
		border-radius: 18px;
		overflow: hidden;
		min-height: 170px;
	}

	.shop-hero::before {
		content: "";
		position: absolute;
		top: 0;
		left: 1.5rem;
		right: 1.5rem;
		height: 3px;
		background: linear-gradient(90deg, transparent, var(--color-sunflower-yellow) 30%, var(--color-sunflower-yellow) 70%, transparent);
		box-shadow: 0 0 14px rgba(255, 197, 18, 0.5);
	}

	.shop-hero-stamp {
		position: relative;
		z-index: 2;
		flex-shrink: 0;
		width: 76px;
		height: 76px;
		object-fit: contain;
		image-rendering: pixelated;
		image-rendering: crisp-edges;
		filter: drop-shadow(2px 3px 4px rgba(0, 49, 83, 0.32));
		transform: rotate(-8deg);
	}

	.shop-hero-text {
		position: relative;
		z-index: 2;
		flex: 1;
		min-width: 0;
	}

	.shop-hero-eyebrow {
		font-family: var(--font-serif);
		font-style: italic;
		color: #c2821b;
		font-size: 0.95rem;
		letter-spacing: 1px;
		margin: 0 0 0.4rem;
	}

	.shop-hero-title {
		font-family: var(--font-serif);
		color: var(--color-prussian-blue);
		font-size: clamp(1.85rem, 4.2vw, 2.6rem);
		margin: 0 0 0.5rem;
		line-height: 1.15;
		letter-spacing: -0.5px;
	}

	.shop-hero-title::after {
		content: "";
		display: block;
		width: 64px;
		height: 3px;
		background-color: var(--color-cypress-green);
		margin-top: 0.6rem;
		border-radius: 2px;
	}

	.shop-hero-subtitle {
		font-family: var(--font-sans);
		color: #555;
		font-size: 0.95rem;
		line-height: 1.55;
		margin: 0.85rem 0 0;
		max-width: 540px;
	}

	/* Mew mascot — mythical guardian floating */
	.shop-hero-mascot {
		position: relative;
		z-index: 2;
		flex-shrink: 0;
		width: 150px;
		height: 150px;
		object-fit: contain;
		transform-origin: center bottom;
		filter: drop-shadow(2px 4px 10px rgba(255, 105, 180, 0.28));
		animation: mew-float 4s ease-in-out infinite;
	}

	@keyframes mew-float {
		0%, 100% { transform: translateY(0) rotate(4deg); }
		50%      { transform: translateY(-8px) rotate(1deg); }
	}

	@media (max-width: 768px) {
		.shop-hero-mascot { width: 110px; height: 110px; }
		.shop-hero-stamp { width: 60px; height: 60px; }
	}

	@media (max-width: 600px) {
		.shop-hero {
			flex-direction: column;
			text-align: center;
			padding: 1.5rem 1.25rem 1rem;
		}
		.shop-hero-title::after {
			margin-left: auto;
			margin-right: auto;
		}
		.shop-hero-subtitle {
			margin-left: auto;
			margin-right: auto;
		}
		.shop-hero-mascot { width: 100px; height: 100px; }
	}

</style>
