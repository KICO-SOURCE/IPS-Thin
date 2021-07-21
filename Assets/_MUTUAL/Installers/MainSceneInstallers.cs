using Mutual.Installers;
using Mutual.Managers;
using UnityEngine;
using Zenject;

namespace Mutual.Installers
{
    public class MainSceneInstallers : MonoInstaller
    {
        private AppManager m_AppManager;

        public override void InstallBindings()
        {
            // Non MonoInstallers

            InstallDependencies();

            // Mono behaviors 

            m_AppManager = GameObject.FindObjectOfType<AppManager>();
            Container.Bind<IAppManager>().FromInstance(m_AppManager);

        }

        private void InstallDependencies()
        {
            // General
            Container.Install<StartupPageInstaller>();
        }
    }
}