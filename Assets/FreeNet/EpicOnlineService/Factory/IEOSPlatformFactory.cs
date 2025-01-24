using Epic.OnlineServices.Platform;
public interface IEOSPlatformFactory
{
    bool LoadDLL();
    bool UnLoadDLL();
    bool MakePlatform(Credential credential, out PlatformInterface OutIPlatform);


}