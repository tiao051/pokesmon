<script setup>
const props = defineProps({
  error: Object
})

const handleError = () => clearError({ redirect: '/' })
</script>

<template>
  <div class="error-page-wrapper">
    <div class="error-card">
      <div class="error-image-wrapper">
        <img src="/images/error_404.jpg" alt="404 Not Found" class="error-image" />
      </div>
      
      <div class="error-content">
        <h1 class="error-code">{{ error?.statusCode || '404' }}</h1>
        <h2 class="error-title">A wild Error appeared!</h2>
        <p class="error-message">
          {{ error?.message || "We couldn't find the page you were looking for." }}
        </p>
        
        <button @click="handleError" class="back-home-btn">
          Run Away! (Back to Home)
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.error-page-wrapper {
  min-height: 100vh;
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--color-linen, #E8E8E8);
  background-image: radial-gradient(circle at 50% 50%, rgba(0, 49, 83, 0.05) 0%, transparent 80%);
  padding: 2rem;
  font-family: var(--font-sans, "Helvetica", sans-serif);
}

.error-card {
  display: flex;
  flex-direction: column;
  background: #fff;
  border-radius: 20px;
  overflow: hidden;
  max-width: 600px;
  width: 100%;
  box-shadow: 8px 8px 0px rgba(0, 49, 83, 1);
  border: 3px solid var(--color-prussian-blue, #003153);
}

.error-image-wrapper {
  width: 100%;
  height: 300px;
  overflow: hidden;
  border-bottom: 3px solid var(--color-prussian-blue, #003153);
}

.error-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
}

.error-content {
  padding: 3rem 2rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.error-code {
  font-size: 5rem;
  font-family: var(--font-serif, serif);
  color: var(--color-sunflower, #FFC107);
  text-shadow: 3px 3px 0px var(--color-prussian-blue, #003153);
  margin: 0 0 1rem 0;
  line-height: 1;
}

.error-title {
  font-size: 1.8rem;
  font-weight: 700;
  color: var(--color-prussian-blue, #003153);
  margin-bottom: 1rem;
}

.error-message {
  font-size: 1.1rem;
  color: #555;
  margin-bottom: 2.5rem;
  line-height: 1.5;
}

.back-home-btn {
  background-color: var(--color-sunflower, #FFC107);
  color: var(--color-prussian-blue, #003153);
  border: 2px solid var(--color-prussian-blue, #003153);
  padding: 1rem 2.5rem;
  font-weight: 800;
  font-size: 1.1rem;
  border-radius: 25px 5px 20px 5px/5px 20px 5px 25px; 
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
  box-shadow: 3px 3px 0px rgba(0, 49, 83, 1);
  text-transform: uppercase;
}

.back-home-btn:hover {
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0px rgba(0, 49, 83, 1);
}

.back-home-btn:active {
  transform: translate(2px, 2px);
  box-shadow: 1px 1px 0px rgba(0, 49, 83, 1);
}
</style>