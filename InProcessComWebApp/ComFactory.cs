using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace InProcessComWebApp;

public class ComFactory<T> : IClassFactory
    where T : class, new()
{
    public static void Register(CancellationToken token)
    {
        const uint INFINITE = 0xFFFFFFFF;

        PInvoke.CoRegisterClassObject(typeof(T).GUID, new ComFactory<T>(), CLSCTX.CLSCTX_INPROC_SERVER, REGCLS.REGCLS_MULTIPLEUSE, out var register).ThrowOnFailure();
#pragma warning disable CS0618 // Type or member is obsolete
        PInvoke.CoWaitForMultipleHandles((uint)COWAIT_FLAGS.COWAIT_DEFAULT, INFINITE, new[] { new HANDLE(token.WaitHandle.Handle) }, out _).ThrowOnFailure();
#pragma warning restore CS0618 // Type or member is obsolete
        PInvoke.CoRevokeClassObject(register).ThrowOnFailure();
    }

    public static T Retrieve()
    {
        PInvoke.CoCreateInstance<T>(typeof(T).GUID, null, CLSCTX.CLSCTX_INPROC_SERVER, out var result).ThrowOnFailure();
        return result;
    }

    public unsafe void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, Guid* riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject)
    {
        if (riid->Equals(typeof(T).GUID))
        {
            ppvObject = new T();
        }
        else
        {
            ppvObject = null!;
        }
    }

    void IClassFactory.LockServer(BOOL fLock)
    {
    }
}
