<script setup lang="ts">
import {toRef} from "vue";
import type {Product} from "../../types/product";
import {useProductImage} from "../../composables/products/useProductImage";
import {formatPrice} from "../../utils/format";

const props = defineProps<{product: Product}>();

const {add} = useCart();
const toast = useToast();

const handleAdd = () => {
	const result = add(props.product, 1);
	if (result.reason === "sold-out") {
		toast.error(`"${props.product.title}" is no longer in the gallery.`);
		return;
	}
	if (result.reason === "cap-reached") {
		toast.error(`The full reserve of "${props.product.title}" is already in your cart.`);
		return;
	}
	const cartName = props.product.isPreorder ? "pre-order cart" : "cart";
	toast.success(`Added "${props.product.title}" to your ${cartName}.`);
};

const productRef = toRef(props, "product");
const {src: imageSrc, isDataUrl} = useProductImage(productRef);
</script>

<template>
	<NuxtLink :to="`/products/${product.slug}`" class="product-card-link">
		<article class="product-card">
			<div class="product-image-container">
				<img
					v-if="isDataUrl"
					:src="imageSrc"
					:alt="product.title"
					class="product-image"
					width="400"
					height="500"
					loading="lazy"
					decoding="async"
				/>
				<NuxtImg
					v-else-if="imageSrc"
					:src="imageSrc"
					:alt="product.title"
					class="product-image"
					width="400"
					height="500"
					sizes="xs:50vw sm:50vw md:33vw lg:25vw xl:25vw"
					loading="lazy"
					decoding="async"
					placeholder
				/>

				<button
					class="add-to-cart-quick"
					aria-label="Acquire"
					@click.prevent.stop="handleAdd"
				>
					<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
						<line x1="12" y1="5" x2="12" y2="19" />
						<line x1="5" y1="12" x2="19" y2="12" />
					</svg>
				</button>

				<span v-if="product.isPreorder" class="preorder-corner-badge">Pre-order</span>
			</div>
			<div class="product-info">
				<h4 class="product-title">{{ product.title }}</h4>
				<p class="product-price">{{ formatPrice(product.price) }}</p>
			</div>
		</article>
	</NuxtLink>
</template>

<style scoped>
.product-card-link {
	display: block;
	text-decoration: none;
	color: inherit;
}

.product-image-container {
	position: relative;
	aspect-ratio: 4/5;
	background-color: var(--color-linen);
	border-radius: 8px;
	overflow: hidden;
	display: flex;
	align-items: center;
	justify-content: center;
}

.product-image {
	width: 100%;
	height: 100%;
	object-fit: cover;
	display: block;
}

.preorder-corner-badge {
	position: absolute;
	top: 0.65rem;
	left: 0.65rem;
	font-family: var(--font-sans);
	font-size: 0.65rem;
	font-weight: 700;
	letter-spacing: 1.2px;
	text-transform: uppercase;
	padding: 0.3rem 0.55rem;
	background: #b22222;
	color: #fff;
	border-radius: 4px;
	line-height: 1;
	box-shadow: 1.5px 1.5px 0 rgba(0, 49, 83, 0.25);
	z-index: 2;
	pointer-events: none;
}
</style>
