// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
	compatibilityDate: "2025-07-15",
	ssr: false,
	devtools: {enabled: true},
	runtimeConfig: {
		public: {
			apiBaseUrl: "http://localhost:5044/api",
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
