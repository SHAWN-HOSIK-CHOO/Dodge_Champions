using UnityEngine;
using UnityEngine.Pool;

namespace MainScene
{
    public class TalkFloating : MonoBehaviour
    {
        [SerializeField]
        Talk _talkPrefab;
        [SerializeField]
        GameObject _talkElement;
        ObjectPool<Talk> _talkObjPool;
        [SerializeField]
        UIConsole _uiConsole;
        void Start()
        {
            _talkObjPool = new ObjectPool<Talk>(() =>
            {
                Talk obj = Instantiate(_talkPrefab, _talkElement.transform, false);
                obj.transform.SetParent(_talkElement.transform);
                obj.gameObject.SetActive(false);
                return obj;
            });

            _uiConsole.onSubmit += _uiConsole_onSubmit;
        }

        private void _uiConsole_onSubmit(HP.TMPInputField.IInputMode mode, string text)
        {
            TriggerTalk(text);
        }

        public void TriggerTalk(string message)
        {
            Talk talkObj = _talkObjPool.Get();

            if (talkObj != null)
            {
                talkObj.gameObject.SetActive(true);
                talkObj.Show(_talkElement.GetComponent<RectTransform>(), message, () =>
                {
                    _talkObjPool.Release(talkObj);
                });
            }
        }
        private void OnDestroy()
        {
            _talkObjPool.Dispose();
        }
    }
}