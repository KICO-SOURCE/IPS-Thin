using Assets._MUTUAL.Installers;
using Ips.Managers;
using UnityEngine;
using Zenject;

namespace Ips.Installers
{
    public class MainSceneInstallers : MonoInstaller
    {
        private AppManager m_AppManager;

        public override void InstallBindings()
        {
            // Non MonoInstallers

            InstallDependencies();

            // Mono behaviors 

            m_AppManager =  GetComponent<AppManager>();
            Container.Bind<IAppManager>().FromInstance(m_AppManager);

        }

        private void InstallDependencies()
        {
            // General
            Container.Install<StartupPageInstaller>();
            Container.Install<ViewportInstaller>();
        }
    }
}