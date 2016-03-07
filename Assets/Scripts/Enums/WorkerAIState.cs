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
/// AgentCmdState에 따라 AgentCommand들이 만들어진다.
/// </summary>
public enum AgentAIState
{
    IDLE, // no command
    MANAGE, // working command
    OBSERVE, // 

    CAPTURE_BY_CREATURE, // 환상체에게 공격당함
    CAPTURE_BY_AGENT, // 직원에게 공격당함

    PANIC_SUPPRESS_TARGET, // 제압당하는 중

    SUPPRESS_CREATURE,
    SUPPRESS_WORKER, // 제압중

    OPEN_ROOM,

    RETURN_CREATURE, // 환상체 돌려보내는 중

    PANIC_VIOLENCE, // 무작위 직원 공격

    RUN_AWAY, // 도망중
    DEAD
}

/// <summary>
/// Officer의 현재 행동 상태
/// </summary>
public enum OfficerAIState
{
    START, //하루가 시작 될 때 상태
    IDLE, //대기
    MEMO_MOVE, //메모 상태
    MEMO_STAY,
    CHAT, //채팅
    DOCUMENT, //문서를 들고 다른 부서로 이동
    WORKING, //일하는 중?
    PANIC, //패닉 상태
    RETURN, //자신의 부서로 돌아가는 상태
    /*
    OPEN_DOOR, //문엶
    RUN_AWAY, //도주
    */

    DEAD
}