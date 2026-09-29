using UnityEngine;
using UnityEngine.EventSystems;

public delegate void DragStartEvent(UI_DraggableWindow dragTarget, Vector2 startPosition);

public class UI_DraggableWindow : UIBase, IPointerDownHandler
{
	public event DragStartEvent OnDragStart;

	//�巡���ϸ� � Ʈ�������� �������� �ұ�?
	[SerializeField] RectTransform rootTransform;

	/// <summary> ���������� ���Ź��� ���콺�� ��ġ </summary>
	Vector2 currentScreenPosition;

	/// <summary> �̵��Ϸ��� �ߴµ� ������� ��ġ! </summary>
	Vector2 shiftedPosition;

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDragStart?.Invoke(this, eventData.position);
	}

	public void SetMouseStartPosition(Vector2 screenPosition)
	{
		currentScreenPosition = screenPosition;
		shiftedPosition = Vector2.zero;
	}

	public void SetMouseCurrentPosition(Vector2 screenPosition)
	{
		//���콺�� ��ġ�� �ٲ���� ������
		//�󸶳� ���������� ���콺�� ���� �޾ƿ���
		//������ �Ÿ� = ������ - �����
		//               5   -   3    =  2
		//������ �Ÿ��� ���� ������ 1�̾����� 1��ŭ �����̸� ����!
		//������ 1.3�谡 �Ǿ��ٸ� 1��ŭ �����̰� �; 1.3��ŭ �� ���� �Ǿ������!
		//�θ��� ����� ��������� �ϴϱ� 1.3�� 1�� ������� 1.3���� �����ָ� �˴ϴ�!
		Vector2 screenDelta = screenPosition - currentScreenPosition;
		currentScreenPosition = screenPosition;

		//������ �������� �󸶳� �������� �ϴ°�?
		//shiftedPosition�� �����־ �̰� ����Ѵٸ� ��� �ɱ�?
		//    �� ���� ��ȣ�� ���ٴ� �� Ȯ��
		if (shiftedPosition.x * screenDelta.x > 0.0f)
		{
			//���� �� �ִ� ��
			//���콺�� -10��ŭ ������  �̹� -4��ŭ ������ �Ǿ��־��� ��Ȳ!
			//�����̴°� -6�̰�, ���� �������� 0�̴�
			//���콺�� 3��ŭ ������ �������� 6�� �־���
			//�����̴°� 0�̰�, ���� �������� 3�̴�
			//screenDelta	ShiftedPosition
			//  -10               -4         �� �� ���밪�� �� ���� ��
			//  -6                 0         -4�� ����!

			//  3                  6         �� �� ���밪�� �� ���� ��
			//  0                  3          3�� ����!
			float counter = Mathf.Min(Mathf.Abs(screenDelta.x), Mathf.Abs(shiftedPosition.x));
			//�׷��� ���� ���� ���� ������
			//���� ���� ��ȣ�� �־��ֱ�!
			counter *= Mathf.Sign(shiftedPosition.x);
			shiftedPosition.x -= counter;
			screenDelta.x -= counter;
		}
		if (shiftedPosition.y * screenDelta.y > 0.0f)
		{
			float counter = Mathf.Min(Mathf.Abs(screenDelta.y), Mathf.Abs(shiftedPosition.y));
			counter *= Mathf.Sign(shiftedPosition.y);
			shiftedPosition.y -= counter;
			screenDelta.y -= counter;
		}

		//����ó��~!
		//�ƴ� �̰� ���� ���� �Ÿ��� ���µ���?       ���ҷ�~
		//magnitude�� �Ը�!
		//sqr�� ����
		//�� ���� ���̸� �����ؼ� ���� �ɱ�?
		// z^2 = x^2 + y^2
		// z   = sqrt(x^2 + y^2)
		//������ "��"�� �ƴ϶� �������� ���ѰŴ�!
		//�������� �� �ص� �Ǵ� ������ => 0�� �����ϸ� 0��!
		//0�� üũ�� �ǵ�, ��Ʈ�� �� ����?
		if (screenDelta.sqrMagnitude == 0.0f) return;


		//inverseAABB => ����� �����ΰ�?
		//�̰� ���� �ִ� ����
		//rect���� ������ �����ϴ� ��ġ : ���� �� ģ���� ��ġ�� X => ��?
		//���� ������, ����������, Ư�� ��� ��ġ => ��...�޶�� �ž�?
		//�ٵ� �ְ� ����! => ������ ũ��� �½��ϴ�!
		//��ġ�� �⺻������ ��� �ִ°�?
		//������ "Pivot"��ġ�� ��������!
		Rect rootRect = rootTransform.rect;

		//������ �� ��������? �ٲ� �ڿ� ���غ��� �Ѵ�
		//                                   ���� ��ġ             +  �̵���
		rootRect.position += (Vector2)(rootTransform.localPosition / UIManager.UIScale) + screenDelta;

		//�ٲٰ� ���� ��ŭ Ƣ����°��� Ȯ���غ���!
		//Ƣ��� �� �������ִ� ���� InversedAABB�� �����ִϱ�
		//�������ִ� ��ŭ ��ġ �̵��� �����Ѵ�!
		Vector2 overScreen = rootRect.InversedAABB(UIManager.UIBoundary);
		//�̵� ���� �ѷ��� �����س���!
		shiftedPosition += overScreen;
		screenDelta += overScreen;

		Vector3 positionDelta = (Vector3)screenDelta;

		if(UIManager.UIScale > 0.0f) positionDelta /= UIManager.UIScale;

		rootTransform.localPosition += positionDelta;
	}
}
