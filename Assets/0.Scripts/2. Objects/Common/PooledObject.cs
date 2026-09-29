using UnityEngine;

public delegate void PoolEnqueueEvent(GameObject target);
public delegate void PoolDequeueEvent(GameObject target);

public class PooledObject : MonoBehaviour
{
	public event PoolEnqueueEvent OnEnqueueEvent;
	public event PoolDequeueEvent OnDequeueEvent;

	//������Ʈ Ǯ��..�̶�� �ϴ� �� ��������?
	//���� / ������ �ϴ� ��� => �Ѱ� ��� �ɷ� ��ü!
	//���������� �� �ִ� �� �ƴұ��?
	//�����ϴ� ���� ���� => ������ ����
	//�� �տ� ���Ͱ� �ֽ��ϴ�.
	//���ʹ� ������ �����Ѵ�.
	//ȸ������A�� �ƽ�Ʈ���带 �����ϰ� �־���!
	//�� ȸ������A�� �׾���!
	//ȸ�� ������� �����ϴٺ��ϱ� ���ʰ� ȸ������A���ʰ� �Ǿ���!
	//���� �� ȸ�� �����.. �� �ұ�?
	//�ƽ�Ʈ���带 �����Ϸ� ���ϴ�.
	//ȸ���ߴ�! => ������� �ʱ�ȭ�Ǿ�����? ����
	//���� ���·� ��Ȱ�ؼ� �ƽ�Ʈ���带 ������ ���ϱ� => ����

	//ť�� ���ư� �� �� ��
	public void OnEnqueue()
	{
		//���� �̰�.. �̺�Ʈ�� ���� �� �ִ�!
		//������ ���ư��� ����� �־��� ������?
		//������ ���ư��� ����� ���� ģ���� ��� �ɱ�?
		//1�ð� �ȿ� ���� ��ġ�� ���ϰ� �����̴� ����.. �������� ��ٸ� ��
		if(OnEnqueueEvent != null)	OnEnqueueEvent.Invoke(gameObject);
		else Destroy(gameObject);
	}

	//ť���� ���� �� �� ��
	public void OnDequeue()
	{
		OnDequeueEvent?.Invoke(gameObject);
	}
}
