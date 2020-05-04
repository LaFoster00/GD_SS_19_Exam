using System;
using GameEvents;
using UnityEngine;

namespace ExamShooter_Menu
{
    public enum MenuType
    {
        Esc = 0,
        DifficultyServer = 1,
        DifficultyClient = 2,
    }

    public class InGameMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject escapeMenu;
        [SerializeField] private GameObject difficultyServerMenu;
        [SerializeField] private GameObject difficultyClientMenu;

        private bool _escapeMenuActive;
        private bool _difficultyMenuOpen;
        private bool _isServer;

        private GameObject _backGroundMenu;

        private void OnEnable()
        {
            GameEventManager.AddListener<GameEvent_OnMenu>(OnMenu);
        }

        private void OnDisable()
        {
            GameEventManager.RemoveListener<GameEvent_OnMenu>(OnMenu);
        }

        private void OnMenu(GameEvent_OnMenu menu)
        {
            if (menu.menuType == MenuType.Esc)
            {
                EscapeMenu();
            }
            else
            {
                DifficultyMenu(menu);
            }
        }

        #region EscapeMenu

        


        private void EscapeMenu()
        {
            //check if escape menu is opened
            if (!_escapeMenuActive)
            {
                _escapeMenuActive = true;
                //check if difficulty menu open is active so that we can return to it when escape menu is closed
                if (_difficultyMenuOpen)
                {
                    if (_isServer)
                    {
                        difficultyServerMenu.SetActive(false);
                        _backGroundMenu = difficultyServerMenu;
                    }
                    else
                    {
                        difficultyClientMenu.SetActive(false);
                        _backGroundMenu = difficultyClientMenu;
                    }
                }
                //open escape menu
                escapeMenu.SetActive(true);
            }
            else
            {
                _escapeMenuActive = false;
                //close escape menu
                escapeMenu.SetActive(false);
                if (_backGroundMenu)
                {
                    _backGroundMenu.SetActive(true);
                }
            }
        }
        
        #endregion

        #region DifficultyMenu
        
        private void DifficultyMenu(GameEvent_OnMenu difficultyMenu)
        {
            //check if difficulty menu is all ready opened
            if (!_difficultyMenuOpen)
            {
                _difficultyMenuOpen = true;
                //check if client or server menu has to be opened
                if (difficultyMenu.menuType == MenuType.DifficultyServer)
                {
                    //open server menu
                    _isServer = true;
                    difficultyServerMenu.SetActive(true);
                    Debug.Log("Opening Server Menu");
                }
                else
                {
                    //open client menu
                    difficultyClientMenu.SetActive(true);
                    Debug.Log("Opening Client Menu");
                }
            }
            else
            {
                //close difficulty menu
                if (_isServer)
                {
                    difficultyServerMenu.SetActive(false);
                }
                else
                {
                    difficultyClientMenu.SetActive(false);
                }
                _difficultyMenuOpen = false;
            }
        }

        #endregion
        
        #region DifficultyButtons
        
        public void Easy()
        {
            GameEventManager.Raise(new GameEvent_OnDifficultySelected(Difficulty.Easy));
        }
        
        public void Normal()
        {
            GameEventManager.Raise(new GameEvent_OnDifficultySelected(Difficulty.Normal));
        }
        
        public void Hard()
        {
            GameEventManager.Raise(new GameEvent_OnDifficultySelected(Difficulty.Hard));
        }
        
        public void Impossible()
        {
            GameEventManager.Raise(new GameEvent_OnDifficultySelected(Difficulty.Impossible));
        }
        
        #endregion

        public void ExitGame()
        {
            Application.Quit();
        }

        private void OnApplicationQuit()
        {
            GameSaveManager.instance.SaveGame();
        }
    }
}