using UnityEngine;

public class GenericAudioSource : MonoBehaviour
{
    [Range(0f, 1f)][SerializeField]float _pitchRandomness = .2f;
    [SerializeField]AudioClip[] _clips;

    AudioSource _source;

	void Start()
    {
        _source = this.GetComponentInChildren<AudioSource>();

        _source.pitch += Random.Range(-_pitchRandomness, _pitchRandomness);
        _source.clip = _clips.RandomItem();
        _source.Play();
    }
}
