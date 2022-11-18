using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewUserInfo : View
{
    public Text userName;
    public Text txtPoints;
    
    public void Receive(UserInfo userInfo)
    {
        userName.text = "Name: \t\t" + userInfo.name;
        txtPoints.text = $"Points: \t\t {userInfo.points}";
    }
}
