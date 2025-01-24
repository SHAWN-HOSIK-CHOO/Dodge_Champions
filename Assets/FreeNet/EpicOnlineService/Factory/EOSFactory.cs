public class EOSFactory
{
    public static IEOSPlatformFactory GetFactory()
    {
        IEOSPlatformFactory factory = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        factory = new WindowsFactory();
#endif
        return factory;
    }
}