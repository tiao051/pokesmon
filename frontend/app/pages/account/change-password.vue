<script setup lang="ts">
import { reactive, ref, computed } from 'vue'

definePageMeta({
  middleware: 'auth-required',
})

const { changePassword, signOut } = useAuth()
const router = useRouter()

type FieldKey = 'current' | 'next' | 'confirm'

interface PasswordField {
  key: FieldKey
  label: string
  placeholder: string
  autocomplete: string
}

const passwordFields: PasswordField[] = [
  { key: 'current', label: 'Current password', placeholder: '••••••••', autocomplete: 'current-password' },
  { key: 'next', label: 'New password', placeholder: 'At least 8 characters', autocomplete: 'new-password' },
  { key: 'confirm', label: 'Confirm new password', placeholder: 'Repeat your new password', autocomplete: 'new-password' },
]

const form = reactive<Record<FieldKey, string>>({
  current: '',
  next: '',
  confirm: '',
})

const visible = reactive<Record<FieldKey, boolean>>({
  current: false,
  next: false,
  confirm: false,
})

const submitting = ref(false)
const formError = ref<string | null>(null)
const success = ref(false)

const toggleVisibility = (field: FieldKey) => {
  visible[field] = !visible[field]
}

const fieldErrors = computed<Record<FieldKey, string | null>>(() => {
  const errs: Record<FieldKey, string | null> = {
    current: null,
    next: null,
    confirm: null,
  }
  if (form.next && form.next.length < 8) {
    errs.next = 'Use at least 8 characters.'
  }
  if (form.next && form.current && form.next === form.current) {
    errs.next = 'New password must differ from your current one.'
  }
  if (form.confirm && form.confirm !== form.next) {
    errs.confirm = 'Passwords do not match.'
  }
  return errs
})

const canSubmit = computed(() => {
  return (
    !submitting.value &&
    form.current.length > 0 &&
    form.next.length >= 8 &&
    form.confirm === form.next &&
    form.next !== form.current
  )
})

const handleSubmit = async () => {
  if (!canSubmit.value) return
  formError.value = null
  submitting.value = true
  try {
    await changePassword(form.current, form.next)
    success.value = true
    setTimeout(() => {
      signOut()
      router.push('/login')
    }, 1800)
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Could not update your password.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <section class="change-password-page">
    <NuxtLink to="/account" class="back-link">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <line x1="19" y1="12" x2="5" y2="12"></line>
        <polyline points="12 19 5 12 12 5"></polyline>
      </svg>
      <span>Back to Profile</span>
    </NuxtLink>

    <div class="cp-card">
      <div class="cp-header">
        <p class="cp-eyebrow">— Security —</p>
        <h1 class="cp-title">Change Password</h1>
        <p class="cp-subtitle">A fresh key for the trainer's vault.</p>
      </div>

      <Transition name="success-fade">
        <div v-if="success" class="success-panel" role="status">
          <div class="success-mark" aria-hidden="true">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
          </div>
          <h2>Password updated</h2>
          <p>Sign in again with your new credentials.</p>
        </div>
      </Transition>

      <form v-if="!success" class="cp-form" @submit.prevent="handleSubmit" novalidate>
        <Transition name="alert-slide">
          <div v-if="formError" class="form-alert" role="alert">
            <span class="alert-mark" aria-hidden="true">!</span>
            <span>{{ formError }}</span>
          </div>
        </Transition>

        <div
          v-for="field in passwordFields"
          :key="field.key"
          class="cp-field"
        >
          <label :for="field.key">{{ field.label }}</label>
          <div class="cp-input-wrap">
            <input
              :id="field.key"
              v-model="form[field.key]"
              :type="visible[field.key] ? 'text' : 'password'"
              :placeholder="field.placeholder"
              :autocomplete="field.autocomplete"
              :aria-invalid="!!fieldErrors[field.key]"
              required
            />
            <button
              type="button"
              class="cp-toggle"
              :aria-label="visible[field.key] ? 'Hide password' : 'Show password'"
              :aria-pressed="visible[field.key]"
              @click="toggleVisibility(field.key)"
            >
              <svg v-if="!visible[field.key]" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                <circle cx="12" cy="12" r="3"></circle>
              </svg>
              <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M17.94 17.94A10.94 10.94 0 0 1 12 20c-7 0-11-8-11-8a19.6 19.6 0 0 1 5.06-5.94"></path>
                <path d="M9.9 4.24A10.94 10.94 0 0 1 12 4c7 0 11 8 11 8a19.6 19.6 0 0 1-3.17 4.19"></path>
                <path d="M9.88 9.88A3 3 0 0 0 12 15a3 3 0 0 0 2.12-.88"></path>
                <line x1="1" y1="1" x2="23" y2="23"></line>
              </svg>
            </button>
          </div>
          <p v-if="fieldErrors[field.key]" class="field-error">
            {{ fieldErrors[field.key] }}
          </p>
        </div>

        <button
          type="submit"
          class="cp-submit"
          :disabled="!canSubmit"
          :class="{ loading: submitting }"
        >
          <span class="cp-submit-label">
            {{ submitting ? 'Updating…' : 'Update Password' }}
          </span>
        </button>

        <p class="cp-note">
          Updating your password will sign you out of this session.
        </p>
      </form>
    </div>
  </section>
</template>

<style scoped>
.change-password-page {
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

.cp-card {
  position: relative;
  background: #fff;
  border: 1.5px solid rgba(0, 49, 83, 0.15);
  border-radius: 32px 8px 28px 8px / 8px 28px 8px 32px;
  padding: clamp(1.75rem, 4vw, 2.5rem);
  box-shadow: 8px 8px 0 rgba(0, 49, 83, 0.08);
  overflow: hidden;
  animation: card-rise 0.6s 0.05s ease both;
}

.cp-card::before {
  content: '';
  position: absolute;
  top: -40px;
  right: -40px;
  width: 160px;
  height: 160px;
  background-image: radial-gradient(
    circle,
    rgba(255, 197, 18, 0.18) 0%,
    rgba(255, 197, 18, 0) 70%
  );
  pointer-events: none;
}

.cp-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.cp-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  letter-spacing: 2px;
  color: #c2821b;
  font-size: 0.78rem;
  margin-bottom: 0.4rem;
}

.cp-title {
  font-family: var(--font-serif);
  font-size: clamp(1.75rem, 4vw, 2.25rem);
  color: var(--color-prussian-blue);
  margin-bottom: 0.3rem;
  letter-spacing: -0.5px;
}

.cp-subtitle {
  font-family: var(--font-sans);
  color: #6f6f6f;
  font-size: 0.92rem;
}

.cp-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cp-field {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
}

.cp-field label {
  font-family: var(--font-sans);
  font-size: 0.82rem;
  font-weight: 600;
  color: var(--color-prussian-blue);
  letter-spacing: 0.3px;
}

.cp-input-wrap {
  position: relative;
}

.cp-field input {
  width: 100%;
  padding: 0.75rem 2.85rem 0.75rem 1rem;
  border: 2px solid rgba(0, 49, 83, 0.4);
  border-radius: 14px;
  background-color: #fff;
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: #222;
  transition: border-color 0.25s ease, box-shadow 0.25s ease, transform 0.25s ease;
}

.cp-toggle {
  position: absolute;
  right: 8px;
  top: 50%;
  transform: translateY(-50%);
  width: 34px;
  height: 34px;
  display: grid;
  place-items: center;
  background: transparent;
  border: none;
  border-radius: 8px;
  color: rgba(0, 49, 83, 0.55);
  cursor: pointer;
  transition: color 0.2s ease, background-color 0.2s ease, transform 0.2s ease;
}

.cp-toggle:hover {
  color: var(--color-prussian-blue);
  background-color: rgba(0, 49, 83, 0.06);
}

.cp-toggle:active {
  transform: translateY(-50%) scale(0.92);
}

.cp-toggle:focus-visible {
  outline: 2px solid var(--color-sunflower-yellow);
  outline-offset: 1px;
}

.cp-toggle[aria-pressed='true'] {
  color: var(--color-cypress-green);
}

.cp-toggle svg {
  width: 18px;
  height: 18px;
  transition: transform 0.2s ease;
}

.cp-toggle:hover svg {
  transform: scale(1.1);
}

.cp-field input::placeholder {
  color: #b3b3b3;
}

.cp-field input:focus {
  outline: none;
  border-color: var(--color-prussian-blue);
  box-shadow: 0 0 0 4px rgba(0, 49, 83, 0.12);
  transform: translateY(-1px);
}

.cp-field input[aria-invalid='true'] {
  border-color: #b94a3d;
  box-shadow: 0 0 0 4px rgba(185, 74, 61, 0.1);
}

.field-error {
  font-family: var(--font-sans);
  font-size: 0.78rem;
  color: #b94a3d;
  margin-top: 0.1rem;
  animation: field-error-in 0.25s ease both;
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

.cp-submit {
  position: relative;
  margin-top: 0.5rem;
  padding: 0.95rem 1.25rem;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 16px;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  font-family: var(--font-sans);
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.95rem;
  cursor: pointer;
  overflow: hidden;
  box-shadow: 4px 4px 0 var(--color-prussian-blue);
  transition: transform 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease;
}

.cp-submit::before {
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
}

.cp-submit:not(:disabled):hover {
  transform: translate(-2px, -2px);
  box-shadow: 6px 6px 0 var(--color-prussian-blue);
}

.cp-submit:not(:disabled):hover::before {
  transform: translateX(110%);
}

.cp-submit:not(:disabled):active {
  transform: translate(2px, 2px);
  box-shadow: 2px 2px 0 var(--color-prussian-blue);
}

.cp-submit:disabled {
  cursor: not-allowed;
  opacity: 0.55;
}

.cp-submit.loading .cp-submit-label::after {
  content: '';
  display: inline-block;
  width: 0.6em;
  margin-left: 0.2em;
  text-align: left;
  animation: dots 1.2s steps(4, end) infinite;
}

.cp-submit-label {
  position: relative;
  z-index: 1;
}

.cp-note {
  font-family: var(--font-sans);
  font-size: 0.78rem;
  color: #888;
  text-align: center;
  margin-top: 0.25rem;
}

/* ── Success panel ────────────────────────────────────────────── */

.success-panel {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: 2rem 1rem 1rem;
}

.success-panel h2 {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: 1.5rem;
  margin: 0.75rem 0 0.25rem;
}

.success-panel p {
  font-family: var(--font-sans);
  color: #6f6f6f;
  font-size: 0.95rem;
}

.success-mark {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background-color: var(--color-cypress-green);
  color: #fff;
  display: grid;
  place-items: center;
  box-shadow: 0 0 0 6px rgba(15, 94, 60, 0.15);
  animation: stamp 0.5s cubic-bezier(0.18, 0.89, 0.32, 1.28) both;
}

.success-mark svg {
  width: 30px;
  height: 30px;
}

/* ── Transitions & keyframes ──────────────────────────────────── */

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

@keyframes field-error-in {
  from {
    opacity: 0;
    transform: translateY(-3px);
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
</style>
