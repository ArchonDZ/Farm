using UnityEngine;
using UnityEngine.UI;

public abstract class CurtainPanel : MonoBehaviour
{
	[SerializeField] private Button _buttonOpen;

	protected bool _isOpened;

	void Awake()
	{
		_buttonOpen.onClick.AddListener(ChangePanel);
		VirtualAwake();
	}

	protected void ChangePanel()
	{
		if (_isOpened)
		{
			ClosePanel();
		}
		else
		{
			OpenPanel();
		}
	}

	protected virtual void VirtualAwake() { }

	public abstract void ClosePanel();
	public abstract void OpenPanel();
}
