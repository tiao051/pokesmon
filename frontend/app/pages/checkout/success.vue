<script setup>
import { computed } from 'vue'

const route = useRoute()
const acquisitionRef = computed(() => route.query.ref || 'PG-XXXXXX')

useHead({ title: 'Welcome to the Collection — PokéGogh' })
</script>

<template>
  <main class="success-page">
    <article class="success-card">
      <div class="success-emblem">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="12" cy="12" r="10"></circle>
          <polyline points="8 12 11 15 16 9"></polyline>
        </svg>
      </div>

      <span class="success-eyebrow">Acquisition Confirmed</span>
      <h1 class="success-title">Welcome to the Collection</h1>
      <p class="success-message">
        Your acquisition is being prepared by our curators. A confirmation has been dispatched to your patron address. Thank you for supporting the PokéGogh Museum.
      </p>

      <div class="acquisition-ref">
        <span class="ref-label">Acquisition Reference</span>
        <span class="ref-value">{{ acquisitionRef }}</span>
      </div>

      <div class="success-actions">
        <NuxtLink to="/" class="btn-secondary-museum">Return to Museum</NuxtLink>
        <NuxtLink to="/products" class="btn-primary-museum">Continue Browsing</NuxtLink>
      </div>
    </article>
  </main>
</template>

<style scoped>
.success-page {
  max-width: 720px;
  margin: 0 auto;
  padding: clamp(4rem, 8vw, 6rem) 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh - 200px);
}

.success-card {
  background-color: rgba(255, 255, 255, 0.6);
  border-radius: 12px;
  padding: clamp(2.5rem, 6vw, 4rem) 2rem;
  text-align: center;
  box-shadow: 0 20px 50px rgba(0, 49, 83, 0.1);
  border: 1px solid rgba(0, 49, 83, 0.08);
  position: relative;
  overflow: hidden;
  width: 100%;
}
.success-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 6px;
  background: linear-gradient(90deg, var(--color-prussian-blue) 0%, var(--color-cypress-green) 50%, var(--color-sunflower-yellow) 100%);
}

.success-emblem {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 70px;
  height: 70px;
  border-radius: 50%;
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: var(--color-prussian-blue);
  margin-bottom: 1.5rem;
  box-shadow: 0 4px 15px rgba(184, 130, 27, 0.4);
}
.success-emblem svg {
  width: 36px;
  height: 36px;
}

.success-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: 1.05rem;
  letter-spacing: 1px;
  display: block;
  margin-bottom: 0.5rem;
}

.success-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(2rem, 5vw, 2.75rem);
  margin-bottom: 1rem;
  line-height: 1.2;
}

.success-message {
  font-family: var(--font-sans);
  color: #555;
  font-size: 1rem;
  line-height: 1.7;
  max-width: 480px;
  margin: 0 auto 2.5rem;
}

.acquisition-ref {
  display: inline-flex;
  flex-direction: column;
  align-items: center;
  gap: 0.35rem;
  padding: 1rem 2rem;
  background-color: #0b151c;
  color: var(--color-linen);
  border-radius: 8px;
  margin-bottom: 2.5rem;
}

.ref-label {
  font-family: var(--font-sans);
  font-size: 0.7rem;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  color: #B0BCC8;
}
.ref-value {
  font-family: var(--font-sans);
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--color-sunflower-yellow);
  letter-spacing: 1px;
}

.success-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.btn-primary-museum,
.btn-secondary-museum {
  padding: 0.95rem 1.75rem;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.95rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  text-decoration: none;
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  display: inline-block;
  cursor: pointer;
}

.btn-primary-museum {
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.2);
}
.btn-primary-museum:hover {
  background: linear-gradient(135deg, #F5B341 0%, #E6A039 50%, #B07212 100%);
  transform: translateY(-2px) scale(1.02);
  letter-spacing: 1.5px;
  box-shadow:
    inset 1px 2px 4px rgba(255,255,255,0.55),
    0 0 22px rgba(245, 176, 65, 0.45),
    0 6px 16px rgba(0, 0, 0, 0.25);
}

.btn-secondary-museum {
  background-color: transparent;
  border: 2px solid var(--color-prussian-blue);
  color: var(--color-prussian-blue);
  box-shadow: 3px 3px 0px rgba(0, 49, 83, 0.15);
}
.btn-secondary-museum:hover {
  background-color: var(--color-prussian-blue);
  color: var(--color-linen);
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0px rgba(0, 49, 83, 0.4);
}
</style>
