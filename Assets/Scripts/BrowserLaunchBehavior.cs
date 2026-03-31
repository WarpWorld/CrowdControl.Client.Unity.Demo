using CrowdControl.Client.WebSocket.Data;
using UnityEngine;

public class BrowserLaunchBehavior : MonoBehaviour
{
    //this should probably be something a bit more elegant in a non-demo
    //this may or may not work in console builds but should work fine in the editor and standalone windows builds
    public void LaunchBrowser(ApplicationAuthCode authCode) => Application.OpenURL(authCode.Url);
}