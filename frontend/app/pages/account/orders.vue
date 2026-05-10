<script setup lang="ts">
import {computed, ref, watch} from "vue";
import {orderService, type Order, type OrderListView} from "../../services/orders.service";

definePageMeta({
	middleware: "auth-required",
});

useHead({title: "My Orders — PokéGogh"});

const view = ref<OrderListView>("active");
const isPreorder = ref(false);

const params = computed(() => ({
	view: view.value,
	isPreorder: isPreorder.value,
}));

const {data, pending, error} = await useAsyncData<Order[]>(
	"account-orders",
	() => orderService.list(params.value),
	{watch: [params]},
);

// Stable cache: only commit results when a fetch fully settles. This keeps
// the previous list visible during refetches and prevents empty-state flashes
// when `data` momentarily resets between fetches.
const stableOrders = ref<Order[]>(data.value ?? []);
const hasSettledOnce = ref(data.value !== null);

watch(pending, (isPending) => {
	if (isPending) return;
	if (data.value !== null && data.value !== undefined) {
		stableOrders.value = data.value;
	}
	hasSettledOnce.value = true;
});

const orders = computed(() => stableOrders.value);

const setView = (next: OrderListView) => {
	view.value = next;
};

const swapType = () => {
	isPreorder.value = !isPreorder.value;
};

const swapButtonLabel = computed(() =>
	isPreorder.value ? "View Standard Orders" : "View Pre-orders",
);

const currentTypeLabel = computed(() =>
	isPreorder.value ? "Pre-orders" : "Standard Orders",
);

const emptyMessage = computed(() => {
	const phase = view.value === "active" ? "in progress" : "completed";
	const type = isPreorder.value ? "pre-orders" : "standard orders";
	return `No ${type} ${phase} yet.`;
});
</script>

<template>
	<section class="orders-page">
		<header class="orders-hero">
			<div class="orders-hero-text">
				<p class="orders-hero-eyebrow">— Your Order History —</p>
				<h1 class="orders-hero-title">My Orders</h1>
				<p class="orders-hero-subtitle">
					Pelipper is on the case — track your orders from our shop to your door.
				</p>
			</div>
			<img
				src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/279.png"
				alt="Pelipper, our shop's courier"
				class="orders-hero-mascot"
				loading="lazy"
				decoding="async"
			/>
		</header>

		<div class="controls-bar">
			<div class="view-tabs" role="tablist" aria-label="Order view">
				<button
					type="button"
					class="view-tab"
					role="tab"
					:aria-selected="view === 'active'"
					:class="{ active: view === 'active' }"
					@click="setView('active')"
				>
					In Progress
				</button>
				<button
					type="button"
					class="view-tab"
					role="tab"
					:aria-selected="view === 'completed'"
					:class="{ active: view === 'completed' }"
					@click="setView('completed')"
				>
					Completed
				</button>
			</div>

			<div class="type-swap">
				<span class="current-type">{{ currentTypeLabel }}</span>
				<button
					type="button"
					class="swap-button"
					:aria-label="swapButtonLabel"
					@click="swapType"
				>
					<svg
						viewBox="0 0 24 24"
						fill="none"
						stroke="currentColor"
						stroke-width="2"
						stroke-linecap="round"
						stroke-linejoin="round"
					>
						<polyline points="17 1 21 5 17 9" />
						<path d="M3 11V9a4 4 0 0 1 4-4h14" />
						<polyline points="7 23 3 19 7 15" />
						<path d="M21 13v2a4 4 0 0 1-4 4H3" />
					</svg>
					{{ swapButtonLabel }}
				</button>
			</div>
		</div>

		<div class="orders-area">
			<div v-if="!hasSettledOnce" class="orders-loading">
				<div v-for="n in 3" :key="n" class="skeleton order-skeleton" />
			</div>

			<p v-else-if="error" class="inline-error">
				Could not load your orders. Please try again.
			</p>

			<div
				v-else-if="orders.length"
				class="orders-list"
				:class="{ 'is-refetching': pending }"
				:aria-busy="pending"
			>
				<OrderCard v-for="order in orders" :key="order.id" :order="order" />
			</div>

			<div v-else class="orders-empty" :class="{ 'is-refetching': pending }">
				<img
					src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/79.png"
					alt="A drowsy Slowpoke"
					class="orders-empty-mascot"
					loading="lazy"
					decoding="async"
				/>
				<p class="empty-title">{{ emptyMessage }}</p>
				<p class="empty-subtitle">
					Slowpoke is waiting — your orders will show up here.
				</p>
			</div>
		</div>
	</section>
</template>

<style scoped>
.orders-page {
	max-width: clamp(var(--container-max), 90vw, 1500px);
	margin: 0 auto;
	padding: clamp(2rem, 6vw, 4rem) clamp(1rem, 4vw, 2rem) clamp(3rem, 8vw, 5rem);
}

.orders-hero {
	position: relative;
	display: flex;
	align-items: center;
	gap: 1.5rem;
	padding: 1.75rem 1.75rem 1.75rem 1.75rem;
	margin-bottom: clamp(1.5rem, 4vw, 2.5rem);
	background: linear-gradient(
		180deg,
		rgba(15, 94, 60, 0.1) 0%,
		rgba(15, 94, 60, 0.02) 100%
	);
	border: 1.5px solid rgba(15, 94, 60, 0.22);
	border-radius: 18px;
	overflow: hidden;
	min-height: 160px;
}

.orders-hero::before {
	content: "";
	position: absolute;
	top: 0;
	left: 1.5rem;
	right: 1.5rem;
	height: 3px;
	background: linear-gradient(90deg, transparent, var(--color-cypress-green) 30%, var(--color-cypress-green) 70%, transparent);
	box-shadow: 0 0 12px rgba(15, 94, 60, 0.4);
}

.orders-hero-text {
	flex: 1;
	min-width: 0;
}

.orders-hero-eyebrow {
	font-family: var(--font-serif);
	font-style: italic;
	color: var(--color-cypress-green);
	font-size: 0.9rem;
	letter-spacing: 1px;
	margin: 0 0 0.25rem;
}

.orders-hero-title {
	font-family: var(--font-serif);
	color: var(--color-prussian-blue);
	font-size: clamp(1.75rem, 4vw, 2.4rem);
	margin: 0 0 0.4rem;
	line-height: 1.15;
	letter-spacing: -0.5px;
}

.orders-hero-subtitle {
	font-family: var(--font-sans);
	color: #555;
	font-size: 0.9rem;
	line-height: 1.5;
	margin: 0;
	max-width: 520px;
}

.orders-hero-mascot {
	flex-shrink: 0;
	width: 130px;
	height: 130px;
	object-fit: contain;
	filter: drop-shadow(2px 4px 8px rgba(15, 94, 60, 0.25));
	transform: translateY(4px) rotate(6deg);
	animation: pelipper-fly 4s ease-in-out infinite;
}

@keyframes pelipper-fly {
	0%, 100% { transform: translateY(4px) rotate(6deg); }
	50% { transform: translateY(-4px) rotate(2deg); }
}

@media (max-width: 600px) {
	.orders-hero {
		flex-direction: column;
		text-align: center;
		padding: 1.5rem 1.25rem 1rem;
	}
	.orders-hero-mascot {
		width: 100px;
		height: 100px;
	}
	.orders-hero-subtitle {
		margin: 0 auto;
	}
}

.controls-bar {
	display: flex;
	justify-content: space-between;
	align-items: center;
	gap: 1rem;
	margin-bottom: 2rem;
	padding-bottom: 1rem;
	border-bottom: 1px dashed rgba(0, 49, 83, 0.2);
	flex-wrap: wrap;
}

.view-tabs {
	display: inline-flex;
	background: rgba(0, 49, 83, 0.05);
	border: 1.5px solid rgba(0, 49, 83, 0.15);
	border-radius: 999px;
	padding: 0.25rem;
	gap: 0.25rem;
}

.view-tab {
	padding: 0.45rem 1.1rem;
	font-family: var(--font-sans);
	font-weight: 600;
	font-size: 0.85rem;
	letter-spacing: 0.5px;
	color: #777;
	background: transparent;
	border: none;
	border-radius: 999px;
	cursor: pointer;
	transition: color 0.2s ease, background-color 0.2s ease,
		box-shadow 0.2s ease;
}

.view-tab:hover {
	color: var(--color-prussian-blue);
}

.view-tab.active {
	background-color: #fff;
	color: var(--color-prussian-blue);
	box-shadow: 0 1px 4px rgba(0, 49, 83, 0.12);
}

.type-swap {
	display: flex;
	align-items: center;
	gap: 0.85rem;
}

.current-type {
	font-family: var(--font-serif);
	font-style: italic;
	font-size: 0.95rem;
	color: var(--color-prussian-blue);
}

.swap-button {
	display: inline-flex;
	align-items: center;
	gap: 0.5rem;
	padding: 0.5rem 1rem;
	font-family: var(--font-sans);
	font-weight: 700;
	font-size: 0.78rem;
	letter-spacing: 0.5px;
	text-transform: uppercase;
	color: var(--color-prussian-blue);
	background: var(--color-sunflower-yellow);
	border: 1.5px solid var(--color-prussian-blue);
	border-radius: 999px;
	cursor: pointer;
	box-shadow: 2px 2px 0 var(--color-prussian-blue);
	transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.swap-button:hover {
	transform: translate(-1px, -1px);
	box-shadow: 3px 3px 0 var(--color-prussian-blue);
}

.swap-button:active {
	transform: translate(1px, 1px);
	box-shadow: 1px 1px 0 var(--color-prussian-blue);
}

.swap-button svg {
	width: 14px;
	height: 14px;
}

.orders-area {
	min-height: 60vh;
}

.orders-list {
	display: flex;
	flex-direction: column;
	gap: 1.5rem;
	transition: opacity 0.2s ease;
}

.orders-list.is-refetching {
	opacity: 0.5;
	pointer-events: none;
}

.orders-loading {
	display: flex;
	flex-direction: column;
	gap: 1.5rem;
}

.order-skeleton {
	height: 280px;
	border-radius: 12px;
}

.orders-empty {
	text-align: center;
	padding: 3rem 1rem;
	border: 2px dashed rgba(0, 49, 83, 0.15);
	border-radius: 12px;
	background: rgba(255, 255, 255, 0.5);
	transition: opacity 0.2s ease;
}

.orders-empty.is-refetching {
	opacity: 0.5;
}

.orders-empty-mascot {
	width: 160px;
	height: 160px;
	object-fit: contain;
	margin: 0 auto 0.75rem;
	display: block;
	filter: drop-shadow(2px 4px 8px rgba(0, 49, 83, 0.18));
	animation: slowpoke-doze 5s ease-in-out infinite;
}

@keyframes slowpoke-doze {
	0%, 100% { transform: rotate(-2deg); }
	50% { transform: rotate(2deg); }
}

.empty-title {
	font-family: var(--font-serif);
	font-size: 1.2rem;
	color: var(--color-prussian-blue);
	margin: 0 0 0.5rem;
}

.empty-subtitle {
	font-family: var(--font-sans);
	font-size: 0.9rem;
	color: #888;
	margin: 0;
}
</style>
