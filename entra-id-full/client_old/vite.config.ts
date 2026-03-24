import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import mkcert from "vite-plugin-mkcert";

export default defineConfig({
  plugins: [react(), mkcert()],
  server: {
    port: 3000,
    https: true,
    strictPort: true,
    host: true
  },
  preview: {
    port: 3000,
    https: true,
    strictPort: true,
    host: true
  }
});