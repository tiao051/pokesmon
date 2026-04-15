<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<style>
/* Import Google Fonts */
@import url('https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,400;0,600;0,700;1,400&family=Inter:wght@300;400;500;600&display=swap');

:root {
  /* Van Gogh Inspired Palette */
  --color-prussian-blue: #003153;
  --color-sunflower-yellow: #FFC512;
  --color-cypress-green: #0F5E3C;
  --color-linen: #F3F1E9; /* Off-white for canvas */
  --color-brush-dark: #1A1A1A;
  
  /* Typography */
  --font-serif: 'Playfair Display', serif;
  --font-sans: 'Inter', sans-serif;
  
  /* Layout */
  --container-max: 1280px;
  --spacing-base: 1rem;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: var(--font-sans);
  background-color: var(--color-linen);
  color: var(--color-brush-dark);
  line-height: 1.6;
  /* Subtle linen canvas texture */
  background-image: 
    linear-gradient(90deg, rgba(200, 195, 180, 0.05) 50%, transparent 50%),
    linear-gradient(rgba(200, 195, 180, 0.05) 50%, transparent 50%);
  background-size: 4px 4px;
}

.van-gogh-app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

h1, h2, h3, h4, h5, h6 {
  font-family: var(--font-serif);
  color: var(--color-prussian-blue);
  font-weight: 700;
}

/* Header */
.header {
  position: sticky;
  top: 0;
  z-index: 100;
  background-color: rgba(243, 241, 233, 0.95);
  backdrop-filter: blur(8px);
  border-bottom: 2px solid rgba(0, 49, 83, 0.1);
}

.header-container {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

@media (min-width: 768px) {
  .header-container {
    padding: 1rem 2rem;
    gap: 2rem;
  }
}

.logo {
  flex: none;
  display: flex;
  justify-content: flex-start;
}

.logo h1 {
  font-size: clamp(1.25rem, 4vw, 1.75rem);
  letter-spacing: -0.5px;
  margin: 0;
  color: var(--color-prussian-blue);
  flex-shrink: 0;
}

/* Header Search */
.header-search {
  display: none;
}

@media (min-width: 768px) {
  .header-search {
    display: flex;
    flex: 0 1 600px;
    width: 100%;
    position: relative;
  }
}

.header-search .search-input {
  flex: 1;
  padding: 0.85rem 1.5rem;
  border: 1px solid transparent;
  background-color: #E8E8E8;
  font-family: var(--font-sans);
  font-size: 1rem;
  color: #333;
  outline: none;
  transition: background-color 0.3s ease;
  border-radius: 0;
}

.header-search .search-input:focus {
  background-color: #DFDFDF;
}

.header-search .search-btn {
  background-color: #7A7A7A;
  border: none;
  color: #FFFFFF;
  cursor: pointer;
  padding: 0 1.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s ease;
}

.header-search .search-btn:hover {
  background-color: #666666;
}

.header-search .search-btn svg {
  width: 24px;
  height: 24px;
  stroke-width: 2.5;
}

/* Header Actions */
.header-actions {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 2rem;
  flex-shrink: 0;
}

.action-link {
  display: none;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  color: #666666;
  font-family: var(--font-sans);
  font-weight: 500;
  font-size: 0.85rem;
  transition: color 0.3s ease;
}

@media (min-width: 768px) {
  .action-link {
    display: flex;
  }
}

.action-link:hover {
  color: #333333;
}

.action-link svg {
  width: 32px;
  height: 32px;
  color: #7A7A7A;
}

.action-text {
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #5C5C5C;
}

.icon-btn {
  background: none;
  border: none;
  color: var(--color-prussian-blue);
  cursor: pointer;
  padding: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: color 0.2s;
  width: 40px;
  height: 40px;
}

.icon-btn:hover {
  color: var(--color-cypress-green);
}

.icon-btn svg {
  width: 24px;
  height: 24px;
}

.mobile-menu-btn {
  display: block;
}

@media (min-width: 768px) {
  .mobile-menu-btn {
    display: none;
  }
}

/* Mobile Nav */
.mobile-nav {
  display: none;
  flex-direction: column;
  padding: 1rem 2rem 2rem;
  background-color: var(--color-linen);
  border-bottom: 2px solid var(--color-sunflower-yellow);
}

.mobile-nav.open {
  display: flex;
}

.mobile-search {
  display: flex;
  position: relative;
  margin-bottom: 1rem;
}

.mobile-search .search-input {
  width: 100%;
  padding: 0.75rem 3rem 0.75rem 1.5rem;
  border: 1px solid rgba(0, 49, 83, 0.2);
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px; 
  background-color: #fff;
  font-family: var(--font-sans);
  font-size: 0.95rem;
}

.mobile-search .search-btn {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: var(--color-prussian-blue);
}

.mobile-search .search-btn svg {
  width: 20px;
  height: 20px;
}

.mobile-nav .nav-link {
  text-decoration: none;
  color: var(--color-prussian-blue);
  font-family: var(--font-sans);
  font-weight: 500;
  font-size: 1rem;
  padding: 1rem 0;
  border-bottom: 1px solid rgba(0, 49, 83, 0.1);
  transition: color 0.3s;
}

.mobile-nav .nav-link:hover {
  color: var(--color-cypress-green);
}

/* Hero Section */
.hero {
  position: relative;
  display: flex;
  min-height: 80vh;
  overflow: hidden;
  align-items: center;
  justify-content: flex-start;
}

.hero-bg.full-width {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center; /* Mặc định cho màn hình desktop (Căn giữa) */
  z-index: 1;
}

@media (max-width: 768px) {
  .hero-bg.full-width {
    /* Đẩy focus của ảnh nền sang bên phải để thấy rõ Pikachu Van Gogh trên điện thoại/Tablet */
    object-position: 80% center;
  }
}

.hero-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  /* Gradient mờ dần ngang cho desktop nhưng trên mobile nên xoay dọc để nội dung không che lấp Pikachu */
  background: linear-gradient(to right, rgba(0,0,0,0.85) 0%, rgba(0,0,0,0.4) 50%, rgba(0,0,0,0) 100%);
  z-index: 2;
}

@media (max-width: 768px) {
  .hero-overlay {
    background: linear-gradient(to bottom, rgba(0,0,0,0.9) 0%, rgba(0,0,0,0.7) 40%, rgba(0,0,0,0.2) 100%);
  }
}

.hero-content.full-width-content {
  position: relative;
  z-index: 3;
  width: 100%;
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 4rem 2rem;
  display: flex;
  background-color: transparent;
  flex: none;
}

@media (max-width: 768px) {
  .hero-content.full-width-content {
    padding: 2rem 1rem;
    align-items: center;
    justify-content: center;
    text-align: center;
  }
}

.hero-text-box.overlay-text {
  max-width: 550px;
  background-color: transparent;
  padding: 0;
  border-left: none;
  box-shadow: none;
}

@media (max-width: 768px) {
  .hero-text-box.overlay-text {
    max-width: 100%;
    margin-top: 2rem;
  }
}

.gold-text {
  color: #F5B041;
  font-family: var(--font-serif);
  text-transform: none;
  font-style: italic;
  letter-spacing: 0.5px;
  font-size: 1.1rem;
  margin-bottom: 0.5rem;
}

.white-text {
  color: #FFFFFF;
  margin-bottom: 1.25rem;
  font-size: clamp(2.5rem, 8vw, 5rem);
  line-height: 1;
  letter-spacing: -1px;
}

.white-text-muted {
  color: #E0E0E0;
  font-family: var(--font-sans);
  font-size: clamp(0.95rem, 3vw, 1.1rem);
  font-weight: 300;
  line-height: 1.6;
  margin-bottom: 2.5rem;
  padding-right: 1rem;
}

@media (max-width: 768px) {
  .white-text-muted {
    padding-right: 0;
  }
}

.hero-buttons {
  display: flex;
  gap: 1.25rem;
  margin-bottom: 3rem;
  flex-wrap: wrap;
}

@media (max-width: 768px) {
  .hero-buttons {
    justify-content: center;
    gap: 1rem;
    margin-bottom: 2rem;
  }
}

.rounded-pill {
  /* Organic, slightly imperfect hand-painted shape */
  border-radius: 255px 15px 225px 15px / 15px 225px 15px 255px;
  padding: 0.85rem 2.25rem;
  font-weight: 600;
  font-size: 1.05rem;
  font-family: var(--font-sans);
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  position: relative;
}

.btn-primary.rounded-pill {
  /* Rich, thick sunflower yellow with impasto feeling */
  background: linear-gradient(135deg, #E8A931 0%, #DDA74F 50%, #C2821B 100%);
  color: #2C1E04;
  border: 1px solid #B07212;
  box-shadow: 
    inset 1px 2px 3px rgba(255,255,255,0.4),
    2px 4px 8px rgba(0, 0, 0, 0.3);
}
.btn-primary.rounded-pill:hover {
  background: linear-gradient(135deg, #F4B341 0%, #E8A931 50%, #B07212 100%);
  transform: translateY(-3px) rotate(-1deg);
  box-shadow: 
    inset 1px 2px 3px rgba(255,255,255,0.5),
    4px 6px 12px rgba(0, 0, 0, 0.4);
}

.btn-secondary.outline-glass {
  /* Dark teal/green tint matching the starry background */
  background-color: rgba(11, 35, 45, 0.45);
  backdrop-filter: blur(6px);
  border: 2px solid rgba(221, 167, 79, 0.3); /* Muted gold brush-like outline */
  color: #E8A931;
  box-shadow: 2px 4px 8px rgba(0, 0, 0, 0.2);
}
.btn-secondary.outline-glass:hover {
  background-color: rgba(221, 167, 79, 0.15);
  border-color: rgba(221, 167, 79, 0.8);
  color: #FFF;
  transform: translateY(-3px) rotate(1deg);
  box-shadow: 4px 6px 12px rgba(0, 0, 0, 0.3);
}

.hero-controls {
  display: flex;
  gap: 1rem;
}

.round-dark {
  background-color: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(8px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  color: #FFFFFF;
  border-radius: 50%;
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.round-dark:hover {
  background-color: rgba(0, 0, 0, 0.7);
  border-color: rgba(255, 255, 255, 0.3);
  color: #FFFFFF;
}

.hero-dots {
  position: absolute;
  bottom: 2rem;
  right: 2rem;
  display: flex;
  gap: 0.5rem;
  z-index: 3;
}
@media (min-width: 1024px) {
  .hero-dots {
    right: calc((100vw - var(--container-max)) / 2 + 2rem);
  }
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: rgba(255,255,255,0.4);
  cursor: pointer;
  transition: background-color 0.2s;
}
.dot.active {
  background-color: #FFFFFF;
}

/* What's New Separator */
.whats-new-section {
  background-color: var(--color-linen);
  padding: clamp(3rem, 6vw, 5rem) 2rem clamp(2rem, 5vw, 4rem);
  text-align: center;
  display: flex;
  justify-content: center;
  align-items: center;
}

.whats-new-text {
  font-family: var(--font-serif);
  font-size: clamp(2rem, 5vw, 3rem);
  color: var(--color-prussian-blue);
  font-style: italic;
  font-weight: 600;
  position: relative;
  letter-spacing: -0.5px;
}

.whats-new-text::after {
  content: '';
  display: block;
  width: 60px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 1.5rem auto 0;
  border-radius: 2px;
}

/* News Banner Auto-Slider Section */
.news-banner-section {
  background-color: #0b151c; /* Match deep dark gradient tail from hero */
  padding: clamp(3rem, 6vw, 5rem) 2rem;
  text-align: center;
  position: relative;
  overflow: hidden;
}

.news-banner-background {
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  /* Add subtle canvas texture mimicking the hero's painted vibe */
  background-image: 
    radial-gradient(circle at center, rgba(245, 176, 65, 0.08) 0%, transparent 60%),
    linear-gradient(90deg, rgba(200, 195, 180, 0.03) 50%, transparent 50%),
    linear-gradient(rgba(200, 195, 180, 0.03) 50%, transparent 50%);
  background-size: 100% 100%, 4px 4px, 4px 4px;
  pointer-events: none;
}

.news-banner-container {
  max-width: 900px;
  margin: 0 auto;
  min-height: 200px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  position: relative;
  z-index: 2;
}

.news-banner-content {
  width: 100%;
}

.news-banner-content .gold-text {
  display: inline-block;
  margin-bottom: 1.5rem;
  font-size: 1.15rem;
  opacity: 0.9;
}

.news-banner-title {
  font-size: 2.75rem;
  color: #FFFFFF;
  line-height: 1.25;
  margin-bottom: 1.5rem;
  font-family: var(--font-serif);
  font-weight: 300;
  letter-spacing: -0.5px;
}

.news-banner-content .white-text-muted {
  padding: 0;
  margin-bottom: 0;
  font-size: 1rem;
  text-transform: uppercase;
  letter-spacing: 2px;
}

.news-banner-indicators {
  display: flex;
  gap: 0.75rem;
  margin-top: 3rem;
  justify-content: center;
}

.banner-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.3);
  cursor: pointer;
  transition: all 0.3s ease;
}

.banner-dot.active {
  background-color: #FFFFFF;
  transform: scale(1.2);
}

/* Vue Slide Transition */
.slide-fade-enter-active {
  transition: all 0.5s ease-out;
}
.slide-fade-leave-active {
  transition: all 0.5s ease-in;
}
.slide-fade-enter-from {
  transform: translateX(40px);
  opacity: 0;
}
.slide-fade-leave-to {
  transform: translateX(-40px);
  opacity: 0;
}

/* Shop Section */
.shop-section {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 6rem 2rem;
}

.section-title {
  text-align: center;
  font-size: 2.5rem;
  margin-bottom: 3rem;
  position: relative;
}

.section-title::after {
  content: '';
  display: block;
  width: 80px;
  height: 4px;
  background-color: var(--color-cypress-green);
  margin: 1rem auto 0;
  border-radius: 2px;
}

/* Categories/Filters */
.categories {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 1rem;
  margin-bottom: 4rem;
}

.category-btn {
  background: transparent;
  border: 1px solid rgba(0, 49, 83, 0.2);
  padding: 0.75rem 1.5rem;
  border-radius: 30px;
  font-family: var(--font-sans);
  font-weight: 500;
  color: var(--color-prussian-blue);
  cursor: pointer;
  transition: all 0.2s ease;
}

.category-btn:hover {
  border-color: var(--color-prussian-blue);
}

.category-btn.active {
  background-color: var(--color-prussian-blue);
  color: var(--color-linen);
  border-color: var(--color-prussian-blue);
}

/* Product Grid */
.product-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* Hiển thị 2 items trên mobile */
  gap: 1.5rem 1rem;
  margin-bottom: 5rem;
}

@media (min-width: 640px) {
  .product-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 3rem 2rem;
  }
}

@media (min-width: 1024px) {
  .product-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

.product-card {
  group: product;
  display: flex;
  flex-direction: column;
}

.product-image-container {
  position: relative;
  aspect-ratio: 4/5;
  overflow: hidden;
  border-radius: 8px;
  background-color: #E8E5D8;
  margin-bottom: 1.25rem;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
}

.product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.6s ease;
}

.product-card:hover .product-image {
  transform: scale(1.05);
}

.add-to-cart-quick {
  position: absolute;
  bottom: 1rem;
  right: 1rem;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  border: none;
  width: 44px;
  height: 44px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  opacity: 0;
  transform: translateY(10px);
  transition: all 0.3s ease;
  box-shadow: 0 4px 10px rgba(0,0,0,0.15);
}

.product-card:hover .add-to-cart-quick {
  opacity: 1;
  transform: translateY(0);
}

.add-to-cart-quick:hover {
  background-color: var(--color-prussian-blue);
  color: var(--color-sunflower-yellow);
}

.add-to-cart-quick svg {
  width: 20px;
  height: 20px;
}

.product-title {
  font-size: clamp(0.95rem, 2.5vw, 1.25rem);
  margin-bottom: 0.5rem;
  transition: color 0.2s;
  line-height: 1.3;
}

.product-card:hover .product-title {
  color: var(--color-cypress-green);
}

.product-price {
  font-family: var(--font-sans);
  color: var(--color-cypress-green);
  font-weight: 600;
  font-size: clamp(1rem, 2vw, 1.1rem);
}

.load-more-container {
  text-align: center;
}

/* Footer */
.footer {
  background-color: #0b151c; /* Match deep dark theme from News banner */
  color: var(--color-linen);
  margin-top: auto;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  position: relative;
  overflow: hidden;
}

/* Subtle textured background */
.footer::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background-image: 
    radial-gradient(circle at top center, rgba(245, 176, 65, 0.03) 0%, transparent 60%),
    linear-gradient(90deg, rgba(200, 195, 180, 0.02) 50%, transparent 50%),
    linear-gradient(rgba(200, 195, 180, 0.02) 50%, transparent 50%);
  background-size: 100% 100%, 4px 4px, 4px 4px;
  pointer-events: none;
}

.footer-container {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: 5rem 2rem 2rem;
  position: relative;
  z-index: 1;
}

.footer-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 3rem;
  margin-bottom: 4rem;
}

@media (min-width: 768px) {
  .footer-grid {
    grid-template-columns: 2fr 1fr 1fr 1.5fr;
    gap: 2rem;
  }
}

.footer-logo {
  font-family: var(--font-serif);
  font-size: 2.5rem;
  color: #FFFFFF;
  margin-bottom: 1rem;
  letter-spacing: -0.5px;
}

.footer-desc {
  color: #A0B0C0;
  font-size: 0.95rem;
  line-height: 1.6;
  max-width: 300px;
}

.footer-heading {
  font-family: var(--font-sans);
  color: #F5B041;
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 2px;
  margin-bottom: 1.5rem;
  font-weight: 600;
}

.footer-links nav {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
}

.footer-links a {
  color: #E0E0E0;
  text-decoration: none;
  font-size: 0.95rem;
  transition: color 0.2s ease;
}

.footer-links a:hover {
  color: #F5B041;
}

.footer-newsletter p {
  color: #E0E0E0;
  font-size: 0.95rem;
  margin-bottom: 1.5rem;
  line-height: 1.5;
}

.subscribe-form {
  display: flex;
  border-bottom: 1px solid rgba(255, 255, 255, 0.3);
  padding-bottom: 0.5rem;
  transition: border-color 0.3s ease;
}

.subscribe-form:focus-within {
  border-color: #F5B041;
}

.subscribe-form input {
  background: transparent;
  border: none;
  color: #FFFFFF;
  width: 100%;
  font-family: var(--font-sans);
  font-size: 0.95rem;
}

.subscribe-form input:focus {
  outline: none;
}

.subscribe-form input::placeholder {
  color: #888;
}

.subscribe-form button {
  background: transparent;
  border: none;
  color: #F5B041;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.2s ease;
}

.subscribe-form button:hover {
  transform: translateX(4px);
}

.footer-bottom {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding-top: 2rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  color: #888;
  font-family: var(--font-sans);
  font-size: 0.85rem;
}

@media (min-width: 768px) {
  .footer-bottom {
    flex-direction: row;
    justify-content: space-between;
  }
}

.footer-legal {
  display: flex;
  gap: 1.5rem;
}

.footer-legal a {
  color: #888;
  text-decoration: none;
  transition: color 0.2s ease;
}

.footer-legal a:hover {
  color: #E0E0E0;
}

/* Mobile responsive adjustments */
@media (max-width: 768px) {
  .hero-text-box {
    padding: 2rem;
  }
  
  .hero-title {
    font-size: 2.25rem;
  }
}
</style>

