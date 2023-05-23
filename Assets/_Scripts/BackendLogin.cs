using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;


public class BackendLogin : MonoBehaviour
{
    private static BackendLogin _instance = null;

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
        CustomFunc.WriteLine("회원가입을 요청합니다.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            CustomFunc.WriteLine("회원가입에 성공했습니다: " + bro);
        }
        else
        {
            CustomFunc.WriteLine("회원가입에 실패했습니다: " + bro, true);
        }
    }

    public void CustomLogin(string id, string pw)
    {
        CustomFunc.WriteLine("로그인을 요청합니다.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            CustomFunc.WriteLine("로그인이 성공했습니다: " + bro);
        }
        else
        {
            CustomFunc.WriteLine("로그인이 실패했습니다: " + bro, true);
        }
    }

    public void UpdateNickname(string nickname)
    {
        CustomFunc.WriteLine("닉네임 변경을 요청합니다.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            CustomFunc.WriteLine("닉네임 변경에 성공했습니다: " + bro);
        }
        else
        {
            CustomFunc.WriteLine("닉네임 변경에 실패했습니다: " + bro, true);
        }
    }
}