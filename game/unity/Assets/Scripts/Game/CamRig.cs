using UnityEngine;
using GTA6.Core;

namespace GTA6.Game
{
    public sealed class CamRig
    {
        public Camera Cam;
        public float Yaw, Pitch = 14f;
        public float Distance = 5.2f;
        public float TargetDistance = 5.2f;
        public Vector3 SmoothTarget;
        public float Shake;
        float _vehicleYaw;
        public float LookSensitivity = 1f;
        public bool InvertY;

        public void Init(Transform parent)
        {
            GameObject go = Go.New("MainCamera", parent);
            Cam = go.AddComponent<Camera>();
            Cam.fieldOfView = 62f;
            Cam.nearClipPlane = 0.12f;
            Cam.farClipPlane = 2400f;
            Cam.allowHDR = true;
            Cam.allowMSAA = false;
            go.AddComponent<AudioListener>();
            go.tag = "MainCamera";
        }

        public Vector3 FlatForward
        {
            get { return Quaternion.Euler(0f, Yaw, 0f) * Vector3.forward; }
        }

        public Vector3 FlatRight
        {
            get { return Quaternion.Euler(0f, Yaw, 0f) * Vector3.right; }
        }

        public void AddShake(float amount) { Shake = Mathf.Min(1.6f, Shake + amount); }

        public void Update(float dt, Player player, InputState input, World world, Settings settings)
        {
            float sens = 0.18f * settings.Sensitivity;
            Yaw += input.Look.x * sens;
            float pitchDelta = input.Look.y * sens * 0.85f;
            Pitch += settings.InvertY ? pitchDelta : -pitchDelta;
            Pitch = Mathf.Clamp(Pitch, -28f, 62f);
            Yaw = Mathf.Repeat(Yaw, 360f);

            bool driving = player.Vehicle != null;
            Vector3 focus;
            if (driving)
            {
                Vehicle v = player.Vehicle;
                focus = v.Pos + new Vector3(0f, 1.35f, 0f);
                // slowly swing the camera behind the car while the player is not steering it
                if (input.Look.sqrMagnitude < 0.4f)
                {
                    float targetYaw = v.Heading;
                    _vehicleYaw = Mathf.MoveTowardsAngle(_vehicleYaw, targetYaw, dt * 90f);
                    Yaw = Mathf.MoveTowardsAngle(Yaw, _vehicleYaw, dt * 70f);
                }
                else _vehicleYaw = Yaw;
                TargetDistance = 8.4f + Mathf.Clamp01(v.SpeedKmh / 160f) * 2.2f;
                Pitch = Mathf.Clamp(Pitch, -12f, 46f);
            }
            else
            {
                focus = player.Pos + new Vector3(0f, 1.45f, 0f);
                TargetDistance = player.Aiming ? 2.9f : 5.2f;
                _vehicleYaw = Yaw;
            }

            Distance = Mathf.Lerp(Distance, TargetDistance, 1f - Mathf.Exp(-dt * 7f));
            SmoothTarget = Vector3.Lerp(SmoothTarget, focus, 1f - Mathf.Exp(-dt * 12f));

            Quaternion rot = Quaternion.Euler(Pitch, Yaw, 0f);
            float shoulder = driving ? 0f : 0.62f;
            Vector3 desired = SmoothTarget + rot * new Vector3(shoulder, 0.35f, -Distance);

            // keep the camera out of walls
            Vector3 dir = desired - SmoothTarget;
            float len = dir.magnitude;
            if (len > 0.01f)
            {
                Box3 hit;
                float d = world.Colliders.Raycast(SmoothTarget, dir / len, len + 0.35f, out hit);
                if (d >= 0f) desired = SmoothTarget + dir / len * Mathf.Max(0.6f, d - 0.4f);
            }

            if (Shake > 0.001f)
            {
                Vector3 jitter = new Vector3(
                    (Mathf.PerlinNoise(Time.time * 34f, 0.3f) - 0.5f),
                    (Mathf.PerlinNoise(0.7f, Time.time * 31f) - 0.5f),
                    (Mathf.PerlinNoise(Time.time * 28f, 2.1f) - 0.5f)) * Shake * 0.35f;
                desired += jitter;
                Shake = Mathf.Max(0f, Shake - dt * 3.2f);
            }

            Cam.transform.position = desired;
            Vector3 lookAt = SmoothTarget + rot * new Vector3(shoulder, 0f, 2.5f);
            if (player.Vehicle != null) lookAt += player.Vehicle.Forward * 4.5f;
            Cam.transform.rotation = Quaternion.LookRotation(lookAt - desired, Vector3.up);

            float fov = player.Aiming ? 46f : 62f;
            if (player.Vehicle != null) fov = 62f + Mathf.Clamp01(player.Vehicle.SpeedKmh / 200f) * 10f;
            Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, fov, 1f - Mathf.Exp(-dt * 8f));
        }

        public Vector3 AimPoint
        {
            get { return SmoothTarget + Cam.transform.forward * 60f; }
        }

        public void SnapTo(Vector3 target) { SmoothTarget = target; }
    }
}
