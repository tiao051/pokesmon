<script setup lang="ts">
import {computed} from "vue";
import {flowFor, STATUS_LABELS, type OrderStatus} from "../services/orderService";

const props = defineProps<{
	status: OrderStatus;
	isPreorder: boolean;
}>();

const flow = computed(() => flowFor(props.isPreorder));
const isCancelled = computed(() => props.status === "Cancelled");
const isRefunded = computed(() => props.status === "Refunded");
const isTerminalAlt = computed(() => isCancelled.value || isRefunded.value);

const currentIndex = computed(() => {
	if (isTerminalAlt.value) return -1;
	return flow.value.indexOf(props.status);
});

const stateOf = (index: number): "done" | "current" | "pending" => {
	if (currentIndex.value < 0) return "pending";
	if (index < currentIndex.value) return "done";
	if (index === currentIndex.value) return "current";
	return "pending";
};
</script>

<template>
	<div class="timeline" :class="{ 'is-terminal-alt': isTerminalAlt }">
		<div
			v-if="isTerminalAlt"
			class="terminal-banner"
			:class="{ cancelled: isCancelled, refunded: isRefunded }"
		>
			{{ STATUS_LABELS[status] }}
		</div>
		<ol v-else class="steps">
			<li
				v-for="(step, idx) in flow"
				:key="step"
				class="step"
				:class="stateOf(idx)"
			>
				<span class="dot" aria-hidden="true" />
				<span class="step-label">{{ STATUS_LABELS[step] }}</span>
			</li>
		</ol>
	</div>
</template>

<style scoped>
.timeline {
	margin-top: 1rem;
}

.steps {
	list-style: none;
	padding: 0;
	margin: 0;
	display: grid;
	grid-auto-flow: column;
	grid-auto-columns: 1fr;
	gap: 0.5rem;
	position: relative;
}

.step {
	display: flex;
	flex-direction: column;
	align-items: center;
	gap: 0.4rem;
	position: relative;
	min-width: 0;
}

.step:not(:last-child)::after {
	content: "";
	position: absolute;
	top: 7px;
	left: calc(50% + 8px);
	right: calc(-50% + 8px);
	height: 2px;
	background: rgba(0, 49, 83, 0.15);
	z-index: 0;
}

.step.done:not(:last-child)::after,
.step.current:not(:last-child)::after {
	background: var(--color-cypress-green);
}

.dot {
	width: 14px;
	height: 14px;
	border-radius: 50%;
	background: #fff;
	border: 2px solid rgba(0, 49, 83, 0.25);
	z-index: 1;
	transition: background 0.25s ease, border-color 0.25s ease,
		transform 0.25s ease;
}

.step.done .dot {
	background: var(--color-cypress-green);
	border-color: var(--color-cypress-green);
}

.step.current .dot {
	background: var(--color-sunflower-yellow);
	border-color: var(--color-prussian-blue);
	transform: scale(1.2);
	box-shadow: 0 0 0 4px rgba(255, 197, 18, 0.25);
}

.step-label {
	font-family: var(--font-sans);
	font-size: 0.72rem;
	color: #777;
	letter-spacing: 0.3px;
	text-align: center;
	line-height: 1.25;
}

.step.done .step-label,
.step.current .step-label {
	color: var(--color-prussian-blue);
	font-weight: 600;
}

.terminal-banner {
	display: inline-block;
	padding: 0.4rem 0.85rem;
	border-radius: 999px;
	font-family: var(--font-sans);
	font-size: 0.78rem;
	font-weight: 700;
	letter-spacing: 0.5px;
	text-transform: uppercase;
}

.terminal-banner.cancelled {
	background-color: rgba(120, 120, 120, 0.15);
	color: #555;
}

.terminal-banner.refunded {
	background-color: rgba(194, 130, 27, 0.15);
	color: #c2821b;
}
</style>
