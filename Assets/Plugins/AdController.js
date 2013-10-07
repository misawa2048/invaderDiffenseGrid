#pragma strict
// AdController plugin script.

import System.Runtime.InteropServices;

public enum AD_POSITION{
	BOTTOM,
	TOP
};

#if UNITY_IPHONE
	private static var banner : ADBannerView = null;
#endif
private static var isBannerLoaded : boolean = false;
private static var instance : AdController = null;
private var publisherId : String;
private static var isBottom : boolean;
private static var isOnceMode : boolean;
private static var messageReceiveObj : GameObject;

// Call this at least once before using the plugin.
static function Install(_messageReceiveObj:GameObject) {
    if (instance == null) {
        var master = new GameObject("AdController");
        DontDestroyOnLoad(master);
        instance = master.AddComponent.<AdController>();
    }
    if(_messageReceiveObj != null){
    	messageReceiveObj = _messageReceiveObj;
    }
}

function Awake() {  InstallPlatform();  }
function Update(){  UpdatePlatform();   }
function OnApplicationPause(_bool : boolean){
	if(isBannerLoaded){
		if(isOnceMode){
			if(_bool && Input.anyKey){
#if (UNITY_ANDROID && !UNITY_EDITOR)
#elif (UNITY_IPHONE && !UNITY_EDITOR)
				banner.Hide();
#endif
				if(messageReceiveObj!=null){
					messageReceiveObj.SendMessage("OnAdBannerClicked");
				}
			}
		}
	}
}
// Set and load ad.
static function SetAndLoadAd(publisherId : String, pos : AD_POSITION, isOnceMode : boolean) {
  if(instance != null){
	instance.publisherId = publisherId;
	instance.isBottom = (pos == AD_POSITION.BOTTOM ? true : false);
	instance.isOnceMode = isOnceMode;
    SetAndLoadAdPlatform();
  }
}

// get locale.
static function GetLocale() : int { return GetLocalePlatform(); }

#if (UNITY_ANDROID && !UNITY_EDITOR)
// Android platform implementation.
private static var activity : AndroidJavaObject = null;
private static function InstallPlatform() {
  var unityPlayerClass : AndroidJavaClass;
  unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
  if(unityPlayerClass != null){
    activity = unityPlayerClass.GetStatic.<AndroidJavaObject>("currentActivity");
  }
}
private static function UpdatePlatform() { }
private static function SetAndLoadAdPlatform() {
  if((instance != null)&&(activity != null)){
    activity.Call("setAndLoadAd", instance.publisherId, instance.isBottom);
    isBannerLoaded = true;
  }
}
private static function GetLocalePlatform() : int {
  var ret : int = 1;
  if((instance != null)&&(activity != null)){
    ret = activity.Call.<int>("getLocale");
  }
  return ret;
}

#elif (UNITY_IPHONE && !UNITY_EDITOR)
private static function InstallPlatform() { }
private static function UpdatePlatform() {
		if(!isBannerLoaded && banner!=null && banner.loaded && banner.error==null){
			isBannerLoaded = true;
			banner.Show();
		}
}
private static function SetAndLoadAdPlatform() { 
		banner = new ADBannerView();
		if(banner!=null){
			banner.autoSize = true;
			banner.autoPosition = (isBottom?ADPosition.Bottom:ADPosition.Top);
		}
}
private static function GetLocalePlatform() : int { return 0; }
#else
private static function InstallPlatform() { }
private static function UpdatePlatform() {}
private static function SetAndLoadAdPlatform() { }
private static function GetLocalePlatform() : int { return 0; }
#endif
