using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class PlayerMovement : NetworkBehaviour
    {
        private float speed;
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        void Start()
        {
            speed = 50f;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                GetNewPosition();
            }
        }

        public void GetNewPosition()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        }

        public void Move()
        {
            float xMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float zMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            if (xMovement != 0f ||  zMovement != 0f) {
                Vector3 movement = new Vector3(xMovement, 0f, zMovement);
                if (NetworkManager.Singleton.IsServer)
                {
                    transform.Translate(movement);
                    Position.Value = transform.position;
                }
                else
                {
                    SubmitMoveRequestServerRpc(movement);
                }
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
        }

        [ServerRpc]
        void SubmitMoveRequestServerRpc(Vector3 movement)
        {
            Position.Value += movement;
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            transform.position = Position.Value;
        }
    }
}