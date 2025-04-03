using UnityEngine;

public class TestCube : MonoBehaviour
{
    [SerializeField] float speed = 1;

    void Update()
    {
        var hz = Input.GetAxis("Horizontal");
        var vt = Input.GetAxis("Vertical");
        var move = new Vector3(
            hz * this.speed * Time.deltaTime
            , vt * this.speed * Time.deltaTime
            , 0
            );
        this.transform.Translate(move);

        if (Input.GetKeyDown(KeyCode.R))
            this.transform.position = Vector3.zero;
    }
}
