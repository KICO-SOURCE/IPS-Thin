using Assets.CaseFile;
using Mutual.Screens;
using Zenject;

namespace Mutual.Installers
{
    class StartupPageInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<Patient>().AsSingle();
            Container.Bind<Project>().AsSingle();
            Container.Bind<CaseFileLoader>().AsSingle();
            Container.Bind<ILoadCaseScreen>().To<LoadCaseScreen>().AsSingle();
        }
    }
}
