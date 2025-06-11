const cacheName = {{{JSON.stringify(COMPANY_NAME + "-" + PRODUCT_NAME + "-" + PRODUCT_VERSION )}}};
const contentToCache = [
    "Build/{{{ LOADER_FILENAME }}}?v={{{ PRODUCT_VERSION }}}",
    "Build/{{{ FRAMEWORK_FILENAME }}}?v={{{ PRODUCT_VERSION }}}",
#if USE_THREADS
    "Build/{{{ WORKER_FILENAME }}}?v={{{ PRODUCT_VERSION }}}",
#endif
    "Build/{{{ DATA_FILENAME }}}?v={{{ PRODUCT_VERSION }}}",
    "Build/{{{ CODE_FILENAME }}}?v={{{ PRODUCT_VERSION }}}",
    "TemplateData/style.css",
    "load-sdk.js",
];

self.addEventListener("install", function (e) {
	
  console.log("[Service Worker] Install cacheName=" + cacheName);

  e.waitUntil(
    (async () => {
      // Delete old caches before caching the new content
      const cacheNames = await caches.keys();
      await Promise.all(
        cacheNames
          .filter((name) => name !== cacheName)
          .map((name) => {
            console.log("[Service Worker] Deleting old cache:", name);
      //callFunctionInClient();
            return caches.delete(name);
          })
      );

      // Open cache and store content
      const cache = await caches.open(cacheName);
      console.log("[Service Worker] Caching all: app shell and content");
      await cache.addAll(contentToCache);
      console.log("Caching completed during install.");

      self.skipWaiting(); // Activate service worker immediately
    })()
  );
  
});

self.addEventListener('activate', event => {
	const currentCaches = [cacheName];
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return cacheNames.filter(cacheName => !currentCaches.includes(cacheName));
    }).then(cachesToDelete => {
      return Promise.all(cachesToDelete.map(cacheToDelete => {
		   console.log("[Service Worker] Deleting 1 old cache:", name);
			//callFunctionInClient();
        return caches.delete(cacheToDelete);
      }));
    }).then(() => {
		console.log('Service Worker is fully activated.');
		})
  );
  
  self.clients.claim().then(() => {
             return self.clients.matchAll({ includeUncontrolled: true }).then(clients => {
                 clients.forEach(client => {
                     client.postMessage('serviceWorkerReady'); // Notify main thread
                 });
             });
         })
});

self.addEventListener("fetch", function (event) {
	if (event.request.method !== 'GET') {
		console.log(`[Service Worker] Skip caching resource: ${event.request.url}`);
        // Skip caching for non-GET requests
        return;
    }
  
  // Only cache if it is game content data
  if (!isStaticResource(event.request))
  {
	  return null;
  }
  
  //new
  if (event.request.method === 'POST') {
    // Always fetch from the network for POST requests
    event.respondWith(
      fetch(event.request)
        .then(response => {
          // Return the response directly from the network
          return response;
        })
        .catch(error => {
          // Handle network errors (e.g., show an error page)
          return new Response('Network error occurred', {
            status: 500,
            statusText: 'Network Error'
          });
        })
    );
  } else {
    // Handle other types of requests (GET, PUT, etc.) as needed
    event.respondWith(
    (async function () {
      let response = await caches.match(event.request);
      console.log(`[Service Worker] Fetching resource: ${event.request.url}`);
      if (response) {
        return response;
      }

      response = await fetchAndCacheIfNeeded(event.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${event.request.url}`);
      cache.put(event.request, response.clone());
      return response;
    })()
  );
  }
});

async function fetchAndCacheIfNeeded(url) {
    const cache = await caches.open('my-cache');
    const cachedResponse = await cache.match(url);
    if (!cachedResponse) {
        const response = await fetch(url);
        await cache.put(url, response.clone());
        return response;
    }
    return cachedResponse;
}

// Listening for messages from the client
self.addEventListener('message', function(event) {
    if (event.data === 'skipWaiting') {
        self.skipWaiting();
    }
});

// Helper function to check if the request is for static resources
function isStaticResource(request) {
  return contentToCache.some((resource) => request.url.includes(resource));
}
// Inform clients about the new version
// Notify all controlled clients when caching is complete
// self.addEventListener('activate', event => {
    // event.waitUntil(
        // self.clients.claim().then(() => {
            // return self.clients.matchAll({ includeUncontrolled: true }).then(clients => {
                // clients.forEach(client => {
                    // client.postMessage('serviceWorkerReady'); // Notify main thread
                // });
            // });
        // })
    // );
// });

// Notify main thread when caching is done
self.addEventListener('message', event => {
    if (event.data === 'isCachingComplete') {
        console.log('Caching complete. Sending confirmation to client.');
        event.source.postMessage('cachingComplete');
    }
});

// self.addEventListener('activate', (event) => {
  // event.waitUntil(
    // clients.matchAll().then(clients => {
      // if (clients && clients.length) {
        // clients[0].postMessage({ action: 'forceReloadPage' });
      // }
    // })
  // );
// });

function callFunctionInClient() {
	console.log('callFunctionInClient');
  self.clients.matchAll().then((clients) => {
    clients.forEach((client) => {
		console.log('callFunctionInClient1');
      client.postMessage('forceReloadPage');
    });
  });
}