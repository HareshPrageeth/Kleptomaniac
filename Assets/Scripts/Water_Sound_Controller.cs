using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public class Water_Sound_Controller : MonoBehaviour
{
    private float distance;
    public GameObject player;
    private Vector3 playerPosition;
    public bool fixX, fixY;
    public float width, height;
    public float minVolume, maxVolume;
    public float audioSourceWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        double maxDistance = -2;
        playerPosition = player.transform.position;
        if (fixX && fixY)
        {
            distance = Vector2.Distance(new Vector2(playerPosition.x, playerPosition.y), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
            maxDistance = Math.Sqrt(width * width + height * height) / 2;
        }
        else if (fixX)
        {
            distance = Vector2.Distance(new Vector2(playerPosition.x, 0), new Vector2(gameObject.transform.position.x, 0));
            maxDistance = width / 2;
        }
        else if (fixY)
        {
            distance = Vector2.Distance(new Vector2(0, playerPosition.y), new Vector2(0, gameObject.transform.position.y));
            maxDistance = height / 2;
        }
        if (distance <= audioSourceWidth / 2)
        {
            gameObject.GetComponent<AudioSource>().volume = maxVolume;
        }
        else if (distance>maxDistance)
        {
            gameObject.GetComponent<AudioSource>().volume = 0f;
        }
        else
        {
            double scaledDist = (distance - audioSourceWidth / 2) / (maxDistance - audioSourceWidth / 2);
            gameObject.GetComponent<AudioSource>().volume = (float)(maxVolume + (minVolume - maxVolume) * scaledDist);
        }
    }

}
