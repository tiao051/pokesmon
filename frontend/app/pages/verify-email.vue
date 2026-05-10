<script setup lang="ts">
import {computed, ref} from "vue";

definePageMeta({
	layout: "auth",
});

useHead({title: "Verify Email — PokéGogh"});

const route = useRoute();
const router = useRouter();
const {verifyEmail} = useAuth();

const email = ref(
	typeof route.query.email === "string" ? route.query.email : "",
);
const pin = ref("");
const submitting = ref(false);
const errorMessage = ref("");

const canSubmit = computed(
	() =>
		!!email.value.trim() &&
		pin.value.trim().length === 6 &&
		!submitting.value,
);

const handleSubmit = async () => {
	if (!canSubmit.value) return;
	errorMessage.value = "";
	submitting.value = true;
	try {
		await verifyEmail(email.value.trim(), pin.value.trim());
		router.push("/");
	} catch (err: any) {
		errorMessage.value =
			err?.message || "Could not verify the PIN. Please try again.";
	} finally {
		submitting.value = false;
	}
};
</script>

<template>
	<section class="verify-page">
		<NuxtLink to="/login" class="back-link">
			<svg
				viewBox="0 0 24 24"
				fill="none"
				stroke="currentColor"
				stroke-width="2"
				stroke-linecap="round"
				stroke-linejoin="round"
			>
				<line x1="19" y1="12" x2="5" y2="12"></line>
				<polyline points="12 19 5 12 12 5"></polyline>
			</svg>
			<span>Back to Sign In</span>
		</NuxtLink>

		<div class="vp-card">
			<div class="vp-header">
				<p class="vp-eyebrow">— Verify Your Email —</p>
				<h1 class="vp-title">Verify Your Email</h1>
				<p class="vp-subtitle">
					We've sent a 6-digit code to your email. Enter it below
					to finish signing up.
				</p>
			</div>

			<form class="vp-form" @submit.prevent="handleSubmit" novalidate>
				<div v-if="errorMessage" class="form-alert" role="alert">
					<span class="alert-mark" aria-hidden="true">!</span>
					<span>{{ errorMessage }}</span>
				</div>

				<div class="vp-field">
					<label for="email">Email</label>
					<input
						id="email"
						v-model="email"
						type="email"
						autocomplete="email"
						placeholder="trainer@pallet-town.vn"
						required
					/>
				</div>

				<div class="vp-field">
					<label for="pin">Verification PIN</label>
					<input
						id="pin"
						v-model="pin"
						inputmode="numeric"
						pattern="\d{6}"
						maxlength="6"
						placeholder="123456"
						class="pin-input"
						required
					/>
				</div>

				<button
					type="submit"
					class="vp-submit"
					:disabled="!canSubmit"
				>
					<span v-if="submitting">Verifying...</span>
					<span v-else>Verify & Enter</span>
				</button>

				<p class="vp-note">
					Didn't get a PIN? Check your spam folder, or
					<NuxtLink to="/login" class="inline-link"
						>try registering again</NuxtLink
					>.
				</p>
			</form>
		</div>
	</section>
</template>

<style scoped>
.verify-page {
	max-width: 560px;
	margin: 0 auto;
	padding: clamp(2rem, 6vw, 4rem) clamp(1rem, 4vw, 2rem)
		clamp(3rem, 8vw, 5rem);
}

.back-link {
	display: inline-flex;
	align-items: center;
	gap: 0.45rem;
	margin-bottom: 1.25rem;
	font-family: var(--font-sans);
	font-size: 0.9rem;
	font-weight: 600;
	color: var(--color-prussian-blue);
	text-decoration: none;
	padding: 0.4rem 0.7rem;
	border-radius: 999px;
	transition:
		background-color 0.2s ease,
		transform 0.2s ease;
}

.back-link:hover {
	background-color: rgba(0, 49, 83, 0.08);
	transform: translateX(-2px);
}

.back-link svg {
	width: 16px;
	height: 16px;
}

.vp-card {
	position: relative;
	background: #fbf9f2;
	border: 2px solid var(--color-prussian-blue);
	border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
	padding: clamp(1.75rem, 4vw, 2.5rem);
	box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.18);
	overflow: hidden;
}

.vp-header {
	text-align: center;
	margin-bottom: 1.5rem;
}

.vp-eyebrow {
	font-family: var(--font-serif);
	font-style: italic;
	letter-spacing: 1.5px;
	color: #c2821b;
	font-size: 0.82rem;
	margin-bottom: 0.4rem;
}

.vp-title {
	font-family: var(--font-serif);
	font-size: clamp(1.75rem, 4vw, 2.25rem);
	color: var(--color-prussian-blue);
	margin-bottom: 0.5rem;
}

.vp-subtitle {
	font-family: var(--font-sans);
	color: #6f6f6f;
	font-size: 0.95rem;
	line-height: 1.55;
	max-width: 380px;
	margin: 0 auto;
}

.vp-form {
	display: flex;
	flex-direction: column;
	gap: 1rem;
}

.vp-field {
	display: flex;
	flex-direction: column;
	gap: 0.35rem;
}

.vp-field label {
	font-family: var(--font-sans);
	font-size: 0.78rem;
	font-weight: 700;
	color: var(--color-prussian-blue);
	letter-spacing: 1px;
	text-transform: uppercase;
}

.vp-field input {
	width: 100%;
	padding: 0.8rem 1rem;
	border: 2px solid var(--color-prussian-blue);
	border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
	background-color: #fff;
	font-family: var(--font-sans);
	font-size: 0.95rem;
	color: #222;
	box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.1);
	transition:
		transform 0.25s ease,
		box-shadow 0.25s ease;
}

.vp-field input:focus {
	outline: none;
	transform: translate(-2px, -2px);
	box-shadow: 4px 4px 0 var(--color-prussian-blue);
}

.pin-input {
	letter-spacing: 0.5em;
	font-size: 1.25rem !important;
	text-align: center;
	font-weight: 700;
}

.form-alert {
	display: flex;
	align-items: center;
	gap: 0.6rem;
	padding: 0.7rem 0.95rem;
	border-radius: 12px;
	background-color: rgba(185, 74, 61, 0.08);
	border: 1px solid rgba(185, 74, 61, 0.3);
	color: #8a3328;
	font-family: var(--font-sans);
	font-size: 0.88rem;
}

.alert-mark {
	display: grid;
	place-items: center;
	width: 22px;
	height: 22px;
	border-radius: 50%;
	background-color: #b94a3d;
	color: #fff;
	font-weight: 800;
	font-size: 0.85rem;
}

.vp-submit {
	margin-top: 0.5rem;
	padding: 0.95rem 1.25rem;
	border: 2px solid var(--color-prussian-blue);
	border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
	background-color: var(--color-sunflower-yellow, #ffd23f);
	color: var(--color-prussian-blue);
	font-family: var(--font-sans);
	font-weight: 800;
	text-transform: uppercase;
	letter-spacing: 1px;
	font-size: 0.95rem;
	cursor: pointer;
	box-shadow: 4px 4px 0 var(--color-prussian-blue);
	transition:
		transform 0.2s ease,
		box-shadow 0.2s ease,
		background-color 0.2s ease;
}

.vp-submit:not(:disabled):hover {
	transform: translate(-2px, -2px);
	box-shadow: 6px 6px 0 var(--color-prussian-blue);
	background-color: #ffcf33;
}

.vp-submit:not(:disabled):active {
	transform: translate(2px, 2px);
	box-shadow: 2px 2px 0 var(--color-prussian-blue);
}

.vp-submit:disabled {
	cursor: not-allowed;
	opacity: 0.55;
}

.vp-note {
	font-family: var(--font-sans);
	font-size: 0.82rem;
	color: #888;
	text-align: center;
	margin-top: 0.25rem;
}

.inline-link {
	color: var(--color-prussian-blue);
	font-weight: 700;
	text-decoration: underline;
	text-underline-offset: 2px;
}

.inline-link:hover {
	color: var(--color-cypress-green);
}
</style>
