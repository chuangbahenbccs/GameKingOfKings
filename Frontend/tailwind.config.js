/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        fantasy: ['Cinzel', 'serif'],
        pixel: ['"VT323"', 'monospace'],
        sans: ['Inter', 'sans-serif'],
      },
      colors: {
        'rpg-dark': '#020617',
        'rpg-gold': '#daa520',
        'rpg-red': '#ff0033',
        'rpg-blue': '#00f3ff',
        'rpg-panel': 'rgba(2, 6, 23, 0.85)',
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
      },
    },
  },
  plugins: [],
}
