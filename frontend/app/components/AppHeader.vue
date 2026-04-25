<script setup>
import { ref } from 'vue'

const isMobileMenuOpen = ref(false)
const { count } = useCart()
const { isLoggedIn, signOut } = useAuth()

const toggleMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}

const closeMenu = () => {
  isMobileMenuOpen.value = false
}

const handleSignOut = () => {
  signOut()
  closeMenu()
}
</script>

<template>
  <header class="header">
    <div class="header-container">
      <NuxtLink to="/" class="logo-link">
        <div class="logo">
          <h1>PokéGogh</h1>
        </div>
      </NuxtLink>

      <!-- Middle Search Bar -->
      <div class="header-search">
        <input type="text" placeholder="Search Pikachu, Plush, T-Shirts..." class="search-input" />
        <button class="search-btn" aria-label="Search">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
        </button>
      </div>

      <!-- Right Actions -->
      <div class="header-actions">
        <NuxtLink v-if="!isLoggedIn" to="/login" class="action-link user-action">
          <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 4.5c1.93 0 3.5 1.57 3.5 3.5S13.93 13.5 12 13.5 8.5 11.93 8.5 10 10.07 6.5 12 6.5zm0 13c-2.65 0-5-1.28-6.5-3.23.04-2.15 4.33-3.34 6.5-3.34s6.46 1.19 6.5 3.34c-1.5 1.95-3.85 3.23-6.5 3.23z"/></svg>
          <span class="action-text">SIGN IN / REGISTER</span>
        </NuxtLink>
        <button v-else class="action-link user-action user-action-button" @click="handleSignOut">
          <svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 4.5c1.93 0 3.5 1.57 3.5 3.5S13.93 13.5 12 13.5 8.5 11.93 8.5 10 10.07 6.5 12 6.5zm0 13c-2.65 0-5-1.28-6.5-3.23.04-2.15 4.33-3.34 6.5-3.34s6.46 1.19 6.5 3.34c-1.5 1.95-3.85 3.23-6.5 3.23z"/></svg>
          <span class="action-text">SIGN OUT</span>
        </button>

        <NuxtLink to="/cart" class="action-link cart-action">
          <span class="cart-icon-wrapper">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="21" r="2"></circle><circle cx="20" cy="21" r="2"></circle><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path></svg>
            <span v-if="count > 0" class="cart-badge">{{ count }}</span>
          </span>
          <span class="action-text">MY CART</span>
        </NuxtLink>

        <button class="icon-btn mobile-menu-btn" @click="toggleMenu" aria-label="Menu">
          <svg v-if="!isMobileMenuOpen" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="3" y1="12" x2="21" y2="12"></line><line x1="3" y1="6" x2="21" y2="6"></line><line x1="3" y1="18" x2="21" y2="18"></line></svg>
          <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
        </button>
      </div>
    </div>

    <!-- Mobile Nav -->
    <nav class="mobile-nav" :class="{ 'open': isMobileMenuOpen }">
      <div class="mobile-search">
        <input type="text" placeholder="Search Pikachu..." class="search-input" />
        <button class="search-btn"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg></button>
      </div>
      <NuxtLink to="/products" class="nav-link" @click="closeMenu">Browse The Collection</NuxtLink>
      <NuxtLink v-if="!isLoggedIn" to="/login" class="nav-link" @click="closeMenu">Sign In / Register</NuxtLink>
      <button v-else class="nav-link nav-link-button" @click="handleSignOut">Sign Out</button>
      <NuxtLink to="/cart" class="nav-link" @click="closeMenu">
        My Cart <span v-if="count > 0" class="cart-badge cart-badge-mobile">{{ count }}</span>
      </NuxtLink>
    </nav>
  </header>
</template>

<style scoped>
.logo-link {
  display: inline-flex;
  text-decoration: none;
  color: inherit;
  flex: none;
}

.cart-icon-wrapper {
  position: relative;
  display: inline-flex;
}

.cart-badge {
  position: absolute;
  top: -8px;
  right: -10px;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  border: 2px solid var(--color-linen);
  border-radius: 50%;
  min-width: 22px;
  height: 22px;
  font-size: 0.72rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 5px;
  font-family: var(--font-sans);
  line-height: 1;
  box-shadow: 0 2px 4px rgba(0, 49, 83, 0.25);
}

.user-action-button,
.nav-link-button {
  background: none;
  border: none;
  cursor: pointer;
  font: inherit;
  color: inherit;
  text-align: left;
  padding: 0;
}

.nav-link-button {
  padding: 1rem 0;
  border-bottom: 1px solid rgba(0, 49, 83, 0.1);
  width: 100%;
  font-family: var(--font-sans);
  font-weight: 500;
  font-size: 1rem;
  color: var(--color-prussian-blue);
}

.nav-link-button:hover {
  color: var(--color-cypress-green);
}

.cart-badge-mobile {
  position: static;
  margin-left: 0.5rem;
  border: none;
  box-shadow: none;
}
</style>
