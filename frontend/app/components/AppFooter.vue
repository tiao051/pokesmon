<template>
  <footer class="footer">
    <div class="footer-container">
      <div class="footer-grid">
        <div class="footer-brand">
          <h2 class="footer-logo">PokéGogh</h2>
          <p class="footer-desc">A post-impressionist journey through the Pokémon universe, capturing the electric essence of the classics.</p>
        </div>
        
        <div class="footer-links">
          <h4 class="footer-heading">Collection</h4>
          <nav>
            <a href="#">Trading Cards</a>
            <a href="#">Elite Trainer Boxes</a>
            <a href="#">Booster Packs</a>
            <a href="#">Exclusive Prints</a>
          </nav>
        </div>
        
        <div class="footer-links">
          <h4 class="footer-heading">Museum</h4>
          <nav>
            <a href="#">About the Artist</a>
            <a href="#">Exhibitions</a>
            <a href="#">FAQ & Support</a>
            <a href="#">Contact Us</a>
          </nav>
        </div>

        <div class="footer-newsletter">
          <h4 class="footer-heading">Stay Inspired</h4>
          <p>Join our newsletter for the latest TCG acquisitions and exhibition updates.</p>
          <form class="subscribe-form" @submit.prevent>
            <input type="email" placeholder="Your email address" required />
            <button type="submit" aria-label="Subscribe">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>
            </button>
          </form>
        </div>
      </div>
      
      <div class="footer-bottom">
        <p>© 2026 PokéGogh Museum Collection | Inspired by Vincent van Gogh</p>
        <div class="footer-legal">
          <a href="#">Privacy Policy</a>
          <a href="#">Terms of Service</a>
        </div>
      </div>
    </div>
  </footer>
</template>
