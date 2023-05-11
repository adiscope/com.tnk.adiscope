using Adiscope;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AdiscopeSdk Example 
/// </summary>
public class AdiscopeExample : MonoBehaviour
{

    private string MEDIA_ID;
    private string USER_ID;
    private string RV_ID;
    private string IT_ID;
    private string OFFERWALL_ID;
    private string OFFERWALL_DETAIL_ID;
    private string OFFERWALL_DETAIL_URL;
    private string OFFERWALL_DEEPLINK_URL;
    private string OFFERWALL_APPLINK_URL;
    private string Find_UNIT_ID;
    private string CALLBACK_TAG;
    private string CHILD_YN;
    private float top;

    public AdiscopeExample()
    {
#if UNITY_IOS
            MEDIA_ID = "86";
            USER_ID = "Unity_Tester_User";
            RV_ID = "UNIT_A";
            IT_ID = "INTER_TEST";
            OFFERWALL_ID = "API_OFFERWALL";
            Find_UNIT_ID = "";
            CALLBACK_TAG = "";
            CHILD_YN = "";
#endif

#if UNITY_ANDROID
            MEDIA_ID = "23";
            USER_ID = "Unity_Tester_User";
            RV_ID = "TEST";
            IT_ID = "INTER_TEST";
            OFFERWALL_ID = "OFFERWALL";
            Find_UNIT_ID = "";
            CALLBACK_TAG = "";
            CHILD_YN = "";
#endif
    }

    // Adiscope Interface
    private Adiscope.Feature.Core core;
    private Adiscope.Feature.OfferwallAd offerwallAd;
    private Adiscope.Feature.RewardedVideoAd rewardedVideoAd;
    private Adiscope.Feature.InterstitialAd interstitialAd;

    // Properties
    private string outputMessage;
    private readonly int fontSize = 30;
    private List<ContentView> views;

    // Mapper
    class AdiscopeItemFetcher {
        private static Dictionary<string, string> offerwallUnitID = new Dictionary<string, string>() {
            { "23", "OFFERWALL" },
            { "86", "API_OFFERWALL" },
            { "96", "API_HEART" }
        };

        private static Dictionary<string, string> secretKeys = new Dictionary<string, string>() {
            { "23", "5b017706d3a145d9a87c1178560114e6" },
            { "86", "3f5ae8e75c2d481d9d0f5ea030e544e9" },
            { "96", "6d7f2cc28fcb446db4981d7ccb7d0db1" }
        };

        public static string FetchMediaScretKey(string mediaID) { return secretKeys[mediaID]; }
        public static string FetchOfferwallUnitID(string mediaID) { return offerwallUnitID[mediaID]; }
    }

    public Vector2 scrollPosition = Vector2.zero;

    private void Start() { this.core = Adiscope.Sdk.GetCoreInstance(); }

    private void OnDisable()
    {
        // Unregister Adiscope Callbacks
        this.offerwallAd.OnOpened -= OnOfferwallAdOpenedCallback;
        this.offerwallAd.OnClosed -= OnOfferwallAdClosedCallback;
        this.offerwallAd.OnFailedToShow -= OnOfferwallAdFailedToShowCallback;


        this.rewardedVideoAd.OnLoaded -= OnRewardedVideoAdLoadedCallback;
        this.rewardedVideoAd.OnFailedToLoad -= OnRewardedVideoAdFailedToLoadCallback;
        this.rewardedVideoAd.OnOpened -= OnRewardedVideoAdOpenedCallback;
        this.rewardedVideoAd.OnClosed -= OnRewardedVideoAdClosedCallback;
        this.rewardedVideoAd.OnRewarded -= OnRewardedCallback;
        this.rewardedVideoAd.OnFailedToShow -= OnRewardedVideoAdFailedToShowCallback;


        this.interstitialAd.OnLoaded -= OnInterstitialAdLoadedCallback;
        this.interstitialAd.OnFailedToLoad -= OnInterstitialAdFailedToLoadCallback;
        this.interstitialAd.OnOpened -= OnInterstitialAdOpenedCallback;
        this.interstitialAd.OnClosed -= OnInterstitialAdClosedCallback;
        this.interstitialAd.OnFailedToShow -= OnInterstitialAdFailedToShowCallback;
    }

    private void RegisterAdiscopeCallback()
    {
        if (this.offerwallAd == null)
        {
            this.offerwallAd = Adiscope.Sdk.GetOfferwallAdInstance();

            this.offerwallAd.OnOpened += OnOfferwallAdOpenedCallback;

            this.offerwallAd.OnClosed += OnOfferwallAdClosedCallback;

            this.offerwallAd.OnFailedToShow += OnOfferwallAdFailedToShowCallback;

        }

        if (this.rewardedVideoAd == null)
        {
            this.rewardedVideoAd = Adiscope.Sdk.GetRewardedVideoAdInstance();

            this.rewardedVideoAd.OnLoaded += OnRewardedVideoAdLoadedCallback;

            this.rewardedVideoAd.OnFailedToLoad += OnRewardedVideoAdFailedToLoadCallback;

            this.rewardedVideoAd.OnOpened += OnRewardedVideoAdOpenedCallback;

            this.rewardedVideoAd.OnClosed += OnRewardedVideoAdClosedCallback;

            this.rewardedVideoAd.OnRewarded += OnRewardedCallback;

            this.rewardedVideoAd.OnFailedToShow += OnRewardedVideoAdFailedToShowCallback;

        }

        if (this.interstitialAd == null)
        {
            this.interstitialAd = Adiscope.Sdk.GetInterstitialAdInstance();

            this.interstitialAd.OnLoaded += OnInterstitialAdLoadedCallback;

            this.interstitialAd.OnFailedToLoad += OnInterstitialAdFailedToLoadCallback;

            this.interstitialAd.OnOpened += OnInterstitialAdOpenedCallback;

            this.interstitialAd.OnClosed += OnInterstitialAdClosedCallback;

            this.interstitialAd.OnFailedToShow += OnInterstitialAdFailedToShowCallback;

        }

    }

    private void OnInitializedCallback(object sender, Adiscope.Model.InitResult args) { this.AddOutputMessage("initialize - args: " + args); }
    private void OnOfferwallAdOpenedCallback(object sender, Adiscope.Model.ShowResult args)
    {
        this.AddOutputMessage("  <= offerwallAd.OnOpened - args: " + args);
    }
    private void OnOfferwallAdClosedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= offerwallAd.OnClosed - args: " + args); }
    private void OnOfferwallAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= offerwallAd.OnFailedToShow - args: " + args); }


    private void OnRewardedVideoAdLoadedCallback(object sender, Adiscope.Model.LoadResult args) { this.AddOutputMessage("  <= rewardedVideoAd.OnLoaded" + args); }
    private void OnRewardedVideoAdFailedToLoadCallback(object sender, Adiscope.Model.LoadFailure args) { this.AddOutputMessage("  <= rewardedVideoAd.OnFailedToLoad - args: " + args); }
    private void OnRewardedVideoAdOpenedCallback(object sender, Adiscope.Model.ShowResult args) { this.AddOutputMessage("  <= rewardedVideoAd.OnOpened - args: " + args); }
    private void OnRewardedVideoAdClosedCallback(object sender, Adiscope.Model.ShowResult args)
    {
        this.AddOutputMessage("  <= rewardedVideoAd.OnClosed - args: " + args);
    }
    private void OnRewardedVideoAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= rewardedVideoAd.OnFailedToShow - args: " + args); }
    private void OnRewardedCallback(object sender, Adiscope.Model.RewardItem args) { this.AddOutputMessage("  <= rewardedVideoAd.OnRewarded - args: " + args); }


    private void OnInterstitialAdLoadedCallback(object sender, Adiscope.Model.LoadResult args) { this.AddOutputMessage("  <= interstitialAd.OnLoaded"); }
    private void OnInterstitialAdFailedToLoadCallback(object sender, Adiscope.Model.LoadFailure args) { this.AddOutputMessage("  <= interstitialAd.OnFailedToLoad - args: " + args); }
    private void OnInterstitialAdOpenedCallback(object sender, EventArgs args) { this.AddOutputMessage("  <= interstitialAd.OnOpened - args: " + args); }
    private void OnInterstitialAdClosedCallback(object sender, EventArgs args) { this.AddOutputMessage("  <= interstitialAd.OnClosed - args: " + args); }
    private void OnInterstitialAdFailedToShowCallback(object sender, Adiscope.Model.ShowFailure args) { this.AddOutputMessage("  <= interstitialAd.OnFailedToShow - args: " + args); }

    #region GUI
    private void AddContentViews()
    {

        GUI.backgroundColor = Color.black;

        // Core
        this.AddLabel("Core");
        this.AddTextField("Media ID", TextFieldType.MediaID);
        this.AddTextField("User ID", TextFieldType.UserID);

        this.AddTextField("Callback Tag", TextFieldType.CallbackTag);
        this.AddTextField("Child YN", TextFieldType.ChildYN);

#if UNITY_ANDROID

        this.AddButton("Initialize", () =>
        {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize((isSuccess) =>
            {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess);
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID);
            });
        });

        this.AddButton("Initialize(listener, callbackTag)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize((isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess);
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID);
            }, CALLBACK_TAG);

        });

        this.AddButton("Initialize(listener, callbackTag, childYN)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize((isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess);
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID);
            }, CALLBACK_TAG, CHILD_YN);

        });
#endif

        this.AddButton("Initialize(mediaId, mediaSecret, callbackTag, listener)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize(MEDIA_ID, secretKey, CALLBACK_TAG, (isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess);
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID);
            });
        });

        this.AddButton("Initialize(mediaId, mediaSecret, callbackTag, childYN, listener)", () => {
            string secretKey = AdiscopeItemFetcher.FetchMediaScretKey(MEDIA_ID);
            if (secretKey == null)
            {
                this.AddOutputMessage("Not Found SecretKey: " + MEDIA_ID);
                return;
            }

            this.core.Initialize(MEDIA_ID, secretKey, CALLBACK_TAG, CHILD_YN, (isSuccess) => {
                if (!isSuccess)
                {
                    this.AddOutputMessage("Initialized: " + isSuccess);
                    return;
                }

                this.RegisterAdiscopeCallback();
                this.core.SetUserId(USER_ID);
                this.AddOutputMessage("Initialized: " + isSuccess + ", Setup UserID: " + USER_ID);
            });
        });


        this.AddButton("is Initialized", () => {
            this.AddOutputMessage("Initialized Flag: " + Adiscope.Sdk.GetCoreInstance().IsInitialized());
        });

        this.AddButton("Print SDK Version", () => {
            this.AddOutputMessage("SDK Versions => " + Adiscope.Sdk.GetOptionGetter().GetSDKVersion());
        });

        this.AddButton("Print Network Version", () => {
            this.AddOutputMessage("Network Versions => " + Adiscope.Sdk.GetOptionGetter().GetNetworkVersions());
        });

        this.AddTextField("Unit ID", TextFieldType.FindUnitID);

        this.AddButton("getUnitStatus", () => {
            this.core.GetUnitStatus(Find_UNIT_ID, (error, result) => {
                if (error != null) this.AddOutputMessage("  <= error : " + error);
                else this.AddOutputMessage("  <= result : " + result);
            });
        });

        // Offerwall
        this.AddSpacer();
        this.AddTextField("Offerwall", TextFieldType.OfferwallUnit);
        this.AddButton("Show Offerwall", () => {
            string unitID = OFFERWALL_ID.ToUpper();
            if (unitID == null)
            {
                this.AddOutputMessage("Not Found unitID: " + MEDIA_ID);
                return;
            }

            OfferwallFilterType[] filter = { };
            if (this.offerwallAd.Show(unitID, filter)) { } else { this.AddOutputMessage("offerwallAd.Show request is duplicated"); }
        });
        this.AddTextField("Offerwall Detail Id", TextFieldType.OfferwallDetailId);
        this.AddButton("Show Detail Func", () => {
            string unitID = OFFERWALL_ID.ToUpper();
            string itemId = OFFERWALL_DETAIL_ID;
            if (unitID == null)
            {
                this.AddOutputMessage("Not Found unitID: " + unitID);
                return;
            }
            if (itemId == null)
            {
                this.AddOutputMessage("Not Found itemId: " + itemId);
                return;
            }

            OfferwallFilterType[] filter = { };
            if (this.offerwallAd.ShowOfferwallDetail(unitID, filter, itemId)) { } else { this.AddOutputMessage("offerwallAd.Show request is duplicated"); }
        });
        this.AddTextField("Offerwall Detail URL", TextFieldType.OfferwallDetailUrl);
        this.AddButton("Show Detail Func Offerwall URL", () => {
            string url = OFFERWALL_DETAIL_URL;
            if (url == null)
            {
                this.AddOutputMessage("Empty URL");
                return;
            }

            OfferwallFilterType[] filter = { };
            if (this.offerwallAd.ShowOfferwallDetailFromUrl(url)) { } else { this.AddOutputMessage("URL is empty"); }
        });
        this.AddTextField("Deeplink URL", TextFieldType.OfferwallDeeplinkUrl);
        this.AddButton("Show Detail from Deeplink", () => {
            string url = OFFERWALL_DEEPLINK_URL;
            if (url == null)
            {
                this.AddOutputMessage("Empty URL");
                return;
            }

            if (!url.StartsWith("adiscope")) {
                this.AddOutputMessage("앱링크 스킴의 링크 형식만 지원합니다. adiscope{media_sub_domain} 스킴을 사용하세요.");
                return;
            }

            Application.OpenURL(url);
            
        });

        this.AddTextField("Applink URL", TextFieldType.OfferwallApplinkUrl);
        this.AddButton("Show Detail from Applink", () => {
            string url = OFFERWALL_APPLINK_URL;
            if (url == null)
            {
                this.AddOutputMessage("Empty URL");
                return;
            }

            if (!url.StartsWith("https"))
            {
                this.AddOutputMessage("https 스킴의 링크 형식만 지원합니다.");
                return;
            }

            Application.OpenURL(url);
        });

        // Rewarded Video
        this.AddSpacer();
        this.AddLabel("Rewared Video");
        this.AddTextField("Rewarded Unit", TextFieldType.RewardedUnit);
        this.AddButton("Video - Load", () => {
            this.rewardedVideoAd.Load(RV_ID);
        });
        this.AddButton("Video - IsLoaded", () => {
            bool isLoaded = this.rewardedVideoAd.IsLoaded(RV_ID);
            this.AddOutputMessage("rewardedVideoAd.IsLoaded => " + isLoaded);
        });
        this.AddButton("Video - Show", () => {
            this.rewardedVideoAd.Show();
        });

        // Interstitial
        this.AddSpacer();
        this.AddLabel("Interstitial");
        this.AddTextField("Interstitial Unit", TextFieldType.InterstitialUnit);
        this.AddButton("Interstitial - Load unit", () => {
            this.interstitialAd.Load(IT_ID);
        });
        this.AddButton("Interstitial - IsLoaded", () => {
            bool isLoaded = this.interstitialAd.IsLoaded(IT_ID);
            this.AddOutputMessage("rewardedVideoAd.IsLoaded => " + isLoaded);
        });
        this.AddButton("Interstitial - Show", () => {
            this.interstitialAd.Show();
        });
    }

    private void OnGUI()
    {

        // Support Only Portrait
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float horizontalMargin = (float)(screenWidth * 0.03);                   // Horizon Margin Height
        float verticalMargin = (float)(screenHeight * 0.015);                   // Vertical Margin Height
#if UNITY_IOS
        top = verticalMargin * 3;
#endif

#if UNITY_ANDROID
        top = verticalMargin;
#endif
        float contentHeight = (float)(screenHeight * 0.3);
        Rect rect = new Rect(horizontalMargin, top, screenWidth - (horizontalMargin * 2), contentHeight);
        GUIStyle logTextStyle = new GUIStyle(GUI.skin.textArea);
        logTextStyle.fontSize = fontSize;
        GUI.Label(rect, this.outputMessage, logTextStyle);

        ClearContentViews();
        AddContentViews();

        contentHeight = (float)(50);                                            // Content Height : 5% for Screen Height
        top = rect.yMax + verticalMargin;

        Rect position = new Rect(0, top, Screen.width, Screen.height - top);
        Rect viewRect = new Rect(0, 0, Screen.width, 0);
        foreach (ContentView view in this.views)
        {
            viewRect.height += view.height;
            viewRect.height += verticalMargin;
        }

        GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.04f;
        GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.04f;

        scrollPosition = GUI.BeginScrollView(
            position, scrollPosition, viewRect
        );

        top = 0;
        float contentWidth = screenWidth - (horizontalMargin * 2);
        foreach (ContentView view in this.views)
        {

            if (view.type == ContentViewType.Spacer)
            {
                rect = new Rect(0, 0, 0, view.height);

            }
            else if (view.type == ContentViewType.Button)
            {
                rect = new Rect(horizontalMargin, top, contentWidth, view.height);
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fontSize = fontSize;
                if (GUI.Button(rect, view.title, style))
                {
                    this.AddOutputMessage("Button Tapped: " + view.title);
                    view.buttonAction();
                };

            }
            else if (view.type == ContentViewType.Label)
            {
                rect = new Rect(horizontalMargin, top, contentWidth, view.height);
                GUIStyle style = new GUIStyle(GUI.skin.textArea);
                style.fontSize = fontSize;
                GUI.Label(rect, view.title, style);

            }
            else if (view.type == ContentViewType.TextField)
            {
                float descriptionWidth = (float)(contentWidth * 0.33);          // Description Label Width 33%
                float textFieldWidth = (float)(contentWidth * 0.63);            // Text Field Width 63%
                                                                                // Margin 4%

                Rect descriptionRect = new Rect(horizontalMargin, top, descriptionWidth, view.height);
                GUIStyle style = new GUIStyle(GUI.skin.textArea);
                style.fontSize = fontSize;
                GUI.Label(descriptionRect, view.title, style);

                float maxX = horizontalMargin + descriptionWidth;               // Calulate Textfield MinX
                maxX += (float)(contentWidth - (descriptionWidth + textFieldWidth));

                rect = new Rect(maxX, top, textFieldWidth, view.height);

                switch (view.textFieldType)
                {
                    case TextFieldType.MediaID: MEDIA_ID = GUI.TextField(rect, MEDIA_ID, style); break;
                    case TextFieldType.UserID: USER_ID = GUI.TextField(rect, USER_ID, style); break;
                    case TextFieldType.OfferwallUnit: OFFERWALL_ID = GUI.TextField(rect, OFFERWALL_ID, style); break;
                    case TextFieldType.OfferwallDetailId: OFFERWALL_DETAIL_ID = GUI.TextField(rect, OFFERWALL_DETAIL_ID, style); break;
                    case TextFieldType.OfferwallDetailUrl: OFFERWALL_DETAIL_URL = GUI.TextField(rect, OFFERWALL_DETAIL_URL, style); break;
                    case TextFieldType.OfferwallDeeplinkUrl: OFFERWALL_DEEPLINK_URL = GUI.TextField(rect, OFFERWALL_DEEPLINK_URL, style); break;
                    case TextFieldType.OfferwallApplinkUrl: OFFERWALL_APPLINK_URL = GUI.TextField(rect, OFFERWALL_APPLINK_URL, style); break;
                    case TextFieldType.FindUnitID: Find_UNIT_ID = GUI.TextField(rect, Find_UNIT_ID, style); break;
                    case TextFieldType.RewardedUnit: RV_ID = GUI.TextField(rect, RV_ID, style).ToUpper(); break;
                    case TextFieldType.InterstitialUnit: IT_ID = GUI.TextField(rect, IT_ID, style).ToUpper(); break;
                    case TextFieldType.CallbackTag: CALLBACK_TAG = GUI.TextField(rect, CALLBACK_TAG, style).ToUpper(); break;
                    case TextFieldType.ChildYN: CHILD_YN = GUI.TextField(rect, CHILD_YN, style).ToUpper(); break;

                }

            }


            top += rect.height;
            top += verticalMargin;
        }

        GUI.EndScrollView();
    }

    private void AddOutputMessage(string message)
    {
        if (this.outputMessage == null || this.outputMessage.Length == 0) { this.outputMessage = message; }
        else { this.outputMessage = message + "\n" + this.outputMessage; }
    }

    private void ClearContentViews()
    {
        if (this.views == null) { this.views = new List<ContentView>(); }
        else { this.views.Clear(); }
    }

    private void AddSpacer()
    {
        this.views.Add(new ContentView(ContentViewType.Spacer));
    }

    private void AddLabel(string title)
    {
        this.views.Add(new ContentView(ContentViewType.Label, title));
    }

    private void AddButton(string title, Action function)
    {
        this.views.Add(new ContentView(ContentViewType.Button, title, function));
    }

    private void AddTextField(string title, TextFieldType type)
    {
        this.views.Add(new ContentView(ContentViewType.TextField, title, type));
    }

#endregion
}

public enum ContentViewType { Button, TextField, Label, Spacer }
public enum TextFieldType { MediaID, UserID, FindUnitID, RewardedUnit, InterstitialUnit, OfferwallUnit, OfferwallDetailId, OfferwallDetailUrl, OfferwallDeeplinkUrl, OfferwallApplinkUrl, CallbackTag, ChildYN }

class ContentView
{
    public TextFieldType textFieldType;
    public ContentViewType type;
    public String title;
    public Action buttonAction;

    public ContentView(ContentViewType type) { this.type = type; }

    public ContentView(ContentViewType type, String title, Action action = null)
    {
        this.type = type;
        this.title = title;
        this.buttonAction = action;
    }

    public ContentView(ContentViewType type, String title, TextFieldType textFieldType)
    {
        this.type = type;
        this.title = title;
        this.textFieldType = textFieldType;
    }

    public float height
    {
        get
        {
            switch (this.type)
            {
                case ContentViewType.Button:
                    return 100;
                case ContentViewType.TextField:
                case ContentViewType.Label:
                    return 50;
                case ContentViewType.Spacer:
                    return 25;
                default:
                    return 0;
            }
        }
    }
}