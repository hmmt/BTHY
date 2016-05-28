using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicalLaserScript : MonoBehaviour {
    [System.Serializable]
    public class LaserSprite {
        public Sprite[] startSprite;
        public Sprite[] endSprite;

        public Texture[] textures;
    }

    LineRenderer line;
    SpriteRenderer referenceSpriteRenderer;

    public SpriteRenderer startSprite;
    public SpriteRenderer endSprite;
    public GameObject startPos;
    public GameObject endPos;
    public float laserDispaly = 0.5f;
    public float prewarmTime = 0f;
    float elased = 0f;
    bool _isEnabled = false;
    public bool isEnabled {
        get { return _isEnabled; }
    }
    WorkerModel currentTarget = null;
    PassageObjectModel currentPassage = null;
    MovableObjectNode movable {
        get {
            return model.GetMovableNode();
        }
    }
    MagicalGirl script;
    MagicalGirlAnim animScript;
    bool canShoot = false;

    CreatureModel model;
    List<WorkerModel> targetList;
    UnitDirection currentDir = UnitDirection.RIGHT;

    public float sameTargetTimer = 1f;
    float sameTargetElapased = 0f;

    float spriteElased = 0f;
    int spriteIndex = 0;

    public LaserSprite sprites;
    
    public void Awake() {
        line = this.GetComponent<LineRenderer>();
        referenceSpriteRenderer = this.GetComponent<SpriteRenderer>();

        line.sortingLayerID = referenceSpriteRenderer.sortingLayerID;
        line.sortingOrder = referenceSpriteRenderer.sortingOrder;
        line.enabled = false;
        startSprite.enabled = false;
        endSprite.enabled = false;

    }
    /*
    public void EnableLaser(Vector3 pos) {
        this.endPos.transform.position = pos;
        ReadyPos();
    }*/


    public void Update() {
        if (_isEnabled) {
            SetLaserSprite();
            WorkerModel tempTarget = null;
            tempTarget = HitScan();
            if (tempTarget == null) {
                elased = 0f;
                sameTargetElapased = 0f;
                LaserDisable();
                //Debug.Log("4");
                return;
            }

            if (currentTarget != tempTarget) {
                Shoot(tempTarget);
                currentTarget = tempTarget;
                //Debug.Log(tempTarget);
                sameTargetElapased = 0f;
            }

            SetPos(currentTarget.GetMovableNode().GetCurrentViewPosition());

            elased += Time.deltaTime;
            sameTargetElapased += Time.deltaTime;
            if (sameTargetElapased > sameTargetTimer) {
                Shoot(currentTarget);
                sameTargetElapased = 0f;
            }

            if (elased > laserDispaly) {
                LaserDisable();
                //Debug.Log("5");
                elased = 0f;
            }
        }

    }
    public void Init(CreatureModel model) {
        //movable = model.GetMovableNode();
        this.model = model;
        script = model.script as MagicalGirl;
        animScript = script.animScript;
    }

    public void EnableLaser(PassageObjectModel passage, List<WorkerModel> targets)
    {
        currentPassage = passage;
        this.targetList = targets;

        float startX = this.model.GetMovableNode().GetCurrentViewPosition().x;
        float minX = 0f;
        MapNode passageEndNode = null;
        UnitDirection rot = UnitDirection.RIGHT;
        if (passage == null) {
            Debug.Log("Null");

            LaserDisable();
            return;
        }

        foreach (MapNode node in passage.GetNodeList()) {
            UnitDirection dir;
            float nodeX = node.GetPosition().x;
            float value = 0f;
            if (startX > nodeX)
            {
                value = startX - nodeX;
                dir = UnitDirection.LEFT;
            }
            else if (startX == nodeX) continue;
            else 
            { 
                value = nodeX - startX;
                dir = UnitDirection.RIGHT;
            }

            if (value > minX) {
                minX = value;
                passageEndNode = node;
                rot = dir;
            }
        }
        script.SetUnitDirection(rot);
        currentDir = rot;
        /*
        minX = float.MaxValue;
        WorkerModel target = null;
        foreach (WorkerModel wm in targets) {
            float currentX = wm.GetMovableNode().GetCurrentViewPosition().x;
            float val = 0f;


        
        }

        */

        //this.endPos.transform.position = target.GetMovableNode().GetCurrentViewPosition();
        
        //currentTarget = target;
        ReadyPos();//Stopping worker needed
    }

    WorkerModel HitScan() {
        float startX = model.GetMovableNode().GetCurrentViewPosition().x;
        float maxVal = float.MaxValue;

        WorkerModel target = null;

        foreach (WorkerModel wm in this.targetList) {
            float currentx = wm.GetMovableNode().GetCurrentViewPosition().x;
            float val = 0f;
            if (currentDir == UnitDirection.RIGHT)
            {
                //ㅁ뱀위치보다 x값이 큰 애들
                if (currentx < startX) continue;
                val = currentx - startX;
            }
            else {
                if (currentx > startX) continue;
                val = startX - currentx;
            }
            if (maxVal > val) {
                target = wm;
                maxVal = val;
            }
        }

        if (target != null)
        {
            endPos.transform.position = target.GetMovableNode().GetCurrentViewPosition();
            //this.currentTarget = target;

            if (line.enabled == false)
            {
                ReadyLine();
            }
            return target;
        }
        else { 
            //fail, release, disable
            return null;
        }

    }

    void ReadyPos()
    {
        _isEnabled = true;
        SetSprite();
    }

    public void SetLaserSprite() {
        int fps = 30;
        spriteElased += Time.deltaTime;
        if (spriteElased > 1 / (float)fps) {
            spriteElased = 0f;

            spriteIndex = (spriteIndex + 1) % sprites.endSprite.Length;
            SetSprite();
        }
    }

    void SetSprite() {
        startSprite.sprite = sprites.startSprite[spriteIndex];
        endSprite.sprite = sprites.endSprite[spriteIndex];

        line.materials[0].mainTexture = sprites.textures[spriteIndex];
    }

    void ReadyLine() {
        line.enabled = true;
        startSprite.enabled = true;
        endSprite.enabled = true;
    }

    public void StartLaser(){
        
    }

    IEnumerator StartingLaser() {
        
        int fps = 30;
        int totalFrame = 0;
        int cnt = 0;
        bool success = false;

        totalFrame =(int)(prewarmTime * fps);
        SetPos( startPos.transform.position);
        if (prewarmTime == 0f || totalFrame == 0) yield return null;

        while (true) {
            if (cnt == totalFrame) break;


            yield return new WaitForSeconds(1 / (float)fps);
            if (movable.GetPassage() == null || movable.GetPassage().GetNodeList() == null)
            {
                success = false;
                break;
            }
            Vector3 reference = endPos.transform.position - startPos.transform.position;
            /*
            Debug.Log(currentTarget);

            Debug.Log(currentTarget.GetMovableNode());
            Debug.Log(currentTarget.GetMovableNode().GetPassage());
            Debug.Log(movable);
            Debug.Log(movable.GetPassage());
            */

            if (currentTarget.GetMovableNode().GetPassage() == movable.GetPassage())
            {
                reference = currentTarget.GetMovableNode().GetCurrentViewPosition() - startPos.transform.position;

            }
            else {
                
                MapNode endNode = null;
                float currentX = movable.GetCurrentViewPosition().x;
                float minX = float.MaxValue;
                foreach (MapNode node in movable.GetPassage().GetNodeList()) {
                    float nodex = node.GetPosition().x;
                    if (movable.GetDirection() == UnitDirection.RIGHT)
                    {
                        if ((nodex - currentX) < minX) {
                            minX = nodex - currentX;
                            endNode = node;
                        }
                    }
                    else {
                        if ((currentX - nodex) < minX) {
                            minX = nodex - currentX;
                            endNode = node;
                        }
                    }
                }

                if (endNode == null)
                {
                    Debug.Log("Error");
                    success = false;

                }

                reference = endNode.GetPosition() - startPos.transform.position;
            }

            
            cnt++;
            SetPos( reference * cnt / totalFrame);

            //Check Agent in Current Pos
        }
        if (success == false)
        {

        }
        else
        {

            Shoot(null);
        }
    }

    public void SetPos( Vector3 end) {
        
        line.SetPosition(0, startPos.transform.position);
        line.SetPosition(1, new Vector3(end.x, startPos.transform.position.y, end.z));

        if (this.currentDir == UnitDirection.RIGHT) {
            if (startPos.transform.position.x > end.x) {
                line.enabled = false;
                return;
            }
        }
        else if (this.currentDir == UnitDirection.LEFT) {
            if (startPos.transform.position.x < end.x) {
                line.enabled = false;
                return;
            }
        }
        line.enabled = true;
    }

    void Shoot(WorkerModel target) {
        if (!_isEnabled) return;
        SetPos( this.endPos.transform.position);
        //공격판정
        if (target == null) {
            script.FailAttack();
            LaserDisable();
            //Debug.Log("1");
            return;
        }

        if (target.isDead() == false) {
            if (target.GetMovableNode().GetPassage() == movable.GetPassage()) {
                target.TakePhysicalDamageByCreature(1f);
                AgentAnim targetAnim = null;
                if (target is AgentModel)
                {
                    targetAnim = AgentLayer.currentLayer.GetAgent(target.instanceId).animTarget;
                }
                else
                {
                    targetAnim = OfficerLayer.currentLayer.GetOfficer(target.instanceId).animTarget;
                }

                targetAnim.TakeDamageAnim(1);
                script.SuccessAttack();
                int cnt = 0;
                foreach (WorkerModel wm in this.targetList) {
                    if (wm.isDead() == false) {
                        cnt++;
                    }
                }
                if (cnt == 0) {
                    LaserDisable();
                    //Debug.Log("2");
                }
                return;
            }
        }

        script.FailAttack();
        LaserDisable();
        //Debug.Log("3");
    }

    public void LaserDisable() {
        _isEnabled = false;
        line.enabled = false;
        startSprite.enabled = false;
        endSprite.enabled = false;

        this.currentTarget = null;
        script.EndAttack();
    }
}
