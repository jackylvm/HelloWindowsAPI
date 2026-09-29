// ***********************************************************************************
// FileName: RandomRollingCube.cs
// Description:
// 
// Version: v1.0.0
// Creator: Jacky(jackylvm@foxmail.com)
// CreationTime: 2026-09-29 22:50:09
// ==============================================
// History update record:
// 
// ==============================================
// *************************************************************************************

using UnityEngine;

// ReSharper disable CheckNamespace
namespace Example
{
    public class RandomRollingCube : MonoBehaviour
    {
        [Header("X-Axis Movement")]
        public float moveSpeedX = 2f; // X轴移动速度

        public float rangeX = 5f; // X轴移动范围（-5 到 5）

        [Header("Z-Axis Movement")]
        public float moveSpeedZ = 1.5f; // Z轴移动速度 (可以与X轴不同，让轨迹更有趣)

        public float minZ = 0f; // Z轴最小值
        public float maxZ = 5f; // Z轴最大值

        [Header("Rotation Settings")]
        public float rotationSpeed = 150f;

        public float changeRotationInterval = 2f;
        private Vector3 randomRotationAxis;

        void Start()
        {
            // 初始化时获取一个随机旋转轴
            ChangeRotationAxis();

            // 定时调用，使Cube每隔一段时间改变一次滚动方向
            InvokeRepeating(nameof(ChangeRotationAxis), changeRotationInterval, changeRotationInterval);
        }

        void Update()
        {
            // 1. 计算 X 轴位置 (范围: -rangeX 到 rangeX)
            var newX = Mathf.Sin(Time.time * moveSpeedX) * rangeX;

            // 2. 计算 Z 轴位置 (范围: minZ 到 maxZ)
            // (Mathf.Sin(t) + 1) / 2 会将原本 [-1, 1] 的正弦值转换成 [0, 1] 的百分比
            var sineValueZ = (Mathf.Sin(Time.time * moveSpeedZ) + 1f) / 2f;
            // 使用 Mathf.Lerp 在 0 和 5 之间平滑取值
            var newZ = Mathf.Lerp(minZ, maxZ, sineValueZ);

            // 更新位置，Y轴保持为0
            transform.position = new Vector3(newX, 0, newZ);

            // 3. 随机滚动
            transform.Rotate(randomRotationAxis * rotationSpeed * Time.deltaTime);
        }

        // 生成一个新的随机旋转轴
        void ChangeRotationAxis()
        {
            randomRotationAxis = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
        }
    }
}