<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { favoritesService, type Favorite } from '../../services/favorites.service'
import { formatTimeAgo } from '../../utils/format'

definePageMeta({
  middleware: 'auth-required',
})

useHead({ title: 'My Favorites — PokéGogh' })

const { ids } = useFavorites()

const { data, pending, error, refresh } = await useAsyncData<Favorite[]>(
  'account-favorites',
  () => favoritesService.list(),
)

watch(ids, (next, prev) => {
  if (!prev) return
  if (next.length === prev.length) return
  refresh()
})

const stableFavorites = ref<Favorite[]>(data.value ?? [])
const hasSettledOnce = ref(data.value !== null)

watch(pending, (isPending) => {
  if (isPending) return
  if (data.value !== null && data.value !== undefined) {
    stableFavorites.value = data.value
  }
  hasSettledOnce.value = true
})

type SortKey = 'recent' | 'oldest' | 'price-desc' | 'price-asc'
const sort = ref<SortKey>('recent')

const sortedFavorites = computed(() => {
  const list = [...stableFavorites.value]
  switch (sort.value) {
    case 'oldest':
      return list.sort((a, b) => +new Date(a.createdAt) - +new Date(b.createdAt))
    case 'price-desc':
      return list.sort((a, b) => b.product.price - a.product.price)
    case 'price-asc':
      return list.sort((a, b) => a.product.price - b.product.price)
    case 'recent':
    default:
      return list.sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt))
  }
})

const count = computed(() => stableFavorites.value.length)
const lastSavedLabel = computed(() => {
  if (!stableFavorites.value.length) return ''
  const newest = stableFavorites.value.reduce((acc, f) =>
    +new Date(f.createdAt) > +new Date(acc.createdAt) ? f : acc,
  )
  return formatTimeAgo(newest.createdAt)
})
</script>

<template>
  <section class="favorites-page">
    <!-- Hero banner with pokeball stamp + Sylveon mascot -->
    <header class="fav-hero">
      <!-- Decorative floating Luvdisc -->
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/370.png"
        alt=""
        aria-hidden="true"
        class="fav-deco fav-deco-luvdisc-1"
        loading="lazy"
        decoding="async"
      />
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/370.png"
        alt=""
        aria-hidden="true"
        class="fav-deco fav-deco-luvdisc-2"
        loading="lazy"
        decoding="async"
      />

      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/items/love-ball.png"
        alt=""
        aria-hidden="true"
        class="fav-hero-stamp"
        loading="eager"
        decoding="async"
      />

      <div class="fav-hero-text">
        <p class="fav-hero-eyebrow">— Your Saved Items —</p>
        <h1 class="fav-hero-title">My Favorites</h1>
        <p class="fav-hero-subtitle">
          Every item you've saved, ready to buy when you are.
        </p>
      </div>

      <!-- Sylveon mascot — fairy guardian of the binder -->
      <img
        src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/700.png"
        alt="Sylveon, guardian of your favorites"
        class="fav-hero-mascot"
        loading="lazy"
        decoding="async"
      />
    </header>

    <!-- Stats + Sort bar -->
    <div v-if="count > 0" class="fav-controls">
      <p class="fav-stats">
        <strong>{{ count }}</strong> {{ count === 1 ? 'item saved' : 'items saved' }}
        <span v-if="lastSavedLabel" class="fav-stats-sep"> · last added {{ lastSavedLabel }}</span>
      </p>

      <label class="fav-sort">
        <span class="fav-sort-label">Sort</span>
        <select v-model="sort" class="fav-sort-select">
          <option value="recent">Recently saved</option>
          <option value="oldest">Oldest first</option>
          <option value="price-desc">Highest price</option>
          <option value="price-asc">Lowest price</option>
        </select>
      </label>
    </div>

    <div class="fav-area">
      <div v-if="!hasSettledOnce" class="fav-loading">
        <div v-for="n in 8" :key="n" class="skeleton fav-skeleton" />
      </div>

      <p v-else-if="error" class="inline-error">
        Could not load your favorites. Please try again.
      </p>

      <div v-else-if="!count" class="fav-empty">
        <img
          src="https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/143.png"
          alt="A sleeping Snorlax"
          class="fav-empty-mascot"
          loading="lazy"
          decoding="async"
        />
        <h2 class="fav-empty-title">Snorlax is guarding an empty list</h2>
        <p class="fav-empty-msg">
          Tap the heart on any item to save it here. Snorlax will keep it safe.
        </p>
        <NuxtLink to="/products" class="fav-empty-cta">Browse Items</NuxtLink>
      </div>

      <div
        v-else
        class="fav-grid"
        :class="{ 'is-refetching': pending }"
        :aria-busy="pending"
      >
        <div v-for="fav in sortedFavorites" :key="fav.productId" class="fav-card">
          <ProductCard :product="fav.product" />
          <p class="fav-saved-caption">
            <svg viewBox="0 0 24 24" fill="currentColor" stroke="none" class="fav-saved-icon">
              <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
            </svg>
            Saved {{ formatTimeAgo(fav.createdAt) }}
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.favorites-page {
  max-width: clamp(var(--container-max), 90vw, 1500px);
  margin: 0 auto;
  padding: clamp(2rem, 6vw, 4rem) clamp(1rem, 4vw, 2rem) clamp(3rem, 8vw, 5rem);
}

/* ── Hero banner ─────────────────────────────────────────── */
.fav-hero {
  position: relative;
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 2rem 2rem 2rem 1.75rem;
  margin-bottom: 1.5rem;
  background:
    radial-gradient(circle at top right, rgba(255, 197, 18, 0.08) 0%, transparent 60%),
    radial-gradient(circle at bottom left, rgba(255, 105, 180, 0.06) 0%, transparent 50%),
    linear-gradient(180deg, rgba(178, 34, 34, 0.1) 0%, rgba(178, 34, 34, 0.02) 100%);
  border: 1.5px solid rgba(178, 34, 34, 0.25);
  border-radius: 18px;
  overflow: hidden;
  min-height: 160px;
}

.fav-hero::before {
  content: "";
  position: absolute;
  top: 0;
  left: 1.5rem;
  right: 1.5rem;
  height: 3px;
  background: linear-gradient(90deg, transparent, #b22222 30%, #b22222 70%, transparent);
  box-shadow: 0 0 12px rgba(178, 34, 34, 0.4);
  z-index: 3;
}

.fav-hero-stamp {
  position: relative;
  z-index: 2;
  flex-shrink: 0;
  width: 72px;
  height: 72px;
  object-fit: contain;
  image-rendering: pixelated;
  image-rendering: crisp-edges;
  filter: drop-shadow(2px 3px 4px rgba(0, 49, 83, 0.3));
  transform: rotate(-8deg);
  transition: transform 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.fav-hero:hover .fav-hero-stamp {
  transform: rotate(0) scale(1.08);
}

.fav-hero-text {
  position: relative;
  z-index: 2;
  flex: 1;
  min-width: 0;
}

.fav-hero-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  color: #b22222;
  font-size: 0.9rem;
  letter-spacing: 1px;
  margin: 0 0 0.25rem;
}

.fav-hero-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: clamp(1.75rem, 4vw, 2.4rem);
  margin: 0 0 0.4rem;
  line-height: 1.15;
  letter-spacing: -0.5px;
}

.fav-hero-subtitle {
  font-family: var(--font-sans);
  color: #555;
  font-size: 0.9rem;
  line-height: 1.5;
  margin: 0;
  max-width: 520px;
}

/* Sylveon mascot — fairy guardian on the right */
.fav-hero-mascot {
  position: relative;
  z-index: 2;
  flex-shrink: 0;
  width: 140px;
  height: 140px;
  object-fit: contain;
  transform-origin: center bottom;
  filter: drop-shadow(2px 4px 8px rgba(178, 34, 34, 0.22));
  animation: sylveon-float 3.5s ease-in-out infinite;
}

@keyframes sylveon-float {
  0%, 100% { transform: translateY(0) rotate(-4deg); }
  50%      { transform: translateY(-7px) rotate(-1deg); }
}

@media (max-width: 768px) {
  .fav-hero-mascot {
    width: 100px;
    height: 100px;
  }
}

@media (max-width: 600px) {
  .fav-hero {
    flex-direction: column;
    text-align: center;
    align-items: center;
    padding: 1.5rem 1.25rem 1rem;
  }
  .fav-hero-subtitle {
    margin: 0 auto;
  }
  .fav-hero-mascot {
    width: 90px;
    height: 90px;
    transform: rotate(-4deg);
  }
}

/* Floating Luvdisc decorations — heart Pokemon scattered behind content */
.fav-deco {
  position: absolute;
  z-index: 1;
  pointer-events: none;
  opacity: 0.18;
  filter: drop-shadow(1px 1px 2px rgba(178, 34, 34, 0.3));
}

.fav-deco-luvdisc-1 {
  width: 80px;
  height: 80px;
  top: -20px;
  left: 35%;
  transform: rotate(-15deg);
}

.fav-deco-luvdisc-2 {
  width: 60px;
  height: 60px;
  bottom: -15px;
  left: 22%;
  transform: rotate(20deg);
  opacity: 0.13;
}

@media (max-width: 600px) {
  .fav-deco {
    display: none;
  }
}

/* ── Controls bar ────────────────────────────────────────── */
.fav-controls {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem 0.25rem 1rem;
  margin-bottom: 1rem;
  border-bottom: 1px dashed rgba(0, 49, 83, 0.18);
  flex-wrap: wrap;
}

.fav-stats {
  font-family: var(--font-serif);
  font-style: italic;
  color: var(--color-prussian-blue);
  font-size: 0.95rem;
  margin: 0;
}

.fav-stats strong {
  color: #b22222;
  font-style: normal;
  font-family: var(--font-sans);
  font-weight: 700;
  margin-right: 0.15rem;
}

.fav-stats-sep {
  color: #777;
  font-style: italic;
}

.fav-sort {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  font-family: var(--font-sans);
}

.fav-sort-label {
  font-size: 0.78rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #777;
  font-weight: 600;
}

.fav-sort-select {
  font-family: var(--font-sans);
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--color-prussian-blue);
  background: #fff;
  border: 1.5px solid rgba(0, 49, 83, 0.15);
  border-radius: 999px;
  padding: 0.4rem 1rem;
  cursor: pointer;
  transition: border-color 0.2s ease;
}

.fav-sort-select:hover,
.fav-sort-select:focus {
  border-color: #b22222;
  outline: none;
}

/* ── Grid + cards ────────────────────────────────────────── */
.fav-area {
  min-height: 50vh;
}

.fav-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.75rem 1rem;
  transition: opacity 0.2s ease;
}

@media (min-width: 640px) {
  .fav-grid {
    grid-template-columns: repeat(3, 1fr);
    gap: 2rem 1.5rem;
  }
}

@media (min-width: 1024px) {
  .fav-grid {
    grid-template-columns: repeat(4, 1fr);
    gap: 2.5rem 1.75rem;
  }
}

.fav-grid.is-refetching {
  opacity: 0.6;
  pointer-events: none;
}

.fav-card {
  display: flex;
  flex-direction: column;
}

.fav-saved-caption {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
  margin: 0.5rem 0 0;
  font-family: var(--font-serif);
  font-style: italic;
  font-size: 0.78rem;
  color: #b22222;
  letter-spacing: 0.3px;
}

.fav-saved-icon {
  width: 11px;
  height: 11px;
}

/* ── Loading skeleton ───────────────────────────────────── */
.fav-loading {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.75rem 1rem;
}

@media (min-width: 640px) {
  .fav-loading {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (min-width: 1024px) {
  .fav-loading {
    grid-template-columns: repeat(4, 1fr);
  }
}

.fav-skeleton {
  aspect-ratio: 4/5;
  border-radius: 8px;
}

/* ── Empty state ────────────────────────────────────────── */
.fav-empty {
  text-align: center;
  padding: clamp(2rem, 5vw, 3rem) 1.5rem clamp(2.5rem, 6vw, 4rem);
  background: rgba(255, 255, 255, 0.5);
  border: 2px dashed rgba(178, 34, 34, 0.25);
  border-radius: 16px;
  margin-top: 1.5rem;
}

.fav-empty-mascot {
  width: 180px;
  height: 180px;
  object-fit: contain;
  margin: 0 auto 0.5rem;
  display: block;
  filter: drop-shadow(2px 4px 8px rgba(0, 49, 83, 0.18));
  animation: snorlax-breathe 4s ease-in-out infinite;
}

@keyframes snorlax-breathe {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.03); }
}

.fav-empty-title {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-size: 1.4rem;
  margin: 0 0 0.5rem;
}

.fav-empty-msg {
  font-family: var(--font-sans);
  color: #666;
  font-size: 0.95rem;
  line-height: 1.5;
  margin: 0 auto 1.5rem;
  max-width: 380px;
}

.fav-empty-cta {
  display: inline-block;
  padding: 0.85rem 1.75rem;
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.9rem;
  letter-spacing: 1px;
  text-transform: uppercase;
  text-decoration: none;
  box-shadow: inset 1px 2px 3px rgba(255,255,255,0.4), 2px 4px 8px rgba(0, 0, 0, 0.18);
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.fav-empty-cta:hover {
  transform: translateY(-2px) scale(1.02);
  box-shadow: inset 1px 2px 4px rgba(255,255,255,0.55), 0 0 22px rgba(245, 176, 65, 0.45), 0 6px 16px rgba(0, 0, 0, 0.25);
}
</style>
