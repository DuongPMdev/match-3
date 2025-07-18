
  function tracking(data)
  {
	  if(telemetreeBuilder)
	  {
		  var trackingData = JSON.parse(data);
		  telemetreeBuilder.track(trackingData.t, trackingData.e)
	  }
  }
