using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    private static ViewManager instance;
    [SerializeField] private View _startView;
    [SerializeField] private View[] _views;
    private View _currentView;
    private readonly Stack<View> _history = new Stack<View>();
    public static T GetView<T>() where T : View
    {
        for (int i = 0; i < instance._views.Length; i++)
        {
            if (instance._views[i] is T tView)
            {
                return tView;
            }
        }
        return null;
    }


    public static void Show<T>(bool remember = true) where T : View
    {
        for (int i = 0; i < instance._views.Length; i++)
        {
            if (instance._views[i] is T)
            {
                if (instance._currentView != null)
                {
                    if (remember)
                    {
                        instance._history.Push(instance._currentView);
                    }
                    instance._currentView.Hide();
                }
                instance._views[i].Show();

                instance._currentView = instance._views[i];
            }
        }
    }

    public static void Show(View view, bool remember = true)
    {
        if (instance._currentView != null)
        {
            if (remember)
            {
                instance._history.Push(instance._currentView);
            }
            instance._currentView.Hide();
        }
        view.Show();
        instance._currentView = view;
    }

    public static void ShowLast()
    {
        if (instance._history.Count != 0)
        {
            Show(instance._history.Pop(), false);
        }
    }
    private void Awake() => instance = this;
    private void Start()
    {
        for (int i = 0; i < _views.Length; i++)
        {
            _views[i].Initialize();
            _views[i].Hide();
        }
        if (_startView != null)
        {
            Show(_startView, true);
        }
    }
}
