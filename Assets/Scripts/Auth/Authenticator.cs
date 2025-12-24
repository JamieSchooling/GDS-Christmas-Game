using System.Text;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;

namespace GDS
{
    public class Authenticator : MonoBehaviour
    {
        [SerializeField] private float m_ArtificialLoadTime = 1.5f;

        public UnityEvent OnSignInBegin;
        public UnityEvent OnSignInSuccessful;
        public UnityEvent OnSignInFailed;

        private async void Start()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed In: {AuthenticationService.Instance.PlayerId}");

                byte[] payload = Encoding.UTF8.GetBytes(AuthenticationService.Instance.PlayerId);
                NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;

                OnSignInSuccessful?.Invoke();
            };

            AuthenticationService.Instance.SignInFailed += (RequestFailedException e) =>
            {
                Debug.Log($"Sign In Failed: {e}");
                OnSignInFailed?.Invoke();
            };

            Invoke(nameof(AttemptSignIn), m_ArtificialLoadTime);
        }

        public async void AttemptSignIn()
        {
            OnSignInBegin?.Invoke();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
