using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MainManager
{
    public int[] TeamIds = new int[TeamCountMax];

    public void Init()
    {
        // 파티원 캐릭터 ID 지정(임시)
        int memberId = 0;
        while (memberId < TeamCountMax)
            TeamIds[memberId++] = memberId;
    }
}
