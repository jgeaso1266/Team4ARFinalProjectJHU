using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System;

public class RopeScript : MonoBehaviour, IMixedRealityPointerHandler
{

    [Header("Rope Settings")]
    [SerializeField] private Transform TransPoint1;
    [SerializeField] private Transform TransPoint2;

    [Header("Prefab")]
    [SerializeField] private GameObject BallPrefab;
    [SerializeField] private GameObject ShootButton;

    private LineRenderer _lineRenderer;
    private Transform _newBall;
    private GameObject newBallObject;
    private Camera mainCamLocal;
    private Vector3 beginPos;
    private bool isMaster = PhotonNetwork.IsMasterClient;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        mainCamLocal = Camera.main;

        ShootButton.SetActive(false);

        beginPos = _lineRenderer.GetPosition(1);

        if (isMaster)
        {
            CoreServices.InputSystem.RegisterHandler<IMixedRealityPointerHandler>(this);
        }

        // Show Shoot button for second player
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            ShootButton.SetActive(true);
        }
    }

    void Update()
    {
        if (isMaster && TransPoint1 && TransPoint2)
        {
            _lineRenderer.SetPosition(0, TransPoint1.position);
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, TransPoint2.position);

            updateClient();
        }

    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (isMaster && eventData.MixedRealityInputAction.Description == "Select" && _newBall == null)
        {
            newBallObject = PhotonNetwork.Instantiate(BallPrefab.name, Vector3.zero, Quaternion.identity);
            _newBall = newBallObject.transform;

            if (_lineRenderer.positionCount < 3)
            {
                _lineRenderer.positionCount = 3;
            }

            Vector3 newPos = _newBall.position;
            _lineRenderer.SetPosition(1, newPos);

            updateClient();
        }
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if (isMaster && _newBall)
        {
            // Update the position of the projectile during dragging
            Vector3 newPos = eventData.Pointer.Position;

            _newBall.position = newPos;

            newPos = _newBall.position;
            newPos.z -= 0.06f;

            _lineRenderer.SetPosition(1, newPos);

            updateClient();
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {

    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {

    }

    [PunRPC]
    void ShootPun()
    {
        if (_newBall)
        {
            // Launch the projectile when the gesture is completed
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(1, beginPos);

            _newBall.GetComponent<Rigidbody>().isKinematic = false;
            Vector3 direction = _newBall.up + _newBall.forward;
            _newBall.GetComponent<Rigidbody>().AddForce(direction * 200);

            // Destroy the projectile after 5 seconds (adjust the time as needed)
            StartCoroutine(DestroyBall(newBallObject));

            newBallObject = null;
            _newBall = null;

            updateClient();
        }
    }

    public void Shoot()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("ShootPun", RpcTarget.MasterClient);
    }

    [PunRPC]
    void UpdateClientPun(Vector3[] positions)
    {
        _lineRenderer.positionCount = positions.Length;

        _lineRenderer.SetPositions(positions);
    }

    private void updateClient()
    {
        PhotonView photonView = PhotonView.Get(this);
        Vector3[] positions = new Vector3[_lineRenderer.positionCount];
        _lineRenderer.GetPositions(positions);
        photonView.RPC("UpdateClientPun", RpcTarget.Others, positions);
    }

    private IEnumerator DestroyBall(GameObject ball)
    {
        yield return new WaitForSeconds(5f);

        if (ball)
        {
            PhotonNetwork.Destroy(ball);
        }
    }
}
