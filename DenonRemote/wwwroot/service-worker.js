const CACHE = 'denon-v1';
const STATIC = [
  '/',
  '/app.css',
  '/theme.js',
  '/icon.svg',
];

self.addEventListener('install', e => {
  e.waitUntil(
    caches.open(CACHE).then(c => c.addAll(STATIC)).then(() => self.skipWaiting())
  );
});

self.addEventListener('activate', e => {
  e.waitUntil(
    caches.keys()
      .then(keys => Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k))))
      .then(() => self.clients.claim())
  );
});

// Network-first for all requests (Blazor Server needs the live connection).
// Fall back to cache only for static assets when offline.
self.addEventListener('fetch', e => {
  const url = new URL(e.request.url);
  const isStatic = STATIC.includes(url.pathname) || url.pathname.startsWith('/app.css');

  if (isStatic) {
    e.respondWith(
      fetch(e.request).then(r => {
        const clone = r.clone();
        caches.open(CACHE).then(c => c.put(e.request, clone));
        return r;
      }).catch(() => caches.match(e.request))
    );
  }
  // All other requests (Blazor circuit, SignalR) go straight to the network.
});
