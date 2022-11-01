using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class ADManager : MonoBehaviour
{
    string bannerPlacement = "Banner_NumberSpider";
    bool bannerShow = false;

#if UNITY_IOS
    public const string gameID = "3638954"; 
#elif UNITY_ANDROID
    public const string gameID = "3638955";
#else
    public const string gameID = "3638955";
#endif
    private void Awake()
    {
        Advertisement.Initialize(gameID, false);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        StartCoroutine(ADStart());
    }
    IEnumerator ADStart()
    {
        while(true)
        {
            yield return GameManager.inst.wait00_05;
            ShowAD();
            if (bannerShow)
                break;
        }
    }
    //광고 보여주기.
    public void ShowAD()
    {
        if (Advertisement.IsReady())
        {
            StartCoroutine(ShowBannerWhenReady());
        }
    }
    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerPlacement))
        {
            yield return GameManager.inst.wait00_05;
        }
        Advertisement.Banner.Show(bannerPlacement);
        bannerShow = true;
    }
    public void ClosedAD()
    {
        bannerShow = false;
        Advertisement.Banner.Hide();
    }

    //비디오 광고 보여주기
    public void ShowVidioAD()
    {
        if (Advertisement.IsReady())
            Advertisement.Show("video");
    }

    //리워드 비디오 광고 보여주기
    public void ShowRewardVidioAD()
    {
        if (Advertisement.IsReady())
            Advertisement.Show("rewardedVideo");
    }
}
