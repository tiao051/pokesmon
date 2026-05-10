<script setup lang="ts">
import { computed, ref } from 'vue'

definePageMeta({
  layout: 'auth',
})

useHead({ title: 'Forgot Password — PokéGogh' })

const { requestPasswordReset } = useAuth()

const email = ref('')
const submitting = ref(false)
const formError = ref<string | null>(null)
const sent = ref(false)
const sentTo = ref('')

const emailLooksValid = computed(() => {
  const v = email.value.trim()
  return v.length > 0 && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v)
})

const canSubmit = computed(() => emailLooksValid.value && !submitting.value)

const handleSubmit = async () => {
  if (!canSubmit.value) return
  formError.value = null
  submitting.value = true
  try {
    await requestPasswordReset(email.value)
    sentTo.value = email.value.trim()
    sent.value = true
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Could not send the reset link.'
  } finally {
    submitting.value = false
  }
}

const handleResend = () => {
  sent.value = false
}
</script>

<template>
  <section class="forgot-page">
    <NuxtLink to="/login" class="back-link">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <line x1="19" y1="12" x2="5" y2="12"></line>
        <polyline points="12 19 5 12 12 5"></polyline>
      </svg>
      <span>Back to Sign In</span>
    </NuxtLink>

    <div class="fp-card">
      <img src="/images/charizard.png" alt="" class="fp-stamp" aria-hidden="true" />

      <div class="fp-header">
        <p class="fp-eyebrow">— Reset Your Password —</p>
        <h1 class="fp-title">Forgot Password</h1>
        <p class="fp-subtitle">
          No worries — enter your email and we'll send a reset link.
        </p>
      </div>

      <Transition name="success-fade">
        <div v-if="sent" class="success-panel" role="status">
          <div class="success-mark" aria-hidden="true">
            <img src="/images/gau_map.png" alt="" class="success-image" />
          </div>
          <h2>Check your inbox</h2>
          <p>
            If an account matches <span class="sent-email">{{ sentTo }}</span>,
            we've sent a reset link. Check your inbox shortly.
          </p>
          <div class="success-actions">
            <button type="button" class="link-button" @click="handleResend">
              Try a different email
            </button>
            <NuxtLink to="/login" class="back-cta">Back to Sign In</NuxtLink>
          </div>
        </div>
      </Transition>

      <form v-if="!sent" class="fp-form" @submit.prevent="handleSubmit" novalidate>
        <Transition name="alert-slide">
          <div v-if="formError" class="form-alert" role="alert">
            <span class="alert-mark" aria-hidden="true">!</span>
            <span>{{ formError }}</span>
          </div>
        </Transition>

        <div class="fp-field">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="email"
            type="email"
            autocomplete="email"
            placeholder="trainer@pallet-town.vn"
            :aria-invalid="!!formError"
            required
          />
        </div>

        <button
          type="submit"
          class="fp-submit"
          :disabled="!canSubmit"
          :class="{ loading: submitting }"
        >
          <span class="fp-submit-pokeball" aria-hidden="true"></span>
          <span class="fp-submit-label">
            {{ submitting ? 'Dispatching' : 'Dispatch Reset Link' }}
          </span>
        </button>

        <p class="fp-note">
          Remembered it? <NuxtLink to="/login" class="inline-link">Back to sign in</NuxtLink>.
        </p>
      </form>
    </div>
  </section>
</template>

<style scoped>
.forgot-page {
  max-width: 560px;
  margin: 0 auto;
  padding: clamp(2rem, 6vw, 4rem) clamp(1rem, 4vw, 2rem) clamp(3rem, 8vw, 5rem);
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
  transition: background-color 0.2s ease, transform 0.2s ease;
}

.back-link:hover {
  background-color: rgba(0, 49, 83, 0.08);
  transform: translateX(-2px);
}

.back-link svg {
  width: 16px;
  height: 16px;
}

.fp-card {
  position: relative;
  background: #fbf9f2;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 30px 10px 25px 10px / 10px 25px 10px 30px;
  padding: clamp(1.75rem, 4vw, 2.5rem);
  box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.18);
  overflow: hidden;
  animation: card-rise 0.6s 0.05s ease both;
}

.fp-card::before {
  content: '';
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(90deg, rgba(200, 195, 180, 0.08) 50%, transparent 50%),
    linear-gradient(rgba(200, 195, 180, 0.08) 50%, transparent 50%);
  background-size: 4px 4px;
  pointer-events: none;
  border-radius: inherit;
  z-index: 0;
}

.fp-card > * {
  position: relative;
  z-index: 1;
}

.fp-stamp {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 100px;
  height: auto;
  transform: rotate(-10deg);
  z-index: 2;
  pointer-events: none;
}

.fp-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.fp-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  letter-spacing: 1.5px;
  color: #c2821b;
  font-size: 0.82rem;
  margin-bottom: 0.4rem;
}

.fp-title {
  font-family: var(--font-serif);
  font-size: clamp(1.75rem, 4vw, 2.25rem);
  color: var(--color-prussian-blue);
  margin-bottom: 0.5rem;
  letter-spacing: -0.5px;
  position: relative;
  display: inline-block;
}

.fp-title::after {
  content: '';
  display: block;
  width: 50px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 0.55rem auto 0;
  border-radius: 2px;
}

.fp-subtitle {
  font-family: var(--font-sans);
  color: #6f6f6f;
  font-size: 0.95rem;
  line-height: 1.55;
  max-width: 380px;
  margin: 0 auto;
}

.fp-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.fp-field {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
}

.fp-field label {
  font-family: var(--font-sans);
  font-size: 0.78rem;
  font-weight: 700;
  color: var(--color-prussian-blue);
  letter-spacing: 1px;
  text-transform: uppercase;
}

.fp-field input {
  width: 100%;
  padding: 0.8rem 1rem;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  background-color: #fff;
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: #222;
  box-shadow: 2px 2px 0 rgba(0, 49, 83, 0.1);
  transition: transform 0.25s ease, box-shadow 0.25s ease, border-color 0.25s ease;
}

.fp-field input::placeholder {
  color: #b3b3b3;
  font-style: italic;
}

.fp-field input:focus {
  outline: none;
  transform: translate(-2px, -2px);
  box-shadow: 4px 4px 0 var(--color-prussian-blue);
}

.fp-field input[aria-invalid='true'] {
  border-color: #b94a3d;
  box-shadow: 2px 2px 0 rgba(185, 74, 61, 0.25);
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

.fp-submit {
  position: relative;
  margin-top: 0.5rem;
  padding: 0.95rem 1.25rem;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  font-family: var(--font-sans);
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.95rem;
  cursor: pointer;
  overflow: hidden;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.6rem;
  box-shadow: 4px 4px 0 var(--color-prussian-blue);
  transition: transform 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease;
}

.fp-submit::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(
    120deg,
    transparent 0%,
    rgba(255, 255, 255, 0.55) 50%,
    transparent 100%
  );
  transform: translateX(-110%);
  transition: transform 0.6s ease;
  pointer-events: none;
}

.fp-submit:not(:disabled):hover {
  transform: translate(-2px, -2px);
  box-shadow: 6px 6px 0 var(--color-prussian-blue);
  background-color: #ffcf33;
}

.fp-submit:not(:disabled):hover::before {
  transform: translateX(110%);
}

.fp-submit:not(:disabled):active {
  transform: translate(2px, 2px);
  box-shadow: 2px 2px 0 var(--color-prussian-blue);
}

.fp-submit:disabled {
  cursor: not-allowed;
  opacity: 0.55;
}

.fp-submit-pokeball {
  position: relative;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: linear-gradient(
    to bottom,
    #d63a36 0%,
    #d63a36 50%,
    #fff 50%,
    #fff 100%
  );
  border: 1.5px solid #1a1a1a;
  flex: none;
  z-index: 1;
}

.fp-submit-pokeball::before {
  content: '';
  position: absolute;
  left: -1.5px;
  right: -1.5px;
  top: 50%;
  height: 1.5px;
  background-color: #1a1a1a;
  transform: translateY(-50%);
}

.fp-submit-pokeball::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background-color: #fff;
  border: 1.5px solid #1a1a1a;
  transform: translate(-50%, -50%);
}

.fp-submit.loading .fp-submit-pokeball {
  animation: pokeball-shake 0.8s ease-in-out infinite;
}

.fp-submit.loading .fp-submit-label::after {
  content: '';
  display: inline-block;
  width: 0.8em;
  margin-left: 0.15em;
  text-align: left;
  animation: dots 1.2s steps(4, end) infinite;
}

.fp-submit-label {
  position: relative;
  z-index: 1;
}

.fp-note {
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

.success-panel {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 2rem 1rem 1rem;
  gap: 0.5rem;
}

.success-panel h2 {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: 1.5rem;
  margin: 0.75rem 0 0.25rem;
}

.success-panel p {
  font-family: var(--font-sans);
  color: #555;
  font-size: 0.95rem;
  line-height: 1.6;
  max-width: 380px;
}

.sent-email {
  color: var(--color-prussian-blue);
  font-weight: 700;
  word-break: break-all;
}

.success-mark {
  display: grid;
  place-items: center;
  margin-bottom: 0.25rem;
  animation: stamp 0.5s cubic-bezier(0.18, 0.89, 0.32, 1.28) both;
}

.success-image {
  display: block;
  width: 96px;
  height: auto;
  animation: pokeball-bob 2.4s ease-in-out infinite;
}

.success-actions {
  margin-top: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 0.7rem;
  align-items: center;
}

.link-button {
  background: none;
  border: none;
  padding: 0;
  font-family: var(--font-sans);
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--color-cypress-green);
  cursor: pointer;
  text-decoration: underline;
  text-underline-offset: 3px;
  transition: color 0.2s ease;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.link-button:hover {
  color: var(--color-prussian-blue);
}

.back-cta {
  display: inline-block;
  padding: 0.75rem 1.75rem;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  font-family: var(--font-sans);
  font-weight: 800;
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  text-decoration: none;
  box-shadow: 3px 3px 0 var(--color-prussian-blue);
  transition: transform 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease;
}

.back-cta:hover {
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0 var(--color-prussian-blue);
  background-color: #ffcf33;
}

.back-cta:active {
  transform: translate(1px, 1px);
  box-shadow: 2px 2px 0 var(--color-prussian-blue);
}

.success-fade-enter-active {
  transition: opacity 0.4s ease, transform 0.4s ease;
}
.success-fade-enter-from {
  opacity: 0;
  transform: translateY(10px);
}

.alert-slide-enter-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}
.alert-slide-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.alert-slide-enter-from,
.alert-slide-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}

@keyframes card-rise {
  from {
    opacity: 0;
    transform: translateY(16px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes stamp {
  0% {
    transform: scale(0.6);
    opacity: 0;
  }
  60% {
    transform: scale(1.1);
    opacity: 1;
  }
  100% {
    transform: scale(1);
  }
}

@keyframes dots {
  0% {
    content: '';
  }
  25% {
    content: '.';
  }
  50% {
    content: '..';
  }
  75%,
  100% {
    content: '...';
  }
}

@keyframes pokeball-shake {
  0%, 100% { transform: rotate(0deg); }
  25% { transform: rotate(-18deg); }
  75% { transform: rotate(18deg); }
}

@keyframes pokeball-bob {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-4px); }
}

</style>
