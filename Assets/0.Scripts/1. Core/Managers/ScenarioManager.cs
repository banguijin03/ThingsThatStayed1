using System.Collections;
using UnityEngine;

public class ScenarioManager : ManagerBase
{
    // 현재 진행 중인 시나리오
    ScenarioData currentScenario;

    // 현재 시나리오의 몇 번째 단계인지
    int currentIndex;

    // 시나리오가 진행 중인지 확인
    bool isScenario;

    public bool IsScenario => isScenario;

    protected override IEnumerator OnConnected(GameManager gameManager)
    {
        yield break;
    }

    protected override void OnDisconnected()
    {
    }

    // 시나리오 시작
    public void StartScenario(ScenarioData data)
    {
        if (data == null) return;
        if (isScenario) return;

        currentScenario = data;
        currentIndex = 0;
        isScenario = true;

        NextScenario();
    }


    // 다음 시나리오 단계로 이동
    public void NextScenario()
    {
        if (!isScenario) return;

        // 모든 시나리오 단계를 끝냈다면 종료
        if (currentIndex >= currentScenario.steps.Length)
        {
            EndScenario();
            return;
        }

        // 현재 단계 실행
        ExecuteScenario(currentScenario.steps[currentIndex]);

        currentIndex++;
    }


    // 현재 시나리오 단계 실행
    void ExecuteScenario(ScenarioStep step)
    {
        // 아직 실제 시나리오 기능은 만들지 않음
        Debug.Log($"시나리오 실행 : {step}");
    }


    // 시나리오 종료
    public void EndScenario()
    {
        currentScenario = null;
        currentIndex = 0;
        isScenario = false;
    }
}