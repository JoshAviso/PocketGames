
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Camera _targetCamera;
    [SerializeField] Vector3 _offsetAngle;
    [SerializeField] bool _lockXRotation;
    [SerializeField] bool _lockYRotation;
    [SerializeField] bool _lockZRotation;

    bool _targetingMainCamera;

    void Start(){
        if(_targetCamera == null) _targetingMainCamera = true;
    }

    void LateUpdate() {
        if(_targetCamera == null && Camera.main == null) return;

        if(_targetingMainCamera) _targetCamera = Camera.main;

        Vector3 targetDirection = _targetCamera.transform.position - transform.position;
        if(_lockXRotation) targetDirection.x = 0f;
        if(_lockYRotation) targetDirection.y = 0f;
        if(_lockZRotation) targetDirection.z = 0f;

        Quaternion offsetX = Quaternion.AngleAxis(_offsetAngle.x, Vector3.right);
        Quaternion offsetY = Quaternion.AngleAxis(_offsetAngle.y, Vector3.up);
        Quaternion offsetZ = Quaternion.AngleAxis(_offsetAngle.z, Vector3.forward);
        
        if(targetDirection == Vector3.zero){
            transform.rotation = offsetX * offsetY * offsetZ;
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(targetDirection, Vector3.up);
        
        transform.rotation = targetRot * offsetX * offsetY * offsetZ;
    }
}