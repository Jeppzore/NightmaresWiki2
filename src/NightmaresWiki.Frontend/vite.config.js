var _a, _b;
import { defineConfig } from "vitest/config";
import react from "@vitejs/plugin-react";
export default defineConfig({
    plugins: [react()],
    server: {
        port: 4173,
        proxy: {
            "/api": {
                target: (_a = process.env.VITE_API_PROXY_TARGET) !== null && _a !== void 0 ? _a : "http://localhost:8080",
                changeOrigin: true,
            },
            "/media": {
                target: (_b = process.env.VITE_API_PROXY_TARGET) !== null && _b !== void 0 ? _b : "http://localhost:8080",
                changeOrigin: true,
            },
        },
    },
    test: {
        globals: true,
        environment: "jsdom",
        setupFiles: "./src/test/setup.ts",
    },
});
