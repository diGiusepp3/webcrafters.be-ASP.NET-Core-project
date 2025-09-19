/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,razor}",
    "./Pages/**/*.{cshtml,razor}",
    "./wwwroot/**/*.html",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      keyframes: {
        slide: { '0%': { transform: 'translateX(0)' }, '100%': { transform: 'translateX(-50%)' } },
        slideReverse: { '0%': { transform: 'translateX(-50%)' }, '100%': { transform: 'translateX(0)' } }
      },
      animation: {
        slide: 'slide 25s linear infinite',
        slideReverse: 'slideReverse 25s linear infinite'
      }
    }
  },
  plugins: []
}
