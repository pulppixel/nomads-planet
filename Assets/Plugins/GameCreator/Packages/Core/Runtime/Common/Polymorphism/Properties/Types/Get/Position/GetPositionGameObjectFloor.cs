using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Floor")]
    [Category("Game Objects/Game Object Floor")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayBar))]
    [Description("Returns the position of the ground below the Game Object")]

    [Serializable]
    public class GetPositionGameObjectFloor : PropertyTypeGetPosition
    {
        [SerializeField] 
        private PropertyGetGameObject m_GameObject = GetGameObjectPlayer.Create();

        [SerializeField] private LayerMask m_LayerMask = -1;

        public GetPositionGameObjectFloor()
        { }
        
        public GetPositionGameObjectFloor(GameObject gameObject)
        {
            this.m_GameObject = GetGameObjectInstance.Create(gameObject);
        }
        
        public override Vector3 Get(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return default;

            bool isHit = Physics.Raycast(
                gameObject.transform.position,
                Vector3.down,
                out RaycastHit hit,
                float.MaxValue,
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );

            return isHit ? hit.point : gameObject.transform.position;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionGameObject()
        );

        public override string String => $"{this.m_GameObject}'s Floor";
    }
}