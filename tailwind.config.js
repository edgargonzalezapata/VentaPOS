/** @type {import('tailwindcss').Config} */
module.exports = {
  prefix: 'tw-',
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml"
  ],
  theme: {
    extend: {
      colors: {
        primary: '#1e40af',
        secondary: '#64748b',
      }
    },
  },
  plugins: [],
  important: true
} 