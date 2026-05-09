<script setup lang="ts">
import { computed, reactive, ref } from 'vue'

definePageMeta({
  middleware: 'auth-required',
})

const {
  email,
  avatar,
  storedDisplayName,
  phone,
  defaultLocation,
  signOut,
  setAvatar,
  updateProfile,
} = useAuth()
const router = useRouter()

const DEFAULT_AVATAR = '/images/red_pokeball.jpg'

const currentAvatar = computed(() => avatar.value || DEFAULT_AVATAR)

const derivedDisplayName = computed(() => {
  const e = email.value
  if (!e) return 'Trainer'
  const local = e.split('@')[0] ?? 'Trainer'
  return local.charAt(0).toUpperCase() + local.slice(1)
})

const displayName = computed(
  () => storedDisplayName.value || derivedDisplayName.value,
)

const hashEmail = (input: string, mod: number) => {
  let h = 0
  for (let i = 0; i < input.length; i++) {
    h = ((h << 5) - h + input.charCodeAt(i)) | 0
  }
  return Math.abs(h) % mod
}

const trainerId = computed(() => {
  const n = hashEmail(email.value || 'trainer', 100000)
  return 'No ' + String(n).padStart(5, '0')
})

const pickerOpen = ref(false)

const openPicker = () => {
  pickerOpen.value = true
}

const handleAvatarSelect = (src: string) => {
  setAvatar(src)
}

const editing = ref(false)
const editForm = reactive({
  displayName: '',
  phone: '',
})

const startEdit = () => {
  editForm.displayName = storedDisplayName.value || derivedDisplayName.value
  editForm.phone = phone.value || ''
  editing.value = true
}

const cancelEdit = () => {
  editing.value = false
}

const saveEdit = () => {
  updateProfile({
    displayName: editForm.displayName,
    phone: editForm.phone,
  })
  editing.value = false
}

const defaultLocationDisplay = computed(() => {
  const d = defaultLocation.value
  if (!d) return ''
  return `${d.label} — ${d.city}`
})

const handleSignOut = () => {
  signOut()
  router.push('/')
}
</script>

<template>
  <section class="profile-page">
    <header class="profile-page-header">
      <p class="eyebrow">— Pokédex Trainer Card —</p>
      <h1 class="page-title">Trainer Profile</h1>
      <p class="page-subtitle">Stamped, registered, and sealed beneath the museum's glass.</p>
    </header>

    <div class="profile-grid">
      <!-- LEFT: identity + nav -->
      <aside class="identity-card">
        <span class="card-stamp" aria-hidden="true">Pokégogh<br />Verified</span>

        <div class="avatar-frame">
          <div class="avatar-glow" aria-hidden="true"></div>
          <NuxtImg
            :src="currentAvatar"
            alt="Trainer avatar"
            class="avatar-img"
            width="120"
            height="120"
            loading="eager"
            decoding="async"
          />
          <button
            type="button"
            class="avatar-edit-btn"
            aria-label="Change avatar"
            @click="openPicker"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 20h9"></path>
              <path d="M16.5 3.5a2.121 2.121 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5z"></path>
            </svg>
          </button>
        </div>

        <h2 class="identity-name">{{ displayName }}</h2>
        <p class="identity-email">{{ email }}</p>

        <div class="brush-divider" aria-hidden="true"></div>

        <dl class="trainer-stats">
          <div class="stat-row">
            <dt>Trainer ID</dt>
            <dd class="mono-tight">{{ trainerId }}</dd>
          </div>
          <div class="stat-row">
            <dt>Trainer rank</dt>
            <dd><span class="rank-pill">Rookie</span></dd>
          </div>
        </dl>

        <div class="brush-divider brush-divider-foot" aria-hidden="true"></div>

        <nav class="account-nav" aria-label="Account sections">
          <NuxtLink to="/account" class="nav-item" exact-active-class="active">
            <span class="nav-glyph" aria-hidden="true"></span>
            <span class="nav-label">Profile</span>
          </NuxtLink>
          <NuxtLink to="/account/change-password" class="nav-item" active-class="active">
            <span class="nav-glyph" aria-hidden="true"></span>
            <span class="nav-label">Security</span>
          </NuxtLink>
          <button type="button" class="nav-item nav-item-button" @click="handleSignOut">
            <span class="nav-glyph" aria-hidden="true"></span>
            <span class="nav-label">Sign Out</span>
          </button>
        </nav>
      </aside>

      <!-- RIGHT: details -->
      <article class="details-card">
        <header class="details-header">
          <h2 class="details-title">Trainer Details</h2>
          <div class="details-actions">
            <button
              v-if="!editing"
              type="button"
              class="btn-edit"
              @click="startEdit"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M12 20h9"></path>
                <path d="M16.5 3.5a2.121 2.121 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5z"></path>
              </svg>
              Edit
            </button>
            <template v-else>
              <button type="button" class="btn-cancel-edit" @click="cancelEdit">
                Cancel
              </button>
              <button type="button" class="btn-save-edit" @click="saveEdit">
                Save
              </button>
            </template>
          </div>
        </header>

        <dl class="details-list">
          <div class="detail-row" :class="{ 'is-editing': editing }">
            <dt>Display name</dt>
            <dd v-if="!editing">{{ displayName }}</dd>
            <dd v-else>
              <input
                v-model="editForm.displayName"
                type="text"
                class="detail-input"
                placeholder="Your trainer name"
              />
            </dd>
          </div>
          <div class="detail-row">
            <dt>Email address</dt>
            <dd class="mono">{{ email }}</dd>
          </div>
          <div class="detail-row" :class="{ 'is-editing': editing }">
            <dt>Phone number</dt>
            <dd v-if="!editing" :class="{ muted: !phone }">{{ phone || '—' }}</dd>
            <dd v-else>
              <input
                v-model="editForm.phone"
                type="tel"
                class="detail-input"
                placeholder="+84 90 000 0000"
              />
            </dd>
          </div>
          <div class="detail-row">
            <dt>Default location</dt>
            <dd :class="{ muted: !defaultLocation }">
              <template v-if="defaultLocation">
                {{ defaultLocationDisplay }}
                <NuxtLink to="/account/locations" class="detail-link">Manage</NuxtLink>
              </template>
              <template v-else>
                <span>—</span>
                <NuxtLink to="/account/locations" class="detail-link">Add one</NuxtLink>
              </template>
            </dd>
          </div>
          <div class="detail-row">
            <dt>Joined</dt>
            <dd class="muted">—</dd>
          </div>
          <div class="detail-row">
            <dt>Account status</dt>
            <dd>
              <span class="status-chip">
                <span class="pokeball-mini" aria-hidden="true"></span>
                Active
              </span>
            </dd>
          </div>
        </dl>

        <figure v-if="!editing" class="details-figure" aria-hidden="true">
          <NuxtImg
            src="/images/list_pokemon.png"
            alt=""
            width="600"
            height="400"
            loading="lazy"
            decoding="async"
          />
        </figure>
      </article>
    </div>

    <AvatarPicker
      v-model:open="pickerOpen"
      :current="currentAvatar"
      @select="handleAvatarSelect"
    />
  </section>
</template>

<style scoped>
.profile-page {
  max-width: var(--container-max);
  margin: 0 auto;
  padding: clamp(2rem, 6vw, 4rem) clamp(1rem, 4vw, 2rem) clamp(3rem, 8vw, 5rem);
}

.profile-page-header {
  text-align: center;
  margin-bottom: clamp(2rem, 5vw, 3rem);
  animation: fade-rise 0.6s 0.05s ease both;
}

.eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  letter-spacing: 2px;
  color: #c2821b;
  font-size: 0.8rem;
  margin-bottom: 0.5rem;
}

.page-title {
  font-family: var(--font-serif);
  font-size: clamp(2.25rem, 5vw, 3rem);
  color: var(--color-prussian-blue);
  margin-bottom: 0.5rem;
  letter-spacing: -0.5px;
}

.page-subtitle {
  font-family: var(--font-sans);
  color: #5b5b5b;
  font-size: clamp(0.95rem, 2vw, 1.05rem);
}

.profile-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1.75rem;
}

@media (min-width: 880px) {
  .profile-grid {
    grid-template-columns: minmax(280px, 320px) 1fr;
    align-items: stretch;
  }
}

/* ── Identity card ────────────────────────────────────────────── */

.identity-card {
  background: var(--color-linen);
  border: 1.5px solid rgba(0, 49, 83, 0.15);
  border-radius: 28px 8px 24px 8px / 8px 24px 8px 28px;
  padding: clamp(1.5rem, 3vw, 2rem) clamp(1.25rem, 2.5vw, 1.75rem);
  box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.08);
  text-align: center;
  position: relative;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  animation: fade-rise 0.7s 0.15s ease both;
}

.card-stamp {
  position: absolute;
  top: 14px;
  right: -36px;
  transform: rotate(18deg);
  font-family: var(--font-serif);
  font-style: italic;
  font-size: 0.62rem;
  letter-spacing: 1.5px;
  text-transform: uppercase;
  color: rgba(185, 74, 61, 0.55);
  border: 1.5px solid rgba(185, 74, 61, 0.45);
  padding: 0.25rem 0.75rem;
  line-height: 1.15;
  text-align: center;
  border-radius: 4px;
  pointer-events: none;
  background-color: rgba(255, 255, 255, 0.4);
  animation: stamp-press 0.6s 0.4s cubic-bezier(0.18, 0.89, 0.32, 1.28) both;
}

.avatar-frame {
  position: relative;
  width: 120px;
  height: 120px;
  margin: 0 auto 1.1rem;
  display: grid;
  place-items: center;
  perspective: 600px;
}

.avatar-edit-btn {
  position: absolute;
  bottom: -4px;
  right: -4px;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 2px solid var(--color-prussian-blue);
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  display: grid;
  place-items: center;
  cursor: pointer;
  box-shadow: 2px 2px 0 var(--color-prussian-blue);
  transform: scale(0.85);
  opacity: 0;
  transition: transform 0.25s cubic-bezier(0.34, 1.56, 0.64, 1),
    opacity 0.2s ease, box-shadow 0.2s ease;
  z-index: 2;
}

.avatar-edit-btn svg {
  width: 16px;
  height: 16px;
}

.avatar-frame:hover .avatar-edit-btn,
.avatar-edit-btn:focus-visible {
  transform: scale(1);
  opacity: 1;
}

.avatar-edit-btn:hover {
  transform: scale(1.08) rotate(-6deg);
  box-shadow: 3px 3px 0 var(--color-prussian-blue);
}

@media (hover: none) {
  .avatar-edit-btn {
    transform: scale(1);
    opacity: 1;
  }
}

.avatar-glow {
  position: absolute;
  inset: -8px;
  border-radius: 50%;
  background: conic-gradient(
    from 0deg,
    var(--color-sunflower-yellow),
    var(--color-cypress-green),
    var(--color-prussian-blue),
    var(--color-sunflower-yellow)
  );
  opacity: 0.55;
  filter: blur(10px);
  animation: avatar-orbit 8s linear infinite;
  transition: opacity 0.35s ease, filter 0.35s ease,
    animation-duration 0.35s ease;
}

.avatar-img {
  position: relative;
  display: block;
  width: 100%;
  height: 100%;
  aspect-ratio: 1 / 1;
  border-radius: 50%;
  object-fit: cover;
  object-position: center;
  border: 3px solid var(--color-linen);
  box-shadow: 0 0 0 2px var(--color-prussian-blue);
  transform-style: preserve-3d;
  transition: transform 0.45s cubic-bezier(0.25, 0.8, 0.25, 1),
    box-shadow 0.35s ease, filter 0.35s ease;
}

.avatar-frame::after {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 50%;
  background: linear-gradient(
    115deg,
    transparent 35%,
    rgba(255, 255, 255, 0.5) 50%,
    transparent 65%
  );
  background-size: 240% 100%;
  background-position: 130% 0;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.25s ease, background-position 0.7s ease;
}

.avatar-frame:hover .avatar-img {
  transform: translateZ(18px) rotateX(8deg) rotateY(-10deg) scale(1.03);
  box-shadow: 0 0 0 3px var(--color-sunflower-yellow),
    0 14px 28px rgba(0, 49, 83, 0.3);
  filter: saturate(1.1);
}

.avatar-frame:hover .avatar-glow {
  opacity: 0.85;
  filter: blur(14px) saturate(1.4);
  animation-duration: 4s;
}

.avatar-frame:hover::after {
  opacity: 1;
  background-position: -130% 0;
}

.identity-name {
  font-family: var(--font-serif);
  font-size: 1.5rem;
  color: var(--color-prussian-blue);
  margin-bottom: 0.15rem;
}

.identity-email {
  font-family: var(--font-sans);
  font-size: 0.85rem;
  color: #6f6f6f;
  word-break: break-all;
}

.brush-divider {
  height: 6px;
  margin: 1.25rem auto;
  width: 80%;
  background-image: radial-gradient(
    ellipse at center,
    rgba(0, 49, 83, 0.35) 0%,
    rgba(0, 49, 83, 0) 70%
  );
}

.trainer-stats {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  text-align: left;
}

.stat-row {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  font-family: var(--font-sans);
  font-size: 0.82rem;
}

.stat-row dt {
  color: #777;
  text-transform: uppercase;
  letter-spacing: 0.7px;
  font-size: 0.7rem;
}

.stat-row dd {
  color: var(--color-prussian-blue);
  font-weight: 600;
}

.rank-pill {
  display: inline-block;
  padding: 0.15rem 0.7rem;
  background: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  border-radius: 999px;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.5px;
}

.mono-tight {
  font-family: 'Courier New', monospace;
  letter-spacing: 0.5px;
}

/* ── Account nav ──────────────────────────────────────────────── */

.account-nav {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0.65rem 0.85rem;
  border-radius: 10px;
  font-family: var(--font-sans);
  font-size: 0.92rem;
  font-weight: 600;
  color: var(--color-prussian-blue);
  text-decoration: none;
  background: none;
  border: none;
  cursor: pointer;
  text-align: left;
  transition: background-color 0.2s ease, color 0.2s ease, transform 0.2s ease;
}

.nav-item-button {
  width: 100%;
  font: inherit;
  color: var(--color-prussian-blue);
}

.nav-glyph {
  position: relative;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: linear-gradient(to bottom, #cfd2d4 0%, #cfd2d4 50%, #fff 50%, #fff 100%);
  border: 1.5px solid rgba(0, 49, 83, 0.45);
  flex: none;
  transition: background 0.25s ease, border-color 0.25s ease,
    transform 0.25s ease;
}

.nav-glyph::before {
  content: '';
  position: absolute;
  left: -1.5px;
  right: -1.5px;
  top: 50%;
  height: 1.5px;
  background-color: rgba(0, 49, 83, 0.55);
  transform: translateY(-50%);
}

.nav-glyph::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background-color: #fff;
  border: 1.5px solid rgba(0, 49, 83, 0.55);
  transform: translate(-50%, -50%);
}

.nav-item:hover {
  background-color: rgba(0, 49, 83, 0.05);
  color: var(--color-cypress-green);
}

.nav-item:hover .nav-glyph {
  background: linear-gradient(
    to bottom,
    var(--color-cypress-green) 0%,
    var(--color-cypress-green) 50%,
    #fff 50%,
    #fff 100%
  );
  border-color: var(--color-prussian-blue);
  transform: rotate(-12deg);
}

.nav-item.active {
  background-color: rgba(255, 197, 18, 0.18);
  color: var(--color-prussian-blue);
}

.nav-item.active .nav-glyph {
  background: linear-gradient(
    to bottom,
    #d63a36 0%,
    #d63a36 50%,
    #fff 50%,
    #fff 100%
  );
  border-color: var(--color-prussian-blue);
  transform: rotate(0deg);
  animation: pokeball-wiggle 1.6s ease-in-out infinite;
}

.nav-item-button:hover {
  color: #b94a3d;
}

.nav-item-button:hover .nav-glyph {
  background: linear-gradient(
    to bottom,
    #b94a3d 0%,
    #b94a3d 50%,
    #fff 50%,
    #fff 100%
  );
  border-color: var(--color-prussian-blue);
}

/* ── Details card ─────────────────────────────────────────────── */

.details-card {
  background: #fff;
  border: 1.5px solid rgba(0, 49, 83, 0.15);
  border-radius: 8px 28px 8px 24px / 24px 8px 28px 8px;
  padding: clamp(0.5rem, 2.5vw, 1.75rem);
  box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.08);
  animation: fade-rise 0.7s 0.25s ease both;
  display: flex;
  flex-direction: column;
}

.details-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.25rem;
  border-bottom: 1px dashed rgba(0, 49, 83, 0.2);
  padding-bottom: 1rem;
  flex-wrap: wrap;
}

.details-title {
  font-family: var(--font-serif);
  font-size: clamp(1.4rem, 3vw, 1.75rem);
  color: var(--color-prussian-blue);
  margin-bottom: 0.25rem;
}

.details-actions {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.btn-edit,
.btn-save-edit,
.btn-cancel-edit {
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.78rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: 0.5rem 0.95rem;
  border-radius: 999px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  transition: transform 0.2s ease, box-shadow 0.2s ease,
    background-color 0.2s ease, color 0.2s ease, border-color 0.2s ease;
}

.btn-edit {
  background-color: transparent;
  border: 1.5px solid var(--color-prussian-blue);
  color: var(--color-prussian-blue);
}

.btn-edit svg {
  width: 14px;
  height: 14px;
}

.btn-edit:hover {
  background-color: var(--color-prussian-blue);
  color: var(--color-linen);
  transform: translateY(-1px);
  box-shadow: 0 4px 10px rgba(0, 49, 83, 0.18);
}

.btn-cancel-edit {
  background-color: transparent;
  border: 1.5px solid rgba(0, 49, 83, 0.4);
  color: #555;
}

.btn-cancel-edit:hover {
  border-color: var(--color-prussian-blue);
  color: var(--color-prussian-blue);
  background-color: rgba(0, 49, 83, 0.05);
}

.btn-save-edit {
  background-color: var(--color-sunflower-yellow);
  border: 1.5px solid var(--color-prussian-blue);
  color: var(--color-prussian-blue);
  box-shadow: 2px 2px 0 var(--color-prussian-blue);
}

.btn-save-edit:hover {
  transform: translate(-1px, -1px);
  box-shadow: 3px 3px 0 var(--color-prussian-blue);
  background-color: #ffcf33;
}

.btn-save-edit:active {
  transform: translate(1px, 1px);
  box-shadow: 1px 1px 0 var(--color-prussian-blue);
}

.details-list {
  display: flex;
  flex-direction: column;
}

.detail-row {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.15rem;
  padding: 0.85rem 0;
  border-bottom: 1px solid rgba(0, 49, 83, 0.08);
  position: relative;
  transition: padding-left 0.25s ease;
}

.detail-row::before {
  content: '';
  position: absolute;
  left: -4px;
  top: 50%;
  transform: translateY(-50%) scaleY(0);
  transform-origin: center;
  width: 3px;
  height: 60%;
  background-color: var(--color-sunflower-yellow);
  border-radius: 2px;
  transition: transform 0.25s ease;
}

.detail-row:hover {
  padding-left: 0.5rem;
}

.detail-row:hover::before {
  transform: translateY(-50%) scaleY(1);
}

.detail-row:last-child {
  border-bottom: none;
}

@media (min-width: 540px) {
  .detail-row {
    grid-template-columns: 180px 1fr;
    align-items: baseline;
    gap: 1rem;
  }
}

.detail-row dt {
  font-family: var(--font-sans);
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: #888;
}

.detail-row dd {
  font-family: var(--font-sans);
  font-size: 0.98rem;
  color: var(--color-prussian-blue);
  font-weight: 500;
}

.details-figure {
  display: none;
}

@media (min-width: 880px) {
  .details-figure {
    display: block;
    position: relative;
    margin: 0;
    flex: 1 1 0;
    min-height: 0;
    overflow: hidden;
    border-radius: 18px;
    pointer-events: none;
  }
}

.details-figure img {
  position: absolute;
  inset: 0;
  display: block;
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
  animation: figure-rise 0.8s 0.3s ease both;
}

@keyframes figure-rise {
  from {
    opacity: 0;
    transform: translateY(12px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.detail-link {
  margin-left: 0.6rem;
  font-family: var(--font-sans);
  font-size: 0.78rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: var(--color-cypress-green);
  text-decoration: underline;
  text-underline-offset: 3px;
  transition: color 0.2s ease;
}

.detail-link:hover {
  color: var(--color-prussian-blue);
}

.detail-row dd.mono {
  font-family: 'Courier New', monospace;
}

.detail-row dd.muted {
  color: #b0b0b0;
}

.detail-row.is-editing {
  padding-left: 0.5rem;
}

.detail-row.is-editing::before {
  transform: translateY(-50%) scaleY(1);
}

.detail-input {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1.5px solid rgba(0, 49, 83, 0.35);
  border-radius: 8px;
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: var(--color-prussian-blue);
  background-color: #fff;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
  animation: detail-input-in 0.25s ease both;
}

.detail-input::placeholder {
  color: #b3b3b3;
}

.detail-input:focus {
  outline: none;
  border-color: var(--color-prussian-blue);
  box-shadow: 0 0 0 3px rgba(255, 197, 18, 0.3);
}

@keyframes detail-input-in {
  from {
    opacity: 0;
    transform: translateY(-3px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.status-chip {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  padding: 0.25rem 0.75rem;
  border-radius: 999px;
  background-color: rgba(15, 94, 60, 0.1);
  color: var(--color-cypress-green);
  font-size: 0.85rem;
  font-weight: 600;
}

.pokeball-mini {
  position: relative;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: linear-gradient(
    to bottom,
    #d63a36 0%,
    #d63a36 50%,
    #fff 50%,
    #fff 100%
  );
  border: 1.5px solid #1a1a1a;
  flex: none;
  animation: pokeball-bob 2.4s ease-in-out infinite;
}

.pokeball-mini::before {
  content: '';
  position: absolute;
  left: -1.5px;
  right: -1.5px;
  top: 50%;
  height: 1.5px;
  background-color: #1a1a1a;
  transform: translateY(-50%);
}

.pokeball-mini::after {
  content: '';
  position: absolute;
  left: 50%;
  top: 50%;
  width: 4px;
  height: 4px;
  border-radius: 50%;
  background-color: #fff;
  border: 1.5px solid #1a1a1a;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 0 0 rgba(214, 58, 54, 0.6);
  animation: pokeball-blink 2.4s ease-in-out infinite;
}

/* ── Animations ───────────────────────────────────────────────── */

@keyframes fade-rise {
  from {
    opacity: 0;
    transform: translateY(14px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes avatar-orbit {
  to {
    transform: rotate(360deg);
  }
}

@keyframes pokeball-wiggle {
  0%,
  100% {
    transform: rotate(0deg);
  }
  25% {
    transform: rotate(-8deg);
  }
  75% {
    transform: rotate(8deg);
  }
}

@keyframes pokeball-bob {
  0%,
  100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-2px);
  }
}

@keyframes pokeball-blink {
  0%,
  60%,
  100% {
    box-shadow: 0 0 0 0 rgba(214, 58, 54, 0);
  }
  30% {
    box-shadow: 0 0 0 4px rgba(214, 58, 54, 0.35);
  }
}

@keyframes stamp-press {
  0% {
    opacity: 0;
    transform: rotate(28deg) scale(1.4);
  }
  60% {
    opacity: 1;
    transform: rotate(15deg) scale(0.94);
  }
  100% {
    opacity: 1;
    transform: rotate(18deg) scale(1);
  }
}
</style>
