// ساده‌ترین سرویس‌ورکر برای PWA Qdoc
const CACHE_NAME = 'qdoc-cache-v1';
const OFFLINE_URLS = [
  '/',
  '/css/site.css',
  '/images/1.jpg',
  '/favicon.ico'
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME).then(cache => cache.addAll(OFFLINE_URLS))
  );
});

self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(keys =>
      Promise.all(
        keys
          .filter(k => k !== CACHE_NAME)
          .map(k => caches.delete(k))
      )
    )
  );
});

self.addEventListener('fetch', event => {
  const { request } = event;

  // فقط درخواست‌های GET را کش کن
  if (request.method !== 'GET') {
    return;
  }

  event.respondWith(
    caches.match(request).then(cachedResponse => {
      if (cachedResponse) {
        return cachedResponse;
      }

      return fetch(request).catch(() => {
        // در صورت آفلاین بودن، اگر صفحه اصلی درخواست شده بود، نسخه کش شده را برگردان
        if (request.mode === 'navigate') {
          return caches.match('/');
        }
      });
    })
  );
});

