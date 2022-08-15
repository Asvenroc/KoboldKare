using System.Collections;
using System.Collections.Generic;
using System.IO;
using JigglePhysics;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SmoothRigidbodyPhoton : MonoBehaviourPun, IPunObservable, ISavable {
    //private JiggleRigBuilder[] jiggleRigs;
    private struct Frame {
        public Vector3 position;
        public Quaternion rotation;
        public float time;

        public Frame(Vector3 pos, Quaternion rotation, float time) {
            position = pos;
            this.rotation = rotation;
            this.time = time;
        }
    }
    private Frame lastFrame;
    private Frame newFrame;
    private bool init = false;

    private void Awake() {
        body = GetComponent<Rigidbody>();
        //jiggleRigs = GetComponentsInChildren<JiggleRigBuilder>();
        lastFrame = new Frame(body.transform.position, body.transform.rotation, Time.time);
        newFrame = new Frame(body.transform.position, body.transform.rotation, Time.time);
    }
    
    private void LateUpdate() {
        if (photonView.IsMine) {
            body.isKinematic = false;
            //foreach (JiggleRigBuilder jiggleRig in jiggleRigs) {
                //jiggleRig.interpolate = true;
            //}
            return;
        }
        //foreach (JiggleRigBuilder jiggleRig in jiggleRigs) {
            //jiggleRig.interpolate = false;
        //}
        body.isKinematic = true;
        float time = Time.time - (1f/PhotonNetwork.SerializationRate);
        float diff = newFrame.time - lastFrame.time;
        if (diff == 0f) {
            body.transform.position = newFrame.position;
            body.transform.rotation = newFrame.rotation;
            return;
        }
        float t = (time - lastFrame.time) / diff;
        //body.velocity = (newFrame.position - lastFrame.position) / (float)diff;
        body.transform.position = Vector3.LerpUnclamped(lastFrame.position, newFrame.position, Mathf.Clamp((float)t, -0.25f, 1.25f));
        body.transform.rotation = Quaternion.LerpUnclamped(lastFrame.rotation, newFrame.rotation, Mathf.Clamp((float)t, -0.25f, 1.25f));
    }
    
    private Rigidbody body;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(body.transform.position);
            stream.SendNext(body.transform.rotation);
            lastFrame = newFrame;
            newFrame = new Frame(body.transform.position, body.transform.rotation, Time.time);
        } else {
            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            lastFrame = newFrame;
            newFrame = new Frame((Vector3)stream.ReceiveNext(), (Quaternion)stream.ReceiveNext(), Time.time);
            if (!init) {
                lastFrame = newFrame;
                init = true;
            }
        }
    }

    public void Save(BinaryWriter writer) {
        writer.Write(body.position.x);
        writer.Write(body.position.y);
        writer.Write(body.position.z);
    }

    public void Load(BinaryReader reader) {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        body.position = new Vector3(x, y, z);
    }
}
