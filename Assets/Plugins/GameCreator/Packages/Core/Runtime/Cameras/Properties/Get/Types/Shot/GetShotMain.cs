using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Cameras
{
    [Title("Main Shot")]
    [Category("Main Shot")]
    
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow, typeof(OverlayDot))]
    [Description("Reference to the current Main Camera Shot")]

    [Serializable]
    public class GetShotMain : PropertyTypeGetShot
    {
        public override ShotCamera Get(Args args)
        {
            return ShortcutMainShot.Get<ShotCamera>();
        }

        public static PropertyGetShot Create => new PropertyGetShot(
            new GetShotMain()
        );

        public override string String => "Main Shot";
    }
}