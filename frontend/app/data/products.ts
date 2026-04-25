export type ProductCategory =
  | 'Elite Trainer Boxes'
  | 'Booster Boxes'
  | 'Booster Packs'
  | 'Premium Collections'

export interface Product {
  id: number
  slug: string
  title: string
  price: number
  image: string
  description: string
  stock: number
  category: ProductCategory
  tags?: string[]
  featured?: boolean
}

export const categories: ProductCategory[] = [
  'Elite Trainer Boxes',
  'Booster Boxes',
  'Booster Packs',
  'Premium Collections',
]

export const products: Product[] = [
  {
    id: 1,
    slug: 'van-gogh-museum-etb',
    title: 'Van Gogh Museum ETB',
    price: 89.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Van+Gogh+ETB',
    description:
      'A limited-run Elite Trainer Box commemorating the museum\'s post-impressionist Pokémon series. Includes nine booster packs, a sleeved Starry Pikachu promo, energy cards, and a custom canvas-textured player guide bound in linen.',
    stock: 12,
    category: 'Elite Trainer Boxes',
    tags: ['museum-exclusive', 'limited'],
    featured: true,
  },
  {
    id: 2,
    slug: 'sv-151-booster-box',
    title: 'Scarlet & Violet: 151 Booster Box',
    price: 120.0,
    image: 'https://placehold.co/400x500/003153/FFC512?text=151+Booster+Box',
    description:
      'A sealed booster box returning to the original 151 Pokémon. Thirty-six packs of meticulously illustrated cards — a quiet act of nostalgia, framed for the collector.',
    stock: 8,
    category: 'Booster Boxes',
  },
  {
    id: 3,
    slug: 'starry-pikachu-promo',
    title: 'Starry Pikachu Promo Card (Graded)',
    price: 250.0,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Pikachu+Promo',
    description:
      'A graded promotional card from the museum\'s flagship exhibition: a Pikachu rendered against a swirling, cosmic firmament. Each card is sealed in an archival case and authenticated.',
    stock: 5,
    category: 'Premium Collections',
    tags: ['graded', 'flagship'],
    featured: true,
  },
  {
    id: 4,
    slug: 'paldean-fates-booster-bundle',
    title: 'Paldean Fates Booster Bundle',
    price: 26.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Booster+Bundle',
    description:
      'Six booster packs bundled together — a small, contained adventure through the Paldean region. Suitable for casual collectors and afternoon openings.',
    stock: 24,
    category: 'Booster Packs',
  },
  {
    id: 5,
    slug: 'crown-zenith-booster-pack',
    title: 'Crown Zenith Booster Pack',
    price: 4.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Booster+Pack',
    description:
      'A single Crown Zenith booster pack. A modest acquisition — ten cards, an unhurried moment, the small thrill of discovery.',
    stock: 64,
    category: 'Booster Packs',
  },
  {
    id: 6,
    slug: 'eevee-evolutions-premium',
    title: 'Eevee Evolutions Premium Collection',
    price: 69.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Eevee+Premium',
    description:
      'A premium collection box honoring Eevee and its many transformations. Includes oversized illustrated cards, sleeved booster packs, and a deck box rendered in muted ochre.',
    stock: 10,
    category: 'Premium Collections',
  },
  {
    id: 7,
    slug: 'temporal-forces-etb',
    title: 'Temporal Forces Elite Trainer Box',
    price: 49.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Temporal+ETB',
    description:
      'The Temporal Forces Elite Trainer Box: nine packs, dice, energy cards, and a heavy-stock player\'s guide. A complete entry point for the new student of the game.',
    stock: 15,
    category: 'Elite Trainer Boxes',
  },
  {
    id: 8,
    slug: 'surging-sparks-sleeve',
    title: 'Surging Sparks Booster Sleeve',
    price: 3.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Sleeved+Pack',
    description:
      'A single Surging Sparks booster pack in a protective sleeve. The smallest and most affordable artifact in the museum shop.',
    stock: 120,
    category: 'Booster Packs',
  },
  {
    id: 9,
    slug: 'obsidian-flames-etb',
    title: 'Obsidian Flames Elite Trainer Box',
    price: 49.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Obsidian+ETB',
    description:
      'The Obsidian Flames ETB: nine packs, a sleeved Charizard promo, and a charcoal-toned player guide. A study in fire and shadow.',
    stock: 9,
    category: 'Elite Trainer Boxes',
  },
  {
    id: 10,
    slug: 'twilight-masquerade-booster-box',
    title: 'Twilight Masquerade Booster Box',
    price: 134.0,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Twilight+Box',
    description:
      'A sealed Twilight Masquerade booster box: thirty-six packs themed around dusk, costume, and the gentle drama of the masquerade ball.',
    stock: 6,
    category: 'Booster Boxes',
  },
  {
    id: 11,
    slug: 'shrouded-fable-premium',
    title: 'Shrouded Fable Premium Collection',
    price: 39.99,
    image: 'https://placehold.co/400x500/003153/FFC512?text=Shrouded+Fable',
    description:
      'A premium collection box exploring the Shrouded Fable expansion. Foil cards, sleeved packs, and a small folio of illustrated lore notes.',
    stock: 18,
    category: 'Premium Collections',
  },
]

export function formatPrice(price: number): string {
  return `$${price.toFixed(2)}`
}

export function findProductBySlug(slug: string): Product | undefined {
  return products.find((p) => p.slug === slug)
}
