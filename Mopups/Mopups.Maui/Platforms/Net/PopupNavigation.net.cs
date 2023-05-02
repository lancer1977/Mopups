using Mopups.Interfaces;

namespace Mopups.Services;
public partial class PopupNavigation
{
    private static partial IPopupPlatform PullPlatformImplementation()
    {
        return new Mopups.MacCatalyst.Implementation.MacOSMopups();
    }

}