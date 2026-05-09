// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
	compatibilityDate: "2025-07-15",
	ssr: false,
	devtools: {enabled: true},
	modules: ["@nuxt/image"],
	runtimeConfig: {
		public: {
			apiBaseUrl: "http://localhost:5044/api",
		},
	},
	image: {
		quality: 80,
		format: ["webp"],
		domains: ["placehold.co"],
		screens: {
			xs: 320,
			sm: 600,
			md: 1024,
			lg: 1400,
			xl: 1920,
		},
	},
	vite: {
		server: {
			watch: {
				usePolling: true,
			},
		},
	},
	typescript: {
		compilerOptions: {
			module: "ESNext",
		},
	},
});
