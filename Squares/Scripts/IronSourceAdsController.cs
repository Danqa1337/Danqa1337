//using System;
//using System.Collections;
//using UnityEngine;

//public class IronSourceAdsController : Singleton<IronSourceAdsController>
//{
//    private void Start()
//    {
//        LoadAd();
//        DestroyBanner();
//    }

//    private void OnApplicationPause(bool pause)
//    {
//        IronSource.Agent.onApplicationPause(pause);
//    }

//    private void OnEnable()
//    {
//        Player.OnPlayerDied += ShowBanner;
//        AdsTimer.OnTimerZero += ShowAd;
//        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
//        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
//        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
//        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
//        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
//        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

//        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
//        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
//        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
//        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
//        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
//        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
//        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
//    }

//    private void OnDisable()
//    {
//        Player.OnPlayerDied -= ShowBanner;
//        AdsTimer.OnTimerZero -= ShowAd;
//        IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
//        IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
//        IronSourceBannerEvents.onAdClickedEvent -= BannerOnAdClickedEvent;
//        IronSourceBannerEvents.onAdScreenPresentedEvent -= BannerOnAdScreenPresentedEvent;
//        IronSourceBannerEvents.onAdScreenDismissedEvent -= BannerOnAdScreenDismissedEvent;
//        IronSourceBannerEvents.onAdLeftApplicationEvent -= BannerOnAdLeftApplicationEvent;

//        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
//        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
//        IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
//        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
//        IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
//        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
//        IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;
//    }

//    private void InterstitialOnAdClosedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Ad closed");
//    }

//    private void InterstitialOnAdShowFailedEvent(IronSourceError arg1, IronSourceAdInfo arg2)
//    {
//        Debug.Log("Ad Show Failed");
//    }

//    private void InterstitialOnAdClickedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Ad Clicked");
//    }

//    private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Ad Succeeded");
//    }

//    private void InterstitialOnAdOpenedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Ad Opened");
//    }

//    private void InterstitialOnAdLoadFailed(IronSourceError obj)
//    {
//        Debug.Log("Ad Load Failed");
//    }

//    private void InterstitialOnAdReadyEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Ad ready");
//    }

//    private void BannerOnAdLeftApplicationEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Banner left");
//    }

//    private void BannerOnAdScreenDismissedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Banner Dismissed");
//    }

//    private void BannerOnAdScreenPresentedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Banner Presented");
//    }

//    private void BannerOnAdClickedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Banner Clicked");
//    }

//    private void BannerOnAdLoadFailedEvent(IronSourceError obj)
//    {
//        Debug.Log("Banner Load Failed");
//    }

//    private void BannerOnAdLoadedEvent(IronSourceAdInfo obj)
//    {
//        Debug.Log("Banner Loaded");
//        IronSource.Agent.displayBanner();
//    }

//    private void LoadBanner()
//    {
//        //IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
//    }

//    public void LoadAd()
//    {
//        //Debug.Log("Loading ad");
//        //IronSource.Agent.loadInterstitial();
//    }

//    public void ShowAd()
//    {
//        //if (IronSource.Agent.isInterstitialReady())
//        //{
//        //    IronSource.Agent.showInterstitial();
//        //}
//    }

//    public void ShowBanner()
//    {
//        //LoadBanner();
//    }

//    private void DestroyBanner()
//    {
//        //IronSource.Agent.destroyBanner();
//    }
//}