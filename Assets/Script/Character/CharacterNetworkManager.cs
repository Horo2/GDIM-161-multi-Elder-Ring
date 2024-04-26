using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Horo
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        [Header("Position")]
        //if you own this character you can edit this character position, if you dont you only can read it.
        // ��ȡ������ӵ�еĽ�ɫ��������ϵ����꣬ת��Ȳ��洢
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.1f;
        public float networkRotationSmoothTime = 0.1f;
    }
}
