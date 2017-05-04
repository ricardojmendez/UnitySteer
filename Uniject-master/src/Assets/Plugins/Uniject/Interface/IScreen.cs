using UnityEngine;

namespace Uniject
{
    public interface IScreen
    {
        void SetResolution(int width, int height, bool fullscreen);
        void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate);

        bool autorotateToLandscapeLeft { set; get; }
        bool autorotateToLandscapeRight { set; get; }
        bool autorotateToPortrait { set; get; }
        bool autorotateToPortraitUpsideDown { set; get; }
        Resolution currentResolution { get; }
        float dpi { get; }
        bool fullScreen { set; get; }
        Resolution[] GetResolution { get; }
        int height { get; }
        bool lockCursor { set; get; }
        ScreenOrientation orientation { set; get; }
        Resolution[] resolutions { get; }
        bool showCursor { set; get; }
        int sleepTimeout { set; get; }
        int width { get; }
    }
}
