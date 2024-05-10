using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Her bir UI Paneli i�in bir view referans� verilecek
/// </summary>
public abstract class View : MonoBehaviour
{
    public abstract void Initialize();
    public virtual void Hide() => gameObject.SetActive(false);
    public virtual void Show() => gameObject.SetActive(true);

}
