#region Usings

using Assets._MUTUAL.Viewport;
using Assets._MUTUAL.Viewport.Tab;
using Zenject;

#endregion

namespace Assets._MUTUAL.Installers
{
    /// <summary>
    /// Viewport component registration class.
    /// </summary>
    public class ViewportInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<TabGroup>().AsSingle();
            Container.Bind<ViewportContainer>().AsSingle();
            Container.Bind<ViewportManager>().AsSingle();
        }
    }
}