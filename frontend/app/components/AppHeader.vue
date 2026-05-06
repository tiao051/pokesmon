<script setup lang="ts">
	import { ref, computed, watch } from 'vue'
	import { useRouter } from 'vue-router'
	import { productService } from '../services/productService'
	import { useSearchHistory } from '../composables/useSearchHistory'
	import { useCart } from '../composables/useCart'
	import { useAuth } from '../composables/useAuth'
	import type { Product } from '../types/product'

	const router = useRouter()
	const isMobileMenuOpen = ref(false)
	const { count } = useCart()
	const { isLoggedIn, username, signOut } = useAuth()
	const { history, addSearchTerm } = useSearchHistory()

	const searchQuery = ref('')
	const isSearchFocused = ref(false)
	const isInputFocused = ref(false)
	const suggestionResults = ref<Product[]>([])
	let suggestionTimer: ReturnType<typeof setTimeout> | null = null

	const onSearchFocusIn = () => {
	  isInputFocused.value = true
	  isSearchFocused.value = true
	}

	const onSearchFocusOut = () => {
	  setTimeout(() => {
	    isInputFocused.value = false
	    isSearchFocused.value = false
	  }, 200)
	}

	const onSearchMouseEnter = () => {
	  if (isInputFocused.value) isSearchFocused.value = true
	}

	const onSearchMouseLeave = () => {
	  isSearchFocused.value = false
	}

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

	const executeSearch = (term: string | Event) => {
	  const query = (typeof term === 'string' ? term : searchQuery.value || '').trim()
	  if (!query) return
	  addSearchTerm(query)
	  searchQuery.value = query
	  isSearchFocused.value = false
	  closeMenu()
	  router.push({ path: '/search', query: { q: query } })
	}

	watch(searchQuery, (q) => {
	  if (suggestionTimer) clearTimeout(suggestionTimer)
	  const trimmed = q.trim()
	  if (!trimmed) {
	    suggestionResults.value = []
	    return
	  }
	  suggestionTimer = setTimeout(async () => {
	    try {
	      const data = await productService.getProducts({ search: trimmed, limit: 5 })
	      suggestionResults.value = data?.items ?? []
	    } catch {
	      suggestionResults.value = []
	    }
	  }, 250)
	})

	const suggestions = computed(() => suggestionResults.value)
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
			<div
				class="header-search"
				@focusin="onSearchFocusIn"
				@focusout="onSearchFocusOut"
				@mouseenter="onSearchMouseEnter"
				@mouseleave="onSearchMouseLeave"
			>
				<input
					type="text"
					v-model="searchQuery"
					@keyup.enter="executeSearch(searchQuery)"
					placeholder="Search Pikachu, Plush, T-Shirts..."
					class="search-input"
				/>
				<button
					class="search-btn"
					aria-label="Search"
					@click="executeSearch(searchQuery)"
				>
					<svg
						viewBox="0 0 24 24"
						fill="none"
						stroke="currentColor"
						stroke-width="2"
						stroke-linecap="round"
						stroke-linejoin="round"
					>
						<circle cx="11" cy="11" r="8"></circle>
						<line x1="21" y1="21" x2="16.65" y2="16.65"></line>
					</svg>
				</button>
				<div
					v-if="
						isSearchFocused &&
						(history.length > 0 || searchQuery.trim())
					"
					class="search-dropdown"
				>
					<div
						v-if="!searchQuery.trim() && history.length > 0"
						class="search-history"
					>
						<div class="search-dropdown-header">
							Recent Searches
						</div>
						<div
							v-for="item in (history as string[])"
							:key="item"
							class="search-dropdown-item"
							@click="executeSearch(item)"
						>
							<svg
								viewBox="0 0 24 24"
								fill="none"
								stroke="currentColor"
								stroke-width="2"
								class="history-icon"
							>
								<circle cx="12" cy="12" r="10"></circle>
								<polyline points="12 6 12 12 16 14"></polyline>
							</svg>
							{{ item }}
						</div>
					</div>
					<div
						v-else-if="searchQuery.trim() && suggestions.length > 0"
						class="search-suggestions"
					>
						<div class="search-dropdown-header">Products</div>
						<NuxtLink
							v-for="prod in suggestions"
							:key="prod.id"
							:to="`/products/${prod.slug}`"
							class="search-dropdown-item product-suggestion"
							@click="isSearchFocused = false"
						>
							<img
								:src="prod.image"
								:alt="prod.title"
								class="suggestion-img"
							/>
							<div class="suggestion-info">
								<div class="suggestion-title">
									{{ prod.title }}
								</div>
								<div class="suggestion-price">
									${{ prod.price.toFixed(2) }}
								</div>
							</div>
						</NuxtLink>
					</div>
					<div
						v-else-if="
							searchQuery.trim() && suggestions.length === 0
						"
						class="search-dropdown-item no-results"
					>
						No results found for "{{ searchQuery }}"
					</div>
				</div>
			</div>

			<!-- Right Actions -->
			<div class="header-actions">
				<NuxtLink to="/cart" class="action-link cart-action">
					<span class="cart-icon-wrapper">
						<svg
							viewBox="0 0 24 24"
							fill="none"
							stroke="currentColor"
							stroke-width="2"
							stroke-linecap="round"
							stroke-linejoin="round"
						>
							<circle cx="9" cy="21" r="2"></circle>
							<circle cx="20" cy="21" r="2"></circle>
							<path
								d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"
							></path>
						</svg>
						<span v-if="count > 0" class="cart-badge">{{
							count
						}}</span>
					</span>
					<span class="action-text">MY CART</span>
				</NuxtLink>

				<NuxtLink
					v-if="!isLoggedIn"
					to="/login"
					class="action-link user-action"
				>
					<svg viewBox="0 0 24 24" fill="currentColor">
						<path
							d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 4.5c1.93 0 3.5 1.57 3.5 3.5S13.93 13.5 12 13.5 8.5 11.93 8.5 10 10.07 6.5 12 6.5zm0 13c-2.65 0-5-1.28-6.5-3.23.04-2.15 4.33-3.34 6.5-3.34s6.46 1.19 6.5 3.34c-1.5 1.95-3.85 3.23-6.5 3.23z"
						/>
					</svg>
					<span class="action-text">SIGN IN / REGISTER</span>
				</NuxtLink>
				<div v-else class="user-menu">
					<div
						class="action-link user-action user-menu-trigger"
						tabindex="0"
						role="button"
						aria-haspopup="true"
					>
						<svg viewBox="0 0 24 24" fill="currentColor">
							<path
								d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 4.5c1.93 0 3.5 1.57 3.5 3.5S13.93 13.5 12 13.5 8.5 11.93 8.5 10 10.07 6.5 12 6.5zm0 13c-2.65 0-5-1.28-6.5-3.23.04-2.15 4.33-3.34 6.5-3.34s6.46 1.19 6.5 3.34c-1.5 1.95-3.85 3.23-6.5 3.23z"
							/>
						</svg>
						<span v-if="username" class="user-name">{{
							username
						}}</span>
					</div>
					<div class="user-dropdown" role="menu">
						<NuxtLink
							to="/account"
							class="user-dropdown-item"
							role="menuitem"
							>My Profile</NuxtLink
						>
						<button
							type="button"
							class="user-dropdown-item"
							@click="handleSignOut"
						>
							Sign Out
						</button>
					</div>
				</div>

				<button
					class="icon-btn mobile-menu-btn"
					@click="toggleMenu"
					aria-label="Menu"
				>
					<svg
						v-if="!isMobileMenuOpen"
						viewBox="0 0 24 24"
						fill="none"
						stroke="currentColor"
						stroke-width="2"
						stroke-linecap="round"
						stroke-linejoin="round"
					>
						<line x1="3" y1="12" x2="21" y2="12"></line>
						<line x1="3" y1="6" x2="21" y2="6"></line>
						<line x1="3" y1="18" x2="21" y2="18"></line>
					</svg>
					<svg
						v-else
						viewBox="0 0 24 24"
						fill="none"
						stroke="currentColor"
						stroke-width="2"
						stroke-linecap="round"
						stroke-linejoin="round"
					>
						<line x1="18" y1="6" x2="6" y2="18"></line>
						<line x1="6" y1="6" x2="18" y2="18"></line>
					</svg>
				</button>
			</div>
		</div>

		<!-- Mobile Nav -->
		<nav class="mobile-nav" :class="{open: isMobileMenuOpen}">
			<div class="mobile-search">
				<input
					type="text"
					v-model="searchQuery"
					@keyup.enter="executeSearch(searchQuery)"
					placeholder="Search Pikachu..."
					class="search-input"
				/>
				<button class="search-btn" @click="executeSearch(searchQuery)">
					<svg
						viewBox="0 0 24 24"
						fill="none"
						stroke="currentColor"
						stroke-width="2"
					>
						<circle cx="11" cy="11" r="8"></circle>
						<line x1="21" y1="21" x2="16.65" y2="16.65"></line>
					</svg>
				</button>
			</div>
			<NuxtLink to="/products" class="nav-link" @click="closeMenu"
				>Browse The Collection</NuxtLink
			>
			<NuxtLink
				v-if="!isLoggedIn"
				to="/login"
				class="nav-link"
				@click="closeMenu"
				>Sign In / Register</NuxtLink
			>
			<template v-else>
				<NuxtLink to="/account" class="nav-link" @click="closeMenu"
					>My Profile</NuxtLink
				>
				<button class="nav-link nav-link-button" @click="handleSignOut">
					Sign Out
				</button>
			</template>
			<NuxtLink to="/cart" class="nav-link" @click="closeMenu">
				My Cart
				<span v-if="count > 0" class="cart-badge cart-badge-mobile">{{
					count
				}}</span>
			</NuxtLink>
			<NuxtLink
				v-if="!isLoggedIn"
				to="/login"
				class="nav-link"
				@click="closeMenu"
				>Sign In / Register</NuxtLink
			>
			<button
				v-else
				class="nav-link nav-link-button"
				@click="handleSignOut"
			>
				Sign Out
			</button>
		</nav>
	</header>
</template>

<style scoped>
	.logo-link {
		display: inline-flex;
		text-decoration: none;
		color: inherit;
		flex: 1;
		justify-content: flex-start;
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

	.nav-link-button {
		background: none;
		border: none;
		cursor: pointer;
		font: inherit;
		color: inherit;
		text-align: left;
		padding: 0;
	}

	.user-menu {
		position: relative;
		display: none;
	}

	@media (min-width: 768px) {
		.user-menu {
			display: block;
		}
	}

	.user-menu-trigger {
		cursor: pointer;
	}

	.user-menu-trigger svg {
		width: 44px;
		height: 44px;
	}

	.user-name {
		font-family: var(--font-sans);
		font-weight: 500;
		font-size: 0.9rem;
		color: var(--color-prussian-blue);
		letter-spacing: 0.2px;
	}

	.user-dropdown {
		position: absolute;
		top: 100%;
		right: 0;
		min-width: 150px;
		margin-top: 0.5rem;
		background-color: var(--color-linen);
		border: 1.5px solid rgba(0, 49, 83, 0.2);
		border-radius: 12px;
		box-shadow: 0 6px 18px rgba(0, 49, 83, 0.12);
		padding: 0.4rem;
		opacity: 0;
		visibility: hidden;
		transform: translateY(-4px);
		transition:
			opacity 0.2s ease,
			transform 0.2s ease,
			visibility 0s linear 0.2s;
		z-index: 20;
	}

	.user-menu:hover .user-dropdown,
	.user-menu:focus-within .user-dropdown {
		opacity: 1;
		visibility: visible;
		transform: translateY(0);
		transition:
			opacity 0.2s ease,
			transform 0.2s ease,
			visibility 0s;
	}

	.user-dropdown-item {
		display: block;
		width: 100%;
		background: none;
		border: none;
		padding: 0.55rem 0.85rem;
		font-family: var(--font-sans);
		font-size: 0.9rem;
		font-weight: 500;
		color: var(--color-prussian-blue);
		text-align: left;
		text-decoration: none;
		cursor: pointer;
		border-radius: 8px;
		transition:
			background-color 0.15s ease,
			color 0.15s ease;
	}

	.user-dropdown-item:hover {
		background-color: rgba(0, 49, 83, 0.06);
		color: var(--color-cypress-green);
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
		border: none;
		box-shadow: none;
	}

	.search-dropdown {
		position: absolute;
		top: 100%;
		left: 0;
		right: 0;
		background: white;
		border: 1px solid rgba(0, 49, 83, 0.1);
		border-top: none;
		box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
		z-index: 1000;
		max-height: 400px;
		overflow-y: auto;
	}

	.search-dropdown-header {
		padding: 0.5rem 1rem;
		font-size: 0.8rem;
		text-transform: uppercase;
		color: #888;
		background: #f9f9f9;
		font-family: var(--font-sans);
	}

	.search-dropdown-item {
		padding: 0.75rem 1rem;
		display: flex;
		align-items: center;
		gap: 0.75rem;
		cursor: pointer;
		text-decoration: none;
		color: var(--color-prussian-blue);
		font-family: var(--font-sans);
		border-bottom: 1px solid #eee;
		transition: background-color 0.2s;
	}

	.search-dropdown-item:hover {
		background-color: rgba(0, 49, 83, 0.05);
	}

	.search-dropdown-item:last-child {
		border-bottom: none;
	}

	.history-icon {
		width: 16px;
		height: 16px;
		color: #888;
	}

	.suggestion-img {
		width: 40px;
		height: 40px;
		object-fit: cover;
		border-radius: 4px;
	}

	.suggestion-info {
		display: flex;
		flex-direction: column;
	}

	.suggestion-title {
		font-size: 0.9rem;
		font-weight: 500;
	}

	.suggestion-price {
		font-size: 0.8rem;
		color: var(--color-cypress-green);
	}

	.no-results {
		color: #888;
		cursor: default;
		justify-content: center;
	}
	.no-results:hover {
		background-color: white;
	}
</style>
