using UnityEngine;

public class HingeUndfudger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector2 start_anchor;
    void Start()
    {
        start_anchor = GetComponent<HingeJoint2D>().connectedAnchor;
    }

    public void UnfudgeAnchor()
    {
        GetComponent<HingeJoint2D>().connectedAnchor = start_anchor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
