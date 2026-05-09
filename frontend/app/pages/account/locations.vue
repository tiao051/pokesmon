<script setup lang="ts">
import { computed, reactive, ref } from 'vue'
import type { Location } from '~/composables/auth/useAuth'

definePageMeta({
  middleware: 'auth-required',
})

useHead({ title: 'Locations — PokéGogh' })

const {
  locations,
  addLocation,
  updateLocation,
  removeLocation,
  setDefaultLocation,
} = useAuth()
const toast = useToast()

interface Draft {
  label: string
  address: string
  city: string
  country: string
}

const emptyDraft = (): Draft => ({ label: '', address: '', city: '', country: '' })

const newForm = reactive<Draft>(emptyDraft())
const editingId = ref<string | null>(null)
const editForm = reactive<Draft>(emptyDraft())
const submitting = ref(false)

const pendingDelete = ref<Location | null>(null)
const confirmOpen = ref(false)

const isDraftReady = (d: Draft) =>
  Boolean(d.label.trim() && d.address.trim() && d.city.trim() && d.country.trim())

const newCanSubmit = computed(() => isDraftReady(newForm) && !submitting.value)
const editCanSubmit = computed(() => isDraftReady(editForm) && !submitting.value)

const sortedLocations = computed<Location[]>(() =>
  [...locations.value].sort((a, b) => Number(b.isDefault) - Number(a.isDefault)),
)

const fields: Array<{ key: keyof Draft; label: string; placeholder: string }> = [
  { key: 'label', label: 'Label', placeholder: 'Home, Office, Pewter Loft…' },
  { key: 'address', label: 'Street Address', placeholder: 'Số 1, Đường Hướng Dương' },
  { key: 'city', label: 'City', placeholder: 'Pallet Town' },
  { key: 'country', label: 'Country', placeholder: 'Kanto' },
]

const handleAdd = async () => {
  if (!newCanSubmit.value) return
  submitting.value = true
  try {
    await addLocation({
      label: newForm.label,
      address: newForm.address,
      city: newForm.city,
      country: newForm.country,
    })
    toast.success(`Saved “${newForm.label.trim()}”`)
    Object.assign(newForm, emptyDraft())
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Could not save location')
  } finally {
    submitting.value = false
  }
}

const startEdit = (loc: Location) => {
  editingId.value = loc.id
  editForm.label = loc.label
  editForm.address = loc.address
  editForm.city = loc.city
  editForm.country = loc.country
}

const cancelEdit = () => {
  editingId.value = null
  Object.assign(editForm, emptyDraft())
}

const saveEdit = async () => {
  if (!editingId.value || !editCanSubmit.value) return
  submitting.value = true
  try {
    await updateLocation(editingId.value, {
      label: editForm.label,
      address: editForm.address,
      city: editForm.city,
      country: editForm.country,
    })
    toast.success(`Updated “${editForm.label.trim()}”`)
    cancelEdit()
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Could not update location')
  } finally {
    submitting.value = false
  }
}

const askDelete = (loc: Location) => {
  pendingDelete.value = loc
  confirmOpen.value = true
}

const handleDelete = async () => {
  const target = pendingDelete.value
  if (!target) return
  submitting.value = true
  try {
    await removeLocation(target.id)
    toast.success(`Removed “${target.label}”`)
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Could not remove location')
  } finally {
    submitting.value = false
    pendingDelete.value = null
  }
}

const handleSetDefault = async (loc: Location) => {
  if (loc.isDefault || submitting.value) return
  submitting.value = true
  try {
    await setDefaultLocation(loc.id)
    toast.success(`“${loc.label}” is now your default`)
  } catch (e) {
    toast.error(e instanceof Error ? e.message : 'Could not set default')
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <section class="loc-page">
    <NuxtLink to="/account" class="back-link">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <line x1="19" y1="12" x2="5" y2="12"></line>
        <polyline points="12 19 5 12 12 5"></polyline>
      </svg>
      <span>Back to Profile</span>
    </NuxtLink>

    <header class="loc-header">
      <p class="loc-eyebrow">— Pokémart Postage Roster —</p>
      <h1 class="loc-title">Saved Locations</h1>
      <p class="loc-subtitle">
        Manage where your acquisitions arrive. Star one as default and we'll preselect it at checkout.
      </p>
    </header>

    <div v-if="locations.length === 0" class="loc-empty">
      <p class="loc-empty-eyebrow">— interlude —</p>
      <h2>No addresses yet</h2>
      <p>Add your first patron postage below to get started.</p>
    </div>

    <ul v-else class="loc-list">
      <li
        v-for="loc in sortedLocations"
        :key="loc.id"
        class="loc-card"
        :class="{ 'is-default': loc.isDefault, 'is-editing': editingId === loc.id }"
      >
        <template v-if="editingId === loc.id">
          <form class="loc-form loc-form-inline" @submit.prevent="saveEdit" novalidate>
            <div
              v-for="field in fields"
              :key="field.key"
              class="loc-field"
              :class="{ 'is-full': field.key === 'address' || field.key === 'label' }"
            >
              <label :for="`edit-${field.key}`">{{ field.label }}</label>
              <input
                :id="`edit-${field.key}`"
                v-model="editForm[field.key]"
                type="text"
                :placeholder="field.placeholder"
                required
              />
            </div>
            <div class="loc-form-actions loc-form-actions-inline">
              <button type="button" class="btn-ghost" @click="cancelEdit">Cancel</button>
              <button type="submit" class="btn-save" :disabled="!editCanSubmit">
                {{ submitting ? 'Saving…' : 'Save Changes' }}
              </button>
            </div>
          </form>
        </template>

        <template v-else>
          <div class="loc-card-head">
            <div class="loc-card-title-wrap">
              <h3 class="loc-card-title">{{ loc.label }}</h3>
              <span v-if="loc.isDefault" class="default-pill">
                <span class="default-dot" aria-hidden="true"></span>
                Default
              </span>
            </div>
            <div class="loc-card-meta">
              <p>{{ loc.address }}</p>
              <p class="muted">{{ loc.city }}, {{ loc.country }}</p>
            </div>
          </div>

          <div class="loc-card-actions">
            <button
              v-if="!loc.isDefault"
              type="button"
              class="btn-ghost"
              :disabled="submitting"
              @click="handleSetDefault(loc)"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
              </svg>
              <span>Set as default</span>
            </button>
            <button
              type="button"
              class="btn-ghost"
              :disabled="submitting"
              @click="startEdit(loc)"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <path d="M12 20h9"></path>
                <path d="M16.5 3.5a2.121 2.121 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5z"></path>
              </svg>
              <span>Edit</span>
            </button>
            <button
              type="button"
              class="btn-ghost btn-danger"
              :disabled="submitting"
              @click="askDelete(loc)"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                <polyline points="3 6 5 6 21 6"/>
                <path d="M19 6l-2 14a2 2 0 0 1-2 2H9a2 2 0 0 1-2-2L5 6"/>
                <path d="M10 11v6"/>
                <path d="M14 11v6"/>
                <path d="M9 6V4a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2v2"/>
              </svg>
              <span>Delete</span>
            </button>
          </div>
        </template>
      </li>
    </ul>

    <section class="loc-add-card">
      <header class="loc-add-head">
        <span class="loc-add-eyebrow">— Add a Location —</span>
        <h2 class="loc-add-title">New Postage Address</h2>
      </header>

      <form class="loc-form" @submit.prevent="handleAdd" novalidate>
        <div
          v-for="field in fields"
          :key="field.key"
          class="loc-field"
          :class="{ 'is-full': field.key === 'address' || field.key === 'label' }"
        >
          <label :for="`new-${field.key}`">{{ field.label }}</label>
          <input
            :id="`new-${field.key}`"
            v-model="newForm[field.key]"
            type="text"
            :placeholder="field.placeholder"
            required
          />
        </div>
        <div class="loc-form-actions">
          <button type="submit" class="btn-save" :disabled="!newCanSubmit">
            {{ submitting ? 'Saving…' : 'Save Location' }}
          </button>
        </div>
      </form>
    </section>

    <ConfirmDialog
      v-model:open="confirmOpen"
      title="Remove this address?"
      :message="pendingDelete ? `Remove “${pendingDelete.label}” from your saved locations?` : ''"
      confirm-label="Yes, remove"
      cancel-label="Keep it"
      @confirm="handleDelete"
    />
  </section>
</template>

<style scoped>
.loc-page {
  max-width: 760px;
  margin: 0 auto;
  padding: clamp(2rem, 6vw, 4rem) clamp(1rem, 4vw, 2rem) clamp(3rem, 8vw, 5rem);
}

.back-link {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  margin-bottom: 1.25rem;
  font-family: var(--font-sans);
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--color-prussian-blue);
  text-decoration: none;
  padding: 0.4rem 0.7rem;
  border-radius: 999px;
  transition: background-color 0.2s ease, transform 0.2s ease;
}

.back-link:hover {
  background-color: rgba(0, 49, 83, 0.08);
  transform: translateX(-2px);
}

.back-link svg {
  width: 16px;
  height: 16px;
}

.loc-header {
  text-align: center;
  margin-bottom: 1.75rem;
  animation: fade-rise 0.6s 0.05s ease both;
}

.loc-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  letter-spacing: 1.5px;
  color: #c2821b;
  font-size: 0.82rem;
  margin-bottom: 0.35rem;
}

.loc-title {
  font-family: var(--font-serif);
  font-size: clamp(1.85rem, 4vw, 2.4rem);
  color: var(--color-prussian-blue);
  margin-bottom: 0.5rem;
  letter-spacing: -0.5px;
  position: relative;
  display: inline-block;
}

.loc-title::after {
  content: '';
  display: block;
  width: 60px;
  height: 3px;
  background-color: var(--color-cypress-green);
  margin: 0.55rem auto 0;
  border-radius: 2px;
}

.loc-subtitle {
  font-family: var(--font-sans);
  color: #6f6f6f;
  font-size: 0.95rem;
  line-height: 1.55;
  max-width: 460px;
  margin: 0 auto;
}

.loc-empty {
  text-align: center;
  padding: clamp(2rem, 5vw, 3rem) 1.5rem;
  background-color: rgba(255, 255, 255, 0.5);
  border: 1px dashed rgba(0, 49, 83, 0.25);
  border-radius: 12px;
  margin-bottom: 1.75rem;
}

.loc-empty-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  letter-spacing: 1.5px;
  color: #c2821b;
  font-size: 0.78rem;
  margin-bottom: 0.65rem;
  text-transform: uppercase;
}

.loc-empty h2 {
  font-family: var(--font-serif);
  font-size: 1.4rem;
  color: var(--color-prussian-blue);
  margin-bottom: 0.4rem;
}

.loc-empty p {
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: #6f6f6f;
}

.loc-list {
  list-style: none;
  padding: 0;
  margin: 0 0 2rem;
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
}

.loc-card {
  position: relative;
  background-color: #fbf9f2;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 28px 8px 24px 8px / 8px 24px 8px 28px;
  padding: clamp(1rem, 3vw, 1.25rem) clamp(1rem, 4vw, 1.5rem);
  box-shadow: 4px 4px 0 rgba(0, 49, 83, 0.15);
  transition: transform 0.25s ease, box-shadow 0.25s ease;
  animation: fade-rise 0.5s ease both;
}

.loc-card:hover {
  transform: translate(-1px, -1px);
  box-shadow: 5px 5px 0 rgba(0, 49, 83, 0.2);
}

.loc-card.is-default {
  border-color: var(--color-cypress-green);
  box-shadow: 4px 4px 0 rgba(15, 94, 60, 0.22);
}

.loc-card.is-editing {
  background-color: #fff;
  box-shadow: 5px 5px 0 rgba(0, 49, 83, 0.25);
}

.loc-card-head {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.loc-card-title-wrap {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  flex-wrap: wrap;
}

.loc-card-title {
  font-family: var(--font-serif);
  font-size: 1.2rem;
  color: var(--color-prussian-blue);
  margin: 0;
}

.default-pill {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
  background-color: rgba(15, 94, 60, 0.12);
  color: var(--color-cypress-green);
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.7rem;
  letter-spacing: 1px;
  text-transform: uppercase;
  padding: 0.2rem 0.6rem;
  border-radius: 999px;
}

.default-dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background-color: var(--color-cypress-green);
}

.loc-card-meta {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: var(--color-prussian-blue);
}

.loc-card-meta p {
  margin: 0;
}

.loc-card-meta .muted {
  color: #6f6f6f;
  font-size: 0.88rem;
}

.loc-card-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  padding-top: 0.75rem;
  border-top: 1px dashed rgba(0, 49, 83, 0.18);
}

.btn-ghost {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  font-family: var(--font-sans);
  font-weight: 700;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: 0.45rem 0.9rem;
  border: 1.5px solid rgba(0, 49, 83, 0.4);
  background-color: transparent;
  color: var(--color-prussian-blue);
  border-radius: 999px;
  cursor: pointer;
  transition: background-color 0.2s ease, color 0.2s ease, border-color 0.2s ease,
    transform 0.2s ease, opacity 0.2s ease;
}

.btn-ghost svg {
  width: 14px;
  height: 14px;
  flex: none;
}

@media (max-width: 380px) {
  .loc-card-actions {
    gap: 0.4rem;
  }
  .btn-ghost {
    padding: 0.4rem 0.7rem;
    font-size: 0.7rem;
  }
}

.btn-ghost:not(:disabled):hover {
  background-color: var(--color-prussian-blue);
  color: var(--color-linen);
  transform: translateY(-1px);
}

.btn-ghost:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-ghost.btn-danger {
  border-color: rgba(178, 34, 34, 0.45);
  color: #b22222;
}

.btn-ghost.btn-danger:not(:disabled):hover {
  background-color: #b22222;
  color: #fff;
  border-color: #b22222;
}

.loc-form {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.85rem 1rem;
}

@media (min-width: 640px) {
  .loc-form {
    grid-template-columns: repeat(2, 1fr);
  }
  .loc-field.is-full {
    grid-column: 1 / -1;
  }
}

.loc-form-inline {
  margin: 0;
}

.loc-field {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.loc-field label {
  font-family: var(--font-sans);
  font-size: 0.72rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  color: rgba(0, 49, 83, 0.7);
}

.loc-field input {
  width: 100%;
  padding: 0.65rem 0.9rem;
  border: 2px solid rgba(0, 49, 83, 0.3);
  border-radius: 14px;
  background-color: #fff;
  font-family: var(--font-sans);
  font-size: 0.95rem;
  color: var(--color-prussian-blue);
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.loc-field input::placeholder {
  color: #b3b3b3;
  font-style: italic;
}

.loc-field input:focus {
  outline: none;
  border-color: var(--color-prussian-blue);
  box-shadow: 0 0 0 3px rgba(0, 49, 83, 0.12);
}

.loc-form-actions {
  display: flex;
  justify-content: flex-end;
  flex-wrap: wrap;
  gap: 0.75rem;
  margin-top: 0.5rem;
  grid-column: 1 / -1;
}

@media (max-width: 480px) {
  .loc-form-actions {
    justify-content: stretch;
  }
  .loc-form-actions > * {
    flex: 1 1 100%;
  }
}

.loc-form-actions-inline {
  border-top: 1px dashed rgba(0, 49, 83, 0.18);
  padding-top: 0.85rem;
}

.btn-save {
  font-family: var(--font-sans);
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  font-size: 0.85rem;
  padding: 0.7rem 1.6rem;
  border: 2px solid var(--color-prussian-blue);
  border-radius: 25px 5px 20px 5px / 5px 20px 5px 25px;
  background-color: var(--color-sunflower-yellow);
  color: var(--color-prussian-blue);
  cursor: pointer;
  box-shadow: 3px 3px 0 var(--color-prussian-blue);
  transition: transform 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease,
    opacity 0.2s ease;
}

.btn-save:not(:disabled):hover {
  transform: translate(-2px, -2px);
  box-shadow: 5px 5px 0 var(--color-prussian-blue);
  background-color: #ffcf33;
}

.btn-save:not(:disabled):active {
  transform: translate(1px, 1px);
  box-shadow: 1px 1px 0 var(--color-prussian-blue);
}

.btn-save:disabled {
  cursor: not-allowed;
  opacity: 0.55;
}

.loc-add-card {
  background: #fff;
  border: 1.5px solid rgba(0, 49, 83, 0.18);
  border-radius: 8px 28px 8px 24px / 24px 8px 28px 8px;
  padding: clamp(1.25rem, 3vw, 2rem);
  box-shadow: 6px 6px 0 rgba(0, 49, 83, 0.08);
  animation: fade-rise 0.7s 0.15s ease both;
}

.loc-add-head {
  margin-bottom: 1.25rem;
}

.loc-add-eyebrow {
  font-family: var(--font-serif);
  font-style: italic;
  letter-spacing: 1.5px;
  color: #c2821b;
  font-size: 0.82rem;
  display: block;
  margin-bottom: 0.3rem;
}

.loc-add-title {
  font-family: var(--font-serif);
  font-size: 1.35rem;
  color: var(--color-prussian-blue);
  margin: 0;
  letter-spacing: -0.3px;
}

@keyframes fade-rise {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
