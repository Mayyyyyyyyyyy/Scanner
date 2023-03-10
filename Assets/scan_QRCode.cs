using UnityEngine;
using System.Collections;
using ZXing;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Text = TMPro.TextMeshProUGUI;
using System;

public class scan_QRCode : MonoBehaviour
{

    public Text num;
    private WebCamTexture myCam;//接收攝影機返回的圖片數據
    private BarcodeReader reader = new BarcodeReader();//ZXing的解碼
    private Result res;//儲存掃描後回傳的資訊
    private bool flag = true;//判斷掃描是否執行完畢
    private bool draw = true;

    void Start()
    {
        StartCoroutine(open_Camera());//開啟攝影機鏡頭
    }

    void OnGUI()
    {
        if (myCam != null)//若有攝影機則將攝影機拍到的畫面畫出
        {
            if (myCam.isPlaying == true)//若攝影機已開啟
            {
                if (draw)
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), myCam);//將攝影機讀取到的影像繪製到螢幕上
                    if (flag == true)//若掃描已執行完畢，則再繼續進行掃描，防止第一個掃描還沒結束就又再次掃描，造成嚴重的記憶體耗盡
                    {
                        StartCoroutine(scan());
                    }
                }
                
            }
        }
    }

    private IEnumerator open_Camera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);    //授權開啟鏡頭
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            //設置攝影機要攝影的區域
            myCam = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height, 60);/* (攝影機名稱, 攝影機要拍到的寬度, 攝影機要拍到的高度, 攝影機的FPS) */
            myCam.Play();//開啟攝影機
        }
    }

    private IEnumerator scan()
    {
        flag = false;//掃描開始執行


        Texture2D t2D = new Texture2D(Screen.width, Screen.height);//掃描後的影像儲存大小，越大會造成效能消耗越大，若影像嚴重延遲，請降低儲存大小。
        yield return new WaitForEndOfFrame();//等待攝影機的影像繪製完畢

        t2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);//掃描的範圍，設定為整個攝影機拍到的影像，若影像嚴重延遲，請降低掃描大小。
        t2D.Apply();//開始掃描

        res = reader.Decode(t2D.GetPixels32(), t2D.width, t2D.height);//對剛剛掃描到的影像進行解碼，並將解碼後的資料回傳

        //若是掃描不到訊息，則res為null
        if (res != null)
        {
            UnityEngine.Debug.Log(res.Text);//將解碼後的資料列印出來
            num.text = res.Text;
            //看要輸幾個商品
            if(num.text== "4710018323201")
            {
                num.text = res.Text + "\nTea\n2023.01.19";
            }
            draw = false;
            GUI.TextArea(new Rect(0, 0, 300, 300), num.text);
        }
        flag = true;//掃描完畢
    }

    void OnDisable()
    {
        //當程式關閉時會自動呼叫此方法關閉攝影機
        myCam.Stop();
    }
}