
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import mkcert from "vite-plugin-mkcert";

export default defineConfig({
  plugins: [react(), mkcert()],
  server: {
    port: 3000,       // Force port 3000
    https: true,      // Force HTTPS
    strictPort: true, // Fail if 3000 is taken (no auto bump)
    host: true        // Bind 0.0.0.0 (helps with LAN/Docker and some localhost issues)
  },
  preview: {
    port: 3000,
    https: true,
    strictPort: true,
    host: true
  }
});
