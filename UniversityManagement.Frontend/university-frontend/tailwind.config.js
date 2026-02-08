/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,jsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#0f172a",
      },
      boxShadow: {
        glow: "0 20px 45px -25px rgba(99, 102, 241, 0.6)",
      },
    },
  },
  plugins: [],
};
