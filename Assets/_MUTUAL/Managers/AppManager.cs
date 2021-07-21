using System.Collections;
using Mutual.Screens;
using UnityEngine;
using Zenject;

namespace Mutual.Managers
{
    public class AppManager : MonoBehaviour, IAppManager
    {
        #region Dependencies

        private ILoadCaseScreen m_LoadCaseScreen;

        #endregion

        [Inject]
        public void Construct(ILoadCaseScreen loadCaseScreen)
        {
            m_LoadCaseScreen = loadCaseScreen;
            DontDestroyOnLoad(this);

            AppStarter();
        }

        #region Private Functions

        
        private void AppStarter()
        {
            Debug.Log("Constructed");
            m_LoadCaseScreen.ActivateScreen();
        }

        private void OnApplicationQuit()
        {
            m_LoadCaseScreen.DeactivateScreen();
        }

        #endregion

    }
}
