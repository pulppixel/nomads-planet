using System.Collections.Generic;
using MameshibaGames.Common.Helpers;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
#endif

namespace MameshibaGames.Kekos.DemoScene
{
    [RequireComponent(typeof(Button))]
    public class ShuffleCharacters : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> charactersList;

        [SerializeField]
        private GameObject[] sceneCharacters;

        private List<ClonePose> _poses = new List<ClonePose>();

        private void Awake()
        {
            #if ENABLE_INPUT_SYSTEM && UNITY_EDITOR
            StandaloneInputModule standaloneInputModule = FindObjectOfType<StandaloneInputModule>();
            GameObject standAloneGameObject = standaloneInputModule.gameObject;
            Destroy(standaloneInputModule);
            InputSystemUIInputModule inputSystem = standAloneGameObject.AddComponent<InputSystemUIInputModule>();
            inputSystem.enabled = false;
            inputSystem.enabled = true;
            #endif
            
            GetComponent<Button>().onClick.AddListener(Shuffle);

            for (int i = 0; i < sceneCharacters.Length; i++)
            {
                ClonePose clonePose = ScriptableObject.CreateInstance<ClonePose>();
                clonePose.SavePose(sceneCharacters[i]);
                clonePose.SaveFaceExpression(sceneCharacters[i]);
                _poses.Add(clonePose);
            }
            
            Shuffle();
        }

        private void Shuffle()
        {
            List<GameObject> shuffleList = new List<GameObject>(charactersList);
            shuffleList.Shuffle();

            for (int i = 0; i < sceneCharacters.Length; i++)
            {
                GameObject characterToReplace = sceneCharacters[i];

                sceneCharacters[i] = Instantiate(shuffleList[i]);
                _poses[i].ApplyPose(sceneCharacters[i]);
                _poses[i].ApplyFaceExpression(sceneCharacters[i]);
                
                Destroy(characterToReplace);
            }
        }
    }
}
