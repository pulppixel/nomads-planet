using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using NomadsPlanet.Utils;


public class BackendLogin
{
    private static BackendLogin _instance;

    public static BackendLogin Instance {
        get {
            if (_instance == null) {
                _instance = new BackendLogin();
            }

            return _instance;
        }
    }

    public void CustomSignUp(string id, string pw)
    {
        Debug.Log("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다: " + bro);
        }
        else
        {
            Debug.LogError("회원가입에 실패했습니다: " + bro);
        }
    }

    public static void CustomLogin(string id, string pw)
    {
        Debug.Log("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다: " + bro);
        }
        else
        {
            Debug.LogError("로그인이 실패했습니다: " + bro);
        }
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("닉네임 변경을 요청합니다.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니다: " + bro);
        }
        else
        {
            Debug.LogError("닉네임 변경에 실패했습니다: " + bro);
        }
    }
}