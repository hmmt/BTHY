using UnityEngine;
using System.Collections;

public class ParticleItem : MonoBehaviour {
   
    public ParticleSystem particle;//target
    public bool isLoop;

    public void Start() {
        particle.gameObject.SetActive(false);
    }

    public void OnEffect() {

    }

    public void InstantiatedEffect() { 
        
    }

    public void NormalEffect() {
        particle.gameObject.SetActive(true);
        particle.Play();
    }

}
