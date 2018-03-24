using Photon;
using System;
using UnityEngine;

namespace TAHL.WAVE_BENDER
{
    public class FakeNetoworkManager: PunBehaviour
    {
        private GameObject _gameController;

        public void Awake()
        {
            _gameController = GameObject.FindGameObjectWithTag(Globals.Tags.GameController);
            if(_gameController == null)
            {
                throw new Exception("Game controller not found in fakeNetworkManager");
            }
            _gameController.SetActive(false);
            PhotonNetwork.offlineMode = true;
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            _gameController.SetActive(true);
        }
    }
}
