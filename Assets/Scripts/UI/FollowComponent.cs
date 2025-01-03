using UnityEngine;

public class FollowComponent : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed = 5;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position, _followSpeed * Time.deltaTime);
    }
}