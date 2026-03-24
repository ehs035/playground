/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './index.html',
    './src/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        brand: {
          DEFAULT: '#2563eb', // Indigo-600
          dark: '#1e40af',    // Indigo-800
          light: '#60a5fa',   // Indigo-400
        },
      },
    },
  },
  darkMode: 'class', // you can toggle a `dark` class at the root if needed
  plugins: [],
}