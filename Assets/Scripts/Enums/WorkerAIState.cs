using UnityEngine;
using System.Collections;
/*
/// <summary>
/// Agent의 현재 행동 상태
/// </summary>
public enum AgentAIState
{
    IDLE, // 세피라 코어에서 대기
    MANAGE, // 
    OBSERVE, //

    SUPPRESS_WORKER,
    SUPPRESS_CREATURE,
    RETURN_CREATURE, // 환상체 돌려보내는 중

    DEAD
}
*/
/// <summary>
/// 직원의 상태
/// AgentAIState에 따라 AgentCommand들이 만들어진다.
/// </summary>
public enum AgentAIState
{
    IDLE, // no command

    MANAGE, // working command
    OBSERVE, // 

	ENCOUNTER_CREATURE,
	ENCOUNTER_PANIC_WORKER,

    CAPTURE_BY_CREATURE, // 환상체에게 공격당함
    CAPTURE_BY_AGENT, // 직원에게 공격당함

    PANIC_SUPPRESS_TARGET, // 제압당하는 중

    SUPPRESS_CREATURE,
    SUPPRESS_WORKER, // 제압중

    OPEN_ISOLATE,

    RETURN_CREATURE, // 환상체 돌려보내는 중

    PANIC_VIOLENCE, // 무작위 직원 공격

    RUN_AWAY, // 도망중
    DEAD
}

public enum OfficerAIState { 
	START, //窍风啊 矫累 瞪 锭 惑怕
	IDLE, //措扁
	MEMO_MOVE, //皋葛 惑怕
	MEMO_STAY,
	CHAT, //盲泼
	DOCUMENT, //巩辑甫 甸绊 促弗 何辑肺 捞悼
	WORKING, //老窍绰 吝?
	PANIC, //菩葱 惑怕
	RETURN, //磊脚狼 何辑肺 倒酒啊绰 惑怕
	OPEN_DOOR, //巩慨
	RUN_AWAY, //档林
	DEAD
}