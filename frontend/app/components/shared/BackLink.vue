<script setup lang="ts">
defineProps<{
	to: string;
	label?: string;
}>();
</script>

<template>
	<NuxtLink :to="to" class="back-link">
		<svg
			viewBox="0 0 24 24"
			fill="none"
			stroke="currentColor"
			stroke-width="2"
			stroke-linecap="round"
			stroke-linejoin="round"
		>
			<line x1="19" y1="12" x2="5" y2="12" />
			<polyline points="12 19 5 12 12 5" />
		</svg>
		<span>{{ label ?? "Back" }}</span>
	</NuxtLink>
</template>

<style scoped>
.back-link {
	display: inline-flex;
	align-items: center;
	gap: 0.5rem;
	font-family: var(--font-sans);
	font-size: 0.85rem;
	font-weight: 600;
	letter-spacing: 0.5px;
	color: var(--color-prussian-blue);
	text-decoration: none;
	margin-bottom: 1.5rem;
	transition: color 0.2s ease, transform 0.2s ease;
}

.back-link:hover {
	color: var(--color-cypress-green);
	transform: translateX(-2px);
}

.back-link svg {
	width: 16px;
	height: 16px;
}
</style>
