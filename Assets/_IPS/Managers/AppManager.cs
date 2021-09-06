using Ips.Screens;
using UnityEngine;

namespace Ips.Managers
{
    public class AppManager : MonoBehaviour, IAppManager
    {
        #region Dependencies

        private ILoadCaseScreen m_LoadCaseScreen;

        #endregion

        #region Private Functions

        private void Awake()
        {
            m_LoadCaseScreen = new LoadCaseScreen();
        }

        private void Start()
        {
            Debug.Log("Started");
            m_LoadCaseScreen.ActivateScreen();
        }

        private void OnApplicationQuit()
        {
            m_LoadCaseScreen.DeactivateScreen();
        }

        #endregion

    }
}
