using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ư�� ����� �Ǵ� �߻� Ŭ�����Դϴ�.
/// </summary>
public abstract class BaseButton : MonoBehaviour
{
    RectTransform _rectTransform;   // rectTransform ĳ��
    Camera _camera;                 // ī�޶� ĳ��

    protected bool isClicked;       // Ŭ���� �������� üũ
    protected Image buttonImage;    // ��ư���� ����ϴ� �̹���

    void Start()
    {
        // �ʱ�ȭ
        _camera = Camera.main.GetComponent<Camera>();
        _rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();

        // �̺�Ʈ ����
        InputManager.Instance.OnInputDown += HandleInputDown;
        InputManager.Instance.OnInputUp += HandleInputUp;
    }

    /// <summary>
    /// �Է��� ���۵Ǹ� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ</param>
    void HandleInputDown(Vector2 inputPosition)
    {
        if (IsPointerOverButton(inputPosition))
        {
            // �Է� ��ǥ�� ��ư�ȿ� ������ Ŭ���� �� ������ ó���ϰ� ��ư ��������Ʈ ������Ʈ
            isClicked = true;
            UpdateButtonSprite();
        }
    }

    /// <summary>
    /// �Է��� ����Ǹ� ȣ��Ǵ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ǥ��</param>
    void HandleInputUp(Vector2 inputPosition)
    {
        if (isClicked)
        {
            // Ŭ���� ���¿��� �Է� ��ǥ�� ��ư �ȿ� ������ ��ư Ŭ�� �޼��� ����
            if (IsPointerOverButton(inputPosition))
            {
                HandleButtonClick();
            }
            // Ŭ���� ���� ���� ������ ó���ϰ� ��ư ��������Ʈ ������Ʈ
            isClicked = false;
            UpdateButtonSprite();
        }
    }

    /// <summary>
    /// �Է� ��ġ�� ��ư ���� �ִ��� üũ�ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="inputPosition">�Է� ��ġ</param>
    /// <returns>�Է� ��ġ�� ��ư ���� ������ true, �ƴ϶�� false�� ��ȯ�մϴ�.</returns>
    bool IsPointerOverButton(Vector2 inputPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, inputPosition, _camera);
    }

    /// <summary>
    /// ��ư�� Ŭ���Ǿ��� �� ȣ��Ǵ� �߻� �޼����Դϴ�.
    /// �Ļ� Ŭ�������� �����˴ϴ�.
    /// </summary>
    protected abstract void HandleButtonClick();

    /// <summary>
    /// ��ư ��������Ʈ�� ������Ʈ�ϴ� �߻� �޼����Դϴ�.
    /// �Ļ� Ŭ�������� �����մϴ�.
    /// </summary>
    protected abstract void UpdateButtonSprite();
}
