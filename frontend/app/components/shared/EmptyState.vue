<script setup>
defineProps({
  title: { type: String, required: true },
  message: { type: String, required: true },
  ctaLabel: { type: String, default: '' },
  ctaTo: { type: String, default: '' },
})
</script>

<template>
  <div class="empty-state">
    <span class="empty-eyebrow">— interlude —</span>
    <h2 class="empty-title">{{ title }}</h2>
    <p class="empty-message">{{ message }}</p>
    <NuxtLink v-if="ctaLabel && ctaTo" :to="ctaTo" class="empty-cta">
      {{ ctaLabel }}
    </NuxtLink>
  </div>
</template>

<style scoped>
.empty-state {
  text-align: center;
  padding: clamp(3rem, 6vw, 5rem) 2rem;
  background-color: rgba(255, 255, 255, 0.5);
  border-radius: 8px;
  border: 1px dashed rgba(0, 49, 83, 0.2);
  max-width: 600px;
  margin: 2rem auto;
}

.empty-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  letter-spacing: 1.5px;
  text-transform: uppercase;
  font-size: 0.85rem;
  display: block;
  margin-bottom: 1rem;
}

.empty-title {
  font-family: var(--font-serif);
  font-style: italic;
  font-size: clamp(1.5rem, 3.5vw, 2rem);
  color: var(--color-prussian-blue);
  margin-bottom: 1rem;
  position: relative;
  display: inline-block;
}

.empty-title::after {
  content: '';
  display: block;
  width: 50px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 0.75rem auto 0;
  border-radius: 2px;
}

.empty-message {
  font-family: var(--font-sans);
  color: #555;
  margin-bottom: 2rem;
  line-height: 1.6;
}

.empty-cta {
  display: inline-block;
  padding: 0.85rem 2rem;
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  text-decoration: none;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.95rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.2);
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.empty-cta:hover {
  background: linear-gradient(135deg, #F5B341 0%, #E6A039 50%, #B07212 100%);
  transform: translateY(-2px) scale(1.02);
  letter-spacing: 1.5px;
  box-shadow:
    inset 1px 2px 4px rgba(255,255,255,0.55),
    0 0 22px rgba(245, 176, 65, 0.45),
    0 6px 16px rgba(0, 0, 0, 0.25);
}
</style>
