<script setup lang="ts">
import {computed, toRef} from "vue";
import {usePagination} from "../composables/usePagination";

const props = defineProps<{
	currentPage: number;
	totalItems: number;
	pageSize: number;
}>();

const emit = defineEmits<{
	(e: "update:currentPage", value: number): void;
}>();

const totalItemsRef = toRef(props, "totalItems");
const pageSizeRef = toRef(props, "pageSize");
const {totalPages, visiblePagesFor} = usePagination(totalItemsRef, pageSizeRef);

const visiblePages = computed(() => visiblePagesFor(props.currentPage));

const goTo = (page: number) => {
	if (page < 1 || page > totalPages.value || page === props.currentPage) return;
	emit("update:currentPage", page);
};
</script>

<template>
	<nav v-if="totalPages > 1" class="pagination" aria-label="Pagination">
		<button
			class="page-btn prev-btn"
			type="button"
			:disabled="currentPage === 1"
			aria-label="Previous page"
			@click="goTo(currentPage - 1)"
		>
			<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
				<polyline points="15 18 9 12 15 6" />
			</svg>
		</button>

		<div class="page-numbers">
			<template v-for="(p, idx) in visiblePages" :key="idx">
				<span v-if="p === '...'" class="page-ellipsis">…</span>
				<button
					v-else
					class="page-number"
					type="button"
					:class="{active: currentPage === p}"
					:aria-current="currentPage === p ? 'page' : undefined"
					@click="goTo(p as number)"
				>
					{{ p }}
				</button>
			</template>
		</div>

		<button
			class="page-btn next-btn"
			type="button"
			:disabled="currentPage === totalPages"
			aria-label="Next page"
			@click="goTo(currentPage + 1)"
		>
			<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
				<polyline points="9 18 15 12 9 6" />
			</svg>
		</button>
	</nav>
</template>

<style scoped>
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
</style>
