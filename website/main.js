/**
 * PasteApeart 官网 · 交互脚本
 * 原生 JS，无依赖
 */
(function () {
  'use strict';

  document.addEventListener('DOMContentLoaded', init);

  function init() {
    initYear();
    const nav = document.getElementById('nav');
    const backToTop = document.getElementById('backToTop');

    initScrollEffects(nav, backToTop);
    initMobileNav();
    initSmoothScroll();
    initRevealOnScroll();
    initActiveNavLink();
    initAttrSearch();
    initCodeCopy();
  }

  // ---------- 年份 ----------
  function initYear() {
    const el = document.getElementById('year');
    if (el) el.textContent = String(new Date().getFullYear());
  }

  // ---------- 滚动：导航栏样式 + 回顶按钮 ----------
  function initScrollEffects(nav, backToTop) {
    if (!nav && !backToTop) return;

    let ticking = false;

    function update() {
      const y = window.scrollY || window.pageYOffset;

      if (nav) {
        if (y > 8) nav.classList.add('scrolled');
        else nav.classList.remove('scrolled');
      }

      if (backToTop) {
        if (y > 400) backToTop.classList.add('show');
        else backToTop.classList.remove('show');
      }

      ticking = false;
    }

    window.addEventListener(
      'scroll',
      function () {
        if (!ticking) {
          ticking = true;
          window.requestAnimationFrame(update);
        }
      },
      { passive: true }
    );

    update();

    if (backToTop) {
      backToTop.addEventListener('click', function () {
        window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });
      });
    }
  }

  // ---------- 移动端导航 ----------
  function initMobileNav() {
    const toggle = document.getElementById('navToggle');
    const links = document.getElementById('navLinks');
    if (!toggle || !links) return;

    const ICON_OPEN = '<i class="bi bi-list"></i>';
    const ICON_CLOSE = '<i class="bi bi-x-lg"></i>';

    function close() {
      links.classList.remove('active');
      toggle.innerHTML = ICON_OPEN;
      toggle.setAttribute('aria-expanded', 'false');
    }

    toggle.addEventListener('click', function (e) {
      e.stopPropagation();
      const opened = links.classList.toggle('active');
      toggle.innerHTML = opened ? ICON_CLOSE : ICON_OPEN;
      toggle.setAttribute('aria-expanded', opened ? 'true' : 'false');
    });

    links.addEventListener('click', function (e) {
      const a = e.target.closest('a');
      if (a) close();
    });

    document.addEventListener('click', function (e) {
      if (!links.contains(e.target) && !toggle.contains(e.target)) close();
    });

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') close();
    });

    window.addEventListener('resize', function () {
      if (window.innerWidth > 1080) close();
    });
  }

  // ---------- 平滑滚动（带导航栏偏移） ----------
  function initSmoothScroll() {
    var NAV_OFFSET = 72;

    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
      anchor.addEventListener('click', function (e) {
        var href = anchor.getAttribute('href');
        if (!href || href === '#') return;

        var target = document.querySelector(href);
        if (!target) return;

        e.preventDefault();

        var top = target.getBoundingClientRect().top + (window.scrollY || window.pageYOffset) - NAV_OFFSET;
        window.scrollTo({ top: top, behavior: 'smooth' });

        // 更新 URL hash（不触发额外跳转）
        if (window.history && window.history.replaceState) {
          window.history.replaceState(null, '', href);
        }
      });
    });
  }

  // ---------- 滚动入场动画 ----------
  function initRevealOnScroll() {
    var els = document.querySelectorAll('.fade-up');
    if (!els.length) return;

    if (!('IntersectionObserver' in window)) {
      els.forEach(function (el) {
        el.classList.add('in');
      });
      return;
    }

    var observer = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            entry.target.classList.add('in');
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.08, rootMargin: '0px 0px -20px 0px' }
    );

    els.forEach(function (el) {
      observer.observe(el);
    });
  }

  // ---------- 导航高亮当前板块 ----------
  function initActiveNavLink() {
    var links = document.querySelectorAll('#navLinks a[href^="#"]');
    if (!links.length) return;

    var map = {};
    var sections = [];

    links.forEach(function (link) {
      var id = link.getAttribute('href').slice(1);
      var section = document.getElementById(id);
      if (!section) return;
      map[id] = link;
      sections.push(section);
    });

    if (!sections.length) return;

    if (!('IntersectionObserver' in window)) return;

    var observer = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) return;
          var id = entry.target.id;
          links.forEach(function (l) {
            l.classList.remove('active');
          });
          if (map[id]) map[id].classList.add('active');
        });
      },
      { rootMargin: '-45% 0px -50% 0px', threshold: 0 }
    );

    sections.forEach(function (section) {
      observer.observe(section);
    });
  }

  // ---------- 特性系统搜索 ----------
  function initAttrSearch() {
    var input = document.getElementById('attrSearch');
    var clearBtn = document.getElementById('attrClear');
    var totalEl = document.getElementById('attrTotal');
    var emptyEl = document.getElementById('attrEmpty');
    var groups = Array.prototype.slice.call(document.querySelectorAll('.attr-group'));
    if (!input || !groups.length) return;

    var items = [];
    groups.forEach(function (group) {
      var groupItems = Array.prototype.slice.call(group.querySelectorAll('.attr-item'));
      groupItems.forEach(function (item) {
        var nameEl = item.querySelector('.attr-name');
        var descEl = item.querySelector('.attr-desc');
        var name = nameEl ? nameEl.textContent : '';
        var desc = descEl ? descEl.textContent : '';
        var keywords = item.getAttribute('data-keywords') || '';
        items.push({
          el: item,
          group: group,
          haystack: (name + ' ' + desc + ' ' + keywords).toLowerCase()
        });
      });
    });

    if (totalEl) totalEl.textContent = String(items.length);

    function apply(keyword) {
      var kw = keyword.trim().toLowerCase();
      var visible = 0;

      if (clearBtn) clearBtn.classList.toggle('show', kw.length > 0);

      items.forEach(function (entry) {
        var matched = kw === '' || entry.haystack.indexOf(kw) !== -1;
        entry.el.hidden = !matched;
        if (matched) visible++;
      });

      // 分组内无可见项时隐藏整个分组
      groups.forEach(function (group) {
        var anyVisible = group.querySelectorAll('.attr-item:not([hidden])').length > 0;
        group.hidden = kw !== '' && !anyVisible;
      });

      if (emptyEl) {
        emptyEl.hidden = visible !== 0;
      }
    }

    input.addEventListener('input', function () {
      apply(input.value);
    });

    if (clearBtn) {
      clearBtn.addEventListener('click', function () {
        input.value = '';
        apply('');
        input.focus();
      });
    }

    // 支持从 URL hash 之外的方式触发：#attributes?image 之类不做处理，保持简单
    apply('');
  }

  // ---------- 代码复制 ----------
  function initCodeCopy() {
    var btn = document.getElementById('codeCopy');
    var block = document.querySelector('.code-block');
    if (!btn || !block) return;

    var codeEl = block.querySelector('code');
    if (!codeEl) return;

    var ICON_COPY = btn.innerHTML;

    btn.addEventListener('click', function () {
      var text = codeEl.innerText;

      function done(ok) {
        btn.classList.toggle('copied', ok);
        btn.innerHTML = ok ? '<i class="bi bi-check-lg"></i>' : '<i class="bi bi-x-lg"></i>';
        setTimeout(function () {
          btn.classList.remove('copied');
          btn.innerHTML = ICON_COPY;
        }, 1800);
      }

      if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(
          function () {
            done(true);
          },
          function () {
            done(fallbackCopy(text));
          }
        );
      } else {
        done(fallbackCopy(text));
      }
    });

    function fallbackCopy(text) {
      try {
        var ta = document.createElement('textarea');
        ta.value = text;
        ta.setAttribute('readonly', '');
        ta.style.position = 'absolute';
        ta.style.left = '-9999px';
        document.body.appendChild(ta);
        ta.select();
        var ok = document.execCommand('copy');
        document.body.removeChild(ta);
        return ok;
      } catch (err) {
        return false;
      }
    }
  }
})();
