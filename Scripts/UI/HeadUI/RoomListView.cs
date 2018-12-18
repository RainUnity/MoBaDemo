﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomListView : MonoBehaviour {
    public Button createRoomButton;
    public Button attendRoomButton;
    public InputField roomNameInputField;
    public RoomView roomView;

    private System.Action OnHideCompelteAction;

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        transform.DOScale(new Vector3(0, 0, 0), 1f).onComplete += () => {
            if (OnHideCompelteAction != null) OnHideCompelteAction();
        };
    }

    private void Start() {

        InitListener();

        createRoomButton.onClick.AddListener(SendCreateRoomProtocol);


        //attendRoomButton.onClick.AddListener(()=> {

        //});
    }

    public void SendCreateRoomProtocol() {
        ProtocolBytes protocolBytes = new ProtocolBytes();

        // 协议名
        protocolBytes.AddString("CreateRoom");

        // 协议参数
        protocolBytes.AddString(NetWorkManager.Instance.NowPlayerID);
        protocolBytes.AddString("rooms1");

        Debug.Log("当前用户创建房间rooms1");

        // 发送协议
        NetWorkManager.Instance.Send(protocolBytes);
    }

    public void InitListener() {
        NetWorkManager.Instance.AddListener("CreateRoomResult", TreateCreateRoomResult);
        NetWorkManager.Instance.AddListener("AttendRoomResult", TreateAttendRoomResult);
    }

    public void TreateCreateRoomResult(ProtocolBytes protocolBytes) {
        // 获得用户名
        string userName = protocolBytes.GetString();
        // 获得创建的房间的名字
        string roomName = protocolBytes.GetString();
        // 获得创建房间的结果
        string result = protocolBytes.GetString();
        // 失败原因,默认为空
        string failReason = null;
        if (result == "Fail") {
            failReason = protocolBytes.GetString();
        }

        Debug.Log("创建房间的结果是:"+result+" 失败原因:"+failReason);

        if (result == "Success") {
            // 创建成功,发送加入此房间的信息

            ProtocolBytes protocol = new ProtocolBytes();

            // 协议名
            protocol.AddString("AttendRoom");

            // 参数
            protocol.AddString("rooms1");
            protocol.AddString(NetWorkManager.Instance.NowPlayerID);

            NetWorkManager.Instance.Send(protocol);
        }
    }

    public void TreateAttendRoomResult(ProtocolBytes protocolBytes) {
        // 获得房间名
        string roomName = protocolBytes.GetString();
        // 获得用户名
        string userName = protocolBytes.GetString();
        // 获得结果
        string result = protocolBytes.GetString();
        // 失败原因,默认为空
        string failReason = null;
        if (result == "Fail") {
            failReason = protocolBytes.GetString();
        }
        if (result == "Success") {
            Hide();
            OnHideCompelteAction += () => { roomView.Show(); };
        }
    }
}
