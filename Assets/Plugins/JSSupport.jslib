mergeInto(LibraryManager.library, {

	IsIOSBrowser: function () {
        return (/iPhone|iPad|iPod/i.test(navigator.userAgent));
      },
	  
    IsAndroidBrowser: function () {
        return (/Android/i.test(navigator.userAgent));
      },
	  
    IsDesktopBrowser__deps: ['IsIOSBrowser', 'IsAndroidBrowser'],
    IsDesktopBrowser: function () {
        return !_IsIOSBrowser() && !_IsAndroidBrowser();
      },
	  
	GetLaunchParams: function() {
        var result = getLaunchParams();
		//console.log("getLaunchParams:: " + result);
		
		//Get size of the string
	   var bufferSize = lengthBytesUTF8(result) + 1;
	   //Allocate memory space
	   var buffer = _malloc(bufferSize);
	   //Copy old data to the new one then return it
	   stringToUTF8(result, buffer, bufferSize);
	   //console.log("getLaunchParams 2:: " + buffer);
	   return buffer;
    },
	GetURIParams: function() {
        var result = getUriParams();
		//console.log("getLaunchParams:: " + result);
		
		//Get size of the string
	   var bufferSize = lengthBytesUTF8(result) + 1;
	   //Allocate memory space
	   var buffer = _malloc(bufferSize);
	   //Copy old data to the new one then return it
	   stringToUTF8(result, buffer, bufferSize);
	   //console.log("getLaunchParams 2:: " + buffer);
	   return buffer;
    },
	GetURIFromIframeParams: function() {
        var result = getUriFromIframeParams();
		//console.log("getLaunchParams:: " + result);
		
		//Get size of the string
	   var bufferSize = lengthBytesUTF8(result) + 1;
	   //Allocate memory space
	   var buffer = _malloc(bufferSize);
	   //Copy old data to the new one then return it
	   stringToUTF8(result, buffer, bufferSize);
	   //console.log("getLaunchParams 2:: " + buffer);
	   return buffer;
    },
	IsRunningInTelegramApp: function () {
		//console.log("location " + location);
		//console.log("navigator.userAgent " + navigator.userAgent);
		return location.hash.includes("tgWebAppData");
    },
	IsMetaMask: function () {
		if (window.ethereum) {
			console.log('Ethereum support is available');
			if (window.ethereum.isMetaMask) {
			  console.log('MetaMask is active');
			  return true;
			} else {
			  console.log('MetaMask is not available');
			  return false;
			}
		  } else {
			console.log('Ethereum support is not found');
			return false;
		}
    },
	NewVersionControl: function () {
		notifyUserAboutUpdate();
    }
});
