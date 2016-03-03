using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentHistory {
    public class History {
        public int workSuccess { get; set; } /* 작업 성공 횟수 */
        public int workFail { get; set; } /* 작업 실패 횟수 */
        public int takePhysicalDamage { get; set; } /* 받은 누적 물리 피해 */
        public int takeMentalDamage { get; set; } /* 받은 누적 정신 피해 */
        public int deathByCreature { get; set; } /* 환상체에 의한 직원의 죽음 목격 횟수 */
        public int panicByCreature { get; set; } /* 환상체에 의한 직원의 패닉 목격 횟수 */
        public int deathByWorker { get; set; } /* 직원에 의한 직원의 죽음 목격 횟수 */
        public int panic { get; set; } /* 패닉에 빠진 횟수 */
        public int creatureDamage { get; set; } /* 환상체에 가한 피해 */
        public int workerDamage { get; set; } /* 직원에게 가한 피해 */
        public int panicWorkerDamage { get; set; } /* 패닉에 빠진 상태로 직원에게 가한 피해 */
        public int suppressDamage { get; set; } /* 제압 명령을 통해 직원에게 가한 피해 */
        public int disposal { get; set; } /* 처분 당하는 직원을 목격한 횟수 */

        public History() {
            Clear();
        }

        public void Clear() {
            this.workSuccess = 0;
            this.workFail = 0;
            this.takePhysicalDamage = 0;
            this.takeMentalDamage = 0;
            this.deathByCreature = 0;
            this.panicByCreature = 0;
            this.deathByWorker = 0;
            this.panic = 0;
            this.creatureDamage = 0;
            this.workerDamage = 0;
            this.panicWorkerDamage = 0;
            this.suppressDamage = 0;
            this.disposal = 0;
            this.panic = 0;
        }

        public void AddWorkSuccess()
        {
            this.workSuccess++;
        }
        public void AddWorkFail()
        {
            this.workFail++;
        }
        public void PhysicalDamage(int damage)
        {
            this.takePhysicalDamage += damage;
        }
        public void MentalDamage(int damage)
        {
            this.takeMentalDamage += damage;
        }
        public void WitnessDeathByCreature()
        {
            this.deathByCreature++;
        }
        public void WitnessPanicByCreature()
        {
            this.panicByCreature++;
        }
        public void WitnessDeathByWorker()
        {
            this.deathByWorker++;
        }
        public void AddPanic()
        {
            this.panic++;
        }
        public void CreatureAttack(int damage)
        {
            this.creatureDamage += damage;
        }
        public void WorkerAttack(int damage)
        {
            this.workerDamage += damage;
        }
        public void PanicWorkerAttack(int damage)
        {
            this.panicWorkerDamage += damage;
            this.WorkerAttack(damage);
        }
        public void Suppress(int damage)
        {
            this.suppressDamage += damage;
            this.WorkerAttack(damage);
        }
        public void Disposition()
        {
            this.disposal++;
        }
        public void WorkResult(bool result)
        {
            if (result)
            {
                this.workSuccess++;
            }
            else this.workFail++;
        }
    }

    private int workDay { get; set; } /* 근무일 */
    private History oneday { get; set; } /* 하룻 동안 */
    private History total { get; set; } /* 전체 누적 */

    public int WorkDay { get { return workDay; } }
    public History Oneday { get { return oneday; } }
    public History Total { get { return total; } }

    public AgentHistory() {
        this.workDay = 0;
        oneday = new History();
        total = new History();
    }
    
    public void AddWorkDay()
    {
        this.workDay++;
    }

    public void EndOneDay() {
        total.workSuccess += oneday.workSuccess;
        total.workFail += oneday.workFail;
        total.takePhysicalDamage += oneday.takePhysicalDamage;
        total.takeMentalDamage += oneday.takeMentalDamage;
        total.deathByCreature += oneday.deathByCreature;
        total.panicByCreature += oneday.panicByCreature;
        total.deathByWorker += oneday.deathByWorker;
        total.panic += oneday.panic;
        total.creatureDamage += oneday.creatureDamage;
        total.workerDamage += oneday.workerDamage;
        total.panicWorkerDamage += oneday.panicWorkerDamage;
        total.suppressDamage += oneday.suppressDamage;
        total.disposal += oneday.disposal;

        oneday.Clear();
    }

    public Dictionary<string, object> GetSaveData(Dictionary<string, object> dic) {
        Dictionary<string, object> output = dic;

        output.Add("workDay", workDay);
        output.Add("workSuccess", total.workSuccess);
        output.Add("workFail", total.workFail);
        output.Add("physicalDamage", total.takePhysicalDamage);
        output.Add("mentalDamage", total.takeMentalDamage);
        output.Add("deathByCreature", total.deathByCreature);
        output.Add("panicByCreature", total.panicByCreature);
        output.Add("deathByWorker", total.deathByWorker);
        output.Add("panic", total.panic);
        output.Add("creatureDagmae", total.creatureDamage);
        output.Add("workerDamage", total.workerDamage);
        output.Add("panicWorkerDamage", total.panicWorkerDamage);
        output.Add("suppressDamage", total.suppressDamage);
        output.Add("disposition", total.disposal);

        return output;
    }

    public void LoadData(Dictionary<string, object> dic) {
        int[] temp = new int[14];
        for (int i = 0; i < temp.Length; i++) { temp[i] = 0; }

        TryGetValue(dic, "workDay", ref temp[0]);
        TryGetValue(dic, "workSuccess", ref temp[1]);
        TryGetValue(dic, "workFail", ref temp[2]);
        TryGetValue(dic, "physicalDamage", ref temp[3]);
        TryGetValue(dic, "mentalDamage", ref temp[4]);
        TryGetValue(dic, "deathByCreature", ref temp[5]);
        TryGetValue(dic, "panicByCreature", ref temp[6]);
        TryGetValue(dic, "deathByWorker", ref temp[7]);
        TryGetValue(dic, "panic", ref temp[8]);
        TryGetValue(dic, "creatureDagmae", ref temp[9]);
        TryGetValue(dic, "workerDamage", ref temp[10]);
        TryGetValue(dic, "panicWorkerDamage", ref temp[11]);
        TryGetValue(dic, "suppressDamage", ref temp[12]);
        TryGetValue(dic, "disposition", ref temp[13]);

        this.workDay = temp[0];
        this.total.workSuccess = temp[1];
        this.total.workFail = temp[2];
        this.total.takePhysicalDamage = temp[3];
        this.total.takeMentalDamage = temp[4];
        this.total.deathByCreature = temp[5];
        this.total.panicByCreature = temp[6];
        this.total.deathByWorker = temp[7];
        this.total.panic = temp[8];
        this.total.creatureDamage = temp[9];
        this.total.workerDamage = temp[10];
        this.total.panicWorkerDamage = temp[11];
        this.total.suppressDamage = temp[12];
        this.total.disposal = temp[13];
    }

    private static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field)
    {
        object output;
        if (dic.TryGetValue(name, out output)) {
            return true;
        }
        return false;
    }

}


