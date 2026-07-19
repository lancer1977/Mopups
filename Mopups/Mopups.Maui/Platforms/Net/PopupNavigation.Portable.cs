using System;

using Mopups.Interfaces;

namespace Mopups.Services;

public partial class PopupNavigation
{
    private static partial IPopupPlatform PullPlatformImplementation()
    {
        throw new PlatformNotSupportedException(
            "Mopups requires a platform-specific MAUI target framework.");
    }
}
