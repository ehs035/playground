/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      boxShadow: {
        glow: '0 12px 40px rgba(34, 211, 238, 0.12)',
        soft: '0 10px 30px rgba(2, 6, 23, 0.30)',
      },
      backgroundImage: {
        mesh:
          'radial-gradient(circle at top left, rgba(56,189,248,0.18), transparent 24%), radial-gradient(circle at top right, rgba(168,85,247,0.16), transparent 20%), radial-gradient(circle at bottom left, rgba(16,185,129,0.14), transparent 22%)',
      },
    },
  },
  plugins: [],
};
