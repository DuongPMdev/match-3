var fullSize = false;
const cacheName = {{{JSON.stringify(COMPANY_NAME + "-" + PRODUCT_NAME + "-" + PRODUCT_VERSION )}}};
  var unityInstanceRef;
  var unsubscribe;
  var container = document.querySelector("#unity-container");
  var canvas = document.querySelector("#unity-canvas");
  var loadingBar = document.querySelector("#unity-loading-bar");
  var progressBarFull = document.querySelector("#unity-progress-bar-full");
  var warningBanner = document.querySelector("#unity-warning");

  // Shows a temporary message banner/ribbon for a few seconds, or
  // a permanent error message on top of the canvas if type=='error'.
  // If type=='warning', a yellow highlight color is used.
  // Modify or remove this function to customize the visually presented
  // way that non-critical warnings and error messages are presented to the
  // user.
  function unityShowBanner(msg, type) {
    function updateBannerVisibility() {
      warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
    }
    var div = document.createElement('div');
    div.innerHTML = msg;
    warningBanner.appendChild(div);
    if (type == 'error') div.style = 'background: red; padding: 10px;';
    else {
      if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
      setTimeout(function() {
        warningBanner.removeChild(div);
        updateBannerVisibility();
      }, 5000);
    }
    updateBannerVisibility();
  }
	
	// loading
	var progress = 0;  // Simulate progress for testing
	var fakeProgress = 0;  // Simulate progress for testing
  var dotCount = 1;
  var loadingInterval;
  var inPreparingState = false;
  const progressContainer = document.querySelector('.progress-container');
  setPercentage(5);
  function startLoadingLoop() {
    loadingInterval = setInterval(() => {
      dotCount = (dotCount % 3) + 1;  // Cycle between 1, 2, and 3 dots
      document.getElementById('dots').innerHTML = '.'.repeat(dotCount);
      
      // Simulate increasing progress (remove this in the actual project)
      fakeProgress += 2;
      if(progress < 80)
      {
        if(fakeProgress < progress)
        {
          fakeProgress = progress;
        }
        if(fakeProgress >= 80)
        {
          fakeProgress = 80;
          if(!inPreparingState)
          {
            document.getElementById('loadingText').innerHTML = 'Preparing data<span id="dots">.</span>';
          }
          
          inPreparingState = true;
        }
        
        setPercentage(fakeProgress);
      }
    }, 200);  // Update every 200ms
  }
  
  function stopLoadingLoop() {
    clearInterval(loadingInterval);
  }
        
  // Call this function to start the loading loop
  startLoadingLoop();

  // Simulate Unity's loading completion (for testing purposes)
  // setTimeout(() => {
      // progress = 100;
  // }, 5000); 

  var buildUrl = "Build";
  var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}?v={{{PRODUCT_VERSION}}}";
  var config = {
    dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}?v={{{PRODUCT_VERSION}}}",
    frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}?v={{{PRODUCT_VERSION}}}",
#if USE_THREADS
    workerUrl: buildUrl + "/{{{ WORKER_FILENAME }}}?v={{{PRODUCT_VERSION}}}",
#endif
#if USE_WASM
    codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}?v={{{PRODUCT_VERSION}}}",
#endif
#if MEMORY_FILENAME
    memoryUrl: buildUrl + "/{{{ MEMORY_FILENAME }}}?v={{{PRODUCT_VERSION}}}",
#endif
#if SYMBOLS_FILENAME
    symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}?v={{{PRODUCT_VERSION}}}",
#endif
    streamingAssetsUrl: "StreamingAssets",
    companyName: {{{ JSON.stringify(COMPANY_NAME) }}},
    productName: {{{ JSON.stringify(PRODUCT_NAME) }}},
    productVersion: {{{ JSON.stringify(PRODUCT_VERSION) }}},
    showBanner: unityShowBanner,
	  cacheControl: function (url) {
  //return "immutable";
     return "no-store";
    },
  };

  // By default Unity keeps WebGL canvas render target size matched with
  // the DOM size of the canvas element (scaled by window.devicePixelRatio)
  // Set this to false if you want to decouple this synchronization from
  // happening inside the engine, and you would instead like to size up
  // the canvas DOM size and WebGL render target sizes yourself.
  // config.matchWebGLToCanvasSize = false;

  render();

#if BACKGROUND_FILENAME
  canvas.style.background = "url('" + buildUrl + "/{{{ BACKGROUND_FILENAME.replace(/'/g, '%27') }}}') center / cover";
#endif
  loadingBar.style.display = "block";
var isChangeText = false;

if ('serviceWorker' in navigator) {
  navigator.serviceWorker.register('ServiceWorker.js')
      .then(function(registration) {
          console.log('ServiceWorker registration successful with scope: ', registration.scope);

          registration.addEventListener('updatefound', function() {
              const newWorker = registration.installing;

              newWorker.addEventListener('statechange', function() {
                  if (newWorker.state === 'installed') {
                      if (navigator.serviceWorker.controller) {
                          // Check if this is the first install
                          if (!localStorage.getItem('firstSWInstalled')) {
                              // Set the flag for future updates
                              localStorage.setItem('firstSWInstalled', 'true');
                          } else {
                              // Not the first launch, so notify for updates
                              notifyUserAboutUpdate();
                          }
                      }
                  }
              });
          });
			
      }).catch(function(error) {
          console.log('ServiceWorker registration failed: ', error);
      });

  // Listening for messages from the Service Worker
  navigator.serviceWorker.addEventListener('message', function(event) {
      if (event.data === 'newVersionAvailable') {
          if (localStorage.getItem('firstSWInstalled')) {
              notifyUserAboutUpdate();
          }
      }
  });
	
	navigator.serviceWorker.addEventListener('message', (event) => {
			  if (event.data.action === 'forceReloadPage') {
				//forceReloadPage();
			  }
			});
}

var cacheChecking = checkAndClearCache(cacheName);

var script = document.createElement("script");
script.src = loaderUrl;
script.onload = () => {
createUnityInstance(canvas, config, (progressUnity) => {
  //progressBarFull.style.width = 100 * progress + "%";
  progress = 100 * progressUnity;
  if(progress > fakeProgress)
  {
    setPercentage(progress);
  }
  //console.log("Progress=" + progress);
  if(progress >= 80 && isChangeText == false)
  {
    isChangeText = true;
    document.getElementById('loadingText').innerHTML = 'Loading data<span id="dots">.</span>';
  }
  //Module.setStatus("Loading... " + Math.round(progress * 100) + "%");
}).then((unityInstance) => {
  unityInstanceRef = unityInstance;
  //stopLoadingLoop();
  //Module.setStatus("Loading complete!");
  //document.getElementById('loadingText').style.display = 'none';
  loadingBar.style.display = "none";
  stopLoadingLoop();
}).catch((message) => {
  stopLoadingLoop();
  console.log("Error loading Unity");
  //alert(message);
  self.location.reload();
});
};
document.body.appendChild(script);
	
  
// Resize
function render() {
  if(fullSize)
	{
		 renderFullSize();
		 return;
	}
	
  const userAgent = navigator.userAgent.toLowerCase();
  if (userAgent.includes("mac os")) {
	var width = canvas.clientWidth;
	var height = canvas.clientHeight;
	if (canvas.width != width || canvas.height != height) {
		canvas.width = width;
		canvas.height = height;
			  
		// in this case just render when the window is resized.
	    if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
			// Mobile device style: fill the whole browser client area with the game canvas:
			var meta = document.createElement('meta');
			meta.name = 'viewport';
			meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
			document.getElementsByTagName('head')[0].appendChild(meta);
		}
		else {
			var ratio = 1080/2160;

			canvas.style.width  = window.innerHeight * ratio + "px";
			canvas.style.height = "100%";

			canvas.height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
			canvas.width = canvas.height * ratio;
			canvas.style.position = "fixed";
			canvas.style.left = ((window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) / 2 - canvas.width / 2) + "px";
		}
	}  
  }
  else 
  {
	  if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
			// Mobile device style: fill the whole browser client area with the game canvas:
			var meta = document.createElement('meta');
			meta.name = 'viewport';
			meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
			document.getElementsByTagName('head')[0].appendChild(meta);
	  }
	  else if (/Android/i.test(navigator.userAgent)) {
		  // Mobile device style: fill the whole browser client area with the game canvas:
		  var meta = document.createElement('meta');
		  meta.name = 'viewport';
		  meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no';
		  document.getElementsByTagName('head')[0].appendChild(meta);
		  
		  // Lock rotation handling for portrait-only mode
		  lockOrientation();
	  
		  canvas.style.width = "100%";
		  canvas.style.height = "100%";
		  canvas.style.position = "fixed";
		  canvas.style.left = "0px";
	  }
	  else {
		  var ratio = 1080/2160;

		  canvas.style.width  = window.innerHeight * ratio + "px";
		  canvas.style.height = "100%";

		  canvas.width = window.innerHeight * ratio;
		  canvas.height = window.innerHeight;
		  
		  canvas.style.position = "fixed";
		  canvas.style.left = ((window.innerWidth - canvas.width) / 2) + "px";
	  }
  }
};

function renderFullSize() {
  const userAgent = navigator.userAgent.toLowerCase();
  if (userAgent.includes("mac os")) {
	var width = canvas.clientWidth;
	var height = canvas.clientHeight;
	if (canvas.width != width || canvas.height != height) {
		canvas.width = width;
		canvas.height = height;
			  
		// in this case just render when the window is resized.
	    if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
			// Mobile device style: fill the whole browser client area with the game canvas:
			var meta = document.createElement('meta');
			meta.name = 'viewport';
			meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
			document.getElementsByTagName('head')[0].appendChild(meta);
		}
		else {
			var ratio = 1080/2160;

			canvas.style.width  = "100%";
			canvas.style.height = "100%";

			//canvas.height = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
			//canvas.width = canvas.height * ratio;
			canvas.style.position = "fixed";
			canvas.style.left = ((window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) / 2 - canvas.width / 2) + "px";
			canvas.style.left = "0px";
		}
	}  
  }
  else 
  {
	  if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
			// Mobile device style: fill the whole browser client area with the game canvas:
			var meta = document.createElement('meta');
			meta.name = 'viewport';
			meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
			document.getElementsByTagName('head')[0].appendChild(meta);
	  }
	  else if (/Android/i.test(navigator.userAgent)) {
		  // Mobile device style: fill the whole browser client area with the game canvas:
		  var meta = document.createElement('meta');
		  meta.name = 'viewport';
		  meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no';
		  document.getElementsByTagName('head')[0].appendChild(meta);
		  
		  // Lock rotation handling for portrait-only mode
		  lockOrientation();
	  
		  canvas.style.width = "100%";
		  canvas.style.height = "100%";
		  canvas.style.position = "fixed";
		  canvas.style.left = "0px";
	  }
	  else {
		  var ratio = 1080/2160;

		  //canvas.style.width  = window.innerHeight * ratio + "px";
		  canvas.style.width  = "100%";
		  canvas.style.height = "100%";

		  //canvas.width = window.innerHeight * ratio;
		  //canvas.height = window.innerHeight;
		  
		  canvas.style.position = "fixed";
		  //canvas.style.left = ((window.innerWidth - canvas.width) / 2) + "px";
		  canvas.style.left = "0px";
	  }
  }
};

function lockOrientation() {
  window.addEventListener('resize', handleOrientationChange);
  window.addEventListener('orientationchange', handleOrientationChange);

  function handleOrientationChange() {
    if (window.innerWidth > window.innerHeight) {
      // Display an alert or custom UI for locked portrait orientation
      alert("Please rotate your device back to portrait mode.");
    }
  }
}
		
function resizeCanvas() {
  // var width = canvas.clientWidth;
  // var height = canvas.clientHeight;
  // if (canvas.width != width ||
    // canvas.height != height) {
  // canvas.width = width;
  // canvas.height = height;
      
  // // in this case just render when the window is resized.
  // render();
  
  // Do nothing or set fixed size
    //canvas.width = 1080;
    //canvas.height = 1920;

}

// Call the render function when the page is ready
window.addEventListener('load', render);
window.addEventListener('resize', render); // Update on window resize


function setPercentage(pecent) {
	if(pecent < 5)
	{
		pecent = 5;
	}
  const percentage = pecent
  + '%';
  
  const progressEl = progressContainer.querySelector('.progress');
  //const percentageEl = progressContainer.querySelector('.percentage');
  
  progressEl.style.width = percentage;
  //percentageEl.innerText = percentage;
  //percentageEl.style.left = percentage;
}

// Caching control
var isReload = false;
var unityReady = false;

async function checkAndClearCache(fileUrl) {
  let fileFound = false;
  const specificName = {{{JSON.stringify(COMPANY_NAME + "-" + PRODUCT_NAME)}}}; // Replace with the name or substring to search
  
  const cacheNames = await caches.keys();
      await Promise.all(
        cacheNames
          .filter((name) => {
			if (name !== cacheName) {
			  if (name.includes(specificName)) {  // Check if name contains the target name
				//console.log(`[Service Worker] Deleting cache containing "${specificName}":`, name);
				//alert("cache containing " + name);
				fileFound = true;
			  } else {
				//console.log(`[Service Worker] Skipping cache: ${name}`);
			  }
			  return true;
			}
			return false;
		  })
          .map((name) => {
            console.log("[Service Worker] Deleting old cache:", name);
            return caches.delete(name);
          })
      );

  if (fileFound) {
    // Delete all caches if the file is not found
    console.log('Delete all cache!');
	//notifyUserAboutUpdate();
	//alert("Delete all cache and reload page!");
	self.location.reload();
  } else {
    console.log('Using cache for fast loading!');
  }
}

function forceReloadPage() 
{
	console.log('Service worker triggered this function!');
	alertAndForceUserAboutUpdate();
}
			
function notifyUserAboutUpdate() {
    alertAndForceUserAboutUpdate();
}

function notifyUserAboutUpdateByClickButton() {
	// Show a custom update message to the user
  const updateMessage = document.createElement('div');
  
  //updateMessage.innerText = 'New version available. Click to update!';
  updateMessage.style.position = 'fixed';
  updateMessage.style.bottom = '0';
  updateMessage.style.width = '100%';
  updateMessage.style.height = '100%';
  updateMessage.style.backgroundColor = 'transparent';
  updateMessage.style.textAlign = 'center';
  //updateMessage.style.color = '#FFFFFF';
  updateMessage.style.fontSize = '23px';
  updateMessage.style.padding = '10px';
  updateMessage.style.zIndex = '1000';
	document.body.appendChild(updateMessage);
	
	const popMessage = document.createElement('div');
	popMessage.innerText = 'New version available. Click to update!';
  popMessage.style.position = 'fixed';
  popMessage.style.bottom = '0';
  popMessage.style.width = '100%';
  popMessage.style.backgroundColor = '#29f051';
  popMessage.style.textAlign = 'center';
  popMessage.style.color = '#FFFFFF';
  popMessage.style.fontSize = '20px';
  popMessage.style.padding = '10px';
  popMessage.style.zIndex = '1000';
	
	updateMessage.appendChild(popMessage);
	
  updateMessage.addEventListener('click', () => {
      ForceReload();
    //alert('The application has been updated. Please clear your browser cache to ensure you have the latest version.');
  });
	
}

function alertAndForceUserAboutUpdate() {
	alert("New version available. Press OK to update!");
    ForceReload();
}

function ForceReload()
{
	//console.log('ServiceWorker Update 2!!!');
	if (navigator.serviceWorker.controller) {
            navigator.serviceWorker.controller.postMessage('skipWaiting');
        }
		
		self.location.reload();
		//alert('The application has been updated. Please clear your browser cache to ensure you have the latest version.');
}

// Purchase
function sendTelegramPayment(botToken, providerToken, chatId, amount, currency) {
    const url = `https://api.telegram.org/bot${botToken}/sendInvoice`;

    const payload = JSON.stringify({
        unique_id: `${chatId}_${Date.now()}`,  // Unique payload ID
        data: '100 stars purchase'
    });

    const data = {
        chat_id: chatId,
        title: 'Buy 100 Stars',
        description: 'Purchase 100 Stars to use in the app',
        payload: payload,
        provider_token: providerToken,
        //start_parameter: 'get_100_stars',
        currency: currency,
        prices: [
            { label: '100 Stars', amount: amount }  // Define the item and its price
        ],
        photo_url: 'https://example.com/star-icon.png',  // Optional: image URL for the product
        photo_width: 512,
        photo_height: 512,
        need_shipping_address: false,  // Set true if you need the shipping address
        is_flexible: false  // Set true if the final price depends on shipping
    };

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(response => response.json())
    .then(result => {
        console.log('Payment sent:', result);
    })
    .catch(error => {
        console.error('Error sending payment:', error);
    });
}

function openInvoice(invoice_url)
{
	// Open the invoice
	try
	{
		window.Telegram.WebApp.openInvoice(invoice_url);
	}
	catch(error)
	{
		console.error(error.message);
		//console.log("TUK -- " + error.message);
		unityInstanceRef.SendMessage("GameElement", "ShowLogPopup", "Errors detected! Use Telegram App or update to a version above 6.0!"); 
	}
}

window.Telegram.WebView.onEvent('invoice_closed', onInvoiceCloseCustom);

function onInvoiceCloseCustom(eventType, eventData)
{
	console.log("T-", JSON.stringify(eventData));
	if(unityInstanceRef != null)
	{
		if(eventData.status == "paid")
		{
			unityInstanceRef.SendMessage("GameElement", "OnPurchaseSuccess", JSON.stringify(eventData)); 
		}
	}
}

function isSupportStarPurchase()
{
	//if(Telegram && Telegram.WebApp.isVersionAtLeast('6.1'))
		return true;
	//return false;
}

// Ads
async function showADBanner(type)
  {
	  var data = JSON.parse(type);
	  //console.log("TUK data" + data.blockId);
	  //console.log("TUK data type" + data.type);
	  const BannerAdController = await window.Adsgram.init({ blockId: data.blockId, debug: false, debugBannerType: "FullscreenMedia" });
	  if(BannerAdController)
	  {
		  BannerAdController.show().then((result) => {
			// user watch ad till the end
			// your code to reward user
			//console.log("ADr" + result);
			
			console.log("AD Completed: " + JSON.stringify(result));
			if(unityInstanceRef)
			{
				unityInstanceRef.SendMessage("MegaADHandler", "OnRewardCompleted", JSON.stringify(data.type));
			}
			telemetreeTrackingStr("ADGram-Success|" + data.type);
			}).catch((result) => {
				// user get error during playing ad or skip ad
				// do nothing or whatever you want
				console.log("AD Failed: " + JSON.stringify(result));
				if(unityInstanceRef)
				{
					unityInstanceRef.SendMessage("MegaADHandler", "OnLoadFail", JSON.stringify(data.type));
				}
		  })
	  }
  }
  
  async function showADReward(type)
  {
	  var data = JSON.parse(type);
	  //console.log("TUK data" + data.blockId);
	  //console.log("TUK data type" + data.type);
	  const BannerAdController = await window.Adsgram.init({ blockId: data.blockId, debug: false, debugBannerType: "RewardedVideo" });
	  if(BannerAdController)
	  {
		  BannerAdController.show().then((result) => {
			// user watch ad till the end
			// your code to reward user
			//console.log("ADr" + result);
			
			console.log("AD Completed: " + JSON.stringify(result));
			if(unityInstanceRef)
			{
				unityInstanceRef.SendMessage("MegaADHandler", "OnRewardCompleted", JSON.stringify(data.type));
			}
			telemetreeTrackingStr("ADGram-Success|" + data.type);
			}).catch((result) => {
				// user get error during playing ad or skip ad
				// do nothing or whatever you want
				console.log("AD Failed: " + JSON.stringify(result));
				if(unityInstanceRef)
				{
					unityInstanceRef.SendMessage("MegaADHandler", "OnLoadFail", JSON.stringify(data.type));
				}
		  })
	  }
  }
  
// Param
function getLaunchParams()
{
	var launchParams = JSON.stringify(window.Telegram.WebApp);
	//console.log('launchParams = ', launchParams);
	return launchParams;
}

function getUriParams()
{
	var launchParams = JSON.stringify(window.location.href);
	console.log('URI = ', launchParams);
	try{
		console.log("iframes1:" + window.top.location.href);
		console.log("iframes2:" + window.self.location.href);
	}
	catch(error)
	{
		
	}
	return launchParams;
}

function getUriFromIframeParams()
{
	// Detect if inside an iframe
      const iframe = window.top.document.querySelector('iframe.payment-verification');
	  console.log("iframes:" + iframe);
	  console.log("iframes1:" + window.top.location.href);
	  console.log("iframes2:" + window.self.location.href);
      if (iframe) {
          return iframe.src; // Found the current iframe
      }
  
  return "";
  
	// var launchParams = JSON.stringify(window.location.href);
	// console.log('URI = ', launchParams);
	// return launchParams;
}

window.addEventListener("message", (event) => {
  if (event.data && event.data.action === "getIframeSrc") {
    event.source.postMessage(
      { action: "iframeSrcResponse", src: window.location.href },
      event.origin
    );
  }
});
  
  // Tracking
function telemetreeTrackingStr(data)
{
  if(telemetreeBuilder)
  {
    telemetreeBuilder.track(data, data);
  }
}

function telemetreeTracking(data)
{
  if(telemetreeBuilder)
  {
    var trackingData = JSON.parse(data);
    telemetreeBuilder.track(trackingData.t, trackingData.e);
  }
}
  
function getTelegramID()
{
  const tg = window.Telegram.WebApp;

          // Check if user data is available
          if (tg.initDataUnsafe && tg.initDataUnsafe.user) {
              const userId = tg.initDataUnsafe.user.id;
      console.log('[T1]userId = ', userId);
      console.log('[T1]userId_ = ', JSON.stringify(userId));
      return userId;
              //document.getElementById("userId").textContent = `User ID: ${userId}`;
          } else {
              //document.getElementById("userId").textContent = "User ID not available.";
      return "";
          }
}
  
function openLink(data)
{
  console.log('openLink = ', data);
  Telegram?.WebApp?.openLink(data);
}

// ---------------------------- Sol --------------------------

function GetSOLAccount() {
		if(solPlugin)
		{
			console.log("SOL key: " + solPlugin.getPublicKey());
			return solPlugin.getPublicKey();
		}
		else
		{
			return "";
		}
      //document.getElementById("showGetAccount").innerHTML = plugin.getPublicKey();
    }
	
async function GetSOLBalance() {
      const balance = await  plugin.getBalance();
	  console.log("SOL balance: " + balance);
	  return balance;
      //document.getElementById("showGetBalance").innerHTML = balance;
    }

async function SOLPurchase(userId, itemId, price) {
      //const result = await plugin.purchaseItem("test-user-id", "test-item-id", "0,1");
      const result = await plugin.purchaseItem(userId, itemId, price);
      console.log("SOL Purchase: " + result);
    }
