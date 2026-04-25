<script setup>
import { ref, reactive } from 'vue'

definePageMeta({
  layout: 'auth'
})

const route = useRoute()
const router = useRouter()
const { signIn } = useAuth()

const isLogin = ref(true)

const form = reactive({
  email: '',
  password: '',
  confirmPassword: '',
})

const toggleAuthMode = () => {
  isLogin.value = !isLogin.value
}

const DEMO_EMAIL = 'curator@pokegogh.museum'
const DEMO_PASSWORD = 'starrynight'

const fillDemo = () => {
  form.email = DEMO_EMAIL
  form.password = DEMO_PASSWORD
  if (!isLogin.value) form.confirmPassword = DEMO_PASSWORD
}

const handleSubmit = () => {
  signIn(form.email)
  const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/'
  router.push(redirect)
}
</script>

<template>
  <div class="auth-page-wrapper">
    <div class="auth-split-layout">
      <!-- Left Side: Image -->
      <div class="auth-image-side">
        <!-- Separate divs for opacity transition to avoid background-image lag -->
        <div class="bg-layer login-bg" :class="{ 'active': isLogin }"></div>
        <div class="bg-layer register-bg" :class="{ 'active': !isLogin }"></div>
        
        <NuxtLink to="/" class="back-home-btn">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="19" y1="12" x2="5" y2="12"></line><polyline points="12 19 5 12 12 5"></polyline></svg>
          <span>Back to Museum</span>
        </NuxtLink>
      </div>

      <!-- Right Side: Form -->
      <div class="auth-form-side">
        <div class="form-wrapper">
          <div class="auth-header-icon">
            <img src="/images/pokeball.jpg" alt="Pokeball" class="pokeball-icon" />
          </div>
          <h1 class="auth-title">{{ isLogin ? 'Sign In' : 'Register' }}</h1>
          <p class="auth-subtitle" v-if="isLogin">Welcome Back, Trainer!</p>
          <p class="auth-subtitle" v-else>Start Your Journey!</p>
          
          <div v-if="isLogin" class="demo-credentials">
            <span class="demo-eyebrow">— for the curious patron —</span>
            <div class="demo-rows">
              <div class="demo-row">
                <span class="demo-label">Email</span>
                <span class="demo-value">{{ DEMO_EMAIL }}</span>
              </div>
              <div class="demo-row">
                <span class="demo-label">Password</span>
                <span class="demo-value">{{ DEMO_PASSWORD }}</span>
              </div>
            </div>
            <button type="button" class="demo-fill-btn" @click="fillDemo">Use demo patron</button>
          </div>

          <form class="auth-form" @submit.prevent="handleSubmit">
            <div class="form-group">
              <label for="email">Email Address</label>
              <input id="email" v-model="form.email" type="email" placeholder="pikachu@thi-tran-pallet.vn" required />
            </div>

            <div class="form-group">
              <div class="label-row">
                <label for="password">Password</label>
                <a v-if="isLogin" href="#" class="forgot-pass">Forgot password?</a>
              </div>
              <input id="password" v-model="form.password" type="password" placeholder="••••••••" required />
            </div>

            <div v-if="!isLogin" class="form-group">
              <label for="confirm-password">Confirm Password</label>
              <input id="confirm-password" v-model="form.confirmPassword" type="password" placeholder="••••••••" required />
            </div>

            <button type="submit" class="auth-submit-btn">
              {{ isLogin ? 'Sign In' : 'Create Account' }}
            </button>
          </form>

          <div class="auth-divider">
            <span>or continue with</span>
          </div>

          <div class="social-logins">
            <button type="button" class="social-btn google-btn">
              <svg viewBox="0 0 24 24" fill="currentColor" width="20" height="20">
                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
              </svg>
              <span>Google</span>
            </button>
            <button type="button" class="social-btn facebook-btn">
              <svg viewBox="0 0 24 24" fill="currentColor" width="20" height="20">
                <path d="M22 12c0-5.52-4.48-10-10-10S2 6.48 2 12c0 4.99 3.66 9.13 8.44 9.88v-6.99h-2.54V12h2.54V9.8c0-2.51 1.49-3.89 3.78-3.89 1.09 0 2.24.2 2.24.2v2.46h-1.26c-1.24 0-1.63.77-1.63 1.56V12h2.77l-.44 2.89h-2.33v6.99C18.34 21.13 22 16.99 22 12z" fill="#1877F2"/>
              </svg>
              <span>Facebook</span>
            </button>
          </div>

          <div class="auth-switch">
            <p v-if="isLogin">
              Don't have an account? 
              <button type="button" class="switch-btn" @click="toggleAuthMode">Register here</button>
            </p>
            <p v-else>
              Already have an account? 
              <button type="button" class="switch-btn" @click="toggleAuthMode">Sign in</button>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-page-wrapper {
  min-height: 100vh;
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #E8E8E8;
  background-image: radial-gradient(circle at 50% 50%, rgba(0, 49, 83, 0.05) 0%, transparent 80%);
  padding: clamp(1rem, 2vh, 2rem);
  width: 100%;
  box-sizing: border-box;
  overflow: hidden; /* Ngăn chặn cuộn ngang/dọc không mong muốn trên wrapper */
}

.auth-split-layout {
  display: flex;
  width: 100%;
  max-width: 1100px;
  /* Set cố định height hoặc min-height bằng clamp để cả Login và Register đều dùng chung một bộ khung không thay đổi kích thước */
  height: clamp(600px, 85vh, 750px);
  background: var(--color-linen);
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 20px 40px rgba(0, 49, 83, 0.1);
  box-sizing: border-box;
}

/* Left Image Side */
.auth-image-side {
  display: none; /* Hide on mobile */
  position: relative;
  min-height: 400px;
  overflow: hidden;
}

/* Background layers for smooth opacity transition */
.bg-layer {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: cover;
  background-position: center;
  opacity: 0;
  transition: opacity 0.5s ease-in-out;
  z-index: 1;
}

.bg-layer.active {
  opacity: 1;
  z-index: 2;
}

.login-bg {
  background-image: url('/images/login.webp');
}

.register-bg {
  background-image: url('/images/register.jpeg');
}

@media (min-width: 900px) {
  .auth-image-side {
    display: block;
    flex: 1; /* Thay vì 1.2, set về 1 để chia đều 50-50 với form-side */
    width: 50%;
  }
}

.back-home-btn {
  position: absolute;
  top: clamp(1rem, 3vh, 2rem);
  left: clamp(1rem, 3vh, 2rem);
  z-index: 10;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-linen);
  text-decoration: none;
  font-family: var(--font-sans);
  font-weight: 600;
  padding: 0.5rem 1rem;
  font-size: clamp(0.85rem, 2vh, 1rem);
  border-radius: 25px;
  background: rgba(0, 49, 83, 0.7);
  backdrop-filter: blur(5px);
  transition: all 0.3s ease;
}

.back-home-btn:hover {
  background: var(--color-sunflower);
  color: var(--color-prussian-blue);
}

.back-home-btn svg {
  width: 18px;
  height: 18px;
}

/* Right Form Side */
.auth-form-side {
  flex: 1; /* Chia đều 50% với bên image-side */
  width: 50%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: var(--color-linen);
  padding: clamp(1rem, 3vh, 3rem) clamp(1rem, 4vw, 2rem);
  overflow: hidden; /* Cấm scroll hoàn toàn cả dọc lẫn ngang */
  position: relative;
}

/* Watermark background on the form side */
.auth-form-side::after {
  content: "";
  position: absolute;
  bottom: -20%;
  right: -10%;
  width: 300px;
  height: 300px;
  background-image: url('data:image/svg+xml;utf8,<svg viewBox="0 0 24 24" fill="none" stroke="%23003153" stroke-width="0.5" xmlns="http://www.w3.org/2000/svg"><circle cx="12" cy="12" r="10"></circle><circle cx="12" cy="12" r="3"></circle><line x1="2" y1="12" x2="9" y2="12"></line><line x1="15" y1="12" x2="22" y2="12"></line></svg>');
  background-repeat: no-root;
  opacity: 0.05;
  pointer-events: none;
  z-index: 0;
}

.form-wrapper {
  width: 100%;
  max-width: 400px;
  position: relative;
  z-index: 1;
}

.auth-header-icon {
  display: flex;
  justify-content: center;
  margin-bottom: clamp(0.2rem, 1vh, 0.5rem);
}

.pokeball-icon {
  width: clamp(36px, 6vh, 48px);
  height: clamp(36px, 6vh, 48px);
  border-radius: 50%;
  object-fit: cover;
  box-shadow: 0 4px 8px rgba(0,0,0,0.1);
  border: 2px solid white;
}

.auth-title {
  font-family: var(--font-serif);
  font-size: clamp(1.8rem, 4vh, 2.4rem);
  color: var(--color-prussian-blue);
  margin-bottom: 0.25rem;
  text-align: center;
}

.auth-subtitle {
  font-family: var(--font-sans);
  text-align: center;
  color: #555;
  margin-bottom: clamp(0.75rem, 3vh, 1.5rem);
  font-size: clamp(0.85rem, 1.8vh, 1rem);
}

.demo-credentials {
  position: relative;
  margin: 0 0 clamp(0.75rem, 2vh, 1.25rem);
  padding: clamp(0.65rem, 1.6vh, 0.9rem) clamp(0.85rem, 2vw, 1.1rem);
  background-color: rgba(255, 197, 18, 0.08);
  border: 1px dashed rgba(0, 49, 83, 0.35);
  border-radius: 18px 4px 14px 4px / 4px 14px 4px 18px;
}

.demo-eyebrow {
  display: block;
  font-family: var(--font-serif);
  font-style: italic;
  color: #C2821B;
  font-size: clamp(0.7rem, 1.4vh, 0.78rem);
  letter-spacing: 1px;
  margin-bottom: 0.4rem;
}

.demo-rows {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
  margin-bottom: 0.55rem;
}

.demo-row {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  gap: 0.75rem;
  font-family: var(--font-sans);
  font-size: clamp(0.72rem, 1.4vh, 0.8rem);
}

.demo-label {
  text-transform: uppercase;
  letter-spacing: 0.8px;
  color: #777;
  font-size: 0.7rem;
  flex-shrink: 0;
}

.demo-value {
  color: var(--color-prussian-blue);
  font-weight: 600;
  font-family: 'Courier New', monospace;
  font-size: clamp(0.72rem, 1.4vh, 0.82rem);
  text-align: right;
  word-break: break-all;
}

.demo-fill-btn {
  background: none;
  border: none;
  padding: 0;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: clamp(0.72rem, 1.4vh, 0.78rem);
  text-transform: uppercase;
  letter-spacing: 1px;
  color: var(--color-cypress-green);
  cursor: pointer;
  text-decoration: underline;
  text-underline-offset: 3px;
  transition: color 0.2s ease;
}
.demo-fill-btn:hover {
  color: var(--color-prussian-blue);
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: clamp(0.5rem, 2vh, 1rem);
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.label-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.form-group label {
  font-family: var(--font-sans);
  font-size: clamp(0.8rem, 1.5vh, 0.9rem);
  font-weight: 600;
  color: var(--color-prussian-blue);
}

.forgot-pass {
  font-family: var(--font-sans);
  font-size: clamp(0.75rem, 1.5vh, 0.8rem);
  color: #666;
  text-decoration: none;
  transition: color 0.2s;
}

.forgot-pass:hover {
  color: var(--color-prussian-blue);
  text-decoration: underline;
}

.form-group input {
  padding: clamp(0.5rem, 1.6vh, 0.8rem) 1rem;
  border: 2px solid var(--color-prussian-blue);
  /* Organic nét cọ nhẹ nhàng hơn cho ô input */
  border-radius: 25px 5px 20px 5px/5px 20px 5px 25px; 
  font-family: var(--font-sans);
  font-size: clamp(0.85rem, 1.8vh, 0.95rem);
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
  background-color: #fff;
  color: #333;
  /* Shadow mỏng lúc chưa tương tác */
  box-shadow: 2px 2px 0px rgba(0, 49, 83, 0.1); 
}

.form-group input::placeholder {
  color: #aaa;
}

.form-group input:focus {
  outline: none;
  background-color: #fff;
  /* Hiệu ứng nổi bật, lồi lên khi focus giống nút Sign In */
  transform: translate(-2px, -2px);
  box-shadow: 4px 4px 0px rgba(0, 49, 83, 1);
}

.auth-submit-btn {
  margin-top: clamp(0.5rem, 1.5vh, 1rem);
  background-color: var(--color-sunflower);
  color: var(--color-prussian-blue);
  border: 2px solid var(--color-prussian-blue);
  padding: clamp(0.8rem, 2.5vh, 1.2rem);
  font-family: var(--font-sans);
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: clamp(1rem, 2.5vh, 1.15rem);
  cursor: pointer;
  /* Trả lại nét cọ Van Gogh hữu cơ */
  border-radius: 255px 15px 225px 15px/15px 225px 15px 255px; 
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
  /* Đổ bóng khối solid nổi bần bật */
  box-shadow: 4px 4px 0px rgba(0, 49, 83, 1); 
}

.auth-submit-btn:hover {
  transform: translate(-2px, -2px);
  box-shadow: 6px 6px 0px rgba(0, 49, 83, 1);
  background-color: #FFCF33; 
}

.auth-submit-btn:active {
  transform: translate(2px, 2px);
  box-shadow: 2px 2px 0px rgba(0, 49, 83, 1);
}

.auth-divider {
  display: flex;
  align-items: center;
  text-align: center;
  margin: clamp(0.7rem, 2vh, 1.5rem) 0;
  color: #999;
  font-family: var(--font-sans);
  font-size: clamp(0.75rem, 1.5vh, 0.85rem);
}

.auth-divider::before, .auth-divider::after {
  content: '';
  flex: 1;
  border-bottom: 1px solid #ddd;
}

.auth-divider span {
  padding: 0 10px;
}

.social-logins {
  display: flex;
  gap: 1rem;
  margin-bottom: clamp(0.5rem, 2vh, 1rem);
}

.social-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: clamp(0.5rem, 1.5vh, 0.75rem);
  border: 2px solid var(--color-prussian-blue);
  /* Organic curves ngẫu hứng, lệch nhịp so với nút Submit để tạo cảm giác vẽ tay */
  border-radius: 15px 225px 15px 255px/255px 15px 225px 15px; 
  background: white;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: clamp(0.85rem, 1.8vh, 0.95rem);
  color: var(--color-prussian-blue);
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
  box-shadow: 3px 3px 0px rgba(0, 49, 83, 1);
}

.social-btn:hover {
  background: var(--color-linen);
  transform: translate(-1px, -1px);
  box-shadow: 4px 4px 0px rgba(0, 49, 83, 1);
}

.social-btn:active {
  transform: translate(2px, 2px);
  box-shadow: 1px 1px 0px rgba(0, 49, 83, 1);
}

.auth-switch {
  text-align: center;
  font-family: var(--font-sans);
  color: #555;
  font-size: clamp(0.8rem, 1.6vh, 0.9rem);
  margin-top: clamp(0.5rem, 1.5vh, 1rem);
}

.switch-btn {
  background: none;
  border: none;
  color: var(--color-prussian-blue);
  font-weight: 700;
  cursor: pointer;
  text-decoration: underline;
  padding: 0;
  font-family: inherit;
  font-size: inherit;
  transition: color 0.3s;
}

.switch-btn:hover {
  color: var(--color-sunflower);
}
</style>