<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const categories = ['All TCG Products', 'Elite Trainer Boxes', 'Booster Boxes', 'Booster Packs', 'Premium Collections']

const products = ref([
  { id: 1, title: 'Van Gogh Museum ETB', price: '$89.99', image: 'https://placehold.co/400x500/003153/FFC512?text=Van+Gogh+ETB' },
  { id: 2, title: 'Scarlet & Violet: 151 Booster Box', price: '$120.00', image: 'https://placehold.co/400x500/003153/FFC512?text=151+Booster+Box' },
  { id: 3, title: 'Starry Pikachu Promo Card (Graded)', price: '$250.00', image: 'https://placehold.co/400x500/003153/FFC512?text=Pikachu+Promo' },
  { id: 4, title: 'Paldean Fates Booster Bundle', price: '$26.99', image: 'https://placehold.co/400x500/003153/FFC512?text=Booster+Bundle' },
  { id: 5, title: 'Crown Zenith Booster Pack', price: '$4.99', image: 'https://placehold.co/400x500/003153/FFC512?text=Booster+Pack' },
  { id: 6, title: 'Eevee Evolutions Premium Collection', price: '$69.99', image: 'https://placehold.co/400x500/003153/FFC512?text=Eevee+Premium' },
  { id: 7, title: 'Temporal Forces Elite Trainer Box', price: '$49.99', image: 'https://placehold.co/400x500/003153/FFC512?text=Temporal+ETB' },
  { id: 8, title: 'Surging Sparks Booster Sleeve', price: '$3.99', image: 'https://placehold.co/400x500/003153/FFC512?text=Sleeved+Pack' },
])

const newsItems = ref([
  { id: 1, tag: 'New Arrival', title: 'The "Sunflowers Eevee" canvas painting has officially been restocked in the Museum Shop.', date: 'April 15, 2026' },
  { id: 2, tag: 'Exhibition', title: 'Join our special guided tour exploring the brushstrokes of the Pokémon masterclass collection.', date: 'April 14, 2026' },
  { id: 3, tag: 'Limited Release', title: 'Pre-order the exclusive Snorlax Bedroom diorama miniature set starting this weekend.', date: 'April 12, 2026' },
  { id: 4, tag: 'Museum News', title: 'Our gallery hours are extending! Now open until 9 PM on weekends for night viewings.', date: 'April 10, 2026' },
  { id: 5, tag: 'Shop Update', title: 'New post-impressionist mugs featuring Charizard and Bulbasaur are now available.', date: 'April 05, 2026' },
])

const currentNewsIndex = ref(0)
let newsInterval = null

const nextNews = () => {
  currentNewsIndex.value = (currentNewsIndex.value + 1) % newsItems.value.length
}

onMounted(() => {
  newsInterval = setInterval(nextNews, 3000)
})

onUnmounted(() => {
  if (newsInterval) clearInterval(newsInterval)
})
</script>

<template>
  <main>
    <!-- Hero Section -->
    <section class="hero full-width-hero">
      <img src="/images/pikachu_vangogh.webp" alt="Starry Pikachu" class="hero-bg full-width" />
      <div class="hero-overlay"></div>
      
      <div class="hero-content full-width-content">
        <div class="hero-text-box overlay-text">
          <span class="eyebrow gold-text">New Acquisition</span>
          <h2 class="hero-title white-text">Starry<br />Pikachu</h2>
          <p class="hero-desc white-text-muted">A post-impressionist masterpiece capturing the electric essence of the forest guardian beneath a swirling, cosmic firmament.</p>
          
          <div class="hero-buttons">
            <button class="btn-primary rounded-pill">Explore The Series</button>
            <button class="btn-secondary rounded-pill outline-glass">View Details</button>
          </div>
          
          <div class="hero-controls">
            <button class="icon-btn round-dark">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M15 18l-6-6 6-6"/></svg>
            </button>
            <button class="icon-btn round-dark">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9 18l6-6-6-6"/></svg>
            </button>
          </div>
        </div>
      </div>
      
      <div class="hero-dots">
        <span class="dot active"></span>
        <span class="dot"></span>
        <span class="dot"></span>
      </div>
    </section>

    <!-- What's New Separator -->
    <section class="whats-new-section">
      <h2 class="whats-new-text">What's new?</h2>
    </section>

    <!-- News Banner Auto-Slider Section -->
    <section class="news-banner-section">
      <div class="news-banner-background"></div>
      <div class="news-banner-container">
        
        <transition name="slide-fade" mode="out-in">
          <div :key="currentNewsIndex" class="news-banner-content">
            <span class="gold-text">{{ newsItems[currentNewsIndex].tag }}</span>
            <h3 class="news-banner-title">{{ newsItems[currentNewsIndex].title }}</h3>
            <p class="white-text-muted">{{ newsItems[currentNewsIndex].date }}</p>
          </div>
        </transition>
        
        <!-- Pagination Dots -->
        <div class="news-banner-indicators">
          <span 
            v-for="(item, index) in newsItems" 
            :key="index"
            class="banner-dot"
            :class="{ active: index === currentNewsIndex }"
            @click="currentNewsIndex = index"
          ></span>
        </div>
        
      </div>
    </section>

    <!-- Shop Section -->
    <section class="shop-section">
      <h3 class="section-title">Trading Card Game</h3>
      
      <!-- Filters -->
      <div class="categories">
        <button 
          v-for="(cat, index) in categories" 
          :key="cat" 
          class="category-btn"
          :class="{ active: index === 0 }"
        >
          {{ cat }}
        </button>
      </div>

      <!-- Product Grid -->
      <div class="product-grid">
        <ProductCard 
          v-for="product in products" 
          :key="product.id" 
          :product="product" 
        />
      </div>
    </section>
  </main>
</template>
