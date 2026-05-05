export function fuzzySearch(query: string, text: string): boolean {
  if (!query) return true
  const q = query.toLowerCase()
  const t = text.toLowerCase()
  
  if (t.includes(q)) return true

  const qChars = q.replace(/\s+/g, '')
  let qIdx = 0
  for (let i = 0; i < t.length; i++) {
    if (t[i] === qChars[qIdx]) {
      qIdx++
      if (qIdx === qChars.length) return true
    }
  }
  return false
}
