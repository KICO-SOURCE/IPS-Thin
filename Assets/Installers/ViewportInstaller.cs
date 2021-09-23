#region Usings
using Assets.Viewport;
using Assets.Viewport.Tab;
using Zenject;

#endregion

namespace Assets.Installers
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
            Container.Bind<ViewportFactory>().AsSingle();
        }
    }
}