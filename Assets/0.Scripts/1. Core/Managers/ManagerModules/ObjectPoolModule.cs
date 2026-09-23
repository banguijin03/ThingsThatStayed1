using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ObjectPoolModule
{
	//������Ʈ �ϳ��� �����ϴ� ��û ���!
	//������Ʈ �ϳ��� ����� �Ŷ��, � �͵��� �ʿ��ұ�?
	//���� �� �ϸ� �Ǵµ�?
	PoolSetting _setting;
	public PoolSetting Setting => _setting;

	Transform rootTransform;

	//"��⿭"�� ���� �̴ϴ�!
	//                                          ��  ��  ��  ��
	//                                Queue => ���� �� �ְ� ���� ����
	//"��⿭"�� ������ ���� ���! => ť�� ��´�! / ������!
	//������ => �ۿ� �ִ� �Ÿ� ���� �����Ϳ�
	//          �� ��������
	//          ��   ��         ��   �� => Stack
	Queue<GameObject> prepareQueue = new();

	//�۾����� �ֵ��� �� Queue�� Stack�� �ƴ���?
	//���µ� ���� ����
	//List<GameObject> inProgressList = new();

	//������ ��, "4���� ���� �̻�, ������ ���� �ڰ��� 2�� �̻�, ������ ���� ��� 3�� �̻�"
	//������ �� ������ �־��ֱ�!
	//������! ��ȯ�� => ����, �̸� => ����
	public ObjectPoolModule(PoolSetting newSetting)
	{
		_setting = newSetting;
	}

	public void Initialize()
	{
		//�θ� - �ڽ� ���踦 �����!
		//����ó�� �� �� �ִ� ���� �����Կ�!
		rootTransform = new GameObject(Setting.poolName).transform;

		//Ǯ���Ϸ��� �ϴ� ���� �����տ� "PooledObject"��� �ϴ� ����
		//�ȵ��������! => �ʴ� Ǯ���� ģ����! => �߰����� �ʿ䰡 ���� ������?
		//�������� Ǯ���Ǵ� ������Ʈ�� �� �� PooledObject�� ������ ��!
		Setting.target?.TryAddComponent<PooledObject>();

		//���� �ϸ鼭 �̴Ͼ��� 30�� �� �Ŵϱ�! �� ��ŭ �غ� �̸� �س��ƾ���!
		//���ο� ������Ʈ�� �̸� ����ų ����!
		//��Ŀ 7�� ������ּ���.
		//�巳�� �տ� �� �ذ� �ִ� ģ�� ����
		PrepareObjects(Setting.countInitial);

	}

	//�����! => ������ ���ַ���!
	//����ϰ� �ִ� �ֵ� �߿� �ƹ��� �����͵� �Ǵ°�?
	//���� 6�� ���, ��ħ 9�� ���, ��ħ 10�� ���
	//1�� ���������� �� => ��ħ 9�� �� ����!
	//���� �� �ִ�???
	GameObject PrepareObject()
	{
		//Fake Null Check
		if(!Setting.target) return null;
		GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
		EnqueueObject(result);
		return result;
	}

	//uint => ���̳ʽ��� �����ϸ� �ȵ�!
	//unsigned => ��ȣ ����!
	void PrepareObjects(uint count)
	{
		if (!Setting.target) return;
		for (uint i = 0; i < count; i++)
		{
			GameObject result = CreateFromPrefab();
			EnqueueObject(result);
		}
	}

	//�������� ������ �޾ƿ�, ������ �⸧���� ��Դϴ�. => ������ �����ؼ� => ����� ���� ������ => �װ� �޾ƿ;� ��
	//													���ѹα����� ���� ��۰Ÿ��� �� ��
	//���⿡���� ���� �۾��� ���ʿ� ���ϴ� �� �־�� => �������� �� �־�� ���� �� ���ɻ� ����!
	void PrepareObjects(uint count, out GameObject activeObject)
	{
		if (!Setting.target)
		{
			activeObject = null;
			return;
		}

		activeObject = CreateFromPrefab();

		for (uint i = 1; i < count; i++)
		{
			GameObject result = CreateFromPrefab();
			EnqueueObject(result);
		}
	}

	public GameObject CreateFromPrefab()
	{
		GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);

		if (result)
		{
			result.name = Setting.poolName;

			if(result.TryGetComponent(out PooledObject pool))
			{
				//��.. ���� �� �Ǹ� ���� ���� �Լ��� ���Ŷ�!
				pool.OnEnqueueEvent -= DestroyObject;
				pool.OnEnqueueEvent += DestroyObject;
			}
		}

		return result;
	}

	//������Ʈ�� �����ش޶�� ��Ź!
	public GameObject CreateObject(Transform parent = null)
	{
		GameObject result;
		//��⿭�� �ƹ��� ���� ��
		if(!prepareQueue.TryDequeue(out result))
		{
			//���� ����ڸ� �̾Ƽ� �������� �˴ϴ�!
			//�߰��� ������ �� ���� ������� �ϴ� ���� ���ڷ� �����س��ұ� ����!
			PrepareObjects(Setting.countAdditional, out result);
		}

		if(result) //��������ٸ�!
		{
			//PooledObject�� ����ִ��� Ȯ���ϱ�
			if(result.TryGetComponent(out PooledObject pool))
			{
				pool.OnDequeue(); //���� ����!
			}
			result.SetActive(true);
			Transform currentTransform = result.transform;
			Transform originTransform = Setting.target.transform;

			currentTransform.SetParent(parent);
			//��ġ,ũ��,ȸ���� "�θ� ��������" �ʱ�ȭ�������!
			//2���� ��Ȳ (�Ϲ����� ��Ȳ, UI�� ��Ȳ)
			//����.. ��ƮƮ�������ε�.. ������.. ��ƮƮ�������̰�...��?
			//�� �� ��ƮƮ�������̶��!
			if(currentTransform is RectTransform asRectTransform 
				&& originTransform is RectTransform originRectTransform)
			{
				//1.��Ŀ�� �����ؿ���
				asRectTransform.anchorMin = originRectTransform.anchorMin;
				asRectTransform.anchorMax = originRectTransform.anchorMax;
				//2.�ǹ��� �����ؿ���
				asRectTransform.pivot = originRectTransform.pivot;

				//ȭ���� ����!
				if(parent)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(parent.transform as RectTransform);
				}

				//�� ģ���� stretch�� ���� Ȯ���� �� �ִ� ���!
				bool stretchX = asRectTransform.anchorMin.x != asRectTransform.anchorMax.x;
				bool stretchY = asRectTransform.anchorMin.y != asRectTransform.anchorMax.y;
				if(stretchX || stretchY)
				{
					//��ġ ���ذ��� �����´�.
					asRectTransform.offsetMin = originRectTransform.offsetMin;
					asRectTransform.offsetMax = originRectTransform.offsetMax;

					//if(stretchX)
					//{
					//	asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, originRectTransform.offsetMin.x, 0);
					//	//                                                           ������             +�������� ���� �����ʿ��� �������̴ϱ�
					//	//                                                                              -�������� ���� �ʹ�!
					//	asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -originRectTransform.offsetMax.x, 0);
					//}
					//if(stretchY)
					//{
					//	asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, originRectTransform.offsetMin.y, 0);
					//	asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -originRectTransform.offsetMax.y, 0);
					//}
				}
				else
				{
					//3.��Ŀ�� �������� ���� "��ġ"���� �����;� ��!
					asRectTransform.anchoredPosition = originRectTransform.anchoredPosition;
					//4.UI�� "������ ��"�� �����´�
					asRectTransform.sizeDelta = originRectTransform.sizeDelta;
				}
			}
			else
			{
				currentTransform.localPosition = originTransform.localPosition;
			}
			currentTransform.localRotation = originTransform.localRotation;
			currentTransform.localScale = originTransform.localScale;

		}

		return result;
	}

	//������Ʈ�� �����ش޶�� ��Ź!
	public void DestroyObject(GameObject target)
	{
		//�����ϴ� ����� ��� �ɱ�?
		EnqueueObject(target);
		if(target)
		{
			target.transform.SetParent(rootTransform);
		}
	}

	public void EnqueueObject(GameObject target)
	{
		if (!target) return;	

		target.SetActive(false);

		//��⿭�� �ֱ�!
		prepareQueue.Enqueue(target);
	}
}
