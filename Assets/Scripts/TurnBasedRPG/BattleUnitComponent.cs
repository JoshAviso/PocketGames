using System.Collections;
using UnityEngine;

public class BattleUnitComponent : MonoBehaviour, IBattleUnit
{
    public bool AddToBattle;
    public bool RemoveFromBattle;
    private bool Added;
    public int targetPos = 0;

    public void Move(Transform from, Transform to, IBattleUnit.MoveMode move_mode = IBattleUnit.MoveMode.Normal)
    {
        if(to == null) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        to.GetPositionAndRotation(out var toPos, out var toRot);
        if(from == null)
        {
            transform.SetPositionAndRotation(toPos, toRot);
            return;
        }
        
        StopAllCoroutines();
        StartCoroutine(MoveStaggered(from, to, 2f, 10f, move_mode == IBattleUnit.MoveMode.Station_To_Offscreen));
    }

    IEnumerator MoveStaggered(Transform from, Transform to, float moveSpeed, float rotSpeed, bool disableOnFinish)
    {
        float moveTargetTimeSqrd = Vector3.SqrMagnitude(from.position - to.position) / (moveSpeed * moveSpeed);
        float rotTargetTime = Mathf.Abs(Quaternion.Angle(from.rotation, to.rotation)) / rotSpeed;
        float elapsedTime = 0f;

        to.GetPositionAndRotation(out var toPos, out var toRot);
        from.GetPositionAndRotation(out var fromPos, out var fromRot);
        transform.SetPositionAndRotation(
            moveTargetTimeSqrd <= 0 ? toPos : fromPos, 
            rotTargetTime <= 0 ? toRot : fromRot
        );

        while(moveTargetTimeSqrd > elapsedTime * elapsedTime || rotTargetTime > elapsedTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;

            float moveProgress = Mathf.Min(1f, elapsedTime * elapsedTime / (moveTargetTimeSqrd + 0.001f));
            float rotProgress = Mathf.Min(1f, elapsedTime / (rotTargetTime + 0.001f));

            transform.SetPositionAndRotation(
                Vector3.Lerp(fromPos, toPos, moveProgress),
                Quaternion.Slerp(fromRot, toRot, rotProgress)
            );
        }

        transform.SetPositionAndRotation(toPos, toRot);
        if(disableOnFinish) gameObject.SetActive(false);
    }

    void Update()
    {
        if(!Added && AddToBattle)
        {
            AddToBattle = false;
            Added = true;
            BattleStationManager.AddUnit(this, targetPos);
        }
        if(Added && RemoveFromBattle)
        {
            RemoveFromBattle = false;
            Added = false;
            BattleStationManager.RemoveUnit((uint) targetPos, out _);
        }
    }
}
