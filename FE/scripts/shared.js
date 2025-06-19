document.addEventListener('DOMContentLoaded', function () {
  const navbar = document.querySelector('.navbar');

  if (!navbar) return;

  function handleScroll() {
    if (window.scrollY > 20) {
      navbar.classList.add('scrolled');
      navbar.classList.remove('transparent');
    } else {
      navbar.classList.remove('scrolled');
      navbar.classList.add('transparent');
    }
  }

  window.addEventListener('scroll', handleScroll);
  handleScroll(); // run once on load
});
