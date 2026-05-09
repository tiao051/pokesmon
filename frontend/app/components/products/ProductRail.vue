<script setup lang="ts">
import {ref, computed, watch, onMounted, onBeforeUnmount} from "vue";
import type {Product} from "../../types/product";

const props = defineProps<{
	title: string;
	subtitle?: string;
	products: Product[];
	pending?: boolean;
	error?: unknown;
	viewAllLink?: string;
	theme?: "display" | "master" | "vault" | "curator" | "preorder";
	eyebrow?: string;
}>();

const visibleCount = ref(4);
const currentPage = ref(0);

const updateVisibleCount = () => {
	const w = window.innerWidth;
	if (w <= 600) visibleCount.value = 2;
	else if (w <= 1024) visibleCount.value = 3;
	else visibleCount.value = 4;
};

onMounted(() => {
	updateVisibleCount();
	window.addEventListener("resize", updateVisibleCount);
});

onBeforeUnmount(() => {
	window.removeEventListener("resize", updateVisibleCount);
});

const maxPage = computed(() =>
	Math.max(0, Math.ceil(props.products.length / visibleCount.value) - 1),
);

const canPaginate = computed(() => maxPage.value > 0);

watch(maxPage, (newMax) => {
	if (currentPage.value > newMax) currentPage.value = newMax;
});

watch(
	() => props.products,
	() => {
		currentPage.value = 0;
	},
);

const trackStyle = computed(() => ({
	transform: `translateX(calc(${-currentPage.value * 100}% - ${
		currentPage.value * 1.5
	}rem))`,
}));

const prev = () => {
	if (currentPage.value > 0) currentPage.value--;
};

const next = () => {
	if (currentPage.value < maxPage.value) currentPage.value++;
};
</script>

<template>
	<section class="rail" :class="theme ? `rail--${theme}` : ''">
		<header class="rail-header">
			<div class="rail-heading-block">
				<span v-if="eyebrow" class="rail-eyebrow">{{ eyebrow }}</span>
				<h3 class="rail-title">{{ title }}</h3>
				<p v-if="subtitle" class="rail-subtitle">{{ subtitle }}</p>
			</div>
			<div class="rail-controls">
				<NuxtLink
					v-if="viewAllLink"
					:to="viewAllLink"
					class="rail-view-all"
				>
					View all <span aria-hidden="true">→</span>
				</NuxtLink>
				<div v-if="canPaginate" class="rail-nav">
					<button
						class="rail-arrow"
						type="button"
						:disabled="currentPage === 0"
						aria-label="Previous"
						@click="prev"
					>
						<svg
							viewBox="0 0 24 24"
							fill="none"
							stroke="currentColor"
							stroke-width="2.5"
							stroke-linecap="round"
							stroke-linejoin="round"
						>
							<polyline points="15 18 9 12 15 6" />
						</svg>
					</button>
					<button
						class="rail-arrow"
						type="button"
						:disabled="currentPage >= maxPage"
						aria-label="Next"
						@click="next"
					>
						<svg
							viewBox="0 0 24 24"
							fill="none"
							stroke="currentColor"
							stroke-width="2.5"
							stroke-linecap="round"
							stroke-linejoin="round"
						>
							<polyline points="9 18 15 12 9 6" />
						</svg>
					</button>
				</div>
			</div>
		</header>

		<div class="rail-viewport">
			<div
				v-if="pending && !products.length"
				class="rail-track is-static"
				aria-busy="true"
			>
				<div v-for="n in 4" :key="n" class="rail-item skeleton skeleton-card" />
			</div>

			<p v-else-if="error" class="rail-state-msg rail-error">
				This wing is closed for the moment. Please return shortly.
			</p>

			<div
				v-else-if="products.length"
				class="rail-track"
				:style="trackStyle"
			>
				<div
					v-for="product in products"
					:key="product.id"
					class="rail-item"
				>
					<ProductCard :product="product" />
				</div>
			</div>

			<p v-else class="rail-state-msg">
				No pieces on display in this wing yet.
			</p>
		</div>
	</section>
</template>

<style scoped>
.rail {
	margin-bottom: 4rem;
}

/* ── Themed rails: each one a museum wing ────────────────── */
.rail--display,
.rail--master,
.rail--vault,
.rail--curator,
.rail--preorder {
	position: relative;
	padding: 2.75rem 2rem 2.25rem;
	border-radius: 18px;
	overflow: hidden;
	border: 1px solid rgba(0, 49, 83, 0.08);
	box-shadow:
		0 1px 0 rgba(255, 255, 255, 0.6) inset,
		0 12px 32px -20px rgba(0, 49, 83, 0.35),
		0 2px 6px -4px rgba(0, 49, 83, 0.12);
}

.rail--display::before,
.rail--master::before,
.rail--vault::before,
.rail--curator::before,
.rail--preorder::before {
	content: "";
	position: absolute;
	top: 0;
	left: 1.5rem;
	right: 1.5rem;
	height: 3px;
	border-radius: 0 0 3px 3px;
	pointer-events: none;
}

.rail--display::after,
.rail--master::after,
.rail--vault::after,
.rail--curator::after,
.rail--preorder::after {
	content: "";
	position: absolute;
	bottom: 0;
	left: 30%;
	right: 30%;
	height: 1px;
	background: linear-gradient(
		90deg,
		transparent,
		rgba(0, 49, 83, 0.18),
		transparent
	);
	pointer-events: none;
}

.rail--display {
	background: linear-gradient(
		180deg,
		rgba(255, 197, 18, 0.12) 0%,
		rgba(255, 197, 18, 0.02) 75%
	);
	border-color: rgba(194, 130, 27, 0.18);
}

.rail--display::before {
	background: linear-gradient(
		90deg,
		transparent,
		var(--color-sunflower-yellow) 30%,
		var(--color-sunflower-yellow) 70%,
		transparent
	);
	box-shadow: 0 0 12px rgba(255, 197, 18, 0.45);
}

.rail--master {
	background: linear-gradient(
		180deg,
		rgba(0, 49, 83, 0.09) 0%,
		rgba(0, 49, 83, 0.015) 75%
	);
	border-color: rgba(0, 49, 83, 0.14);
}

.rail--master::before {
	background: linear-gradient(
		90deg,
		transparent,
		#c2821b 30%,
		#c2821b 70%,
		transparent
	);
	box-shadow: 0 0 12px rgba(194, 130, 27, 0.4);
}

.rail--vault {
	background: linear-gradient(
		180deg,
		rgba(15, 94, 60, 0.1) 0%,
		rgba(15, 94, 60, 0.015) 75%
	);
	border-color: rgba(15, 94, 60, 0.18);
}

.rail--vault::before {
	background: linear-gradient(
		90deg,
		transparent,
		var(--color-cypress-green) 30%,
		var(--color-cypress-green) 70%,
		transparent
	);
	box-shadow: 0 0 12px rgba(15, 94, 60, 0.35);
}

.rail--curator {
	background: linear-gradient(
		180deg,
		rgba(194, 130, 27, 0.12) 0%,
		rgba(194, 130, 27, 0.02) 75%
	);
	border-color: rgba(139, 90, 43, 0.22);
}

.rail--curator::before {
	height: 2px;
	background-image: linear-gradient(
		90deg,
		#c2821b 50%,
		transparent 50%
	);
	background-size: 10px 2px;
	background-repeat: repeat-x;
	box-shadow: 0 0 10px rgba(194, 130, 27, 0.35);
}

.rail--preorder {
	background: linear-gradient(
		180deg,
		rgba(178, 34, 34, 0.1) 0%,
		rgba(178, 34, 34, 0.015) 75%
	);
	border-color: rgba(178, 34, 34, 0.2);
}

.rail--preorder::before {
	background: linear-gradient(
		90deg,
		transparent,
		#b22222 30%,
		#b22222 70%,
		transparent
	);
	box-shadow: 0 0 12px rgba(178, 34, 34, 0.4);
}

.rail-eyebrow {
	display: block;
	font-family: var(--font-serif);
	font-style: italic;
	letter-spacing: 2px;
	text-transform: uppercase;
	font-size: 0.75rem;
	margin-bottom: 0.4rem;
	color: #c2821b;
}

.rail--master .rail-eyebrow {
	color: var(--color-prussian-blue);
}

.rail--vault .rail-eyebrow {
	color: var(--color-cypress-green);
}

.rail--curator .rail-eyebrow {
	color: #8b5a2b;
}

.rail--preorder .rail-eyebrow {
	color: #b22222;
}

@media (max-width: 768px) {
	.rail--display,
	.rail--master,
	.rail--vault,
	.rail--curator,
	.rail--preorder {
		padding: 2.25rem 1.25rem 1.75rem;
	}

	.rail--display::before,
	.rail--master::before,
	.rail--vault::before,
	.rail--curator::before,
	.rail--preorder::before {
		left: 1rem;
		right: 1rem;
	}
}

.rail-header {
	display: flex;
	justify-content: space-between;
	align-items: flex-end;
	gap: 1rem;
	margin-bottom: 1.5rem;
	padding-bottom: 0.75rem;
	border-bottom: 1px solid rgba(0, 49, 83, 0.12);
}

.rail-title {
	font-family: var(--font-serif);
	font-size: clamp(1.4rem, 3vw, 1.9rem);
	color: var(--color-prussian-blue);
	margin: 0;
	line-height: 1.2;
}

.rail-subtitle {
	font-family: var(--font-sans);
	font-size: 0.9rem;
	color: #777;
	margin: 0.25rem 0 0;
	font-style: italic;
}

.rail-controls {
	display: flex;
	align-items: center;
	gap: 1.5rem;
}

.rail-view-all {
	font-family: var(--font-sans);
	font-size: 0.9rem;
	color: var(--color-cypress-green);
	text-decoration: none;
	font-weight: 600;
	letter-spacing: 0.02em;
	white-space: nowrap;
	transition: color 0.2s ease;
}

.rail-view-all:hover {
	color: var(--color-prussian-blue);
}

.rail-nav {
	display: flex;
	gap: 0.5rem;
}

.rail-arrow {
	width: 42px;
	height: 42px;
	border-radius: 50%;
	border: 2px solid var(--color-prussian-blue);
	background: #fff;
	color: var(--color-prussian-blue);
	display: flex;
	align-items: center;
	justify-content: center;
	cursor: pointer;
	transition: transform 0.3s cubic-bezier(0.23, 1, 0.32, 1),
		box-shadow 0.3s cubic-bezier(0.23, 1, 0.32, 1),
		background-color 0.25s ease, color 0.25s ease,
		opacity 0.2s ease, border-color 0.25s ease;
	box-shadow: 3px 3px 0 rgba(0, 49, 83, 0.1);
}

.rail-arrow:hover:not(:disabled) {
	background: var(--color-prussian-blue);
	color: #fff;
	transform: translateY(-2px);
	box-shadow: 5px 5px 0 rgba(0, 49, 83, 0.18);
}

.rail-arrow:active:not(:disabled) {
	transform: translateY(0);
	box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.12);
}

.rail-arrow:disabled {
	opacity: 0.3;
	cursor: not-allowed;
	border-color: #ccc;
	box-shadow: none;
}

.rail-arrow svg {
	width: 18px;
	height: 18px;
}

.rail-viewport {
	overflow: hidden;
	padding: 0.5rem 0;
}

.rail-track {
	display: flex;
	gap: 1.5rem;
	transition: transform 0.55s cubic-bezier(0.5, 0.05, 0.2, 1);
	will-change: transform;
}

.rail-track.is-static {
	transition: none;
}

.rail-item {
	flex: 0 0 calc((100% - 3 * 1.5rem) / 4);
}

@media (max-width: 1024px) {
	.rail-item {
		flex: 0 0 calc((100% - 2 * 1.5rem) / 3);
	}
}

@media (max-width: 600px) {
	.rail-item {
		flex: 0 0 calc((100% - 1.5rem) / 2);
	}
}

.rail-state-msg {
	font-family: var(--font-serif);
	font-style: italic;
	color: #888;
	font-size: 0.95rem;
	padding: 2rem 0;
	text-align: center;
}

.rail-error {
	color: #c2821b;
}
</style>
