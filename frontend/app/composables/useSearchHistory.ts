import { ref, onMounted } from 'vue'

export function useSearchHistory() {
  const history = ref<string[]>([])

  onMounted(() => {
    const saved = localStorage.getItem('pokesmon_search_history')
    if (saved) {
      try {
        history.value = JSON.parse(saved)
      } catch (e) {
        history.value = []
      }
    }
  })

  const addSearchTerm = (term: string) => {
    if (!term.trim()) return
    const current = [...history.value]
    const index = current.indexOf(term.trim())
    if (index > -1) {
      current.splice(index, 1)
    }
    current.unshift(term.trim())
    if (current.length > 5) {
      current.pop()
    }
    history.value = current
    localStorage.setItem('pokesmon_search_history', JSON.stringify(current))
  }

  const clearHistory = () => {
    history.value = []
    localStorage.removeItem('pokesmon_search_history')
  }

  return {
    history,
    addSearchTerm,
    clearHistory
  }
}
