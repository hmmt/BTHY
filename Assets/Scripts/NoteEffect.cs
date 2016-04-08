using UnityEngine;
using System.Collections;

public class NoteEffect : MonoBehaviour {
    [System.Serializable]
    public class Note {
        public Material mainMaterial;
        public Texture[] textures;
        public int LoadIndex;

        private int index;

        public void Init() {
            textures = Resources.LoadAll<Texture>("Texture/Notes/" + this.LoadIndex);
            
            this.index = Random.Range(0, textures.Length - 3);
            mainMaterial.mainTexture = textures[0];
        }

        public void Change() {
            mainMaterial.mainTexture = textures[++index];
            if (index == textures.Length - 1) index = 0;
        }
    }

    public Note[] notes;
    public ParticleSystemRenderer[] particleRenderer;
    
    public void Awake() {
        for (int i = 0; i < notes.Length; i++)
        {
            notes[i].Init();
            particleRenderer[i].material = notes[i].mainMaterial;
            
        }
    }
    
    public void OnEnable() {
        StartChange();
    }

    public void StartChange() {
        for (int i = 0; i < notes.Length; i++) {
            StartCoroutine(Changing(notes[i]));
        }
    }

    public void OnDisable() {
        StopAllCoroutines();
    }

    public IEnumerator Changing(Note target)
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / 20f);
            target.Change();
        }
    }

}
