using UnityEngine;

public class MusicPersistence : MonoBehaviour
{
    private static MusicPersistence instance;
    public string currentTrack;
    private AudioSource AudioSource;


    void Awake()
    {
        // Singleton pattern to prevent duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Keeps object alive

        AudioSource = GetComponent<AudioSource>();

    }

    public void SetTrack(string name)
    {
        //Debug.Log(name);
        if (currentTrack != name && name != null && name != "")
        {
            currentTrack = name;
            AudioSource.clip = Resources.Load<AudioClip>("music/" + name);
            AudioSource.Play();
        }
    }

}
