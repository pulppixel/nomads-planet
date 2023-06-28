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
            CustomFunc.WriteLine("초기화 성공: " + bro);
        }
        else
        {
            CustomFunc.WriteLine("초기화 실패: " + bro, true);
        }

        Test();
    }

    private async void Test()
    {
        await Task.Run(() =>
        {
            BackendLogin.Instance.CustomLogin("user1", "1234");
            // BackendLogin.Instance.UpdateNickname("바보2");
            CustomFunc.WriteLine("테스트 종료");
        });
    }
}
