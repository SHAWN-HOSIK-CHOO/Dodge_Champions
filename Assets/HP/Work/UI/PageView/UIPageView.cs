using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPageView : MonoBehaviour
{
    class Page
    {
        Vector2Int _grid;
        int _page;
        public Dictionary<(int, int), UIElement> _contents;
        public bool SpaceFull()
        {
            int x = _grid.x;
            int y = _grid.y;
            return (_contents.Count < x * y);
        }

        public void ReAlign()
        {
            var oldContents = _contents;
            _contents = new Dictionary<(int, int), UIElement>();
            foreach (var item in oldContents)
            {
                AddContent(item.Value);
            }
        }
        public bool RemoveContent(UIElement content)
        {
            foreach (var item in _contents)
            {
                if (item.Value == content)
                {
                   _contents.Remove(item.Key);
                    ReAlign();
                    return true;
                }
            }
            return false;
        }

        public void AddContent(UIElement content)
        {
            int x = _grid.x;
            int y = _grid.y;

            int lastX = _contents.Count % x +1;
            int lastY = _contents.Count / x +1;
            _contents[(lastY, lastX)] = content;
        }
        public Page(Vector2Int grid)
        {
            _grid = grid;
            _contents = new Dictionary<(int, int), UIElement>();
        }
    }

    [SerializeField]
    TMP_Text _text;
    [SerializeField]
    GameObject _YElementPref;

    [SerializeField]
    GameObject _XElementPref;

    [SerializeField]
    GameObject _YLayout;

    [SerializeField]
    UIImgButton _rightPage;

    [SerializeField]
    UIImgButton _leftPage;

    [SerializeField]
    Vector2Int _grid;
    Dictionary<int, Page>  _pages;

    int _currentPage;
    public UIElement _currentContent { get; private set; }


    void Start()
    {
        _leftPage.OnPointerClickAction += OnLeftPageClick;
        _rightPage.OnPointerClickAction += OnRightPageClick;
        
        _pages = new Dictionary<int, Page>();
        _currentPage = 1;
        _pages[_currentPage] = new Page(_grid);
        UpdatePage();
    }
    public void RemoveContent(UIElement content)
    {
        foreach (var item in _pages)
        {
            if(item.Value.RemoveContent(content))
            {
                content.OnPointerClickAction -= OnContentClicked;
                if (item.Value._contents.Count == 0)
                {
                    _currentPage--;
                    if (_currentPage < 1)
                    {
                        _currentPage = 1;
                    }
                    else
                    {
                        _pages.Remove(item.Key);
                    }
                    UpdatePage();
                }
                if(item.Key == _currentPage)
                {
                    UpdatePage();
                }
                return;
            }
        }
    }

    public void AddContent(UIElement content)
    {
        int lastPage = _pages.Count;
        if (_pages.TryGetValue(lastPage , out var page))
        {
            if(page.SpaceFull())
            {
                page = new Page(_grid);
                lastPage += 1;
                _pages[lastPage] = page;
            }
            page.AddContent(content);
            content.OnPointerClickAction += OnContentClicked;
        }

        if(_currentPage == lastPage) UpdatePage();
    }

    private void OnContentClicked(BaseEventData data)
    {
        _currentContent = data.selectedObject.GetComponent<UIElement>();
    }


    public void UpdatePage()
    {
        var page = _pages[_currentPage];
        int x = _grid.x;
        int y = _grid.y;

        int lastY = page._contents.Count / x + 1;
        int lastX = page._contents.Count % x + 1;

        foreach (Transform child in _YLayout.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < y; i++)
        {
            var yElement = Instantiate(_YElementPref, _YLayout.transform);
            Transform xLayout = yElement.transform.Find("XLayout");
            for (int j = 0; j < x; j++)
            {
                var xElement = Instantiate(_XElementPref, xLayout.transform);
                if(page._contents.TryGetValue((i + 1, j + 1),out var element))
                {
                    element.transform.SetParent(xElement.transform);
                }
            }
        }

        _text.text = $"{_currentPage}/{_pages.Count}";
    }
    private void OnRightPageClick(BaseEventData data)
    {
        _currentPage++;
        int lastPage = _pages.Count;
        if (_currentPage > lastPage) _currentPage = lastPage;

        UpdatePage();
    }
    private void OnLeftPageClick(BaseEventData data)
    {
        _currentPage--;
        if (_currentPage < 1) _currentPage = 1;
        UpdatePage();
    }
}
