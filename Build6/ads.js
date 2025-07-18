
  function showBanner(type)
  {
	  var data = JSON.parse(type);
	  console.log("TUK data" + data.blockId);
	  console.log("TUK data type" + data.type);
	  const BannerAdController = window.Adsgram.init({ blockId: data.blockId });
	  if(BannerAdController)
	  {
		  BannerAdController.show().then((result) => {
			// user watch ad till the end
			// your code to reward user
			console.log("ADr" + result);
			if(unityInstanceRef)
			{
				unityInstanceRef.SendMessage("MegaADHandler", "OnRewardCompleted", JSON.stringify(data.type));
			}
			}).catch((result) => {
				// user get error during playing ad or skip ad
				// do nothing or whatever you want
				console.log("TUK false:" + result);
				if(unityInstanceRef)
				{
					unityInstanceRef.SendMessage("MegaADHandler", "OnLoadFail", JSON.stringify(data.type));
				}
		  })
	  }
  }
  
  function showReward(type)
  {
	  var data = JSON.parse(type);
	  console.log("TUK data" + data.blockId);
	  console.log("TUK data type" + data.type);
	  const BannerAdController = window.Adsgram.init({ blockId: data.blockId });
	  if(BannerAdController)
	  {
		  BannerAdController.show().then((result) => {
			// user watch ad till the end
			// your code to reward user
			console.log("ADr" + result);
			if(unityInstanceRef)
			{
				unityInstanceRef.SendMessage("MegaADHandler", "OnRewardCompleted", JSON.stringify(data.type));
			}
			}).catch((result) => {
				// user get error during playing ad or skip ad
				// do nothing or whatever you want
				console.log("AD false:" + result);
				if(unityInstanceRef)
				{
					unityInstanceRef.SendMessage("MegaADHandler", "OnLoadFail", JSON.stringify(data.type));
				}
		  })
	  }
  }
