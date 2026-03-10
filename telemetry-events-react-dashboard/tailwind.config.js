/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      boxShadow: {
        soft: '0 12px 30px rgba(2, 6, 23, 0.25)',
      },
    },
  },
  plugins: [],
};
