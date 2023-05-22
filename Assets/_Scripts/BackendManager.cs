using System.Threading.Tasks;
using UnityEngine;

using BackEnd;

public class BackendManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

    async void Test()
    {
        await Task.Run(() =>
        {
            BackendLogin.Instance.CustomLogin("user1", "1234");
            BackendLogin.Instance.UpdateNickname("바보");
            CustomFunc.WriteLine("테스트 종료");
        });
    }
}
