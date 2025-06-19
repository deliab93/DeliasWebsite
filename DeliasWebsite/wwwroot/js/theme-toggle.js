const root = document.documentElement;
let isDark;

function setTheme(isDark) {
  if (isDark) {
    root.setAttribute('data-theme', 'dark');
    localStorage.setItem('theme', 'dark');
  } else {
    root.removeAttribute('data-theme');
    localStorage.setItem('theme', 'light');
  }
}

function initThemeToggle() {
  const savedTheme = localStorage.getItem('theme');
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  isDark = savedTheme === 'dark' || (!savedTheme && prefersDark);
  setTheme(isDark);

  document.querySelectorAll('.theme-toggle').forEach(btn => {
    btn.addEventListener('click', () => {
      isDark = !isDark;
      setTheme(isDark);
    });
  });

  window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
    if (!localStorage.getItem('theme')) {
      isDark = e.matches;
      setTheme(isDark);
    }
  });
}

initThemeToggle();
