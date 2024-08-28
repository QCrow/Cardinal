using UnityEngine;

//TODO: Use DOTween to improve game feel
public static class TransformUtil
{
    public static void MoveTo(GameObject gameObject, GameObject target)
    {
        RectTransform sourceRectTransform = gameObject.GetComponent<RectTransform>();
        RectTransform targetRectTransform = target.GetComponent<RectTransform>();

        Vector3 worldPosition = targetRectTransform.TransformPoint(targetRectTransform.rect.center);
        Vector3 localPosition = sourceRectTransform.parent.InverseTransformPoint(worldPosition);

        sourceRectTransform.localPosition = localPosition;
    }

    public static void MoveToAndSetParent(GameObject gameObject, GameObject target)
    {
        RectTransform sourceRectTransform = gameObject.GetComponent<RectTransform>();
        RectTransform targetRectTransform = target.GetComponent<RectTransform>();

        Vector3 worldPosition = targetRectTransform.TransformPoint(targetRectTransform.rect.center);
        Vector3 localPosition = sourceRectTransform.parent.InverseTransformPoint(worldPosition);

        sourceRectTransform.localPosition = localPosition;
        gameObject.transform.SetParent(target.transform);
        sourceRectTransform.localScale = new Vector3(1, 1, 1);
    }
}