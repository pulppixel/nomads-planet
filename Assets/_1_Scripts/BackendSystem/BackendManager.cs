using System.Threading.Tasks;
using UnityEngine;

using BackEnd;
using NomadsPlanet.Utils;

public class BackendManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var bro = Backend.Initialize(true);

        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공: " + bro);
        }
        else
        {
            Debug.LogError("초기화 실패: " + bro);
        }

        Test();
    }

    private async void Test()
    {
        await Task.Run(() =>
        {
            BackendLogin.CustomLogin("user1", "1234");
            // BackendLogin.Instance.UpdateNickname("바보2");
            Debug.Log("테스트 종료");
        });
    }
}
