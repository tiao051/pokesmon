<script setup lang="ts">
import {computed} from "vue";
import {STATUS_LABELS, type Order} from "../services/orderService";
import {formatPrice} from "../utils/format";

const props = defineProps<{order: Order}>();

const itemCount = computed(() =>
	props.order.items.reduce((sum, i) => sum + i.quantity, 0),
);

const formattedDate = computed(() => {
	const d = new Date(props.order.createdAt);
	return d.toLocaleDateString("en-GB", {
		day: "2-digit",
		month: "short",
		year: "numeric",
	});
});

const statusBadgeClass = computed(() => {
	switch (props.order.status) {
		case "Delivered":
			return "delivered";
		case "Cancelled":
			return "cancelled";
		case "Refunded":
			return "refunded";
		default:
			return "active";
	}
});

const orderRef = computed(() => `PG-${props.order.id.slice(-8).toUpperCase()}`);
</script>

<template>
	<article class="order-card">
		<header class="order-header">
			<div class="order-meta">
				<span class="order-ref">{{ orderRef }}</span>
				<span class="order-date">{{ formattedDate }}</span>
				<span v-if="order.isPreorder" class="preorder-tag">PRE-ORDER</span>
			</div>
			<span class="status-badge" :class="statusBadgeClass">
				{{ STATUS_LABELS[order.status] }}
			</span>
		</header>

		<ul class="item-strip">
			<li v-for="item in order.items" :key="item.productId" class="item">
				<NuxtLink :to="`/products/${item.slug}`" class="item-thumb">
					<NuxtImg
						v-if="item.image"
						:src="item.image"
						:alt="item.title"
						width="80"
						height="100"
						sizes="xs:80px"
						loading="lazy"
						decoding="async"
						placeholder
					/>
				</NuxtLink>
				<div class="item-info">
					<NuxtLink :to="`/products/${item.slug}`" class="item-title">
						{{ item.title }}
					</NuxtLink>
					<p class="item-meta">
						{{ formatPrice(item.price) }} × {{ item.quantity }}
					</p>
				</div>
			</li>
		</ul>

		<OrderStatusTimeline :status="order.status" :is-preorder="order.isPreorder" />

		<footer class="order-footer">
			<div class="footer-meta">
				<span class="footer-label">{{ itemCount }} {{ itemCount === 1 ? "item" : "items" }}</span>
				<span v-if="order.isPreorder && order.depositAmount" class="deposit-info">
					Deposit: {{ formatPrice(order.depositAmount) }}
					<span v-if="order.depositPaid" class="deposit-paid">paid</span>
					<span v-else class="deposit-pending">pending</span>
				</span>
			</div>
			<div class="footer-total">
				<span class="footer-total-label">Total</span>
				<span class="footer-total-value">{{ formatPrice(order.total) }}</span>
			</div>
		</footer>
	</article>
</template>

<style scoped>
.order-card {
	background: #fff;
	border: 1.5px solid rgba(0, 49, 83, 0.15);
	border-radius: 12px;
	padding: 1.5rem;
	box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.06);
	transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.order-card:hover {
	transform: translateY(-2px);
	box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.1);
}

.order-header {
	display: flex;
	justify-content: space-between;
	align-items: flex-start;
	gap: 1rem;
	flex-wrap: wrap;
	margin-bottom: 1.25rem;
}

.order-meta {
	display: flex;
	align-items: center;
	gap: 0.75rem;
	flex-wrap: wrap;
}

.order-ref {
	font-family: "Courier New", monospace;
	font-weight: 700;
	color: var(--color-prussian-blue);
	font-size: 0.95rem;
	letter-spacing: 0.5px;
}

.order-date {
	font-family: var(--font-sans);
	font-size: 0.82rem;
	color: #888;
}

.preorder-tag {
	font-family: var(--font-sans);
	font-size: 0.65rem;
	font-weight: 700;
	letter-spacing: 1px;
	color: var(--color-prussian-blue);
	background: var(--color-sunflower-yellow);
	padding: 0.15rem 0.5rem;
	border-radius: 4px;
}

.status-badge {
	font-family: var(--font-sans);
	font-size: 0.75rem;
	font-weight: 700;
	letter-spacing: 0.5px;
	padding: 0.3rem 0.85rem;
	border-radius: 999px;
	text-transform: uppercase;
}

.status-badge.active {
	background: rgba(0, 49, 83, 0.1);
	color: var(--color-prussian-blue);
}

.status-badge.delivered {
	background: rgba(15, 94, 60, 0.12);
	color: var(--color-cypress-green);
}

.status-badge.cancelled {
	background: rgba(120, 120, 120, 0.15);
	color: #555;
}

.status-badge.refunded {
	background: rgba(194, 130, 27, 0.15);
	color: #c2821b;
}

.item-strip {
	list-style: none;
	padding: 0;
	margin: 0;
	display: flex;
	flex-direction: column;
	gap: 0.85rem;
	border-top: 1px dashed rgba(0, 49, 83, 0.15);
	padding-top: 1rem;
}

.item {
	display: flex;
	gap: 0.85rem;
	align-items: center;
}

.item-thumb {
	flex: 0 0 64px;
	width: 64px;
	height: 80px;
	border-radius: 8px;
	overflow: hidden;
	background: var(--color-linen);
	display: block;
}

.item-thumb :deep(img) {
	width: 100%;
	height: 100%;
	object-fit: cover;
	display: block;
}

.item-info {
	display: flex;
	flex-direction: column;
	gap: 0.2rem;
	min-width: 0;
}

.item-title {
	font-family: var(--font-serif);
	color: var(--color-prussian-blue);
	font-size: 0.95rem;
	font-weight: 600;
	text-decoration: none;
	line-height: 1.3;
	transition: color 0.2s ease;
}

.item-title:hover {
	color: var(--color-cypress-green);
}

.item-meta {
	font-family: var(--font-sans);
	color: #888;
	font-size: 0.82rem;
	margin: 0;
}

.order-footer {
	display: flex;
	justify-content: space-between;
	align-items: flex-end;
	gap: 1rem;
	margin-top: 1.25rem;
	padding-top: 1rem;
	border-top: 1px dashed rgba(0, 49, 83, 0.15);
	flex-wrap: wrap;
}

.footer-meta {
	display: flex;
	flex-direction: column;
	gap: 0.25rem;
	font-family: var(--font-sans);
	font-size: 0.82rem;
}

.footer-label {
	color: #888;
}

.deposit-info {
	color: var(--color-prussian-blue);
}

.deposit-paid {
	color: var(--color-cypress-green);
	font-weight: 700;
	margin-left: 0.25rem;
}

.deposit-pending {
	color: #c2821b;
	font-weight: 700;
	margin-left: 0.25rem;
}

.footer-total {
	display: flex;
	flex-direction: column;
	align-items: flex-end;
	text-align: right;
}

.footer-total-label {
	font-family: var(--font-sans);
	font-size: 0.7rem;
	text-transform: uppercase;
	letter-spacing: 1px;
	color: #777;
}

.footer-total-value {
	font-family: var(--font-sans);
	color: var(--color-cypress-green);
	font-weight: 700;
	font-size: 1.15rem;
}
</style>
