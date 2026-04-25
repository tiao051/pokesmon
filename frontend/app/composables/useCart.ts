import type { Product } from '~/data/products'

export interface CartItem {
  product: Product
  quantity: number
}

const STORAGE_KEY = 'pokegogh-cart'
let hydrated = false

export const useCart = () => {
  const items = useState<CartItem[]>('cart', () => [])

  if (import.meta.client && !hydrated) {
    hydrated = true
    try {
      const raw = localStorage.getItem(STORAGE_KEY)
      if (raw) {
        const parsed = JSON.parse(raw)
        if (Array.isArray(parsed)) items.value = parsed
      }
    } catch {
      // localStorage unavailable or corrupt — start empty
    }
    watch(
      items,
      (v) => {
        try {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(v))
        } catch {
          // ignore quota/availability errors
        }
      },
      { deep: true },
    )
  }

  const add = (product: Product, qty = 1) => {
    if (product.stock <= 0) {
      return { added: 0, reason: 'sold-out' as const }
    }
    const existing = items.value.find((i) => i.product.id === product.id)
    const currentQty = existing?.quantity ?? 0
    const targetQty = Math.min(currentQty + qty, product.stock)
    const added = targetQty - currentQty
    if (added <= 0) {
      return { added: 0, reason: 'cap-reached' as const }
    }
    if (existing) existing.quantity = targetQty
    else items.value.push({ product, quantity: targetQty })
    return { added, reason: null }
  }

  const remove = (productId: number) => {
    items.value = items.value.filter((i) => i.product.id !== productId)
  }

  const updateQty = (productId: number, qty: number) => {
    const item = items.value.find((i) => i.product.id === productId)
    if (!item) return
    if (qty <= 0) {
      remove(productId)
      return
    }
    item.quantity = Math.min(qty, item.product.stock)
  }

  const clear = () => {
    items.value = []
  }

  const count = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0))
  const subtotal = computed(() =>
    items.value.reduce((sum, i) => sum + i.product.price * i.quantity, 0),
  )

  return { items, count, subtotal, add, remove, updateQty, clear }
}
