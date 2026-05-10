<template>
	<footer class="footer">
		<div class="footer-container">
			<div class="footer-grid">
				<div class="footer-brand">
					<h2 class="footer-logo">PokéGogh</h2>
					<p class="footer-desc">
						A post-impressionist take on the Pokémon universe,
						bringing classic cards to life.
					</p>
				</div>

				<div class="footer-links">
					<h4 class="footer-heading">Shop</h4>
					<nav>
						<a href="#">Trading Cards</a>
						<a href="#">Elite Trainer Boxes</a>
						<a href="#">Booster Packs</a>
						<a href="#">Exclusive Prints</a>
					</nav>
				</div>

				<div class="footer-links">
					<h4 class="footer-heading">About</h4>
					<nav>
						<a href="#">Our Story</a>
						<a href="#">Events</a>
						<a href="#">FAQ & Support</a>
						<a href="#">Contact Us</a>
					</nav>
				</div>

				<div class="footer-newsletter">
					<h4 class="footer-heading">Stay in Touch</h4>
					<p>
						Sign up for news on new arrivals and special drops.
					</p>
					<form class="subscribe-form" @submit.prevent>
						<input
							type="email"
							placeholder="Your email address"
							required
						/>
						<button type="submit" aria-label="Subscribe">
							<svg
								width="20"
								height="20"
								viewBox="0 0 24 24"
								fill="none"
								stroke="currentColor"
								stroke-width="2"
								stroke-linecap="round"
								stroke-linejoin="round"
							>
								<line x1="5" y1="12" x2="19" y2="12"></line>
								<polyline points="12 5 19 12 12 19"></polyline>
							</svg>
						</button>
					</form>
				</div>
			</div>

			<div class="footer-bottom">
				<p>
					© 2026 PokéGogh | Inspired by Vincent van Gogh
				</p>
				<div class="footer-legal">
					<NuxtLink to="/privacy">Privacy Policy</NuxtLink>
					<NuxtLink to="/terms">Terms of Service</NuxtLink>
				</div>
			</div>
		</div>
	</footer>
</template>
