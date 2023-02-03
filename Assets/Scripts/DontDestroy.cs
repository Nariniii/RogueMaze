using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField]
    private bool singleton = true;

    private void Awake()
    {
        if(singleton)
        {
            if (tag == "Untagged")
            {
                Debug.LogWarning("Untagged", gameObject);
                return;
            }

            GameObject[] sameTagObjs = GameObject.FindGameObjectsWithTag(tag);
            foreach (var sameTagObj in sameTagObjs)
            {
                if (sameTagObj != gameObject && sameTagObj.name == gameObject.name)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
