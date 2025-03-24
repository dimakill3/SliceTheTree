using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Conveyor
{
    public class Conveyor : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        
        [SerializeField] private Renderer rend;

        private CancellationTokenSource _moveCts;

        private void Start()
        {
            if (rend == null)
                rend = GetComponent<Renderer>();
        }

        public void StartConveyor(float conveyorSpeed)
        {
            _moveCts = new CancellationTokenSource();
            
            MoveConveyor(conveyorSpeed, _moveCts.Token).Forget();
        }
        
        public void StopConveyor()
        {
            _moveCts.Cancel();
            _moveCts.Dispose();
            _moveCts = null;
        }

        private async UniTask MoveConveyor(float speed, CancellationToken token)
        {
            if (rend == null)
                return;

            while (!token.IsCancellationRequested)
            {
                var currentOffset = rend.material.GetTextureOffset(MainTex);
                var newOffset = currentOffset + new Vector2(speed * Time.deltaTime, 0);
            
                rend.material.SetTextureOffset(MainTex, newOffset);
                await UniTask.WaitForEndOfFrame(token);
            }
        }
    }
}