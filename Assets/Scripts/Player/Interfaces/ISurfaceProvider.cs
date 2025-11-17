using Audio.Data;

namespace Player.Interfaces
{
    public interface ISurfaceProvider
    {
        SurfaceMaterialType GetCurrentSurface();
    }
}