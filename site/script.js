/* NoxtiCord — interactions du site */

(() => {
  "use strict";

  /* ── Menu mobile ── */
  const burger = document.getElementById("burger");
  const navLinks = document.getElementById("navLinks");
  const nav = document.querySelector(".nav");

  const closeMenu = () => {
    navLinks.classList.remove("is-open");
    nav.classList.remove("is-open");
    burger.setAttribute("aria-expanded", "false");
  };

  burger.addEventListener("click", () => {
    const open = navLinks.classList.toggle("is-open");
    nav.classList.toggle("is-open", open);
    burger.setAttribute("aria-expanded", String(open));
  });

  navLinks.querySelectorAll("a").forEach((a) => a.addEventListener("click", closeMenu));

  /* ── Apparition au scroll ── */
  const revealables = document.querySelectorAll(".reveal");

  if ("IntersectionObserver" in window) {
    const io = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (!entry.isIntersecting) return;
          entry.target.classList.add("is-visible");
          io.unobserve(entry.target);
        });
      },
      { threshold: 0.15, rootMargin: "0px 0px -40px 0px" }
    );
    revealables.forEach((el) => io.observe(el));
  } else {
    revealables.forEach((el) => el.classList.add("is-visible"));
  }

  /* ── Compteurs du hero ── */
  const animateCount = (el) => {
    const target = Number(el.dataset.count || 0);
    const suffix = el.dataset.suffix || "";
    const duration = 1100;
    const start = performance.now();

    const step = (now) => {
      const p = Math.min((now - start) / duration, 1);
      const eased = 1 - Math.pow(1 - p, 3);
      el.textContent = Math.round(target * eased) + suffix;
      if (p < 1) requestAnimationFrame(step);
    };

    requestAnimationFrame(step);
  };

  const counters = document.querySelectorAll(".stat__value[data-count]");
  const startCounters = () => counters.forEach((el) => !el.dataset.done && ((el.dataset.done = "1"), animateCount(el)));

  if ("IntersectionObserver" in window) {
    const statsBox = document.querySelector(".stats");
    if (statsBox) {
      const ioStats = new IntersectionObserver(
        (entries) => {
          if (entries.some((e) => e.isIntersecting)) {
            startCounters();
            ioStats.disconnect();
          }
        },
        { threshold: 0.4 }
      );
      ioStats.observe(statsBox);
    }
  } else {
    startCounters();
  }

  /* ── Lien de section actif dans la nav ── */
  const sections = [...document.querySelectorAll("main section[id]")];
  const links = [...navLinks.querySelectorAll("a")];

  const setActive = (id) => {
    links.forEach((a) => a.classList.toggle("is-active", a.getAttribute("href") === `#${id}`));
  };

  if ("IntersectionObserver" in window && sections.length) {
    const ioSection = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) setActive(entry.target.id);
        });
      },
      { rootMargin: "-45% 0px -50% 0px" }
    );
    sections.forEach((s) => ioSection.observe(s));
  }
})();
